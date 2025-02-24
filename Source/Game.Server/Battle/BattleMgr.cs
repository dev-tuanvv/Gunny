namespace Game.Server.Battle
{
    using Game.Server.Rooms;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using System.Reflection;

    public class BattleMgr
    {
        public static bool AutoReconnect = true;
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static List<BattleServer> m_list = new List<BattleServer>();

        public static void AddBattleServer(BattleServer battle)
        {
            if (battle != null)
            {
                m_list.Add(battle);
                battle.Disconnected += new EventHandler(BattleMgr.battle_Disconnected);
            }
        }

        public static BattleServer AddRoom(BaseRoom room)
        {
            BattleServer server = FindActiveServer(room.isCrosszone);
            if ((server != null) && server.AddRoom(room))
            {
                return server;
            }
            return null;
        }

        private static void battle_Disconnected(object sender, EventArgs e)
        {
            BattleServer item = sender as BattleServer;
            log.WarnFormat("Disconnect from battle server {0}:{1}", item.Ip, item.Port);
            if (((item != null) && AutoReconnect) && m_list.Contains(item))
            {
                RemoveServer(item);
                TimeSpan span = (TimeSpan) (DateTime.Now - item.LastRetryTime);
                if (span.TotalMinutes > 3.0)
                {
                    item.RetryCount = 0;
                }
                if (item.RetryCount < 3)
                {
                    BattleServer battle = item.Clone();
                    AddBattleServer(battle);
                    battle.RetryCount++;
                    battle.LastRetryTime = DateTime.Now;
                    try
                    {
                        battle.Start();
                    }
                    catch (Exception exception)
                    {
                        log.ErrorFormat("Batter server {0}:{1} can't connected!", item.Ip, item.Port);
                        log.Error(exception.Message);
                        item.RetryCount = 0;
                    }
                }
            }
        }

        public static void ConnectTo(int id, string ip, int port, string key)
        {
            BattleServer battle = new BattleServer(id, ip, port, key);
            AddBattleServer(battle);
            try
            {
                battle.Start();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("Batter server {0}:{1} can't connected!", battle.Ip, battle.Port);
                log.Error(exception.Message);
            }
        }

        public static void Disconnet(int id)
        {
            BattleServer server = GetServer(id);
            if ((server != null) && server.IsActive)
            {
                server.LastRetryTime = DateTime.Now;
                server.RetryCount = 4;
                server.Connector.Disconnect();
            }
        }

        public static BattleServer FindActiveServer(bool isCrosszone)
        {
            lock (m_list)
            {
                foreach (BattleServer server in m_list)
                {
                    if ((isCrosszone && (server.ServerId == 2)) && server.IsActive)
                    {
                        return server;
                    }
                    if (!(isCrosszone || !server.IsActive))
                    {
                        return server;
                    }
                }
            }
            return null;
        }

        public static List<BattleServer> GetAllBattles()
        {
            return m_list;
        }

        public static BattleServer GetServer(int id)
        {
            foreach (BattleServer server in m_list)
            {
                if (server.ServerId == id)
                {
                    return server;
                }
            }
            return null;
        }

        public static void RemoveServer(BattleServer server)
        {
            if (server != null)
            {
                m_list.Remove(server);
                server.Disconnected += new EventHandler(BattleMgr.battle_Disconnected);
            }
        }

        public static bool Setup()
        {
            if (File.Exists("battle.xml"))
            {
                try
                {
                    foreach (XElement element in XDocument.Load("battle.xml").Root.Nodes())
                    {
                        try
                        {
                            int serverId = int.Parse(element.Attribute("id").Value);
                            string ip = element.Attribute("ip").Value;
                            int port = int.Parse(element.Attribute("port").Value);
                            string loginKey = element.Attribute("key").Value;
                            AddBattleServer(new BattleServer(serverId, ip, port, loginKey));
                            log.InfoFormat("Battle server {0}:{1} loaded...", ip, port);
                        }
                        catch (Exception exception)
                        {
                            log.Error("BattleMgr setup error:", exception);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    log.Error("BattleMgr setup error:", exception2);
                }
            }
            log.InfoFormat("Total {0} battle server loaded.", m_list.Count);
            return true;
        }

        public static void Start()
        {
            foreach (BattleServer server in m_list)
            {
                try
                {
                    server.Start();
                }
                catch (Exception exception)
                {
                    log.ErrorFormat("Batter server {0}:{1} can't connected!", server.Ip, server.Port);
                    log.Error(exception.Message);
                }
            }
        }
    }
}


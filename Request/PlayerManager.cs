namespace Tank.Request
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using System.Threading;

    public class PlayerManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<string, PlayerData> m_players = new Dictionary<string, PlayerData>();
        private static int m_timeout = 30;
        private static Timer m_timer;
        private static object sys_obj = new object();

        public static void Add(string name, string pass)
        {
            object obj2 = sys_obj;
            lock (obj2)
            {
                if (m_players.ContainsKey(name))
                {
                    m_players[name].Name = name;
                    m_players[name].Pass = pass;
                    m_players[name].Date = DateTime.Now;
                    m_players[name].Count = 0;
                }
                else
                {
                    PlayerData data = new PlayerData {
                        Name = name,
                        Pass = pass,
                        Date = DateTime.Now
                    };
                    m_players.Add(name, data);
                }
            }
        }

        protected static bool CheckTimeOut(DateTime dt)
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - dt);
            return (span.TotalMinutes > m_timeout);
        }

        private static void CheckTimerCallback(object state)
        {
            object obj2 = sys_obj;
            lock (obj2)
            {
                List<string> list = new List<string>();
                foreach (PlayerData data in m_players.Values)
                {
                    if (CheckTimeOut(data.Date))
                    {
                        list.Add(data.Name);
                    }
                }
                foreach (string str in list)
                {
                    m_players.Remove(str);
                }
            }
        }

        public static bool GetByUserIsFirst(string name)
        {
            object obj2 = sys_obj;
            lock (obj2)
            {
                if (m_players.ContainsKey(name))
                {
                    return (m_players[name].Count == 0);
                }
            }
            return false;
        }

        public static bool Login(string name, string pass)
        {
            pass = pass.Replace("chinhaus=", "");
            object obj2 = sys_obj;
            lock (obj2)
            {
                if (m_players.ContainsKey(name))
                {
                    log.Error(name + "|" + m_players[name].Pass);
                }
                else
                {
                    log.Error("NOHAVE " + name);
                }
                if (m_players.ContainsKey(name) && (m_players[name].Pass == pass))
                {
                    PlayerData data = m_players[name];
                    if ((data.Pass == pass) && !CheckTimeOut(data.Date))
                    {
                        return true;
                    }
                    log.Error(name + "|timeout:" + m_players[name].Date);
                    return false;
                }
                return false;
            }
        }

        public static bool Remove(string name)
        {
            object obj2 = sys_obj;
            lock (obj2)
            {
                return m_players.Remove(name);
            }
        }

        public static void Setup()
        {
            m_timeout = int.Parse(ConfigurationSettings.AppSettings["LoginSessionTimeOut"]);
            m_timer = new Timer(new TimerCallback(PlayerManager.CheckTimerCallback), null, 0, 0xea60);
        }

        public static bool Update(string name, string pass)
        {
            object obj2 = sys_obj;
            lock (obj2)
            {
                if (m_players.ContainsKey(name))
                {
                    m_players[name].Pass = pass;
                    PlayerData local1 = m_players[name];
                    local1.Count++;
                    return true;
                }
            }
            return false;
        }

        private class PlayerData
        {
            public int Count;
            public DateTime Date;
            public string Name;
            public string Pass;
        }
    }
}


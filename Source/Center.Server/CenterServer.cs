namespace Center.Server
{
    using Bussiness;
    using Bussiness.Managers;
    using Bussiness.Protocol;
    using Center.Server.Managers;
    using Center.Server.Statics;
    using Game.Base;
    using Game.Base.Events;
    using Game.Base.Packets;
    using Game.Server.Managers;
    using log4net;
    using log4net.Config;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading;

    public class CenterServer : BaseServer
    {
        private bool _aSSState;
        private CenterServerConfig _config;
        private bool _dailyAwardState;
        private static CenterServer _instance;
        private readonly int awardTime = 20;
        private bool boss = true;
        private bool close = true;
        private string Edition = "2612558";
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private System.Threading.Timer m_consortiaboss;
        private System.Threading.Timer m_loginLapseTimer;
        private System.Threading.Timer m_saveDBTimer;
        private System.Threading.Timer m_saveRecordTimer;
        private System.Threading.Timer m_scanAuction;
        private System.Threading.Timer m_scanConsortia;
        private System.Threading.Timer m_scanMail;
        private System.Threading.Timer m_sytemNotice;
        private System.Threading.Timer m_worldEvent;
        private int minute = 5;
        private DateTime minute1;
        private Random rand = new Random();

        public CenterServer(CenterServerConfig config)
        {
            this._config = config;
            this.LoadConfig();
        }

        public bool AvailTime(DateTime startTime, int min)
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - startTime);
            int num = min - ((int) span.TotalMinutes);
            return (num > 0);
        }

        public static void CreateInstance(CenterServerConfig config)
        {
            if (Instance == null)
            {
                FileInfo configFile = new FileInfo(config.LogConfigFile);
                if (!configFile.Exists)
                {
                    ResourceUtil.ExtractResource(configFile.Name, configFile.FullName, Assembly.GetAssembly(typeof(CenterServer)));
                }
                XmlConfigurator.ConfigureAndWatch(configFile);
                _instance = new CenterServer(config);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
            if (e.IsTerminating)
            {
                LogManager.Shutdown();
            }
        }

        public void DisposeGlobalTimers()
        {
            if (this.m_saveDBTimer != null)
            {
                this.m_saveDBTimer.Dispose();
            }
            if (this.m_loginLapseTimer != null)
            {
                this.m_loginLapseTimer.Dispose();
            }
            if (this.m_saveRecordTimer != null)
            {
                this.m_saveRecordTimer.Dispose();
            }
            if (this.m_scanAuction != null)
            {
                this.m_scanAuction.Dispose();
            }
            if (this.m_scanMail != null)
            {
                this.m_scanMail.Dispose();
            }
            if (this.m_scanConsortia != null)
            {
                this.m_scanConsortia.Dispose();
            }
            if (this.m_worldEvent != null)
            {
                this.m_worldEvent.Dispose();
            }
            if (this.m_sytemNotice != null)
            {
                this.m_sytemNotice.Dispose();
            }
            if (this.m_consortiaboss != null)
            {
                this.m_consortiaboss.Dispose();
            }
        }

        public ServerClient[] GetAllClients()
        {
            ServerClient[] array = null;
            lock (base._clients.SyncRoot)
            {
                array = new ServerClient[base._clients.Count];
                base._clients.Keys.CopyTo(array, 0);
            }
            return array;
        }

        protected override BaseClient GetNewClient()
        {
            return new ServerClient(this);
        }

        protected bool InitComponent(bool componentInitState, string text)
        {
            log.Info(text + ": " + componentInitState);
            if (!componentInitState)
            {
                this.Stop();
            }
            return componentInitState;
        }

        public bool InitGlobalTimers()
        {
            int dueTime = (this._config.SaveIntervalInterval * 60) * 0x3e8;
            if (this.m_saveDBTimer == null)
            {
                this.m_saveDBTimer = new System.Threading.Timer(new TimerCallback(this.SaveTimerProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_saveDBTimer.Change(dueTime, dueTime);
            }
            dueTime = (this._config.SystemNoticeInterval * 60) * 0x3e8;
            if (this.m_sytemNotice == null)
            {
                this.m_sytemNotice = new System.Threading.Timer(new TimerCallback(this.SystemNoticeTimerProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_sytemNotice.Change(dueTime, dueTime);
            }
            dueTime = (this._config.LoginLapseInterval * 60) * 0x3e8;
            if (this.m_loginLapseTimer == null)
            {
                this.m_loginLapseTimer = new System.Threading.Timer(new TimerCallback(this.LoginLapseTimerProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_loginLapseTimer.Change(dueTime, dueTime);
            }
            dueTime = (this._config.SaveRecordInterval * 60) * 0x3e8;
            if (this.m_saveRecordTimer == null)
            {
                this.m_saveRecordTimer = new System.Threading.Timer(new TimerCallback(this.SaveRecordProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_saveRecordTimer.Change(dueTime, dueTime);
            }
            dueTime = (this._config.ScanAuctionInterval * 60) * 0x3e8;
            if (this.m_scanAuction == null)
            {
                this.m_scanAuction = new System.Threading.Timer(new TimerCallback(this.ScanAuctionProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_scanAuction.Change(dueTime, dueTime);
            }
            dueTime = (this._config.ScanMailInterval * 60) * 0x3e8;
            if (this.m_scanMail == null)
            {
                this.m_scanMail = new System.Threading.Timer(new TimerCallback(this.ScanMailProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_scanMail.Change(dueTime, dueTime);
            }
            dueTime = (this._config.ScanConsortiaInterval * 60) * 0x3e8;
            if (this.m_scanConsortia == null)
            {
                this.m_scanConsortia = new System.Threading.Timer(new TimerCallback(this.ScanConsortiaProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_scanConsortia.Change(dueTime, dueTime);
            }
            dueTime = 0xea60;
            if (this.m_worldEvent == null)
            {
                this.m_worldEvent = new System.Threading.Timer(new TimerCallback(this.ScanWorldEventProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_worldEvent.Change(dueTime, dueTime);
            }
            dueTime = 0xea60;
            if (this.m_consortiaboss == null)
            {
                this.m_consortiaboss = new System.Threading.Timer(new TimerCallback(this.ScanConsortiabossProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_consortiaboss.Change(dueTime, dueTime);
            }
            return true;
        }

        public void LoadConfig()
        {
            this._aSSState = bool.Parse(ConfigurationManager.AppSettings["AAS"]);
            this._dailyAwardState = bool.Parse(ConfigurationManager.AppSettings["DailyAwardState"]);
        }

        protected void LoginLapseTimerProc(object sender)
        {
            try
            {
                Player[] allPlayer = LoginMgr.GetAllPlayer();
                long ticks = DateTime.Now.Ticks;
                long num2 = (this._config.LoginLapseInterval * 10L) * 0x3e8L;
                foreach (Player player in allPlayer)
                {
                    if (player.State == ePlayerState.NotLogin)
                    {
                        if ((player.LastTime + num2) < ticks)
                        {
                            LoginMgr.RemovePlayer(player.Id);
                        }
                    }
                    else
                    {
                        player.LastTime = ticks;
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("LoginLapseTimer callback", exception);
                }
            }
        }

        public int NoticeServerUpdate(int serverId, int type)
        {
            ServerClient[] allClients = this.GetAllClients();
            if (allClients != null)
            {
                foreach (ServerClient client in allClients)
                {
                    if (client.Info.ID == serverId)
                    {
                        GSPacketIn pkg = new GSPacketIn(11);
                        pkg.WriteInt(type);
                        client.SendTCP(pkg);
                        return 0;
                    }
                }
            }
            return 1;
        }

        public int RateUpdate(int serverId)
        {
            ServerClient[] allClients = this.GetAllClients();
            if (allClients != null)
            {
                foreach (ServerClient client in allClients)
                {
                    if (client.Info.ID == serverId)
                    {
                        GSPacketIn pkg = new GSPacketIn(0xb1);
                        pkg.WriteInt(serverId);
                        client.SendTCP(pkg);
                        return 0;
                    }
                }
            }
            return 1;
        }

        public bool RecompileScripts()
        {
            string path = this._config.RootDirectory + Path.DirectorySeparatorChar + "scripts";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] strArray = this._config.ScriptAssemblies.Split(new char[] { ',' });
            return ScriptMgr.CompileScripts(false, path, this._config.ScriptCompilationTarget, strArray);
        }

        protected void SaveRecordProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                LogMgr.Save();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record complete!");
                }
                if (tickCount > 0x1d4c0)
                {
                    log.WarnFormat("Saved all Record  in {0} ms!", tickCount);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("SaveRecordProc", exception);
                }
            }
        }

        protected void SaveTimerProc(object state)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving database...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                ServerMgr.SaveToDatabase();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving database complete!");
                    log.Info("Saved all databases " + tickCount + "ms");
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("SaveTimerProc", exception);
                }
            }
        }

        protected void ScanAuctionProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                string noticeUserID = "";
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    bussiness.ScanAuction(ref noticeUserID);
                }
                foreach (string str2 in noticeUserID.Split(new char[] { ',' }))
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        GSPacketIn pkg = new GSPacketIn(0x75);
                        pkg.WriteInt(int.Parse(str2));
                        pkg.WriteInt(1);
                        this.SendToALL(pkg);
                    }
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan Auction complete!");
                }
                if (tickCount > 0x1d4c0)
                {
                    log.WarnFormat("Scan all Auction  in {0} ms", tickCount);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ScanAuctionProc", exception);
                }
            }
        }

        protected void ScanConsortiabossProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan Consortiaboss...");
                    log.Debug("Scan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                ConsortiaBossMgr.UpdateTime();
                ConsortiaBossMgr.TimeCheckingAward++;
                if (ConsortiaBossMgr.TimeCheckingAward > 5)
                {
                    List<int> allConsortiaGetAward = ConsortiaBossMgr.GetAllConsortiaGetAward();
                    GSPacketIn pkg = new GSPacketIn(0xb9);
                    pkg.WriteInt(allConsortiaGetAward.Count);
                    foreach (int num2 in allConsortiaGetAward)
                    {
                        pkg.WriteInt(num2);
                    }
                    this.SendToALL(pkg);
                    ConsortiaBossMgr.TimeCheckingAward = 0;
                    log.Info("Scan Consortiaboss award complete!");
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan Consortiaboss complete!");
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ScanConsortiabossProc", exception);
                }
            }
        }

        protected void ScanConsortiaProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                string noticeID = "";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    bussiness.ScanConsortia(ref noticeID);
                }
                foreach (string str2 in noticeID.Split(new char[] { ',' }))
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        GSPacketIn pkg = new GSPacketIn(0x80);
                        pkg.WriteByte(2);
                        pkg.WriteInt(int.Parse(str2));
                        this.SendToALL(pkg);
                    }
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan Consortia complete!");
                }
                if (tickCount > 0x1d4c0)
                {
                    log.WarnFormat("Scan all Consortia in {0} ms", tickCount);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ScanConsortiaProc", exception);
                }
            }
        }

        protected void ScanMailProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                string noticeUserID = "";
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    bussiness.ScanMail(ref noticeUserID);
                }
                foreach (string str2 in noticeUserID.Split(new char[] { ',' }))
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        GSPacketIn pkg = new GSPacketIn(0x75);
                        pkg.WriteInt(int.Parse(str2));
                        pkg.WriteInt(1);
                        this.SendToALL(pkg);
                    }
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan Mail complete!");
                }
                if (tickCount > 0x1d4c0)
                {
                    log.WarnFormat("Scan all Mail in {0} ms", tickCount);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ScanMailProc", exception);
                }
            }
        }

        protected void ScanWorldEventProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan  WorldEvent ...");
                    log.Debug("Scan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                this.SendUpdateWorldEvent();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan WorldEvent complete!");
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Scan WorldEvent Proc", exception);
                }
            }
        }

        public bool SendAAS(bool state)
        {
            if (StaticFunction.UpdateConfig("Center.Service.exe.config", "AAS", state.ToString()))
            {
                this.ASSState = state;
                GSPacketIn pkg = new GSPacketIn(7);
                pkg.WriteBoolean(state);
                this.SendToALL(pkg);
                return true;
            }
            return false;
        }

        public void SendBattleGoundOpenClose(bool value)
        {
            GSPacketIn pkg = new GSPacketIn(0x58);
            pkg.WriteBoolean(value);
            this.SendToALL(pkg);
        }

        public void SendConfigState()
        {
            GSPacketIn pkg = new GSPacketIn(8);
            pkg.WriteBoolean(this.ASSState);
            pkg.WriteBoolean(this.DailyAwardState);
            this.SendToALL(pkg);
        }

        public bool SendConfigState(int type, bool state)
        {
            string name = string.Empty;
            switch (type)
            {
                case 1:
                    name = "AAS";
                    break;

                case 2:
                    name = "DailyAwardState";
                    break;

                default:
                    return false;
            }
            if (!StaticFunction.UpdateConfig("Center.Service.exe.config", name, state.ToString()))
            {
                return false;
            }
            switch (type)
            {
                case 1:
                    this.ASSState = state;
                    break;

                case 2:
                    this.DailyAwardState = state;
                    break;
            }
            this.SendConfigState();
            return true;
        }

        public void SendConsortiaDelete(int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(5);
            pkg.WriteInt(consortiaID);
            this.SendToALL(pkg);
        }

        public void SendFightFootballTime(bool value)
        {
            GSPacketIn pkg = new GSPacketIn(0x59);
            pkg.WriteBoolean(value);
            this.SendToALL(pkg);
        }

        public void SendLeagueOpenClose(bool value)
        {
            GSPacketIn pkg = new GSPacketIn(0x57);
            pkg.WriteBoolean(value);
            this.SendToALL(pkg);
        }

        public void SendPrivateInfo()
        {
            int index = WorldMgr.currentPVE_ID;
            GSPacketIn pkg = new GSPacketIn(80);
            pkg.WriteLong(WorldMgr.MAX_BLOOD);
            pkg.WriteLong(WorldMgr.current_blood);
            pkg.WriteString(WorldMgr.name[index]);
            pkg.WriteString(WorldMgr.bossResourceId[index]);
            pkg.WriteInt(WorldMgr.Pve_Id[index]);
            pkg.WriteBoolean(WorldMgr.fightOver);
            pkg.WriteBoolean(WorldMgr.roomClose);
            pkg.WriteDateTime(WorldMgr.begin_time);
            pkg.WriteDateTime(WorldMgr.end_time);
            pkg.WriteInt(WorldMgr.fight_time);
            pkg.WriteBoolean(WorldMgr.worldOpen);
            this.SendToALL(pkg);
        }

        public void SendPrivateInfo(string name)
        {
            if (WorldMgr.CheckName(name))
            {
                GSPacketIn pkg = new GSPacketIn(0x55);
                RankingPersonInfo singleRank = WorldMgr.GetSingleRank(name);
                pkg.WriteString(name);
                pkg.WriteInt(singleRank.Damage);
                pkg.WriteInt(singleRank.Honor);
                this.SendToALL(pkg);
            }
        }

        public bool SendReload(eReloadType type)
        {
            return this.SendReload(type.ToString());
        }

        public bool SendReload(string str)
        {
            try
            {
                eReloadType type = (eReloadType) System.Enum.Parse(typeof(eReloadType), str, true);
                //switch (type)
                {
                    if (type == eReloadType.server)
                    {
                        this._config.Refresh();
                        this.InitGlobalTimers();
                        this.LoadConfig();
                        ServerMgr.ReLoadServerList();
                        this.SendConfigState();
                    }
                }
                GSPacketIn pkg = new GSPacketIn(11);
                pkg.WriteInt((int) type);
                this.SendToALL(pkg, null);
                return true;
            }
            catch (Exception exception)
            {
                log.Error("Order is not Exist!", exception);
            }
            return false;
        }

        public void SendRoomClose(byte type)
        {
            GSPacketIn pkg = new GSPacketIn(0x53);
            pkg.WriteByte(type);
            this.SendToALL(pkg);
        }

        public void SendShutdown()
        {
            GSPacketIn pkg = new GSPacketIn(15);
            this.SendToALL(pkg);
        }

        public void SendSystemNotice(string msg)
        {
            GSPacketIn pkg = new GSPacketIn(10);
            pkg.WriteInt(0);
            pkg.WriteString(msg);
            this.SendToALL(pkg, null);
        }

        public void SendToALL(GSPacketIn pkg)
        {
            this.SendToALL(pkg, null);
        }

        public void SendToALL(GSPacketIn pkg, ServerClient except)
        {
            ServerClient[] allClients = this.GetAllClients();
            if (allClients != null)
            {
                foreach (ServerClient client in allClients)
                {
                    if (client != except)
                    {
                        client.SendTCP(pkg);
                    }
                }
            }
        }

        public void SendUpdateRank(bool type)
        {
            List<RankingPersonInfo> list = WorldMgr.SelectTopTen();
            if (list.Count != 0)
            {
                GSPacketIn pkg = new GSPacketIn(0x51);
                pkg.WriteBoolean(type);
                pkg.WriteInt(list.Count);
                foreach (RankingPersonInfo info in list)
                {
                    pkg.WriteInt(info.ID);
                    pkg.WriteString(info.Name);
                    pkg.WriteInt(info.Damage);
                }
                this.SendToALL(pkg);
            }
        }

        public void SendUpdateWorldBlood()
        {
            GSPacketIn pkg = new GSPacketIn(0x4f);
            pkg.WriteLong(WorldMgr.MAX_BLOOD);
            pkg.WriteLong(WorldMgr.current_blood);
            this.SendToALL(pkg);
        }

        public void SendUpdateWorldEvent()
        {
            int hour = DateTime.Now.Hour;
            if (!((((hour != 0) && (hour != 10)) && (hour != 0x12)) || this.boss))
            {
                this.boss = true;
            }
            if (((hour >= 9) && (hour < 10)) || ((hour >= 0x12) && (hour < 0x13)))
            {
                if (!WorldMgr.IsBattleGoundOpen)
                {
                    this.SendBattleGoundOpenClose(true);
                    WorldMgr.IsBattleGoundOpen = true;
                    WorldMgr.BattleGoundOpenTime = DateTime.Now;
                    this.SendSystemNotice("Đấu trường đ\x00e3 mở. Nhanh ch\x00f3ng tham gia n\x00e0o c\x00e1c Gunner.");
                }
            }
            else if (WorldMgr.IsBattleGoundOpen)
            {
                this.SendBattleGoundOpenClose(false);
                WorldMgr.IsBattleGoundOpen = false;
                this.SendSystemNotice("Đấu trường h\x00f4m nay kết th\x00fac.");
            }
            if ((hour >= 20) && (hour < 0x16))
            {
                if (!WorldMgr.IsLeagueOpen)
                {
                    this.SendLeagueOpenClose(true);
                    WorldMgr.IsLeagueOpen = true;
                    WorldMgr.LeagueOpenTime = DateTime.Now;
                    this.SendSystemNotice("Chiến thần đ\x00e3 mở. Nhanh ch\x00f3ng tham gia n\x00e0o c\x00e1c Gunner.");
                }
            }
            else if (WorldMgr.IsLeagueOpen)
            {
                this.SendLeagueOpenClose(false);
                WorldMgr.IsLeagueOpen = false;
                this.SendSystemNotice("Chiến thần h\x00f4m nay kết th\x00fac.");
            }
            string[] strArray = GameProperties.FightFootballTime.Split(new char[] { '|' });
            if (WorldMgr.IsFightFootballTime)
            {
                int min = int.Parse(strArray[1]);
                if (!this.AvailTime(WorldMgr.FightFootballTime, min))
                {
                    this.SendFightFootballTime(false);
                    WorldMgr.IsFightFootballTime = false;
                    this.SendSystemNotice("Quyết chiến h\x00f4m nay kết th\x00fac.");
                }
            }
            else if (!(!(hour.ToString() == strArray[0]) || WorldMgr.IsFightFootballTime))
            {
                this.SendFightFootballTime(true);
                WorldMgr.IsFightFootballTime = true;
                WorldMgr.FightFootballTime = DateTime.Now;
                this.SendSystemNotice("Quyết chiến đ\x00e3 mở. Nhanh ch\x00f3ng tham gia n\x00e0o c\x00e1c Gunner.");
            }
            if (!WorldMgr.worldOpen)
            {
                if ((hour == 13) & this.boss)
                {
                    this.SendPrivateInfo();
                    WorldMgr.SetupWorldBoss(0);
                    this.SendSystemNotice(string.Format("Boss thế giới {0} đ\x00e3 mở, tham gia ngay!", WorldMgr.name[0]));
                    this.boss = true;
                }
                else if ((hour == 0x13) && this.boss)
                {
                    this.SendPrivateInfo();
                    WorldMgr.SetupWorldBoss(3);
                    this.SendSystemNotice(string.Format("Boss thế giới {0} đ\x00e3 mở, tham gia ngay", WorldMgr.name[3]));
                    this.boss = true;
                }
                else
                {
                    this.SendRoomClose(1);
                    this.SendRoomClose(0);
                }
            }
            if (WorldMgr.worldOpen)
            {
                if (((hour == 14) || (hour == 20)) || (WorldMgr.current_blood <= 0L))
                {
                    if (this.boss)
                    {
                        this.minute1 = DateTime.Now.AddMinutes(5.0);
                        int index = WorldMgr.currentPVE_ID;
                        this.SendSystemNotice(string.Format("Boss thế giới {0} đ\x00e3 kết th\x00fac. Ph\x00f2ng sẽ đ\x00f3ng sau {1} ph\x00fat.", WorldMgr.name[index], 5));
                        this.boss = false;
                    }
                    if (!WorldMgr.fightOver)
                    {
                        WorldMgr.WorldBossFightOver();
                        this.SendWorldBossFightOver();
                    }
                    if ((DateTime.Now >= this.minute1) && !WorldMgr.roomClose)
                    {
                        WorldMgr.WorldBossRoomClose();
                        WorldMgr.WorldBossClose();
                        this.SendRoomClose(0);
                        this.SendUpdateRank(true);
                    }
                }
                this.SendPrivateInfo();
                WorldMgr.UpdateFightTime();
            }
            int num4 = DateTime.Now.Hour;
            if ((num4 > this.awardTime) && WorldMgr.CanSendLightriddleAward)
            {
                WorldMgr.SendLightriddleTopEightAward();
            }
            if ((num4 > this.awardTime) && WorldMgr.CanSendLuckyStarAward)
            {
                WorldMgr.SendLuckyStarTopTenAward();
            }
            if ((num4 > 1) && (num4 < this.awardTime))
            {
                if (!WorldMgr.CanSendLightriddleAward)
                {
                    WorldMgr.CanSendLightriddleAward = true;
                    WorldMgr.ResetLightriddleRank();
                }
                if (!WorldMgr.CanSendLuckyStarAward)
                {
                    WorldMgr.CanSendLuckyStarAward = true;
                    WorldMgr.ResetLuckStar();
                }
            }
            WorldMgr.SaveLuckyStarRewardRecord();
        }

        public void SendWorldBossFightOver()
        {
            GSPacketIn pkg = new GSPacketIn(0x52);
            this.SendToALL(pkg);
        }

        public override bool Start()
        {
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
                GameProperties.Refresh();
                if (!this.InitComponent(this.RecompileScripts(), "Recompile Scripts"))
                {
                    return false;
                }
                if (!this.InitComponent(this.StartScriptComponents(), "Script components"))
                {
                    return false;
                }
                if (!this.InitComponent(GameProperties.EDITION == this.Edition, "Check Server Edition:" + this.Edition))
                {
                    return false;
                }
                if (!this.InitComponent(this.InitSocket(IPAddress.Parse(this._config.Ip), this._config.Port), "InitSocket Port:" + this._config.Port))
                {
                    return false;
                }
                if (!this.InitComponent(CenterService.Start(), "Center Service"))
                {
                    return false;
                }
                if (!this.InitComponent(ServerMgr.Start(), "Load serverlist"))
                {
                    return false;
                }
                //if (!this.InitComponent(ConsortiaExtraMgr.Init(), "Init ConsortiaLevelMgr"))
                //{
                //    return false;
                //}
                if (!this.InitComponent(MacroDropMgr.Init(), "Init MacroDropMgr"))
                {
                    return false;
                }
                if (!this.InitComponent(WorldEventMgr.Init(), "WorldEventMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(WorldMgr.Start(), "WorldMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(this.InitGlobalTimers(), "Init Global Timers"))
                {
                    return false;
                }
                GameEventMgr.Notify(ScriptEvent.Loaded);
                MacroDropMgr.Start();
                if (!this.InitComponent(base.Start(), "base.Start()"))
                {
                    return false;
                }
                GameEventMgr.Notify(GameServerEvent.Started, this);
                GC.Collect(GC.MaxGeneration);
                log.Info("GameServer is now open for connections!");
                GameProperties.Save();
                return true;
            }
            catch (Exception exception)
            {
                log.Error("Failed to start the server", exception);
                return false;
            }
        }

        protected bool StartScriptComponents()
        {
            try
            {
                ScriptMgr.InsertAssembly(typeof(CenterServer).Assembly);
                ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
                foreach (Assembly assembly in ScriptMgr.Scripts)
                {
                    GameEventMgr.RegisterGlobalEvents(assembly, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
                    GameEventMgr.RegisterGlobalEvents(assembly, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
                    GameEventMgr.RegisterGlobalEvents(assembly, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
                    GameEventMgr.RegisterGlobalEvents(assembly, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
                }
                log.Info("Registering global event handlers: true");
                return true;
            }
            catch (Exception exception)
            {
                log.Error("StartScriptComponents", exception);
                return false;
            }
        }

        public override void Stop()
        {
            this.DisposeGlobalTimers();
            this.SaveTimerProc(null);
            this.SaveRecordProc(null);
            CenterService.Stop();
            base.Stop();
        }

        protected void SystemNoticeTimerProc(object state)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("System Notice ...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                List<string> notceList = WorldMgr.NotceList;
                if (notceList.Count > 0)
                {
                    int num2 = this.rand.Next(notceList.Count);
                    Instance.SendSystemNotice(notceList[num2]);
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("System Notice complete!");
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("SystemNoticeTimerProc", exception);
                }
            }
        }

        public bool ASSState
        {
            get
            {
                return this._aSSState;
            }
            set
            {
                this._aSSState = value;
            }
        }

        public bool DailyAwardState
        {
            get
            {
                return this._dailyAwardState;
            }
            set
            {
                this._dailyAwardState = value;
            }
        }

        public static CenterServer Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}


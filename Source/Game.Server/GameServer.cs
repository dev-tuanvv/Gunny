namespace Game.Server
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base;
    using Game.Base.Events;
    using Game.Logic;
    using Game.Server.Battle;
    using Game.Server.GameObjects;
    using Game.Server.Games;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using Game.Server.RingStation;
    using Game.Server.Rooms;
    using Game.Server.Statics;
    using log4net;
    using log4net.Config;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    public class GameServer : BaseServer
    {
        private LoginServerConnector _loginServer;
        private int _shutdownCount = 6;
        private System.Threading.Timer _shutdownTimer;
        private const int BUF_SIZE = 0x2000;
        public static readonly string Edition = "2612558";
        public static bool KeepRunning = false;
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected System.Threading.Timer m_bagMailScanTimer;
        protected System.Threading.Timer m_buffScanTimer;
        private static bool m_compiled = false;
        private GameServerConfig m_config;
        private bool m_debugMenory;
        private static GameServer m_instance = null;
        private bool m_isRunning;
        private System.Collections.Queue m_packetBufPool;
        protected System.Threading.Timer m_pingCheckTimer;
        protected System.Threading.Timer m_qqTipScanTimer;
        protected System.Threading.Timer m_saveDbTimer;
        protected System.Threading.Timer m_saveRecordTimer;
        private static int m_tryCount = 4;

        protected GameServer(GameServerConfig config)
        {
            this.m_config = config;
            if (log.IsDebugEnabled)
            {
                log.Debug("Current directory is: " + Directory.GetCurrentDirectory());
                log.Debug("Gameserver root directory is: " + this.Configuration.RootDirectory);
                log.Debug("Changing directory to root directory");
            }
            Directory.SetCurrentDirectory(this.Configuration.RootDirectory);
        }

        public byte[] AcquirePacketBuffer()
        {
            lock (this.m_packetBufPool.SyncRoot)
            {
                if (this.m_packetBufPool.Count > 0)
                {
                    return (byte[]) this.m_packetBufPool.Dequeue();
                }
            }
            log.Warn("packet buffer pool is empty!");
            return new byte[0x2000];
        }

        private bool AllocatePacketBuffers()
        {
            int capacity = this.Configuration.MaxClientCount * 3;
            this.m_packetBufPool = new System.Collections.Queue(capacity);
            for (int i = 0; i < capacity; i++)
            {
                this.m_packetBufPool.Enqueue(new byte[0x2000]);
            }
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("allocated packet buffers: {0}", capacity.ToString());
            }
            return true;
        }

        protected void BagMailScanTimerProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("BagMail Scaning ...");
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                WorldMgr.ScanBagMail();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("BagMail Scan complete!");
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("BagMailScanTimerProc", exception);
                }
            }
        }

        protected void BuffScanTimerProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Buff Scaning ...");
                    log.Debug("BuffScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                int num2 = 0;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                {
                    if (player.BufferList != null)
                    {
                        player.BufferList.Update();
                        num2++;
                    }
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Buff Scan complete!");
                    log.Info(string.Concat(new object[] { "Buff all ", num2, " players in ", tickCount, "ms" }));
                }
                if (tickCount > 0x1d4c0)
                {
                    log.WarnFormat("Scan all Buff and {0} players in {1} ms", num2, tickCount);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("BuffScanTimerProc", exception);
                }
            }
        }

        public static void CreateInstance(GameServerConfig config)
        {
            if (m_instance == null)
            {
                FileInfo configFile = new FileInfo(config.LogConfigFile);
                if (!configFile.Exists)
                {
                    ResourceUtil.ExtractResource(configFile.Name, configFile.FullName, Assembly.GetAssembly(typeof(GameServer)));
                }
                XmlConfigurator.ConfigureAndWatch(configFile);
                m_instance = new GameServer(config);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
                if (e.IsTerminating)
                {
                    this.Stop();
                }
            }
            catch
            {
                try
                {
                    using (FileStream stream = new FileStream(@"c:\testme.log", FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                        {
                            writer.WriteLine(e.ExceptionObject);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public GameClient[] GetAllClients()
        {
            GameClient[] array = null;
            lock (base._clients.SyncRoot)
            {
                array = new GameClient[base._clients.Count];
                base._clients.Keys.CopyTo(array, 0);
            }
            return array;
        }

        protected override BaseClient GetNewClient()
        {
            return new GameClient(this, this.AcquirePacketBuffer(), this.AcquirePacketBuffer());
        }

        protected bool InitComponent(bool componentInitState, string text)
        {
            if (this.m_debugMenory)
            {
                log.Debug(string.Concat(new object[] { "Start Memory ", text, ": ", (GC.GetTotalMemory(false) / 0x400L) / 0x400L }));
            }
            if (log.IsInfoEnabled)
            {
                log.Info(text + ": " + componentInitState);
            }
            if (!componentInitState)
            {
                this.Stop();
            }
            if (this.m_debugMenory)
            {
                log.Debug(string.Concat(new object[] { "Finish Memory ", text, ": ", (GC.GetTotalMemory(false) / 0x400L) / 0x400L }));
            }
            return componentInitState;
        }

        public bool InitGlobalTimer()
        {
            int dueTime = (this.Configuration.DBSaveInterval * 60) * 0x3e8;
            if (this.m_saveDbTimer == null)
            {
                this.m_saveDbTimer = new System.Threading.Timer(new TimerCallback(this.SaveTimerProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_saveDbTimer.Change(dueTime, dueTime);
            }
            dueTime = (this.Configuration.PingCheckInterval * 60) * 0x3e8;
            if (this.m_pingCheckTimer == null)
            {
                this.m_pingCheckTimer = new System.Threading.Timer(new TimerCallback(this.PingCheck), null, dueTime, dueTime);
            }
            else
            {
                this.m_pingCheckTimer.Change(dueTime, dueTime);
            }
            dueTime = (this.Configuration.SaveRecordInterval * 60) * 0x3e8;
            if (this.m_saveRecordTimer != null)
            {
                this.m_saveRecordTimer.Change(dueTime, dueTime);
            }
            dueTime = 0xea60;
            if (this.m_buffScanTimer == null)
            {
                this.m_buffScanTimer = new System.Threading.Timer(new TimerCallback(this.BuffScanTimerProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_buffScanTimer.Change(dueTime, dueTime);
            }
            dueTime = 0x493e0;
            if (this.m_qqTipScanTimer == null)
            {
                this.m_qqTipScanTimer = new System.Threading.Timer(new TimerCallback(this.QQTipScanTimerProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_qqTipScanTimer.Change(dueTime, dueTime);
            }
            dueTime = 0xdbba0;
            if (this.m_bagMailScanTimer == null)
            {
                this.m_bagMailScanTimer = new System.Threading.Timer(new TimerCallback(this.BagMailScanTimerProc), null, dueTime, dueTime);
            }
            else
            {
                this.m_bagMailScanTimer.Change(dueTime, dueTime);
            }
            return true;
        }

        private bool InitLoginServer()
        {
            this._loginServer = new LoginServerConnector(this.m_config.LoginServerIp, this.m_config.LoginServerPort, this.m_config.ServerID, this.m_config.ServerName, this.AcquirePacketBuffer(), this.AcquirePacketBuffer());
            this._loginServer.Disconnected += new ClientEventHandle(this.loginServer_Disconnected);
            return this._loginServer.Connect();
        }

        private void loginServer_Disconnected(BaseClient client)
        {
            bool isRunning = this.m_isRunning;
            this.Stop();
            if (isRunning && (m_tryCount > 0))
            {
                m_tryCount--;
                log.Error("Center Server Disconnect! Stopping Server");
                log.ErrorFormat("Start the game server again after 1 second,and left try times:{0}", m_tryCount);
                Thread.Sleep(0x3e8);
                if (this.Start())
                {
                    log.Error("Restart the game server success!");
                }
            }
            else
            {
                if (m_tryCount == 0)
                {
                    log.ErrorFormat("Restart the game server failed after {0} times.", 4);
                    log.Error("Server Stopped!");
                }
                LogManager.Shutdown();
            }
        }

        protected void PingCheck(object sender)
        {
            try
            {
                log.Info("Begin ping check....");
                long num = (((this.Configuration.PingCheckInterval * 60L) * 0x3e8L) * 0x3e8L) * 10L;
                GameClient[] allClients = this.GetAllClients();
                if (allClients != null)
                {
                    foreach (GameClient client in allClients)
                    {
                        if (client != null)
                        {
                            if (client.IsConnected)
                            {
                                if (client.Player != null)
                                {
                                    client.Out.SendPingTime(client.Player);
                                    if (AntiAddictionMgr.ISASSon && (AntiAddictionMgr.count == 0))
                                    {
                                        AntiAddictionMgr.count++;
                                    }
                                }
                                else if ((client.PingTime + num) < DateTime.Now.Ticks)
                                {
                                    client.Disconnect();
                                }
                            }
                            else
                            {
                                client.Disconnect();
                            }
                        }
                    }
                }
                log.Info("End ping check....");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("PingCheck callback", exception);
                }
            }
            try
            {
                log.Info("Begin ping center check....");
                Instance.LoginServer.SendPingCenter();
                log.Info("End ping center check....");
            }
            catch (Exception exception2)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("PingCheck center callback", exception2);
                }
            }
        }

        protected void QQTipScanTimerProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("QQTips Scaning ...");
                }
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                QQtipsMessagesInfo qQtipsMessages = QQTipsMgr.GetQQtipsMessages();
                foreach (GamePlayer player in WorldMgr.GetAllPlayersNoGame())
                {
                    player.Out.SendQQtips(player.PlayerId, qQtipsMessages);
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("QQTips Scan complete!");
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("QQTipScanTimerProc", exception);
                }
            }
        }

        public bool RecompileScripts()
        {
            if (!m_compiled)
            {
                string path = this.Configuration.RootDirectory + Path.DirectorySeparatorChar + "scripts";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string[] strArray = this.Configuration.ScriptAssemblies.Split(new char[] { ',' });
                m_compiled = ScriptMgr.CompileScripts(false, path, this.Configuration.ScriptCompilationTarget, strArray);
            }
            return m_compiled;
        }

        public void ReleasePacketBuffer(byte[] buf)
        {
            if ((buf != null) && (GC.GetGeneration(buf) >= GC.MaxGeneration))
            {
                lock (this.m_packetBufPool.SyncRoot)
                {
                    this.m_packetBufPool.Enqueue(buf);
                }
            }
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
                    log.WarnFormat("Saved all Record  in {0} ms", tickCount);
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

        protected void SaveTimerProc(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving database...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                int num2 = 0;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                {
                    player.SaveNewsItemIntoDatabase();
                    num2++;
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving New Item to database complete!");
                    log.Info(string.Concat(new object[] { "Saved all databases and ", num2, " players in ", tickCount, "ms" }));
                }
                if (tickCount > 0x1d4c0)
                {
                    log.WarnFormat("Saved all databases and {0} players in {1} ms", num2, tickCount);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("SaveTimerProc", exception);
                }
            }
            finally
            {
                GameEventMgr.Notify(GameServerEvent.WorldSave);
            }
        }

        public void Shutdown()
        {
            Instance.LoginServer.SendShutdown(true);
            this._shutdownTimer = new System.Threading.Timer(new TimerCallback(this.ShutDownCallBack), null, 0, 0xea60);
        }

        private void ShutDownCallBack(object state)
        {
            try
            {
                this._shutdownCount--;
                Console.WriteLine(string.Format("Server will shutdown after {0} mins!", this._shutdownCount));
                foreach (GameClient client in Instance.GetAllClients())
                {
                    if (client.Out != null)
                    {
                        client.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1", new object[0]), this._shutdownCount, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2", new object[0])));
                    }
                }
                if (this._shutdownCount == 0)
                {
                    Console.WriteLine("Server has stopped!");
                    Instance.LoginServer.SendShutdown(false);
                    this._shutdownTimer.Dispose();
                    this._shutdownTimer = null;
                    Instance.Stop();
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
            }
        }

        public override bool Start()
        {
            bool flag;
            if (this.m_isRunning)
            {
                return false;
            }
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                GameProperties.Refresh();
                if (!this.InitComponent(this.RecompileScripts(), "Recompile Scripts"))
                {
                    flag = false;
                }
                if (!this.InitComponent(ConsortiaLevelMgr.Init(), "ConsortiaLevelMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(this.StartScriptComponents(), "Script components"))
                {
                    return false;
                }
                if (!this.InitComponent(GameProperties.EDITION == Edition, "Edition: " + Edition))
                {
                    return false;
                }
                if (!this.InitComponent(this.InitSocket(IPAddress.Parse("103.226.249.12"), this.Configuration.Port), "InitSocket Port: " + this.Configuration.Port))
                {
                    return false;
                }
                if (!this.InitComponent(this.AllocatePacketBuffers(), "AllocatePacketBuffers()"))
                {
                    return false;
                }
                if (!this.InitComponent(LogMgr.Setup(this.Configuration.GAME_TYPE, this.Configuration.ServerID, this.Configuration.AreaID), "LogMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(WorldMgr.Init(), "WorldMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(MapMgr.Init(), "MapMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(ItemMgr.Init(), "ItemMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(ItemBoxMgr.Init(), "ItemBox Init"))
                {
                    return false;
                }
                if (!this.InitComponent(BallMgr.Init(), "BallMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(ExerciseMgr.Init(), "ExerciseMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(LevelMgr.Init(), "levelMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(BallConfigMgr.Init(), "BallConfigMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(FusionMgr.Init(), "FusionMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(UserBoxMgr.Init(), "UserBoxMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(AwardMgr.Init(), "AwardMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(AchievementMgr.Init(), "AchievementMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(MissionInfoMgr.Init(), "MissionInfoMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(MissionEnergyMgr.Init(), "MissionEnergyInfoMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(PveInfoMgr.Init(), "PveInfoMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(DropMgr.Init(), "Drop Init"))
                {
                    return false;
                }
                if (!this.InitComponent(FightRateMgr.Init(), "FightRateMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(RefineryMgr.Init(), "RefineryMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(StrengthenMgr.Init(), "StrengthenMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(PropItemMgr.Init(), "PropItemMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(ShopMgr.Init(), "ShopMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(QuestMgr.Init(), "QuestMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(RoomMgr.Setup(this.Configuration.MaxRoomCount), "RoomMgr.Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(GameMgr.Setup(this.Configuration.ServerID, GameProperties.BOX_APPEAR_CONDITION), "GameMgr.Start()"))
                {
                    return false;
                }
                if (!this.InitComponent(ConsortiaMgr.Init(), "ConsortiaMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(ConsortiaExtraMgr.Init(), "ConsortiaExtraMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(RateMgr.Init(this.Configuration), "ExperienceRateMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(WindMgr.Init(), "WindMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(CardMgr.Init(), "CardMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(PetMgr.Init(), "PetMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(GoldEquipMgr.Init(), "GoldEquipMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(RuneMgr.Init(), "RuneMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(TotemMgr.Init(), "TotemMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(TotemHonorMgr.Init(), "TotemHonorMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(TreasureAwardMgr.Init(), "TreasureAwardMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(FairBattleRewardMgr.Init(), "FairBattleRewardMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(FightSpiritTemplateMgr.Init(), "FightSpiritTemplateMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(MacroDropMgr.Init(), "MacroDropMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(MarryRoomMgr.Init(), "MarryRoomMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(RankMgr.Init(), "RankMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(CommunalActiveMgr.Init(), "CommunalActiveMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(ActiveSystemMgr.Init(), "ActiveSystemMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(QQTipsMgr.Init(), "QQTipsMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(LightriddleQuestMgr.Init(), "LightriddleQuestMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(WorldEventMgr.Init(), "WorldEventMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(SubActiveMgr.Init(), "SubActiveMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(DiceLevelAwardMgr.Init(), "DiceLevelMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(EventAwardMgr.Init(), "EventAwardMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(BattleMgr.Setup(), "BattleMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(ClothPropertyTemplateInfoMgr.Init(), "ClothPropertyTemplateInfoMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(ClothGroupTemplateInfoMgr.Init(), "ClothGroupTemplateInfoMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(MagicStoneTemplateMgr.Init(), "MagicStoneTemplateMgr Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(this.InitGlobalTimer(), "Init Global Timers"))
                {
                    return false;
                }
                if (!this.InitComponent(LogMgr.Setup(1, 4, 4), "LogMgr Setup"))
                {
                    return false;
                }
                GameEventMgr.Notify(ScriptEvent.Loaded);
                if (!this.InitComponent(this.InitLoginServer(), "Login To CenterServer"))
                {
                    return false;
                }
                if (!this.InitComponent(HotSpringMgr.Init(), "HotSpringMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(RingStationMgr.Init(), " AutoBot Int"))
                {
                    return false;
                }
                RoomMgr.Start();
                GameMgr.Start();
                BattleMgr.Start();
                MacroDropMgr.Start();
                if (!this.InitComponent(base.Start(), "base.Start()"))
                {
                    flag = false;
                }
                else
                {
                    GameEventMgr.Notify(GameServerEvent.Started, this);
                    GC.Collect(GC.MaxGeneration);
                    if (log.IsInfoEnabled)
                    {
                        log.Info("GameServer is now open for connections!");
                    }
                    this.m_isRunning = true;
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Failed to start the server", exception);
                }
                flag = false;
            }
            return flag;
        }

        protected bool StartScriptComponents()
        {
            try
            {
                if (log.IsInfoEnabled)
                {
                    log.Info("Server rules: true");
                }
                ScriptMgr.InsertAssembly(typeof(GameServer).Assembly);
                ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
                ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
                ArrayList list = new ArrayList(ScriptMgr.Scripts);
                foreach (Assembly assembly in list)
                {
                    GameEventMgr.RegisterGlobalEvents(assembly, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
                    GameEventMgr.RegisterGlobalEvents(assembly, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
                    GameEventMgr.RegisterGlobalEvents(assembly, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
                    GameEventMgr.RegisterGlobalEvents(assembly, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
                }
                if (log.IsInfoEnabled)
                {
                    log.Info("Registering global event handlers: true");
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("StartScriptComponents", exception);
                }
                return false;
            }
            return true;
        }

        public override void Stop()
        {
            if (this.m_isRunning)
            {
                this.m_isRunning = false;
                if (!MarryRoomMgr.UpdateBreakTimeWhereServerStop())
                {
                    Console.WriteLine("Update BreakTime failed");
                }
                RoomMgr.Stop();
                GameMgr.Stop();
                WorldMgr.ScanBagMail();
                ActiveSystemMgr.StopAllTimer();
                if (this._loginServer != null)
                {
                    this._loginServer.Disconnected -= new ClientEventHandle(this.loginServer_Disconnected);
                    this._loginServer.Disconnect();
                }
                if (this.m_pingCheckTimer != null)
                {
                    this.m_pingCheckTimer.Change(-1, -1);
                    this.m_pingCheckTimer.Dispose();
                    this.m_pingCheckTimer = null;
                }
                if (this.m_saveDbTimer != null)
                {
                    this.m_saveDbTimer.Change(-1, -1);
                    this.m_saveDbTimer.Dispose();
                    this.m_saveDbTimer = null;
                }
                if (this.m_saveRecordTimer != null)
                {
                    this.m_saveRecordTimer.Change(-1, -1);
                    this.m_saveRecordTimer.Dispose();
                    this.m_saveRecordTimer = null;
                }
                if (this.m_buffScanTimer != null)
                {
                    this.m_buffScanTimer.Change(-1, -1);
                    this.m_buffScanTimer.Dispose();
                    this.m_buffScanTimer = null;
                }
                if (this.m_qqTipScanTimer != null)
                {
                    this.m_qqTipScanTimer.Change(-1, -1);
                    this.m_qqTipScanTimer.Dispose();
                    this.m_qqTipScanTimer = null;
                }
                if (this.m_bagMailScanTimer != null)
                {
                    this.m_bagMailScanTimer.Change(-1, -1);
                    this.m_bagMailScanTimer.Dispose();
                    this.m_bagMailScanTimer = null;
                }
                base.Stop();
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                log.Info("Server Stopped!");
                Console.WriteLine("Server Stopped!");
            }
        }

        public GameServerConfig Configuration
        {
            get
            {
                return this.m_config;
            }
        }

        public static GameServer Instance
        {
            get
            {
                return m_instance;
            }
        }

        public LoginServerConnector LoginServer
        {
            get
            {
                return this._loginServer;
            }
        }

        public int PacketPoolSize
        {
            get
            {
                return this.m_packetBufPool.Count;
            }
        }
    }
}


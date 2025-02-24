namespace Fighting.Server
{
    using Bussiness;
    using Bussiness.Managers;
    using Fighting.Server.Games;
    using Fighting.Server.Rooms;
    using Game.Base;
    using Game.Base.Events;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server.Managers;
    using log4net;
    using log4net.Config;
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading;

    public class FightServer : BaseServer
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static bool m_compiled = false;
        private FightServerConfig m_config;
        private static FightServer m_instance;
        private bool m_running;

        private FightServer(FightServerConfig config)
        {
            this.m_config = config;
        }

        public static void CreateInstance(FightServerConfig config)
        {
            if (m_instance == null)
            {
                FileInfo configFile = new FileInfo(config.LogConfigFile);
                if (!configFile.Exists)
                {
                    ResourceUtil.ExtractResource(configFile.Name, configFile.FullName, Assembly.GetAssembly(typeof(FightServer)));
                }
                XmlConfigurator.ConfigureAndWatch(configFile);
                m_instance = new FightServer(config);
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

        public override bool Start()
        {
            bool flag;
            if (this.m_running)
            {
                return false;
            }
            try
            {
                this.m_running = true;
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
                if (!this.InitComponent(this.InitSocket(IPAddress.Parse(this.m_config.Ip), this.m_config.Port), "InitSocket Port:" + this.m_config.Port))
                {
                    return false;
                }
                if (!this.InitComponent(this.RecompileScripts(), "Recompile Scripts"))
                {
                    return false;
                }
                if (!this.InitComponent(this.StartScriptComponents(), "Script components"))
                {
                    return false;
                }
                if (!this.InitComponent(ProxyRoomMgr.Setup(), "RoomMgr.Setup"))
                {
                    return false;
                }
                if (!this.InitComponent(GameMgr.Setup(0, 4), "GameMgr.Setup"))
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
                if (!this.InitComponent(PropItemMgr.Init(), "PropItemMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(BallMgr.Init(), "BallMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(BallConfigMgr.Init(), "BallConfigMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(DropMgr.Init(), "DropMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(WindMgr.Init(), "WindMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(GoldEquipMgr.Init(), "GoldEquipMgr Init"))
                {
                    return false;
                }
                if (!this.InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init"))
                {
                    return false;
                }
                GameEventMgr.Notify(ScriptEvent.Loaded);
                if (!this.InitComponent(base.Start(), "base.Start()"))
                {
                    flag = false;
                }
                else
                {
                    ProxyRoomMgr.Start();
                    GameMgr.Start();
                    GameEventMgr.Notify(GameServerEvent.Started, this);
                    GC.Collect(GC.MaxGeneration);
                    log.Info("GameServer is now open for connections!");
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                log.Error("Failed to start the server", exception);
                flag = false;
            }
            return flag;
        }

        protected bool StartScriptComponents()
        {
            try
            {
                ScriptMgr.InsertAssembly(typeof(FightServer).Assembly);
                ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
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
            if (this.m_running)
            {
                try
                {
                    this.m_running = false;
                    GameMgr.Stop();
                    ProxyRoomMgr.Stop();
                }
                catch (Exception exception)
                {
                    log.Error("Server stopp error:", exception);
                }
                finally
                {
                    base.Stop();
                }
            }
        }

        public FightServerConfig Configuration
        {
            get
            {
                return this.m_config;
            }
        }

        public static FightServer Instance
        {
            get
            {
                return m_instance;
            }
        }
    }
}


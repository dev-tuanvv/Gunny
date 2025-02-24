namespace Fighting.Server
{
    using Game.Base.Config;
    using System;
    using System.IO;
    using System.Reflection;

    public class FightServerConfig : BaseAppConfig
    {
        [ConfigProperty("IP", "频道的IP", "127.0.0.1")]
        public string Ip;
        public string LogConfigFile = "logconfig.xml";
        [ConfigProperty("Port", "频道开放端口", 0x23f8)]
        public int Port;
        public string RootDirectory;
        [ConfigProperty("ScriptAssemblies", "脚本编译引用库", "")]
        public string ScriptAssemblies;
        [ConfigProperty("ScriptCompilationTarget", "脚本编译目标名称", "")]
        public string ScriptCompilationTarget;
        [ConfigProperty("ServerName", "频道的名称", "7Road")]
        public string ServerName;
        [ConfigProperty("ZoneId", "服务器编号", 4)]
        public int ZoneId;

        protected override void Load(Type type)
        {
            if (Assembly.GetEntryAssembly() != null)
            {
                this.RootDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            }
            else
            {
                this.RootDirectory = new FileInfo(Assembly.GetAssembly(typeof(FightServer)).Location).DirectoryName;
            }
            base.Load(type);
        }

        public void LoadConfiguration()
        {
            this.Load(typeof(FightServerConfig));
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}


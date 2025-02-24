namespace Game.Server
{
    using Game.Base;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using log4net;
    using System;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class GameClient : BaseClient
    {
        protected GameServer _srvr;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public int Lottery;
        protected IPacketLib m_packetLib;
        protected Game.Base.Packets.PacketProcessor m_packetProcessor;
        protected long m_pingTime;
        protected GamePlayer m_player;
        private static readonly byte[] POLICY = Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><!DOCTYPE cross-domain-policy SYSTEM \"http://www.adobe.com/xml/dtds/cross-domain-policy.dtd\"><cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>\0");
        public string tempData;
        public int Version;

        public GameClient(GameServer svr, byte[] read, byte[] send) : base(read, send)
        {
            this.m_pingTime = DateTime.Now.Ticks;
            this.Lottery = -1;
            this.tempData = string.Empty;
            this.m_pingTime = DateTime.Now.Ticks;
            this._srvr = svr;
            this.m_player = null;
            base.Encryted = true;
            base.AsyncPostSend = true;
        }

        public override void Disconnect()
        {
            base.Disconnect();
        }

        public override void DisplayMessage(string msg)
        {
            base.DisplayMessage(msg);
        }

        protected override void OnConnect()
        {
            base.OnConnect();
            this.m_pingTime = DateTime.Now.Ticks;
        }

        protected override void OnDisconnect()
        {
            try
            {
                GamePlayer player = Interlocked.Exchange<GamePlayer>(ref this.m_player, null);
                if (player != null)
                {
                    player.FightBag.ClearBag();
                    LoginMgr.ClearLoginPlayer(player.PlayerCharacter.ID, this);
                    player.Quit();
                }
                byte[] sendBuffer = base.m_sendBuffer;
                base.m_sendBuffer = null;
                this._srvr.ReleasePacketBuffer(sendBuffer);
                sendBuffer = base.m_readBuffer;
                base.m_readBuffer = null;
                this._srvr.ReleasePacketBuffer(sendBuffer);
                base.OnDisconnect();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("OnDisconnect", exception);
                }
            }
        }

        public override void OnRecv(int num_bytes)
        {
            if (this.m_packetProcessor != null)
            {
                base.OnRecv(num_bytes);
            }
            else if (base.m_readBuffer[0] == 60)
            {
                base.m_sock.Send(POLICY);
            }
            else
            {
                base.OnRecv(num_bytes);
            }
            this.m_pingTime = DateTime.Now.Ticks;
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            if (this.m_packetProcessor == null)
            {
                this.m_packetLib = AbstractPacketLib.CreatePacketLibForVersion(1, this);
                this.m_packetProcessor = new Game.Base.Packets.PacketProcessor(this);
            }
            if (this.m_player != null)
            {
                pkg.ClientID = this.m_player.PlayerId;
                pkg.WriteHeader();
            }
            this.m_packetProcessor.HandlePacket(pkg);
        }

        public override void SendTCP(GSPacketIn pkg)
        {
            base.SendTCP(pkg);
        }

        public override string ToString()
        {
            return new StringBuilder(0x80).Append(" pakLib:").Append((this.Out == null) ? "(null)" : this.Out.GetType().FullName).Append(" IP:").Append(base.TcpEndpoint).Append(" char:").Append((this.Player == null) ? "null" : this.Player.PlayerCharacter.NickName).ToString();
        }

        public IPacketLib Out
        {
            get
            {
                return this.m_packetLib;
            }
            set
            {
                this.m_packetLib = value;
            }
        }

        public Game.Base.Packets.PacketProcessor PacketProcessor
        {
            get
            {
                return this.m_packetProcessor;
            }
        }

        public long PingTime
        {
            get
            {
                return this.m_pingTime;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
            set
            {
                GamePlayer player = Interlocked.Exchange<GamePlayer>(ref this.m_player, value);
                if (player != null)
                {
                    player.Quit();
                }
            }
        }

        public GameServer Server
        {
            get
            {
                return this._srvr;
            }
        }
    }
}


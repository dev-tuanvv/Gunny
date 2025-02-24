namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using log4net;
    using System;

    [PacketHandler(8, "客户端日记")]
    public class ClientErrorLog : IPacketHandler
    {
        public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string message = "Client log: " + packet.ReadString();
            log.Error(message);
            return 0;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x2d, "场景用户离开")]
    public class QequestBadLuckHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ReadBoolean();
            return 0;
        }
    }
}


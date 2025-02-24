namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xd7, "场景用户离开")]
    public class CaddyConvertedHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ReadBoolean();
            packet.ReadInt();
            packet.ReadInt();
            return 0;
        }
    }
}


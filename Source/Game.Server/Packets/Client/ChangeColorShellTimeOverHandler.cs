namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xce, "场景用户离开")]
    public class ChangeColorShellTimeOverHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ReadByte();
            packet.ReadInt();
            return 0;
        }
    }
}


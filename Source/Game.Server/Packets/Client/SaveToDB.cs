namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xac, "场景用户离开")]
    public class SaveToDB : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            return 0;
        }
    }
}


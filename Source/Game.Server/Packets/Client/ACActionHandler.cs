namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [Obsolete("已经不用"), PacketHandler(0x23, "user ac action")]
    public class ACActionHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            return 1;
        }
    }
}


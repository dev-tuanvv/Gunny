namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x4b, "删除道具")]
    public class PropDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int place = packet.ReadInt();
            client.Player.FightBag.RemoveItemAt(place);
            return 0;
        }
    }
}


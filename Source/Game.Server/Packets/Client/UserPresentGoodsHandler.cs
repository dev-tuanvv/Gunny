namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;
    using System.Text;

    [PacketHandler(0x39, "购买物品")]
    public class UserPresentGoodsHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            new StringBuilder();
            packet.ReadString();
            packet.ReadString();
            packet.ReadInt();
            client.Player.SendMessage("T\x00ednh năng n\x00e0y tạm kh\x00f3a");
            return 0;
        }
    }
}


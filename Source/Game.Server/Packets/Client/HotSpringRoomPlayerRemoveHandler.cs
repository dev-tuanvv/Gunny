namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xa9, "礼堂数据")]
    public class HotSpringRoomPlayerRemoveHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentHotSpringRoom != null)
            {
                client.Player.CurrentHotSpringRoom.RemovePlayer(client.Player);
                GSPacketIn pkg = new GSPacketIn(0xa9);
                pkg.WriteString("Đ\x00e3 tho\x00e1t khỏi ph\x00f2ng suối nước n\x00f3ng");
                client.SendTCP(pkg);
            }
            return 0;
        }
    }
}


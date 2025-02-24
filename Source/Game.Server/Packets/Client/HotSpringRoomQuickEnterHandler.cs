namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.HotSpringRooms;
    using Game.Server.Managers;
    using System;

    [PacketHandler(190, "礼堂数据")]
    public class HotSpringRoomQuickEnterHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentHotSpringRoom == null)
            {
                HotSpringRoom randomRoom = HotSpringMgr.GetRandomRoom();
                if (randomRoom != null)
                {
                    if (randomRoom.AddPlayer(client.Player))
                    {
                        client.Out.SendEnterHotSpringRoom(client.Player);
                    }
                }
                else
                {
                    client.Player.SendMessage("Hiện kh\x00f4ng c\x00f3 ph\x00f2ng n\x00e0o khả dụng.");
                }
            }
            else
            {
                client.Player.SendMessage("Bạn đang ở trong ph\x00f2ng kh\x00e1c.");
            }
            return 0;
        }
    }
}


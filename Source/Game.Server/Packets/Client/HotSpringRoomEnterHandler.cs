namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.HotSpringRooms;
    using Game.Server.Managers;
    using System;

    [PacketHandler(0xca, "礼堂数据")]
    public class HotSpringRoomEnterHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            packet.ReadString();
            if (client.Player.CurrentHotSpringRoom == null)
            {
                HotSpringRoom hotSpringRoombyID = HotSpringMgr.GetHotSpringRoombyID(id);
                if (hotSpringRoombyID != null)
                {
                    int num2 = int.Parse(HotSpringMgr.HotSpringEnterPriRoom[0]);
                    if (client.Player.PlayerCharacter.Gold >= num2)
                    {
                        if (hotSpringRoombyID.AddPlayer(client.Player) && (client.Player.RemoveGold(num2) > 0))
                        {
                            client.Out.SendEnterHotSpringRoom(client.Player);
                        }
                    }
                    else
                    {
                        client.Player.SendMessage("V\x00e0ng của bạn kh\x00f4ng đủ.");
                    }
                }
                else
                {
                    client.Player.SendMessage("Ph\x00f2ng bạn chọn đ\x00e3 đầy!");
                }
            }
            return 0;
        }
    }
}


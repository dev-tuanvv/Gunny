namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xbc, "场景用户离开")]
    public class UseConsortiaReworkNameHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            byte num2 = packet.ReadByte();
            int slot = packet.ReadInt();
            string newNickName = packet.ReadString();
            string msg = "";
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                PlayerInventory inventory = client.Player.GetInventory((eBageType) num2);
                SqlDataProvider.Data.ItemInfo itemAt = inventory.GetItemAt(slot);
                if (itemAt.Count < 1)
                {
                    client.Player.SendMessage("Vật phẩm kh\x00f4ng tồn tại");
                    return 0;
                }
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaInfo consortiaSingle = bussiness.GetConsortiaSingle(id);
                    if (consortiaSingle == null)
                    {
                        client.Player.SendMessage("Guild Kh\x00f4ng tồn tại.");
                        return 0;
                    }
                    if (client.Player.PlayerCharacter.ID != consortiaSingle.ChairmanID)
                    {
                        client.Player.SendMessage("Chủ Guild mới c\x00f3 thể đổi t\x00ean.");
                        return 0;
                    }
                    if (bussiness.RenameConsortia(id, client.Player.PlayerCharacter.NickName, newNickName))
                    {
                        inventory.RemoveCountFromStack(itemAt, 1);
                    }
                    else
                    {
                        msg = "T\x00ean Guild đ\x00e3 được sử dụng.";
                    }
                }
                if (msg != "")
                {
                    client.Player.SendMessage(msg);
                }
            }
            return 0;
        }
    }
}


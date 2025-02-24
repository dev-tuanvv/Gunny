namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xed, "更新征婚信息")]
    public class PlayerDressHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num2;
            byte num = packet.ReadByte();
            switch (num)
            {
                case 1:
                {
                    num2 = packet.ReadInt();
                    int num3 = packet.ReadInt();
                    if (((num3 >= 0) && (num2 >= 0)) && (num2 <= 2))
                    {
                        client.Player.DressModel.ClearDressInSlot(num2);
                        for (int i = 0; i < num3; i++)
                        {
                            int num5 = packet.ReadInt();
                            int slot = packet.ReadInt();
                            SqlDataProvider.Data.ItemInfo itemAt = null;
                            PlayerInventory inventory = client.Player.GetInventory((eBageType) num5);
                            if (inventory != null)
                            {
                                itemAt = inventory.GetItemAt(slot);
                            }
                            if (itemAt != null)
                            {
                                UserDressModelInfo dress = new UserDressModelInfo {
                                    UserID = client.Player.PlayerCharacter.ID,
                                    ItemID = itemAt.ItemID,
                                    CategoryID = itemAt.Template.CategoryID,
                                    SlotID = num2,
                                    TemplateID = itemAt.TemplateID
                                };
                                client.Player.DressModel.AddDressModel(dress);
                            }
                            else
                            {
                                client.Player.SendMessage("Vật phẩm lưu kh\x00f4ng tồn tại. Tho\x00e1t t\x00fai [B] v\x00e0 thử lại");
                            }
                        }
                        client.Out.SendDressModelInfo(client.Player.DressModel);
                        return 0;
                    }
                    client.Player.SendMessage("Xảy ra lỗi khi kiểm tra dữ liệu client. F5 Website v\x00e0 thử lại");
                    return 0;
                }
                case 2:
                    num2 = packet.ReadInt();
                    if ((num2 < 0) || (num2 >= 3))
                    {
                        client.Player.SendMessage("Kh\x00f4ng tồn tại kiểu n\x00e0y. Tho\x00e1t t\x00fai [B] v\x00e0 thử lại");
                        return 0;
                    }
                    client.Player.PlayerCharacter.CurrentDressModel = num2;
                    client.Out.SendCurrentDressModel(client.Player.PlayerCharacter);
                    return 0;

                case 6:
                {
                    int num7 = packet.ReadInt();
                    for (int j = 0; j < num7; j++)
                    {
                        int num9 = packet.ReadInt();
                        int num10 = packet.ReadInt();
                        int num11 = packet.ReadInt();
                        int num12 = packet.ReadInt();
                        PlayerInventory inventory2 = client.Player.GetInventory((eBageType) num9);
                        PlayerInventory inventory3 = client.Player.GetInventory((eBageType) num11);
                        if ((inventory2 == null) || (inventory3 == null))
                        {
                            return 0;
                        }
                        SqlDataProvider.Data.ItemInfo item = inventory2.GetItemAt(num10);
                        SqlDataProvider.Data.ItemInfo info4 = inventory3.GetItemAt(num12);
                        if ((((item != null) && (info4 != null)) && (item.TemplateID == info4.TemplateID)) && (item != info4))
                        {
                            if (info4.ValidDate != 0)
                            {
                                if (item.ValidDate != 0)
                                {
                                    info4.ValidDate += item.ValidDate;
                                }
                                else
                                {
                                    info4.ValidDate = item.ValidDate;
                                }
                            }
                            if (item.IsUsed)
                            {
                                info4.IsUsed = true;
                            }
                            inventory3.UpdateItem(info4);
                            inventory2.RemoveItem(item);
                        }
                    }
                    return 0;
                }
                case 7:
                    return 0;
            }
            Console.WriteLine("playerdress cmd: " + num);
            return 0;
        }

        private void SendCurrentModel(GameClient client, GSPacketIn packet)
        {
            new GSPacketIn(0xed).WriteByte(2);
        }
    }
}


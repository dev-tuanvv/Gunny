namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x6a, "场景用户离开")]
    public class WishBeadEquipHandler : IPacketHandler
    {
        public static ThreadSafeRandom random = new ThreadSafeRandom();

        private bool CanWishBeat(int beatID, int CategoryID)
        {
            return ((((beatID == 0x2d28) && (CategoryID == 7)) || ((beatID == 0x2d29) && (CategoryID == 5))) || ((beatID == 0x2d2a) && (CategoryID == 1)));
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int slot = packet.ReadInt();
            int num2 = packet.ReadInt();
            int templateId = packet.ReadInt();
            int place = packet.ReadInt();
            int num5 = packet.ReadInt();
            int num6 = packet.ReadInt();
            int itemCount = client.Player.PropBag.GetItemCount(0x2d28);
            int num8 = client.Player.PropBag.GetItemCount(0x2d2a);
            int num9 = client.Player.PropBag.GetItemCount(0x2d29);
            int num10 = client.Player.PropBag.GetItemCount(0x70);
            int num11 = client.Player.PropBag.GetItemCount(0x65);
            GSPacketIn @in = new GSPacketIn(0x6a, client.Player.PlayerCharacter.ID);
            PlayerInventory inventory = client.Player.GetInventory((eBageType) num2);
            SqlDataProvider.Data.ItemInfo itemAt = inventory.GetItemAt(slot);
            SqlDataProvider.Data.ItemInfo info2 = client.Player.GetItemAt((eBageType) num5, place);
            if ((info2 != null) && (itemAt != null))
            {
                if ((info2.Count >= 1) && (info2.TemplateID == num6))
                {
                    GSPacketIn in2;
                    int num14;
                    if (!this.CanWishBeat(info2.TemplateID, itemAt.Template.CategoryID))
                    {
                        @in.WriteInt(5);
                        client.Out.SendTCP(@in);
                        return 0;
                    }
                    double num12 = 5.0;
                    double num13 = 10.0;
                    GoldEquipTemplateLoadInfo info3 = GoldEquipMgr.FindGoldEquipNewTemplate(templateId);
                    itemAt.IsBinds = true;
                    if ((((itemAt.StrengthenLevel >= 15) && (itemAt.StrengthenLevel < 0x13)) && (num10 > 0)) && (num11 > 0))
                    {
                        in2 = new GSPacketIn(0x8a, client.Player.PlayerCharacter.ID);
                        num14 = (itemAt.Template.Property2 < 10) ? 10 : itemAt.Template.Property2;
                        itemAt.StrengthenLevel++;
                        inventory.UpdateItem(itemAt);
                        inventory.SaveToDatabase();
                        in2.WriteByte(0);
                        in2.WriteInt(num14);
                        client.Out.SendTCP(in2);
                        client.Out.SendMessage(eMessageType.Normal, "Th\x00e0nh C\x00f4ng!, Trang Bị tăng l\x00ean cấp " + (itemAt.StrengthenLevel - 12));
                        GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation2", new object[] { client.Player.PlayerCharacter.NickName, "@", itemAt.StrengthenLevel - 12 }), itemAt.ItemID, itemAt.TemplateID, "", client.Player.ZoneId));
                        @in.WriteInt(0);
                        client.Out.SendTCP(@in);
                        client.Player.RemoveTemplate(0x70, 1);
                        client.Player.RemoveTemplate(0x65, 1);
                        client.Player.RemoveTemplate(num6, 1);
                        return 0;
                    }
                    if (((itemAt.StrengthenLevel >= 15) && (itemAt.StrengthenLevel < 0x13)) && (num10 > 0))
                    {
                        if (num13 > random.Next(100))
                        {
                            in2 = new GSPacketIn(0x8a, client.Player.PlayerCharacter.ID);
                            num14 = (itemAt.Template.Property2 < 10) ? 10 : itemAt.Template.Property2;
                            itemAt.StrengthenLevel++;
                            inventory.UpdateItem(itemAt);
                            inventory.SaveToDatabase();
                            in2.WriteByte(0);
                            in2.WriteInt(num14);
                            client.Out.SendTCP(in2);
                            client.Out.SendMessage(eMessageType.Normal, "Th\x00e0nh C\x00f4ng!, Trang Bị tăng l\x00ean cấp " + (itemAt.StrengthenLevel - 12));
                            GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation2", new object[] { client.Player.PlayerCharacter.NickName, "@", itemAt.StrengthenLevel - 12 }), itemAt.ItemID, itemAt.TemplateID, "", client.Player.ZoneId));
                            @in.WriteInt(0);
                            client.Out.SendTCP(@in);
                        }
                        else
                        {
                            @in.WriteInt(1);
                            client.Out.SendTCP(@in);
                        }
                        client.Player.RemoveTemplate(0x70, 1);
                        client.Player.RemoveTemplate(num6, 1);
                        return 0;
                    }
                    if ((itemAt.StrengthenLevel > GameProperties.WishBeadLimitLv) && GameProperties.IsWishBeadLimit)
                    {
                        @in.WriteInt(5);
                    }
                    else
                    {
                        if ((((itemAt.Template.CategoryID == 7) && (itemCount <= 0)) || ((itemAt.Template.CategoryID == 1) && (num8 <= 0))) || ((itemAt.Template.CategoryID == 5) && (num9 <= 0)))
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Ch\x00e2u b\x00e1u ch\x00fac ph\x00fac kh\x00f4ng đủ.");
                            @in.WriteInt(5);
                            client.Out.SendTCP(@in);
                            return 0;
                        }
                        if (!itemAt.IsGold)
                        {
                            if (num12 > random.Next(100))
                            {
                                itemAt.goldBeginTime = DateTime.Now;
                                itemAt.goldValidDate = 30;
                                itemAt.IsBinds = true;
                                if ((info3 != null) && (itemAt.Template.CategoryID == 7))
                                {
                                    ItemTemplateInfo info4 = ItemMgr.FindItemTemplate(info3.NewTemplateId);
                                    if (info4 != null)
                                    {
                                        itemAt.GoldEquip = info4;
                                    }
                                }
                                inventory.UpdateItem(itemAt);
                                @in.WriteInt(0);
                                inventory.SaveToDatabase();
                            }
                            else
                            {
                                @in.WriteInt(1);
                            }
                            client.Player.RemoveTemplate(num6, 1);
                        }
                        else
                        {
                            @in.WriteInt(6);
                        }
                    }
                    client.Out.SendTCP(@in);
                    return 0;
                }
                client.Out.SendMessage(eMessageType.Normal, "Ch\x00e2u b\x00e1u ch\x00fac ph\x00fac kh\x00f4ng đủ.");
                @in.WriteInt(5);
                client.Out.SendTCP(@in);
                return 0;
            }
            client.Out.SendMessage(eMessageType.Normal, "lỗi vật phẩm.");
            @in.WriteInt(5);
            client.Out.SendTCP(@in);
            return 0;
        }
    }
}


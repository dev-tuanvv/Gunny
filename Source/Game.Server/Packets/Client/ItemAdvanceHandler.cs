namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Text;

    [PacketHandler(0x8a, "物品强化")]
    public class ItemAdvanceHandler : IPacketHandler
    {
        public static ThreadSafeRandom random = new ThreadSafeRandom();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            packet.ReadBoolean();
            packet.ReadBoolean();
            GSPacketIn @in = new GSPacketIn(0x8a, client.Player.PlayerCharacter.ID);
            SqlDataProvider.Data.ItemInfo itemAt = client.Player.StoreBag.GetItemAt(0);
            SqlDataProvider.Data.ItemInfo item = client.Player.StoreBag.GetItemAt(1);
            int strengthenLevel = item.StrengthenLevel;
            if (((itemAt == null) || (item == null)) || (itemAt.Count <= 0))
            {
                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Đặt đ\x00e1 tăng cấp v\x00e0 trang bị cần tăng cấp v\x00e0o!", new object[0]));
                return 0;
            }
            if (strengthenLevel >= 15)
            {
                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Level đ\x00e3 đạt cấp độ cao nhất, kh\x00f4ng thể thăng cấp!", new object[0]));
                return 0;
            }
            int count = 1;
            string str = "";
            if ((((item != null) && item.Template.CanStrengthen) && (item.Template.CategoryID < 0x12)) && (item.Count == 1))
            {
                flag = flag || item.IsBinds;
                builder.Append(string.Concat(new object[] { item.ItemID, ":", item.TemplateID, "," }));
                if (itemAt.TemplateID != 0x2b8e)
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Đặt đ\x00e1 tăng cấp v\x00e0o!", new object[0]));
                    return 0;
                }
                flag = flag || itemAt.IsBinds;
                string str2 = str;
                str = str2 + "," + itemAt.ItemID.ToString() + ":" + itemAt.Template.Name;
                int val = (itemAt.Template.Property2 < 10) ? 10 : itemAt.Template.Property2;
                builder.Append("true");
                bool flag2 = false;
                int num4 = random.Next(0x4e20);
                double num5 = item.StrengthenExp / strengthenLevel;
                if (num5 > num4)
                {
                    item.IsBinds = flag;
                    item.StrengthenLevel++;
                    item.StrengthenExp = 0;
                    @in.WriteByte(0);
                    @in.WriteInt(val);
                    flag2 = true;
                    StrengthenGoodsInfo info3 = StrengthenMgr.FindStrengthenGoodsInfo(item.StrengthenLevel, item.TemplateID);
                    if (((info3 != null) && (item.Template.CategoryID == 7)) && (info3.GainEquip > item.TemplateID))
                    {
                        ItemTemplateInfo goods = ItemMgr.FindItemTemplate(info3.GainEquip);
                        if (goods != null)
                        {
                            SqlDataProvider.Data.ItemInfo info5 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(goods, item);
                            client.Player.StoreBag.RemoveItemAt(1);
                            client.Player.StoreBag.AddItemTo(info5, 1);
                            item = info5;
                        }
                    }
                }
                else
                {
                    item.StrengthenExp += val;
                    @in.WriteByte(1);
                    @in.WriteInt(val);
                }
                client.Player.StoreBag.RemoveCountFromStack(itemAt, count);
                client.Player.StoreBag.UpdateItem(item);
                client.Out.SendTCP(@in);
                if (flag2 && (item.ItemID > 0))
                {
                    string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation2", new object[] { client.Player.PlayerCharacter.NickName, "@", item.StrengthenLevel - 12 });
                    GSPacketIn in2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, item.ItemID, item.TemplateID, "", client.Player.ZoneId);
                    GameServer.Instance.LoginServer.SendPacket(in2);
                }
                builder.Append(item.StrengthenLevel);
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Content1", new object[0]) + itemAt.Template.Name + LanguageMgr.GetTranslation("ItemStrengthenHandler.Content2", new object[0]));
            }
            if (item.Place < 0x1f)
            {
                client.Player.EquipBag.UpdatePlayerProperties();
            }
            return 0;
        }
    }
}


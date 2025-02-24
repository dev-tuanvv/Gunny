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
    using System.Collections.Generic;
    using System.Text;

    [PacketHandler(0x3b, "物品强化")]
    public class ItemStrengthenHandler : IPacketHandler
    {
        public static int countConnect = 0;
        private static Random random_0 = new Random();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (countConnect >= 0xbb8)
            {
                client.Disconnect();
                return 0;
            }
            GSPacketIn @in = packet.Clone();
            @in.ClearContext();
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            int num = GameProperties.PRICE_STRENGHTN_GOLD;
            if (client.Player.PlayerCharacter.Gold < num)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.NoMoney", new object[0]));
                return 0;
            }
            bool flag2 = packet.ReadBoolean();
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataProvider.Data.ItemInfo itemAt = client.Player.StoreBag.GetItemAt(5);
            SqlDataProvider.Data.ItemInfo item = null;
            SqlDataProvider.Data.ItemInfo info3 = null;
            string str = "";
            if (((itemAt != null) && itemAt.Template.CanStrengthen) && (itemAt.Count == 1))
            {
                StrengthenInfo info4;
                if (itemAt.StrengthenLevel > 8)
                {
                    client.Player.SendMessage("Cường h\x00f3a đ\x00e3 đạt cấp tối đa, h\x00e3y tiếp tục luyện h\x00f3a trang bị!");
                    return 0;
                }
                flag = flag || itemAt.IsBinds;
                builder.Append(string.Concat(new object[] { itemAt.ItemID, ":", itemAt.TemplateID, "," }));
                int num2 = 1;
                double num3 = 0.0;
                bool flag3 = false;
                StrengthenGoodsInfo info5 = null;
                if (itemAt.Template.RefineryLevel > 0)
                {
                    info4 = StrengthenMgr.FindRefineryStrengthenInfo(itemAt.StrengthenLevel + 1);
                }
                else
                {
                    info5 = StrengthenMgr.FindStrengthenGoodsInfo(itemAt.StrengthenLevel, itemAt.TemplateID);
                    info4 = StrengthenMgr.FindStrengthenInfo(itemAt.StrengthenLevel + 1);
                }
                if (info4 == null)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.NoStrength", new object[0]));
                    return 0;
                }
                if (client.Player.StoreBag.GetItemAt(3) != null)
                {
                    info3 = client.Player.StoreBag.GetItemAt(3);
                    str = str + "," + info3.ItemID.ToString() + ":" + info3.Template.Name;
                    if (((info3 != null) && (info3.Template.CategoryID == 11)) && (info3.Template.Property1 == 7))
                    {
                        flag = flag || info3.IsBinds;
                        builder.Append(string.Concat(new object[] { info3.ItemID, ":", info3.TemplateID, "," }));
                        flag3 = true;
                    }
                    else
                    {
                        info3 = null;
                    }
                }
                SqlDataProvider.Data.ItemInfo info6 = client.Player.StoreBag.GetItemAt(0);
                if (!((((info6 == null) || (info6.Template.CategoryID != 11)) || ((info6.Template.Property1 != 2) && (info6.Template.Property1 != 0x23))) || list.Contains(info6)))
                {
                    flag = flag || info6.IsBinds;
                    str = str + "," + info6.ItemID.ToString() + ":" + info6.Template.Name;
                    list.Add(info6);
                    num3 += info6.Template.Property2;
                }
                SqlDataProvider.Data.ItemInfo info7 = client.Player.StoreBag.GetItemAt(1);
                if (!((((info7 == null) || (info7.Template.CategoryID != 11)) || ((info7.Template.Property1 != 2) && (info7.Template.Property1 != 0x23))) || list.Contains(info7)))
                {
                    flag = flag || info7.IsBinds;
                    str = str + "," + info7.ItemID.ToString() + ":" + info7.Template.Name;
                    list.Add(info7);
                    num3 += info7.Template.Property2;
                }
                SqlDataProvider.Data.ItemInfo info8 = client.Player.StoreBag.GetItemAt(2);
                if (!((((info8 == null) || (info8.Template.CategoryID != 11)) || ((info8.Template.Property1 != 2) && (info8.Template.Property1 != 0x23))) || list.Contains(info8)))
                {
                    flag = flag || info8.IsBinds;
                    str = string.Concat(new object[] { str, ",", info8.ItemID, ":", info8.Template.Name });
                    list.Add(info8);
                    num3 += info8.Template.Property2;
                }
                foreach (SqlDataProvider.Data.ItemInfo info9 in list)
                {
                    if (info9.Template.Property1 == 0x23)
                    {
                        int categoryID = info9.Template.CategoryID;
                    }
                }
                double num5 = 0.0;
                double num6 = 0.0;
                if (client.Player.StoreBag.GetItemAt(4) != null)
                {
                    item = client.Player.StoreBag.GetItemAt(4);
                    str = str + "," + item.ItemID.ToString() + ":" + item.Template.Name;
                    if (((item != null) && (item.Template.CategoryID == 11)) && (item.Template.Property1 == 3))
                    {
                        flag = flag || item.IsBinds;
                        builder.Append(string.Concat(new object[] { item.ItemID, ":", item.TemplateID, "," }));
                        num5 = item.Template.Property2;
                    }
                    else
                    {
                        item = null;
                    }
                }
                if ((num5 <= 0.0) && (num6 <= 0.0))
                {
                    num3 *= 100.0;
                }
                else
                {
                    num3 *= (num5 + num6) + 100.0;
                }
                bool flag4 = false;
                ConsortiaInfo info10 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                if (flag2)
                {
                    ConsortiaEquipControlInfo info11 = new ConsortiaBussiness().GetConsortiaEuqipRiches(client.Player.PlayerCharacter.ConsortiaID, 0, 2);
                    if (info10 == null)
                    {
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Fail", new object[0]));
                    }
                    else
                    {
                        if (client.Player.PlayerCharacter.Riches < info11.Riches)
                        {
                            client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemStrengthenHandler.FailbyPermission", new object[0]));
                            return 1;
                        }
                        flag4 = true;
                    }
                }
                if (list.Count >= 1)
                {
                    num3 /= (double) info4.Rock;
                    for (int i = 0; i < list.Count; i++)
                    {
                        builder.Append(string.Concat(new object[] { list[i].ItemID, ":", list[i].TemplateID, "," }));
                        SqlDataProvider.Data.ItemInfo info12 = list[i];
                        info12.Count--;
                        client.Player.GetItemInventory(list[i].Template).UpdateItem(list[i]);
                    }
                    if (item != null)
                    {
                        client.Player.GetItemInventory(item.Template).RemoveItem(item);
                    }
                    if (info3 != null)
                    {
                        client.Player.GetItemInventory(info3.Template).RemoveItem(info3);
                    }
                    if (flag4)
                    {
                        num3 *= 1.0 + (0.1 * info10.SmithLevel);
                    }
                    itemAt.IsBinds = (flag || !itemAt.IsBinds) ? flag : true;
                    client.Player.StoreBag.ClearBag();
                    if (num3 > random_0.Next(0x2710))
                    {
                        builder.Append("true");
                        @in.WriteByte(0);
                        if (info5 != null)
                        {
                            ItemTemplateInfo goods = ItemMgr.FindItemTemplate(info5.GainEquip);
                            if (goods != null)
                            {
                                SqlDataProvider.Data.ItemInfo info14 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x74);
                                info14.StrengthenLevel = itemAt.StrengthenLevel + 1;
                                StrengthenMgr.InheritProperty(itemAt, ref info14);
                                client.Player.RemoveItem(itemAt);
                                client.Player.AddTemplate(info14, info14.Template.BagType, info14.Count, eGameView.OtherTypeGet);
                                itemAt = info14;
                                @in.WriteBoolean(false);
                            }
                        }
                        else
                        {
                            @in.WriteBoolean(true);
                            itemAt.StrengthenLevel++;
                            ItemTemplateInfo realWeaponTemplate = StrengthenMgr.GetRealWeaponTemplate(itemAt);
                            if (realWeaponTemplate != null)
                            {
                                SqlDataProvider.Data.ItemInfo info16 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(realWeaponTemplate, itemAt);
                                client.Player.StoreBag.RemoveItem(itemAt);
                                client.Player.StoreBag.AddItemTo(info16, 5);
                            }
                            else
                            {
                                client.Player.StoreBag.AddItemTo(itemAt, 5);
                            }
                        }
                        client.Player.OnItemStrengthen(itemAt.Template.CategoryID, itemAt.StrengthenLevel);
                        if (itemAt.StrengthenLevel >= 5)
                        {
                            object[] args = new object[] { client.Player.PlayerCharacter.NickName, "@", itemAt.StrengthenLevel };
                            string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation", args);
                            GSPacketIn in2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemAt.ItemID, itemAt.TemplateID, "", 4);
                            GameServer.Instance.LoginServer.SendPacket(in2);
                        }
                        if ((itemAt.Template.CategoryID == 7) && client.Player.Extra.CheckNoviceActiveOpen(NoviceActiveType.STRENGTHEN_WEAPON_ACTIVE))
                        {
                            client.Player.Extra.UpdateEventCondition(2, itemAt.StrengthenLevel);
                        }
                    }
                    else
                    {
                        builder.Append("false");
                        @in.WriteByte(1);
                        @in.WriteBoolean(false);
                        if (!flag3)
                        {
                            if (itemAt.Template.Level == 3)
                            {
                                itemAt.StrengthenLevel = (itemAt.StrengthenLevel < 5) ? itemAt.StrengthenLevel : (itemAt.StrengthenLevel - 1);
                                ItemTemplateInfo info17 = StrengthenMgr.GetRealWeaponTemplate(itemAt);
                                if (info17 != null)
                                {
                                    SqlDataProvider.Data.ItemInfo info18 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info17, itemAt);
                                    client.Player.StoreBag.RemoveItem(itemAt);
                                    client.Player.StoreBag.AddItemTo(info18, 5);
                                }
                                else
                                {
                                    client.Player.StoreBag.AddItemTo(itemAt, 5);
                                }
                            }
                            else
                            {
                                itemAt.Count--;
                                client.Player.StoreBag.AddItemTo(itemAt, 5);
                            }
                        }
                        else
                        {
                            client.Player.StoreBag.AddItemTo(itemAt, 5);
                        }
                    }
                    client.Out.SendTCP(@in);
                    builder.Append(itemAt.StrengthenLevel);
                    client.Player.BeginChanges();
                    client.Player.RemoveGold(num);
                    client.Player.CommitChanges();
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Content1", new object[0]) + num2 + LanguageMgr.GetTranslation("ItemStrengthenHandler.Content2", new object[0]));
                }
                if (itemAt.Place < 0x1f)
                {
                    client.Player.EquipBag.UpdatePlayerProperties();
                }
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Success", new object[0]));
            }
            return 0;
        }
    }
}


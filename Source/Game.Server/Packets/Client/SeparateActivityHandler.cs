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

    [PacketHandler(110, "场景用户离开")]
    public class SeparateActivityHandler : IPacketHandler
    {
        private List<int> list_0;

        public SeparateActivityHandler()
        {
            List<int> list = new List<int> { 7, 9, 0x10, 0x11, 0x13 };
            this.list_0 = list;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            SqlDataProvider.Data.ItemInfo itemAt;
            SqlDataProvider.Data.ItemInfo info2;
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();
            int count = 0;
            byte num2 = packet.ReadByte();
            switch (num2)
            {
                case 1:
                {
                    StrengthenGoodsInfo info3;
                    ItemTemplateInfo info4;
                    SqlDataProvider.Data.ItemInfo info5;
                    count = packet.ReadInt();
                    itemAt = client.Player.StoreBag.GetItemAt(0);
                    info2 = client.Player.StoreBag.GetItemAt(1);
                    if (!((((count > 0) && (itemAt != null)) && ((info2 != null) && (info2.Count >= count))) && this.list_0.Contains(itemAt.Template.CategoryID)))
                    {
                        client.Player.SendMessage("Vật phẩm n\x00e0y kh\x00f4ng thể luyện h\x00f3a!");
                        return 0;
                    }
                    if (!itemAt.IsBinds)
                    {
                        itemAt.IsBinds = info2.IsBinds;
                    }
                    if (itemAt.Template.CategoryID == 9)
                    {
                        if (!(StrengthenMgr.RingRefineryTemplate.Contains(itemAt.TemplateID) && (info2.TemplateID == 0x2b29)))
                        {
                            client.Player.SendMessage("Kh\x00f4ng thể luyện h\x00f3a!");
                            return 0;
                        }
                        if (itemAt.LianGrade >= GameProperties.RingMaxRefineryLevel)
                        {
                            client.Player.SendMessage("Đ\x00e3 luyện h\x00f3a đến cấp tối đa!");
                        }
                        else
                        {
                            int num6 = info2.Template.Property2 * count;
                            if ((num6 + itemAt.LianExp) > this.method_2())
                            {
                                num6 = this.method_2() - itemAt.LianExp;
                                count = num6 / info2.Template.Property2;
                            }
                            itemAt.LianExp += num6;
                            itemAt.LianGrade = this.method_0(itemAt.LianExp);
                            int templateId = StrengthenMgr.RingRefineryTemplate[itemAt.LianGrade];
                            if (templateId != itemAt.TemplateID)
                            {
                                info4 = ItemMgr.FindItemTemplate(templateId);
                                if (info4 != null)
                                {
                                    SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info4, itemAt);
                                    item.LianExp = itemAt.LianExp;
                                    item.LianGrade = itemAt.LianGrade;
                                    client.Player.StoreBag.RemoveItemAt(0);
                                    client.Player.StoreBag.AddItemTo(item, 0);
                                }
                            }
                            client.Player.StoreBag.RemoveCountFromStack(info2, count);
                            client.Player.StoreBag.UpdateItem(itemAt);
                            pkg.WriteByte(0);
                            client.Player.SendTCP(pkg);
                            if (itemAt.LianGrade < itemAt.LianGrade)
                            {
                                object[] args = new object[] { client.Player.PlayerCharacter.NickName, "@", itemAt.LianGrade };
                                string translation = LanguageMgr.GetTranslation("ItemRefineryRingHandler.congratulation", args);
                                GSPacketIn in3 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemAt.ItemID, itemAt.TemplateID, "", 4);
                                GameServer.Instance.LoginServer.SendPacket(in3);
                            }
                        }
                        goto Label_08BF;
                    }
                    if ((info2.TemplateID != 0x30e06) || (itemAt.StrengthenLevel < 9))
                    {
                        client.Player.SendMessage("Kh\x00f4ng thể luyện h\x00f3a!");
                        return 0;
                    }
                    if (itemAt.LianGrade >= 5)
                    {
                        client.Player.SendMessage("Đ\x00e3 luyện h\x00f3a đến cấp tối đa!");
                        goto Label_08BF;
                    }
                    int num3 = info2.Template.Property2 * count;
                    if ((num3 + itemAt.LianExp) > this.method_3())
                    {
                        num3 = this.method_3() - itemAt.LianExp;
                        count = num3 / info2.Template.Property2;
                    }
                    if ((itemAt.LianGrade == 0) && (num3 > 500))
                    {
                        num3 = 500;
                        count = num3 / info2.Template.Property2;
                    }
                    itemAt.LianExp += num3;
                    itemAt.LianGrade = this.method_1(itemAt.LianExp);
                    if (((itemAt.LianGrade < 1) || (itemAt.LianGrade >= 2)) || (itemAt.Template.CategoryID != 7))
                    {
                        if (((itemAt.LianGrade >= 2) && (itemAt.LianGrade < 5)) && (itemAt.Template.CategoryID == 7))
                        {
                            info3 = StrengthenMgr.FindStrengthenGoodsInfo(11, itemAt.TemplateID);
                            if (info3 != null)
                            {
                                info4 = ItemMgr.FindItemTemplate(info3.GainEquip);
                                if (info4 != null)
                                {
                                    info5 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info4, itemAt);
                                    info5.LianExp = itemAt.LianExp;
                                    info5.LianGrade = itemAt.LianGrade;
                                    itemAt = info5;
                                    client.Player.StoreBag.RemoveItemAt(0);
                                    client.Player.StoreBag.AddItemTo(itemAt, 0);
                                }
                            }
                            else
                            {
                                client.Player.StoreBag.UpdateItem(itemAt);
                            }
                        }
                        else
                        {
                            int num4;
                            SqlDataProvider.Data.ItemInfo info7;
                            int num5;
                            if ((itemAt.LianGrade == 5) && (itemAt.Template.CategoryID == 7))
                            {
                                info3 = StrengthenMgr.FindStrengthenGoodsInfo(14, itemAt.TemplateID);
                                if (info3 != null)
                                {
                                    info4 = ItemMgr.FindItemTemplate(info3.GainEquip);
                                    if (info4 != null)
                                    {
                                        info5 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info4, itemAt);
                                        num4 = (itemAt.LianExp - 0x1b67a) / info2.Template.Property2;
                                        info7 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(0x30e06), num4, 0x74);
                                        info5.LianExp = 0x1b67a;
                                        info5.LianGrade = itemAt.LianGrade;
                                        itemAt = info5;
                                        client.Player.StoreBag.RemoveItemAt(0);
                                        client.Player.StoreBag.AddItemTo(itemAt, 0);
                                        num5 = client.Player.PropBag.FindFirstEmptySlot();
                                        client.Player.PropBag.AddItemTo(info7, num5);
                                    }
                                }
                                client.Player.StoreBag.UpdateItem(itemAt);
                            }
                            else
                            {
                                if (itemAt.LianGrade == 5)
                                {
                                    num4 = (itemAt.LianExp - 0x1b67a) / info2.Template.Property2;
                                    info7 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(0x30e06), num4, 0x74);
                                    num5 = client.Player.PropBag.FindFirstEmptySlot();
                                    client.Player.PropBag.AddItemTo(info7, num5);
                                    itemAt.LianExp = 0x1b67a;
                                }
                                client.Player.StoreBag.UpdateItem(itemAt);
                            }
                        }
                        break;
                    }
                    info3 = StrengthenMgr.FindStrengthenGoodsInfo(itemAt.StrengthenLevel, itemAt.TemplateID);
                    if (info3 == null)
                    {
                        client.Player.StoreBag.UpdateItem(itemAt);
                        break;
                    }
                    info4 = ItemMgr.FindItemTemplate(info3.GainEquip);
                    if (info4 != null)
                    {
                        info5 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info4, itemAt);
                        info5.LianExp = itemAt.LianExp;
                        info5.LianGrade = itemAt.LianGrade;
                        itemAt = info5;
                        client.Player.StoreBag.RemoveItemAt(0);
                        client.Player.StoreBag.AddItemTo(itemAt, 0);
                    }
                    break;
                }
                case 6:
                    pkg = new GSPacketIn(110);
                    pkg.WriteByte(6);
                    pkg.WriteInt(client.Player.Extra.Info.coupleBossHurt);
                    pkg.WriteInt(client.Player.Extra.Info.coupleBossBoxNum);
                    pkg.WriteInt(client.Player.Extra.Info.coupleBossEnterNum);
                    client.Player.SendTCP(pkg);
                    goto Label_08BF;

                default:
                    if (num2 == 7)
                    {
                        packet.ReadBoolean();
                    }
                    goto Label_08BF;
            }
            client.Player.StoreBag.RemoveCountFromStack(info2, count);
            pkg.WriteByte(0);
            client.Out.SendTCP(pkg);
            if (itemAt.LianGrade < itemAt.LianGrade)
            {
                object[] objArray = new object[] { client.Player.PlayerCharacter.NickName, "@", itemAt.LianGrade };
                string msg = LanguageMgr.GetTranslation("ItemRefineryRingHandler.congratulation", objArray);
                GSPacketIn in2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, msg, itemAt.ItemID, itemAt.TemplateID, "", 4);
                GameServer.Instance.LoginServer.SendPacket(in2);
            }
        Label_08BF:
            return 1;
        }

        private int method_0(int int_0)
        {
            char[] separator = new char[] { '|' };
            string[] strArray = GameProperties.RingLevel.Split(separator);
            for (int i = strArray.Length - 1; i >= 0; i--)
            {
                if (int_0 >= int.Parse(strArray[i]))
                {
                    return (i + 1);
                }
            }
            return 0;
        }

        private int method_1(int int_0)
        {
            char[] separator = new char[] { '|' };
            string[] strArray = GameProperties.EquipRefineryExp.Split(separator);
            for (int i = strArray.Length - 1; i >= 0; i--)
            {
                if (int_0 >= int.Parse(strArray[i]))
                {
                    return (i + 1);
                }
            }
            return 0;
        }

        private int method_2()
        {
            char[] separator = new char[] { '|' };
            return int.Parse(GameProperties.RingLevel.Split(separator)[GameProperties.RingMaxRefineryLevel - 1]);
        }

        private int method_3()
        {
            char[] separator = new char[] { '|' };
            return int.Parse(GameProperties.EquipRefineryExp.Split(separator)[GameProperties.EquipMaxRefineryLevel - 1]);
        }
    }
}


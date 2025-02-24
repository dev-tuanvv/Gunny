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
    using System.Collections.Generic;

    [PacketHandler(0x102, "物品镶嵌")]
    public class MagicStoneHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num3;
            int num4;
            string[] strArray;
            int num8;
            int num19;
            int num27;
            SqlDataProvider.Data.ItemInfo info3;
            byte num = packet.ReadByte();
            PlayerMagicStoneInventory magicStoneBag = client.Player.MagicStoneBag;
            if (client.Player.PlayerCharacter.Grade < 40)
            {
                client.Player.SendMessage("Cấp độ kh\x00f4ng đủ.");
                return 0;
            }
            byte num2 = num;
            switch (num2)
            {
                case 1:
                    num3 = packet.ReadInt();
                    packet.ReadBoolean();
                    num4 = packet.ReadInt();
                    if ((((num4 > 0) && (num4 <= 0x3e7)) && (num3 > 0)) && (num3 <= 3))
                    {
                        strArray = GameProperties.OpenMagicStonePackageMoney.Split(new char[] { '|' });
                        string[] strArray2 = GameProperties.MagicPackageID.Split(new char[] { '|' });
                        string[] strArray3 = GameProperties.MagicStoneOpenPoint.Split(new char[] { '|' });
                        int num5 = num4 * int.Parse(strArray[num3]);
                        int num6 = int.Parse(strArray3[num3]);
                        if ((num5 <= 0) || (client.Player.PlayerCharacter.Money < num5))
                        {
                            client.Player.SendMessage("Xu kh\x00f4ng đủ");
                            return 1;
                        }
                        int place = magicStoneBag.FindFirstEmptySlot();
                        num8 = 0;
                        List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                        int point = 0;
                        int gold = 0;
                        int giftToken = 0;
                        int medal = 0;
                        int exp = 0;
                        int honor = 0;
                        int hardCurrency = 0;
                        int leagueMoney = 0;
                        int useableScore = 0;
                        int prestge = 0;
                        num19 = 0;
                        int dateId = int.Parse(strArray2[num3]);
                        if (place == -1)
                        {
                            client.Player.SendMessage("T\x00fai đầy. Kh\x00f4ng thể nhận th\x00eam.");
                            return 0;
                        }
                        client.Player.RemoveMoney(num5);
                        for (int i = 0; i < num4; i++)
                        {
                            place = magicStoneBag.FindFirstEmptySlot();
                            if (place == -1)
                            {
                                break;
                            }
                            itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                            ItemBoxMgr.CreateItemBox(dateId, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref num19);
                            if (itemInfos.Count < 1)
                            {
                                client.Player.SendMessage("Dữ liệu server lỗi.");
                                return 0;
                            }
                            SqlDataProvider.Data.ItemInfo item = itemInfos[0];
                            if (Equip.isMagicStone(item.Template))
                            {
                                item.Count = 1;
                                magicStoneBag.AddItemTo(item, place);
                                num8++;
                                num19 += num6;
                            }
                        }
                        break;
                    }
                    return 0;

                case 2:
                    if (DateTime.Compare(client.Player.LastDrillUpTime.AddMilliseconds(200.0), DateTime.Now) <= 0)
                    {
                        SqlDataProvider.Data.ItemInfo itemAt = magicStoneBag.GetItemAt(0x1f);
                        if (!((itemAt != null) && Equip.isMagicStone(itemAt.Template)))
                        {
                            client.Player.SendMessage("Ma thạch n\x00e2ng cấp kh\x00f4ng tồn tại");
                            return 0;
                        }
                        int num24 = 0;
                        int num25 = packet.ReadInt();
                        List<int> places = new List<int>();
                        for (int j = 0; j < num25; j++)
                        {
                            num27 = packet.ReadInt();
                            info3 = magicStoneBag.GetItemAt(num27);
                            if (!(((info3 == null) || !Equip.isMagicStone(info3.Template)) || info3.GoodsLock))
                            {
                                num24 += info3.StrengthenExp;
                                places.Add(info3.Place);
                            }
                        }
                        itemAt.StrengthenExp += num24;
                        MagicStoneTemplateMgr.SetupMagicStoneWithLevel(itemAt);
                        itemAt.IsBinds = true;
                        magicStoneBag.UpdateItem(itemAt);
                        magicStoneBag.RemoveAllItem(places);
                        magicStoneBag.SaveToDatabase();
                        client.Player.LastDrillUpTime = DateTime.Now;
                        return 1;
                    }
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Qu\x00e1 nhiều thao t\x00e1c!", new object[0]));
                    client.Player.LastDrillUpTime = DateTime.Now;
                    return 0;

                case 3:
                {
                    num27 = packet.ReadInt();
                    int slot = packet.ReadInt();
                    if (DateTime.Compare(client.Player.LastDrillUpTime.AddMilliseconds(200.0), DateTime.Now) <= 0)
                    {
                        if ((slot <= -1) || (slot > magicStoneBag.Capalility))
                        {
                            slot = magicStoneBag.FindFirstEmptySlot();
                        }
                        if ((slot == -1) || (num27 == slot))
                        {
                            client.Player.SendMessage("T\x00fai ma thạch đ\x00e3 đầy.");
                            return 0;
                        }
                        SqlDataProvider.Data.ItemInfo itemCheck = magicStoneBag.GetItemAt(num27);
                        SqlDataProvider.Data.ItemInfo info5 = magicStoneBag.GetItemAt(slot);
                        if (itemCheck == null)
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Xảy ra lỗi, chuyển k\x00eanh v\x00e0 thử lại!");
                            return 0;
                        }
                        if ((magicStoneBag.IsMagicStoneEquipSlot(slot) && MagicStoneTemplateMgr.IsNormalStone(itemCheck.Template.Property3)) && magicStoneBag.ScanStoneNormalEquip(itemCheck))
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Bạn chỉ c\x00f3 thể trang bị 1 loại ma thạch thường!");
                            return 0;
                        }
                        if (!(((((itemCheck == null) || (info5 == null)) || (!magicStoneBag.IsMagicStoneEquipSlot(num27) || !MagicStoneTemplateMgr.IsNormalStone(info5.Template.Property3))) || !magicStoneBag.ScanStoneNormalEquip(info5)) || MagicStoneTemplateMgr.StoneNormalSame(itemCheck, info5)))
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Bạn chỉ c\x00f3 thể trang bị 1 loại ma thạch thường!");
                            return 0;
                        }
                        if (!magicStoneBag.MoveItem(num27, slot, itemCheck.Count))
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Rương đ\x00e3 đầy kh\x00f4ng thể di chuyển!");
                            return 0;
                        }
                        if ((num27 < (magicStoneBag.BeginSlot - 1)) || (slot < (magicStoneBag.BeginSlot - 1)))
                        {
                            client.Player.EquipBag.UpdatePlayerProperties();
                        }
                        client.Player.LastDrillUpTime = DateTime.Now;
                        return 1;
                    }
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Qu\x00e1 nhiều thao t\x00e1c!", new object[0]));
                    client.Player.LastDrillUpTime = DateTime.Now;
                    return 0;
                }
                case 4:
                    client.Out.SendMagicStonePoint(client.Player.PlayerCharacter);
                    return 1;

                case 5:
                {
                    int iD = packet.ReadInt();
                    packet.ReadBoolean();
                    num4 = packet.ReadInt();
                    if (num4 > 0)
                    {
                        if (DateTime.Compare(client.Player.LastDrillUpTime.AddMilliseconds(200.0), DateTime.Now) > 0)
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Qu\x00e1 nhiều thao t\x00e1c!", new object[0]));
                            client.Player.LastDrillUpTime = DateTime.Now;
                            return 0;
                        }
                        if (magicStoneBag.FindFirstEmptySlot() == -1)
                        {
                            client.Player.SendMessage("T\x00fai ma thạch đ\x00e3 đầy!");
                            return 0;
                        }
                        ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
                        if (!((shopItemInfoById != null) && ShopMgr.IsOnShop(shopItemInfoById.ID)))
                        {
                            return 0;
                        }
                        if ((shopItemInfoById.APrice1 == -1400) && (client.Player.PlayerCharacter.MagicStonePoint >= shopItemInfoById.AValue1))
                        {
                            client.Player.RemoveMagicStonePoint(shopItemInfoById.AValue1);
                            SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID), 1, 0x66);
                            cloneItem.IsBinds = true;
                            if (magicStoneBag.AddTemplate(cloneItem, num4))
                            {
                                client.Player.SendMessage(string.Format("Mua th\x00e0nh c\x00f4ng {1}x {0}", num4, cloneItem.Template.Name));
                            }
                            else
                            {
                                client.Player.SendMessage("Xảy ra lỗi. Li\x00ean hệ BQT để được trợ gi\x00fap.");
                            }
                        }
                        else
                        {
                            client.Player.SendMessage("Điểm ma thạch kh\x00f4ng đủ để đổi.");
                        }
                        client.Player.LastDrillUpTime = DateTime.Now;
                        return 1;
                    }
                    return 0;
                }
                case 6:
                    num27 = packet.ReadInt();
                    info3 = magicStoneBag.GetItemAt(num27);
                    if (info3 != null)
                    {
                        if (info3.GoodsLock)
                        {
                            info3.GoodsLock = false;
                        }
                        else
                        {
                            info3.GoodsLock = true;
                        }
                        magicStoneBag.UpdateItem(info3);
                        return 1;
                    }
                    client.Out.SendMessage(eMessageType.Normal, "Xảy ra lỗi, chuyển k\x00eanh v\x00e0 thử lại.");
                    return 0;

                case 7:
                case 8:
                    goto Label_09FF;

                case 9:
                {
                    int num30 = packet.ReadInt();
                    for (int k = 0; k < num30; k++)
                    {
                        num27 = packet.ReadInt();
                        int toSlot = packet.ReadInt();
                        if (num27 != toSlot)
                        {
                            info3 = magicStoneBag.GetItemAt(num27);
                            if (!((info3 == null) || magicStoneBag.MoveItem(num27, toSlot, info3.Count)))
                            {
                                client.Player.SendMessage("Vật phẩm kh\x00f4ng thể di chuyển!");
                                return 0;
                            }
                        }
                    }
                    return 1;
                }
                default:
                    if (num2 == 0x10)
                    {
                        return 1;
                    }
                    goto Label_09FF;
            }
            if (num8 < num4)
            {
                int num22 = num4 - num8;
                int num23 = num22 * int.Parse(strArray[num3]);
                client.Player.AddMoney(num23);
                client.Player.SendMessage("T\x00fai đầy kh\x00f4ng thể mở th\x00eam.");
            }
            client.Player.SendMessage("Mở th\x00e0nh c\x00f4ng " + num8 + " ma thạch.");
            if (num19 > 0)
            {
                client.Player.AddMagicStonePoint(num19);
                return 1;
            }
            return 1;
        Label_09FF:
            Console.WriteLine("magic stone cmd: " + num);
            return 1;
        }
    }
}


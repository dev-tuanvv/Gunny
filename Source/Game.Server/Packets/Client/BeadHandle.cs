namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0x79, "物品镶嵌")]
    public class BeadHandle : IPacketHandler
    {
        public static ThreadSafeRandom random = new ThreadSafeRandom();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num2;
            int num3;
            SqlDataProvider.Data.ItemInfo itemAt;
            SqlDataProvider.Data.ItemInfo info5;
            byte num = packet.ReadByte();
            PlayerBeadInventory beadBag = client.Player.BeadBag;
            string message = "";
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            switch (num)
            {
                case 1:
                {
                    num2 = packet.ReadInt();
                    num3 = packet.ReadInt();
                    int needLv = 10;
                    if (num3 == -1)
                    {
                        num3 = beadBag.FindFirstEmptySlot();
                    }
                    if ((num3 == -1) || (num3 > beadBag.BeginSlot))
                    {
                    }
                    if (!(((num3 > 12) || (num3 < 4)) || beadBag.canEquip(num3, client.Player.PlayerCharacter.Grade, ref needLv)))
                    {
                        client.Out.SendMessage(eMessageType.Normal, string.Format("Cấp {0} mở", needLv));
                        return 0;
                    }
                    itemAt = beadBag.GetItemAt(num2);
                    SqlDataProvider.Data.ItemInfo info2 = beadBag.GetItemAt(num3);
                    if (itemAt == null)
                    {
                        client.Out.SendMessage(eMessageType.Normal, "Xảy ra lỗi, chuyển k\x00eanh v\x00e0 thử lại!");
                        return 0;
                    }
                    if ((num2 > 0x12) || (num2 < 13))
                    {
                        if ((num3 <= 0x12) && (num3 >= 13))
                        {
                            int drillLv = beadBag.GetDrillLevel(num3);
                            if (!(beadBag.JudgeLevel(itemAt.Hole1, drillLv) && ((info2 == null) || beadBag.JudgeLevel(info2.Hole1, drillLv))))
                            {
                                beadBag.UpdateChangedPlaces();
                                client.Out.SendMessage(eMessageType.Normal, "Cấp ch\x00e2u b\x00e1u v\x00e0 cấp lỗ kh\x00f4ng khớp.");
                                return 0;
                            }
                        }
                        else if ((num3 > 30) && (info2 != null))
                        {
                            RuneTemplateInfo info3 = RuneMgr.FindRuneByTemplateID(info2.TemplateID);
                            if (!((num2 != 1) || info3.IsAttack()))
                            {
                                client.Out.SendMessage(eMessageType.Normal, "Trang bị kh\x00f4ng ph\x00f9 hợp.");
                                return 0;
                            }
                            if (!(((num2 != 2) && (num2 != 3)) || info3.IsDefend()))
                            {
                                client.Out.SendMessage(eMessageType.Normal, "Trang bị kh\x00f4ng ph\x00f9 hợp.");
                                return 0;
                            }
                            if (!(((num2 <= 3) || (num2 >= 30)) || info3.IsProp()))
                            {
                                client.Out.SendMessage(eMessageType.Normal, "Trang bị kh\x00f4ng ph\x00f9 hợp.");
                                return 0;
                            }
                            beadBag.UpdateChangedPlaces();
                        }
                        else
                        {
                            beadBag.UpdateChangedPlaces();
                        }
                        break;
                    }
                    int drillLevel = beadBag.GetDrillLevel(num2);
                    if (!(beadBag.JudgeLevel(itemAt.Hole1, drillLevel) && ((info2 == null) || beadBag.JudgeLevel(info2.Hole1, drillLevel))))
                    {
                        beadBag.UpdateChangedPlaces();
                        client.Out.SendMessage(eMessageType.Normal, "Cấp ch\x00e2u b\x00e1u v\x00e0 cấp lỗ kh\x00f4ng khớp.");
                        return 0;
                    }
                    break;
                }
                case 2:
                {
                    SqlDataProvider.Data.ItemInfo item = beadBag.GetItemAt(0x1f);
                    if (item != null)
                    {
                        List<int> places = new List<int>();
                        int num7 = 0;
                        int num8 = packet.ReadInt();
                        for (int i = 0; i < num8; i++)
                        {
                            int slot = packet.ReadInt();
                            info5 = beadBag.GetItemAt(slot);
                            if (!((info5 == null) || info5.IsUsed))
                            {
                                places.Add(slot);
                                num7 += info5.Hole2;
                            }
                        }
                        if (num7 == 0)
                        {
                            client.Player.SendMessage("Ch\x00e2u b\x00e1u n\x00e2ng cấp kh\x00f4ng đủ. Thao t\x00e1c thất bại.");
                            return 0;
                        }
                        int num11 = item.Hole2;
                        int num12 = item.Hole1;
                        int gP = num7 + num11;
                        int runeLevel = RuneMgr.GetRuneLevel(gP);
                        int num15 = RuneMgr.MaxExp();
                        if (gP > num15)
                        {
                            num7 = num15;
                        }
                        item.Hole2 += num7;
                        item.Hole1 = runeLevel;
                        RuneTemplateInfo info6 = RuneMgr.FindRuneTemplateID(item.TemplateID, runeLevel);
                        if (info6 == null)
                        {
                            client.Player.SendMessage("Dữ liệu server lỗi. Thao t\x00e1c thất bại.");
                            return 0;
                        }
                        if (info6.TemplateID != item.TemplateID)
                        {
                            SqlDataProvider.Data.ItemInfo info7 = new SqlDataProvider.Data.ItemInfo(ItemMgr.FindItemTemplate(info6.TemplateID));
                            item.TemplateID = info6.TemplateID;
                            info7.Copy(item);
                            beadBag.RemoveItemAt(0x1f);
                            beadBag.AddItemTo(info7, 0x1f);
                        }
                        item.IsBinds = true;
                        beadBag.UpdateItem(item);
                        beadBag.RemoveAllItem(places);
                        beadBag.SaveToDatabase();
                        goto Label_0DCE;
                    }
                    client.Player.SendMessage("Ch\x00e2u b\x00e1u kh\x00f4ng tồn tại. Thao t\x00e1c thất bại.");
                    return 0;
                }
                case 3:
                {
                    int index = packet.ReadInt();
                    packet.ReadBoolean();
                    bool flag = false;
                    string[] strArray = GameProperties.OpenRunePackageMoney.Split(new char[] { '|' });
                    string[] strArray2 = GameProperties.RunePackageID.Split(new char[] { '|' });
                    string[] strArray3 = GameProperties.OpenRunePackageRange.Split(new char[] { '|' });
                    if (DateTime.Compare(client.Player.LastOpenPack.AddMilliseconds(500.0), DateTime.Now) <= 0)
                    {
                        if (beadBag.FindFirstEmptySlot() == -1)
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Rương đ\x00e3 đầy kh\x00f4ng thể mở th\x00eam!");
                            return 0;
                        }
                        if ((index < 0) || (index > strArray.Length))
                        {
                            index = strArray.Length - 1;
                        }
                        if (client.Player.Extra.UseKingBless(3))
                        {
                            flag = true;
                        }
                        else if (client.Player.MoneyDirect(int.Parse(strArray[index])))
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                            int dateId = int.Parse(strArray2[0]);
                            string[] strArray4 = strArray3[2].Split(new char[] { ',' });
                            switch (index)
                            {
                                case 1:
                                    strArray4 = strArray3[1].Split(new char[] { ',' });
                                    dateId = int.Parse(strArray2[1]);
                                    break;

                                case 2:
                                    strArray4 = strArray3[0].Split(new char[] { ',' });
                                    dateId = int.Parse(strArray2[2]);
                                    break;

                                case 3:
                                    dateId = int.Parse(strArray2[3]);
                                    break;
                            }
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
                            int magicStonePoint = 0;
                            ItemBoxMgr.CreateItemBox(dateId, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref magicStonePoint);
                            if (itemInfos.Count < 1)
                            {
                                client.Player.SendMessage("Dữ liệu server lỗi.");
                                return 0;
                            }
                            SqlDataProvider.Data.ItemInfo info8 = itemInfos[0];
                            info8.Count = 1;
                            beadBag.BeginChanges();
                            beadBag.AddItem(info8);
                            beadBag.CommitChanges();
                            if (info8 != null)
                            {
                                int level = info8.Template.Level;
                            }
                            RuneTemplateInfo info9 = RuneMgr.FindRuneByTemplateID(info8.TemplateID);
                            if (info9 != null)
                            {
                                client.Player.SendMessage(string.Format("Bạn nhận được {0} lv{1}.", info9.Name, info9.BaseLevel));
                                if (info9.BaseLevel > 13)
                                {
                                    client.Player.SendItemNotice(info8, 4, "bead");
                                }
                            }
                            int rand = random.Next(int.Parse(strArray4[1]));
                            if ((rand == 3) || (rand > 4))
                            {
                                rand = 0;
                            }
                            client.Out.SendRuneOpenPackage(client.Player, rand);
                        }
                        client.Player.LastOpenPack = DateTime.Now;
                        goto Label_0DCE;
                    }
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Qu\x00e1 nhiều thao t\x00e1c!", new object[0]));
                    return 0;
                }
                case 4:
                {
                    int num31 = packet.ReadInt();
                    info5 = beadBag.GetItemAt(num31);
                    if (info5 != null)
                    {
                        if (info5.IsUsed)
                        {
                            info5.IsUsed = false;
                        }
                        else
                        {
                            info5.IsUsed = true;
                        }
                        beadBag.UpdateItem(info5);
                        goto Label_0DCE;
                    }
                    client.Out.SendMessage(eMessageType.Normal, "Xảy ra lỗi, chuyển k\x00eanh v\x00e0 thử lại.");
                    return 0;
                }
                case 5:
                {
                    int num32 = packet.ReadInt();
                    int templateId = packet.ReadInt();
                    if (DateTime.Compare(client.Player.LastDrillUpTime.AddMilliseconds(200.0), DateTime.Now) <= 0)
                    {
                        bool flag2 = false;
                        PlayerInventory inventory = client.Player.GetInventory(eBageType.PropBag);
                        SqlDataProvider.Data.ItemInfo itemByTemplateID = inventory.GetItemByTemplateID(0, templateId);
                        UserDrillInfo drill = beadBag.UserDrills[num32];
                        if (!(((itemByTemplateID != null) && (drill != null)) && itemByTemplateID.isDrill(drill.HoleLv)))
                        {
                            client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Điều kiện kh\x00f4ng đủ!", new object[0]));
                            return 0;
                        }
                        int itemCount = inventory.GetItemCount(templateId);
                        int num35 = 0x2b1b;
                        int num36 = random.Next(2, 6);
                        if (drill.HoleLv == 0)
                        {
                            num35 = 0x2b1b;
                            num36 = random.Next(2, 5);
                        }
                        else if (drill.HoleLv == 1)
                        {
                            num35 = 0x2b1c;
                            num36 = random.Next(4, 7);
                        }
                        else if (drill.HoleLv == 2)
                        {
                            num35 = 0x2b12;
                            num36 = random.Next(6, 9);
                        }
                        else if (drill.HoleLv == 3)
                        {
                            num35 = 0x2b13;
                            num36 = random.Next(8, 11);
                        }
                        else
                        {
                            num35 = 0x2b1a;
                            num36 = random.Next(10, 13);
                        }
                        if ((itemCount <= 0) || (templateId != num35))
                        {
                            client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Mủi khoan kh\x00f4ng đủ!", new object[0]));
                        }
                        else
                        {
                            message = LanguageMgr.GetTranslation("OpenHoleHandler.GetExp", new object[] { num36 });
                            drill.HoleExp += num36;
                            switch (drill.HoleLv)
                            {
                                case 0:
                                    if (drill.HoleExp >= GameProperties.HoleLevelUpExp(0))
                                    {
                                        drill.HoleLv++;
                                        drill.HoleExp = 0;
                                        flag2 = true;
                                    }
                                    break;

                                case 1:
                                    if (drill.HoleExp >= GameProperties.HoleLevelUpExp(1))
                                    {
                                        drill.HoleLv++;
                                        drill.HoleExp = 0;
                                        flag2 = true;
                                    }
                                    break;

                                case 2:
                                    if (drill.HoleExp >= GameProperties.HoleLevelUpExp(2))
                                    {
                                        drill.HoleLv++;
                                        drill.HoleExp = 0;
                                        flag2 = true;
                                    }
                                    break;

                                case 3:
                                    if (drill.HoleExp >= GameProperties.HoleLevelUpExp(3))
                                    {
                                        drill.HoleLv++;
                                        drill.HoleExp = 0;
                                        flag2 = true;
                                    }
                                    break;

                                case 4:
                                    if (drill.HoleExp >= GameProperties.HoleLevelUpExp(4))
                                    {
                                        drill.HoleLv++;
                                        drill.HoleExp = 0;
                                        flag2 = true;
                                    }
                                    break;
                            }
                            beadBag.UpdateDrill(num32, drill);
                        }
                        if (message != "")
                        {
                            client.Out.SendMessage(eMessageType.Normal, message);
                        }
                        beadBag.SendPlayerDrill();
                        inventory.RemoveTemplate(templateId, 1);
                        client.Player.LastDrillUpTime = DateTime.Now;
                        if (flag2)
                        {
                            using (PlayerBussiness bussiness = new PlayerBussiness())
                            {
                                foreach (UserDrillInfo info12 in beadBag.UserDrills.Values)
                                {
                                    bussiness.UpdateUserDrillInfo(info12);
                                }
                            }
                        }
                        goto Label_0DCE;
                    }
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Qu\x00e1 nhiều thao t\x00e1c!", new object[0]));
                    return 0;
                }
                default:
                    goto Label_0DCE;
            }
            if (!beadBag.MoveItem(num2, num3, 1))
            {
                if (itemAt == null)
                {
                    client.Out.SendMessage(eMessageType.Normal, "Rương đ\x00e3 đầy kh\x00f4ng thể di chuyển!");
                    return 0;
                }
                beadBag.TakeOutItem(itemAt);
                client.Player.SendItemToMail(itemAt, "T\x00fai đạo cụ đầy vật phẩm chuyển v\x00e0o thư", "H\x00e0nh trang đ\x00e3 đầy. Gửi v\x00e0o thư.", eMailType.ItemOverdue);
                client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
            client.Player.EquipBag.UpdatePlayerProperties();
        Label_0DCE:
            return 0;
        }
    }
}


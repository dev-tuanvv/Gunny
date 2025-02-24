namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.GameUtils;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0x44, "添加好友")]
    public class PetHandler : IPacketHandler
    {
        private List<PetEquipDataInfo> GetPetEquip(int petID, PetEquipDataInfo[] eqs)
        {
            List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
            for (int i = 0; i < eqs.Length; i++)
            {
                PetEquipDataInfo item = eqs[i];
                if (petID == item.PetID)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num4;
            int num5;
            bool flag2;
            int num20;
            bool flag3;
            int num36;
            byte num = packet.ReadByte();
            string msg = "Xu kh\x00f4ng đủ!";
            int slot = -1;
            PetInventory petBag = client.Player.PetBag;
            int playerLevel = (client.Player.Level > 60) ? 60 : client.Player.Level;
            if (client.Player.PlayerCharacter.Grade < 0x19)
            {
                return 0;
            }
            switch (num)
            {
                case 1:
                    this.UpdatePetHandle(client, packet.ReadInt());
                    return 0;

                case 2:
                {
                    num4 = packet.ReadInt();
                    num5 = packet.ReadInt();
                    int iD = client.Player.PlayerCharacter.ID;
                    int place = petBag.FindFirstEmptySlot();
                    if (client.Player.PlayerCharacter.Grade >= 0x19)
                    {
                        if (place == -1)
                        {
                            client.Player.SendMessage("Số lượng pet đ\x00e3 đạt giới hạn!");
                            return 0;
                        }
                        SqlDataProvider.Data.ItemInfo itemAt = client.Player.GetItemAt((eBageType) num5, num4);
                        PetTemplateInfo info = PetMgr.FindPetTemplate(itemAt.Template.Property5);
                        if (info == null)
                        {
                            client.Player.SendMessage("Dữ liệu server lỗi.");
                            return 0;
                        }
                        UsersPetinfo pet = PetMgr.CreatePet(info, iD, place, playerLevel);
                        pet.IsExit = true;
                        petBag.AddPetTo(pet, place);
                        client.Player.RemoveCountFromStack(itemAt, 1);
                        if (info.StarLevel > 4)
                        {
                            GSPacketIn @in = WorldMgr.SendSysNotice(string.Format("Người chơi [{0}] thật gh\x00ea gớm mở trứng được {1} {2} sao.", client.Player.PlayerCharacter.NickName, info.Name, info.StarLevel));
                            GameServer.Instance.LoginServer.SendPacket(@in);
                        }
                        else
                        {
                            client.Player.SendMessage(string.Format("Bạn nhận được {0} {1} sao", info.Name, info.StarLevel));
                        }
                        petBag.SaveToDatabase(false);
                        return 0;
                    }
                    client.Player.SendMessage("Level chưa đủ, kh\x00f4ng thể mở.");
                    return 0;
                }
                case 4:
                {
                    num4 = packet.ReadInt();
                    num5 = packet.ReadInt();
                    slot = packet.ReadInt();
                    bool flag = false;
                    SqlDataProvider.Data.ItemInfo item = client.Player.GetItemAt((eBageType) num5, num4);
                    if (item != null)
                    {
                        int num8 = Convert.ToInt32(PetMgr.FindConfig("MaxHunger").Value);
                        UsersPetinfo petAt = petBag.GetPetAt(slot);
                        int count = item.Count;
                        int num10 = item.Template.Property2;
                        int num11 = item.Template.Property1;
                        int num12 = count * num11;
                        int num13 = num12 + petAt.Hunger;
                        int gP = count * num10;
                        msg = "";
                        if (item.TemplateID == 0x51914)
                        {
                            gP = item.DefendCompose;
                        }
                        if (petAt.Level < playerLevel)
                        {
                            gP += petAt.GP;
                            int level = petAt.Level;
                            int max = PetMgr.GetLevel(gP, playerLevel);
                            int num17 = PetMgr.GetGP(max + 1, playerLevel);
                            int num18 = PetMgr.GetGP(playerLevel, playerLevel);
                            int num19 = gP;
                            if (gP > num18)
                            {
                                gP -= num18;
                                if ((gP >= num10) && (num10 != 0))
                                {
                                    count -= (int) Math.Ceiling((double) (((double) gP) / ((double) num10)));
                                }
                            }
                            petAt.GP = (num19 >= num18) ? num18 : num19;
                            petAt.Level = max;
                            petAt.MaxGP = (num17 == 0) ? num18 : num17;
                            petAt.Hunger = (num13 > num8) ? num8 : num13;
                            flag = petBag.UpGracePet(petAt, slot, true, level, max, playerLevel, ref msg);
                            if (item.TemplateID == 0x51914)
                            {
                                client.Player.StoreBag.RemoveItem(item);
                            }
                            else
                            {
                                client.Player.StoreBag.RemoveCountFromStack(item, count);
                                client.Player.OnUsingItem(item.TemplateID);
                            }
                        }
                        else if (petAt.Hunger < num8)
                        {
                            petAt.Hunger = num13;
                            client.Player.StoreBag.RemoveCountFromStack(item, count);
                            flag = petBag.UpGracePet(petAt, slot, false, 0, 0, playerLevel, ref msg);
                            msg = "\x00d0ộ vui vẻ tăng th\x00eam " + num12;
                        }
                        else
                        {
                            msg = "\x00d0ộ vui vui đ\x00e3 đạt mức tối da";
                        }
                        if (flag)
                        {
                            petBag.SaveToDatabase(false);
                        }
                        if (!string.IsNullOrEmpty(msg))
                        {
                            client.Player.SendMessage(msg);
                        }
                        client.Player.OnUpLevelPetEvent();
                        return 0;
                    }
                    client.Out.SendMessage(eMessageType.Normal, "Xảy ra lỗi, chuyển k\x00eanh v\x00e0 thử lại.");
                    return 0;
                }
                case 5:
                {
                    flag2 = packet.ReadBoolean();
                    num20 = Convert.ToInt32(PetMgr.FindConfig("AdoptRefereshCost").Value);
                    int templateId = Convert.ToInt32(PetMgr.FindConfig("FreeRefereshID").Value);
                    SqlDataProvider.Data.ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
                    if (!flag2)
                    {
                        goto Label_06CE;
                    }
                    flag3 = true;
                    if (!client.Player.Extra.UseKingBless(2))
                    {
                        if (!client.Player.MoneyDirect(num20))
                        {
                            return 0;
                        }
                        if (itemByTemplateID != null)
                        {
                            client.Player.PropBag.RemoveTemplate(templateId, 1);
                            flag3 = false;
                        }
                        break;
                    }
                    client.Player.SendMessage(string.Format("Ch\x00fac ph\x00fac thần g\x00e0 gi\x00fap bạn tiết kiệm {0}xu.", num20));
                    flag3 = false;
                    break;
                }
                case 6:
                {
                    slot = packet.ReadInt();
                    int num22 = petBag.FindFirstEmptySlot();
                    if (num22 != -1)
                    {
                        if (slot < 0)
                        {
                            client.Out.SendRefreshPet(client.Player, petBag.GetAdoptPet(), null, false);
                            client.Player.SendMessage("Kh\x00f4ng t\x00ecm thấy pet n\x00e0y!");
                            return 0;
                        }
                        UsersPetinfo adoptPetAt = petBag.GetAdoptPetAt(slot);
                        using (PlayerBussiness bussiness = new PlayerBussiness())
                        {
                            if (adoptPetAt.ID > 0)
                            {
                                bussiness.RemoveUserAdoptPet(adoptPetAt.ID);
                                adoptPetAt.ID = 0;
                            }
                        }
                        petBag.RemoveAdoptPet(adoptPetAt);
                        if (petBag.AddPetTo(adoptPetAt, num22))
                        {
                            PetTemplateInfo info5 = PetMgr.FindPetTemplate(adoptPetAt.TemplateID);
                            if (info5.StarLevel > 3)
                            {
                                GSPacketIn in2 = WorldMgr.SendSysNotice(string.Format("Người chơi [{0}] may mắn bắt được {1} {2} sao.", client.Player.PlayerCharacter.NickName, info5.Name, info5.StarLevel));
                                GameServer.Instance.LoginServer.SendPacket(in2);
                            }
                            client.Player.OnAdoptPetEvent();
                        }
                        petBag.SaveToDatabase(false);
                        return 0;
                    }
                    client.Out.SendRefreshPet(client.Player, petBag.GetAdoptPet(), null, false);
                    client.Player.SendMessage("Số lượng pet đ\x00e3 đạt giới hạn!");
                    return 0;
                }
                case 7:
                {
                    slot = packet.ReadInt();
                    int killId = packet.ReadInt();
                    int killindex = packet.ReadInt();
                    if (!petBag.EquipSkillPet(slot, killId, killindex))
                    {
                        client.Player.SendMessage("Skill n\x00e0y đ\x00e3 trang bị!");
                    }
                    return 0;
                }
                case 8:
                {
                    slot = packet.ReadInt();
                    UsersPetinfo petinfo5 = petBag.GetPetAt(slot);
                    if (petBag.RemovePet(petinfo5))
                    {
                        using (PlayerBussiness bussiness2 = new PlayerBussiness())
                        {
                            bussiness2.UpdateUserAdoptPet(petinfo5.ID);
                        }
                    }
                    client.Player.SendMessage("Thả pet th\x00e0nh c\x00f4ng!");
                    petBag.SaveToDatabase(false);
                    return 0;
                }
                case 9:
                {
                    slot = packet.ReadInt();
                    string name = packet.ReadString();
                    int num25 = Convert.ToInt32(PetMgr.FindConfig("ChangeNameCost").Value);
                    if (client.Player.MoneyDirect(num25))
                    {
                        if (petBag.RenamePet(slot, name))
                        {
                            msg = "Đổi t\x00ean th\x00e0nh c\x00f4ng!";
                        }
                        client.Player.SendMessage(msg);
                    }
                    return 0;
                }
                case 0x11:
                {
                    slot = packet.ReadInt();
                    bool isEquip = packet.ReadBoolean();
                    UsersPetinfo petinfo6 = petBag.GetPetAt(slot);
                    if (petinfo6 != null)
                    {
                        if (!((petinfo6.Level <= playerLevel) || petinfo6.IsEquip))
                        {
                            client.Player.SendMessage("Level pet qu\x00e1 cao, kh\x00f4ng thể sử dụng!");
                            return 0;
                        }
                        if (petBag.EquipPet(slot, isEquip))
                        {
                            client.Player.EquipBag.UpdatePlayerProperties();
                            return 0;
                        }
                        client.Player.SendMessage("Độ no bằng kh\x00f4ng, kh\x00f4ng thể chiến đấu!");
                        return 0;
                    }
                    return 0;
                }
                case 0x12:
                {
                    slot = packet.ReadInt();
                    int num26 = Convert.ToInt32(PetMgr.FindConfig("RecycleCost").Value);
                    if (!client.Player.MoneyDirect(num26))
                    {
                        goto Label_0CBC;
                    }
                    UsersPetinfo petinfo7 = client.Player.PetBag.GetPetAt(slot);
                    if (petinfo7 != null)
                    {
                        UsersPetinfo adoptPetSingle = new UsersPetinfo();
                        int petID = petinfo7.ID;
                        using (PlayerBussiness bussiness3 = new PlayerBussiness())
                        {
                            adoptPetSingle = bussiness3.GetAdoptPetSingle(petID);
                        }
                        if (adoptPetSingle == null)
                        {
                            client.Player.SendMessage("Phục hồi thất bại!");
                            return 0;
                        }
                        SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(0x51914), 1, 0x66);
                        cloneItem.IsBinds = true;
                        cloneItem.DefendCompose = petinfo7.GP;
                        cloneItem.AgilityCompose = petinfo7.MaxGP;
                        if (!client.Player.PropBag.AddTemplate(cloneItem, 1))
                        {
                            client.Player.SendItemToMail(cloneItem, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), eMailType.ItemOverdue);
                            client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                        }
                        int num28 = client.Player.PlayerCharacter.ID;
                        int num29 = PetMgr.OldTemplate(petinfo7.TemplateID);
                        petinfo7.TemplateID = num29;
                        petinfo7.Skill = adoptPetSingle.Skill;
                        petinfo7.SkillEquip = adoptPetSingle.SkillEquip;
                        petinfo7.GP = 0;
                        petinfo7.Level = 1;
                        petinfo7.MaxGP = 0x37;
                        PetMgr.CreateNewPropPet(ref petinfo7);
                        List<PetEquipDataInfo> equip = petinfo7.GetEquip();
                        client.Player.PetBag.MoveEqAllToBag(equip);
                        petinfo7.EquipList = client.Player.PetBag.RemoveEq(equip);
                        if (client.Player.PetBag.UpGracePet(petinfo7, slot, false, 0, 0, playerLevel, ref msg))
                        {
                            client.Player.SendMessage("Phục hồi th\x00e0nh c\x00f4ng!");
                        }
                        goto Label_0CBC;
                    }
                    return 0;
                }
                case 0x13:
                {
                    bool flag6 = packet.ReadBoolean();
                    if (!client.Player.PlayerCharacter.HasBagPassword || !client.Player.PlayerCharacter.IsLocked)
                    {
                        UserFarmInfo currentFarm = client.Player.Farm.CurrentFarm;
                        int buyExpRemainNum = currentFarm.buyExpRemainNum;
                        PetExpItemPriceInfo info9 = PetMgr.FindPetExpItemPrice(this.RealMoney(buyExpRemainNum));
                        if ((info9 != null) && (buyExpRemainNum >= 1))
                        {
                            bool flag7 = false;
                            int money = info9.Money;
                            if (flag6)
                            {
                                if (client.Player.MoneyDirect(money))
                                {
                                    client.Player.AddExpVip(money);
                                    flag7 = true;
                                }
                            }
                            else if (money <= client.Player.PlayerCharacter.GiftToken)
                            {
                                client.Player.RemoveGiftToken(money);
                                flag7 = true;
                            }
                            if (!flag7)
                            {
                                if (GameProperties.IsDDTMoneyActive)
                                {
                                    client.Player.SendMessage("Xu kh\x00f3a kh\x00f4ng đủ. Thao t\x00e1c thất bại.");
                                }
                                else
                                {
                                    client.Player.SendMessage("Xu kh\x00f4ng đủ. Thao t\x00e1c thất bại.");
                                }
                                return 0;
                            }
                            SqlDataProvider.Data.ItemInfo info11 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(0x51916), info9.ItemCount, 0x66);
                            info11.IsBinds = true;
                            client.Player.AddTemplate(info11, info11.Template.BagType, info9.ItemCount, eGameView.RouletteTypeGet);
                            currentFarm.buyExpRemainNum--;
                            GSPacketIn pkg = new GSPacketIn(0x44);
                            pkg.WriteByte(0x13);
                            pkg.WriteInt(currentFarm.buyExpRemainNum);
                            client.SendTCP(pkg);
                            client.Player.Farm.UpdateFarm(currentFarm);
                        }
                        return 0;
                    }
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                    return 0;
                }
                case 20:
                {
                    int num32 = packet.ReadInt();
                    slot = packet.ReadInt();
                    int num33 = packet.ReadInt();
                    if (petBag.GetPetAt(num33) != null)
                    {
                        SqlDataProvider.Data.ItemInfo info12 = client.Player.GetItemAt((eBageType) num32, slot);
                        if (!((info12 != null) && info12.IsEquipPet()))
                        {
                            client.Player.RemoveAt((eBageType) num32, slot);
                            return 0;
                        }
                        if (petBag.MoveEqFromBag(num33, info12.eqType(), info12))
                        {
                            client.Player.RemoveAt((eBageType) num32, slot);
                            client.Player.EquipBag.UpdatePlayerProperties();
                            return 0;
                        }
                        client.Player.SendMessage("Cấp kh\x00f4ng đủ, trang bị thất bại!");
                        return 0;
                    }
                    return 0;
                }
                case 0x15:
                {
                    int num34 = packet.ReadInt();
                    slot = packet.ReadInt();
                    UsersPetinfo petinfo9 = petBag.GetPetAt(num34);
                    if (petinfo9 != null)
                    {
                        if (slot <= petinfo9.EquipList.Count)
                        {
                            PetEquipDataInfo ep = petBag.GetPetAt(num34).EquipList[slot];
                            if (ep == null)
                            {
                                return 0;
                            }
                            petBag.MoveEqToBag(ep);
                            ep.eqTemplateID = -1;
                            ep.ValidDate = 7;
                            ep = ep.addTempalte(null);
                            petBag.UpdateQPet(num34, slot, ep);
                            client.Player.EquipBag.UpdatePlayerProperties();
                        }
                        return 0;
                    }
                    return 0;
                }
                case 0x16:
                {
                    int num35 = packet.ReadInt();
                    num36 = packet.ReadInt();
                    slot = packet.ReadInt();
                    if ((num35 == 0x2b9a) && (num36 > 0))
                    {
                        ItemTemplateInfo info14 = ItemMgr.FindItemTemplate(num35);
                        if (info14 != null)
                        {
                            if (client.Player.PropBag.GetItemCount(num35) < num36)
                            {
                                client.Player.SendMessage("Số lượng kh\x00f4ng c\x00f3 thực!");
                                return 0;
                            }
                            UsersPetinfo petinfo10 = petBag.GetPetAt(slot);
                            if (petinfo10 == null)
                            {
                                client.Player.SendMessage("Kh\x00f4ng tồn tại pet n\x00e0y!");
                                return 0;
                            }
                            PetStarExpInfo petStarExp = PetMgr.GetPetStarExp(petinfo10.TemplateID);
                            if (petStarExp == null)
                            {
                                client.Player.SendMessage("Pet của bạn đ\x00e3 đạt mức tối đa!");
                                return 0;
                            }
                            int num38 = petStarExp.Exp - petinfo10.currentStarExp;
                            if (num38 <= 0)
                            {
                                return 0;
                            }
                            int num39 = num38 / info14.Property2;
                            bool val = false;
                            if (num39 <= num36)
                            {
                                client.Player.PropBag.RemoveTemplate(num35, num39);
                                int num40 = PetMgr.UpdateEvolution(petStarExp.NewID, petinfo10.Level);
                                petinfo10.TemplateID = (num40 == 0) ? petStarExp.NewID : num40;
                                PetMgr.CreateNewPropPet(ref petinfo10);
                                val = true;
                            }
                            else
                            {
                                client.Player.PropBag.RemoveTemplate(num35, num36);
                                petinfo10.currentStarExp += info14.Property2 * num36;
                            }
                            petBag.UpdatePet(petinfo10, slot);
                            petBag.SaveToDatabase(false);
                            if (val)
                            {
                                PetTemplateInfo info16 = PetMgr.FindPetTemplate(petinfo10.TemplateID);
                                if (info16.StarLevel > 3)
                                {
                                    GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(string.Format("Người chơi [{0}] đ\x00e3 thăng cấp th\x00e0nh c\x00f4ng th\x00fa cưng {1} l\x00ean {2}*", client.Player.PlayerCharacter.NickName, petinfo10.Name, info16.StarLevel)));
                                }
                                client.Player.EquipBag.UpdatePlayerProperties();
                            }
                            GSPacketIn in4 = new GSPacketIn(0x44);
                            in4.WriteByte(0x16);
                            in4.WriteBoolean(val);
                            client.Player.SendTCP(in4);
                        }
                        return 0;
                    }
                    return 0;
                }
                case 0x17:
                {
                    int num41 = packet.ReadInt();
                    num36 = packet.ReadInt();
                    if ((num41 == 0x2b9b) && (num36 > 0))
                    {
                        ItemTemplateInfo info17 = ItemMgr.FindItemTemplate(num41);
                        if (info17 != null)
                        {
                            if (client.Player.PropBag.GetItemCount(num41) < num36)
                            {
                                client.Player.SendMessage("Số lượng kh\x00f4ng c\x00f3 thực!");
                                return 0;
                            }
                            if (client.Player.PetBag.GetPetIsEquip() == null)
                            {
                                client.Player.SendMessage("Bạn chưa trang bị pet!");
                                return 0;
                            }
                            PetFightPropertyInfo petFightProperty = PetMgr.GetPetFightProperty(client.Player.PlayerCharacter.evolutionGrade + 1);
                            if (petFightProperty == null)
                            {
                                client.Player.SendMessage("Tăng trưởng của pet đ\x00e3 đạt tối đa!");
                                return 0;
                            }
                            int num43 = petFightProperty.Exp - client.Player.PlayerCharacter.evolutionExp;
                            if (num43 <= 0)
                            {
                                return 0;
                            }
                            int num44 = num43 / info17.Property2;
                            bool flag9 = false;
                            if (num44 <= num36)
                            {
                                client.Player.PropBag.RemoveTemplate(num41, num44);
                                PlayerInfo playerCharacter = client.Player.PlayerCharacter;
                                playerCharacter.evolutionGrade++;
                                PlayerInfo info19 = client.Player.PlayerCharacter;
                                info19.evolutionExp += info17.Property2 * num44;
                                flag9 = true;
                            }
                            else
                            {
                                client.Player.PropBag.RemoveTemplate(num41, num36);
                                PlayerInfo info20 = client.Player.PlayerCharacter;
                                info20.evolutionExp += info17.Property2 * num36;
                            }
                            if (flag9)
                            {
                                client.Player.EquipBag.UpdatePlayerProperties();
                            }
                            client.Out.SendUpdatePublicPlayer(client.Player.PlayerCharacter, client.Player.BattleData.MatchInfo);
                            GSPacketIn in5 = new GSPacketIn(0x44);
                            in5.WriteByte(0x17);
                            in5.WriteBoolean(flag9);
                            client.Player.SendTCP(in5);
                        }
                        return 0;
                    }
                    return 0;
                }
                default:
                    Console.WriteLine("????????????????pet_cmd: " + ((ePetType) num));
                    return 0;
            }
            if (flag3)
            {
                client.Player.AddPetScore(num20 / 100);
            }
            List<UsersPetinfo> list = PetMgr.CreateAdoptList(client.Player.PlayerCharacter.ID, playerLevel);
            client.Player.PetBag.ClearAdoptPets();
            foreach (UsersPetinfo petinfo3 in list)
            {
                client.Player.PetBag.AddAdoptPetTo(petinfo3, petinfo3.Place);
            }
        Label_06CE:
            client.Player.Out.SendRefreshPet(client.Player, client.Player.PetBag.GetAdoptPet(), null, flag2);
            return 0;
        Label_0CBC:
            petBag.SaveToDatabase(false);
            return 0;
        }

        private int RealMoney(int timebuy)
        {
            switch (timebuy)
            {
                case 1:
                    return 20;

                case 2:
                    return 0x13;

                case 3:
                    return 0x12;

                case 4:
                    return 0x11;

                case 5:
                    return 0x10;

                case 6:
                    return 15;

                case 7:
                    return 14;

                case 8:
                    return 13;

                case 9:
                    return 12;

                case 10:
                    return 11;

                case 11:
                    return 10;

                case 12:
                    return 9;

                case 13:
                    return 8;

                case 14:
                    return 7;

                case 15:
                    return 6;

                case 0x10:
                    return 5;

                case 0x11:
                    return 4;

                case 0x12:
                    return 3;

                case 0x13:
                    return 2;
            }
            return 1;
        }

        private void UpdatePetHandle(GameClient client, int ID)
        {
            PlayerInfo playerCharacter;
            UsersPetinfo[] pets;
            GamePlayer playerById = WorldMgr.GetPlayerById(ID);
            if (playerById != null)
            {
                playerCharacter = playerById.PlayerCharacter;
                pets = playerById.PetBag.GetPets();
            }
            else
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    playerCharacter = bussiness.GetUserSingleByUserID(ID);
                    pets = bussiness.GetUserPetSingles(ID);
                    PetEquipDataInfo[] eqPetSingles = bussiness.GetEqPetSingles(ID);
                    for (int i = 0; i < pets.Length; i++)
                    {
                        pets[i].EquipList = this.GetPetEquip(pets[i].ID, eqPetSingles);
                    }
                }
            }
            if ((pets != null) && (playerCharacter != null))
            {
                client.Out.SendPetInfo(playerCharacter, pets);
            }
        }
    }
}


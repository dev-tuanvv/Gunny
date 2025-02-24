namespace Game.Server.GameUtils
{
    using Bussiness.Managers;
    using Game.Logic;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    public class PlayerEquipInventory : PlayerInventory
    {
        private const int END_EQUIP_BAG = 80;
        private const int START_EQUIP_BAG = 0x1f;
        private static readonly int[] StyleIndex = new int[] { 1, 2, 3, 4, 5, 6, 11, 13, 14, 15, 0x10, 0x11, 0x12, 0x13, 20 };

        public PlayerEquipInventory(GamePlayer player) : base(player, true, 0x51, 0, 0x1f, true)
        {
        }

        public void AddBaseGemstoneProperty(SqlDataProvider.Data.ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
        {
            List<UserGemStone> gemStone = base.m_player.GemStone;
            foreach (UserGemStone stone in gemStone)
            {
                int figSpiritId = stone.FigSpiritId;
                int lv = Convert.ToInt32(stone.FigSpiritIdValue.Split(new char[] { '|' })[0].Split(new char[] { ',' })[0]);
                int length = stone.FigSpiritIdValue.Split(new char[] { '|' }).Length;
                int place = item.Place;
                switch (item.Place)
                {
                    case 2:
                        attack += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * length;
                        break;

                    case 3:
                        lucky += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * length;
                        break;

                    case 5:
                        agility += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * length;
                        break;

                    case 11:
                        defence += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * length;
                        break;

                    case 13:
                        hp += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * length;
                        break;
                }
            }
        }

        public void AddBaseLatentProperty(SqlDataProvider.Data.ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky)
        {
            if (!((item == null) || item.IsValidLatentEnergy()))
            {
                string[] strArray = item.latentEnergyCurStr.Split(new char[] { ',' });
                attack += Convert.ToInt32(strArray[0]);
                defence += Convert.ToInt32(strArray[1]);
                agility += Convert.ToInt32(strArray[2]);
                lucky += Convert.ToInt32(strArray[3]);
            }
        }

        public void AddBaseTotemProperty(PlayerInfo p, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
        {
            attack += TotemMgr.GetTotemProp(p.totemId, "att");
            defence += TotemMgr.GetTotemProp(p.totemId, "def");
            agility += TotemMgr.GetTotemProp(p.totemId, "agi");
            lucky += TotemMgr.GetTotemProp(p.totemId, "luc");
            hp += TotemMgr.GetTotemProp(p.totemId, "blo");
        }

        public void AddBeadProperty(int place, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
        {
            SqlDataProvider.Data.ItemInfo itemAt = base.m_player.BeadBag.GetItemAt(place);
            if (itemAt != null)
            {
                this.AddRuneProperty(itemAt, ref attack, ref defence, ref agility, ref lucky, ref hp);
            }
        }

        public override bool AddItem(SqlDataProvider.Data.ItemInfo item)
        {
            return base.AddItem(item, 0x1f, 0x4f);
        }

        public override bool AddItemTo(SqlDataProvider.Data.ItemInfo item, int place)
        {
            if (!(!Equip.isDress(item.Template) || base.IsEquipSlot(place)))
            {
                place = this.FindFirstEmptySlot(0x51);
            }
            if (base.AddItemTo(item, place))
            {
                item.UserID = base.m_player.PlayerCharacter.ID;
                item.IsExist = true;
                return true;
            }
            return false;
        }

        public void AddRuneProperty(SqlDataProvider.Data.ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
        {
            RuneTemplateInfo info = RuneMgr.FindRuneByTemplateID(item.TemplateID);
            if (info != null)
            {
                string[] strArray = info.Attribute1.Split(new char[] { '|' });
                string[] strArray2 = info.Attribute2.Split(new char[] { '|' });
                int index = 0;
                int num2 = 0;
                if (item.Hole1 > info.BaseLevel)
                {
                    if (strArray.Length > 1)
                    {
                        index = 1;
                    }
                    if (strArray2.Length > 1)
                    {
                        num2 = 1;
                    }
                }
                int num3 = Convert.ToInt32(strArray[index]);
                int num4 = Convert.ToInt32(strArray2[num2]);
                switch (info.Type1)
                {
                    case 0x1f:
                        attack += num3;
                        hp += num4;
                        break;

                    case 0x20:
                        defence += num3;
                        hp += num4;
                        break;

                    case 0x21:
                        agility += num3;
                        hp += num4;
                        break;

                    case 0x22:
                        lucky += num3;
                        hp += num4;
                        break;

                    case 0x23:
                        hp += num4;
                        break;

                    case 0x24:
                        hp += num4;
                        break;

                    case 0x25:
                        hp += num3;
                        break;
                }
            }
        }

        public override bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem, int count)
        {
            return base.AddTemplate(cloneItem, count, 0x1f, 0x4f);
        }

        public bool CanEquipSlotContains(int slot, ItemTemplateInfo temp)
        {
            if ((temp.CategoryID == 8) || (temp.CategoryID == 0x1c))
            {
                return ((slot == 7) || (slot == 8));
            }
            if ((temp.CategoryID == 9) || (temp.CategoryID == 0x1d))
            {
                if (Equip.isWeddingRing(temp))
                {
                    return (((slot == 9) || (slot == 10)) || (slot == 0x10));
                }
                return ((slot == 9) || (slot == 10));
            }
            if (temp.CategoryID == 13)
            {
                return (slot == 11);
            }
            if (temp.CategoryID == 14)
            {
                return (slot == 12);
            }
            if (temp.CategoryID == 15)
            {
                return (slot == 13);
            }
            if (temp.CategoryID == 0x10)
            {
                return (slot == 14);
            }
            if ((temp.CategoryID == 0x11) || (temp.CategoryID == 0x1f))
            {
                return (slot == 15);
            }
            if (temp.CategoryID == 0x1b)
            {
                return (slot == 6);
            }
            if (temp.CategoryID == 40)
            {
                return (slot == 0x11);
            }
            return ((temp.CategoryID - 1) == slot);
        }

        public void EquipBuffer()
        {
            base.m_player.EquipEffect.Clear();
            for (int i = 0; i < 0x1f; i++)
            {
                SqlDataProvider.Data.ItemInfo itemAt = base.m_player.BeadBag.GetItemAt(i);
                if (itemAt != null)
                {
                    RuneTemplateInfo info2 = RuneMgr.FindRuneByTemplateID(itemAt.TemplateID);
                    if ((info2 != null) && (((info2.Type1 == 0x25) || (info2.Type1 == 0x27)) || (info2.Type1 < 0x1f)))
                    {
                        base.m_player.AddBeadEffect(itemAt);
                    }
                }
            }
        }

        public int FindItemEpuipSlot(ItemTemplateInfo item)
        {
            switch (item.CategoryID)
            {
                case 8:
                case 0x1c:
                    if (base.m_items[7] == null)
                    {
                        return 7;
                    }
                    return 8;

                case 9:
                case 0x1d:
                    if (base.m_items[9] == null)
                    {
                        return 9;
                    }
                    return 10;

                case 13:
                    return 11;

                case 14:
                    return 12;

                case 15:
                    return 13;

                case 0x10:
                    return 14;

                case 0x11:
                    return 15;

                case 0x1b:
                    return 6;

                case 0x1f:
                    return 15;

                case 40:
                    return 0x11;
            }
            return (item.CategoryID - 1);
        }

        public List<SqlDataProvider.Data.ItemInfo> GetAllEquipItems()
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            for (int i = 0; i < 0x1f; i++)
            {
                SqlDataProvider.Data.ItemInfo item = base.m_items[i];
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public Dictionary<string, int> GetProp(int attack, int defence, int agility, int lucky, int hp)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("Attack", attack);
            dictionary.Add("Defence", defence);
            dictionary.Add("Agility", agility);
            dictionary.Add("Luck", lucky);
            dictionary.Add("HP", hp);
            return dictionary;
        }

        public void GetUserNimbus()
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < 0x1f; i++)
            {
                SqlDataProvider.Data.ItemInfo itemAt = this.GetItemAt(i);
                if (itemAt != null)
                {
                    int num4 = itemAt.StrengthenLevel + itemAt.LianGrade;
                    if ((num4 >= 5) && (num4 <= 8))
                    {
                        if ((itemAt.Template.CategoryID == 1) || (itemAt.Template.CategoryID == 5))
                        {
                            num = (num > 1) ? num : 1;
                        }
                        if (itemAt.Template.CategoryID == 7)
                        {
                            num2 = (num2 > 1) ? num2 : 1;
                        }
                    }
                    if ((itemAt.StrengthenLevel == 9) && (itemAt.LianGrade == 0))
                    {
                        if ((itemAt.Template.CategoryID == 1) || (itemAt.Template.CategoryID == 5))
                        {
                            num = (num > 1) ? num : 4;
                        }
                        if (itemAt.Template.CategoryID == 7)
                        {
                            num2 = (num2 > 1) ? num2 : 2;
                        }
                    }
                    if ((itemAt.StrengthenLevel == 9) && ((itemAt.LianGrade >= 1) && (itemAt.LianGrade < 5)))
                    {
                        if ((itemAt.Template.CategoryID == 1) || (itemAt.Template.CategoryID == 5))
                        {
                            num = (num > 1) ? num : 3;
                        }
                        if (itemAt.Template.CategoryID == 7)
                        {
                            num2 = (num2 > 1) ? num2 : 3;
                        }
                    }
                    if (itemAt.LianGrade == 5)
                    {
                        if ((itemAt.Template.CategoryID == 1) || (itemAt.Template.CategoryID == 5))
                        {
                            num = (num > 1) ? num : 5;
                        }
                        if (itemAt.Template.CategoryID == 7)
                        {
                            num2 = (num2 > 1) ? num2 : 5;
                        }
                    }
                }
            }
            base.m_player.PlayerCharacter.Nimbus = (num * 100) + num2;
            base.m_player.Out.SendUpdatePublicPlayer(base.m_player.PlayerCharacter, base.m_player.BattleData.MatchInfo);
        }

        public override void LoadFromDatabase()
        {
            base.BeginChanges();
            try
            {
                base.LoadFromDatabase();
                List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo>();
                for (int i = 0; i < 0x1f; i++)
                {
                    SqlDataProvider.Data.ItemInfo item = base.m_items[i];
                    if ((base.m_items[i] != null) && !base.m_items[i].IsValidItem())
                    {
                        int toSlot = this.FindFirstEmptySlot(0x1f);
                        if (toSlot >= 0)
                        {
                            this.MoveItem(item.Place, toSlot, item.Count);
                        }
                        else
                        {
                            items.Add(item);
                        }
                    }
                    else if ((((base.m_items[i] != null) && Equip.isDress(base.m_items[i].Template)) && (base.m_items[i].Place >= 0x1f)) && (base.m_items[i].Place <= 80))
                    {
                        int num3 = this.FindFirstEmptySlot(0x51);
                        this.MoveItem(item.Place, num3, item.Count);
                    }
                    else if (((base.m_items[i] != null) && !Equip.isDress(base.m_items[i].Template)) && (base.m_items[i].Place > 80))
                    {
                        int num4 = base.FindFirstEmptySlot(0x1f, 80);
                        if (num4 >= 0)
                        {
                            this.MoveItem(item.Place, num4, item.Count);
                        }
                        else
                        {
                            items.Add(item);
                        }
                    }
                }
                if (items.Count > 0)
                {
                    base.m_player.SendItemsToMail(items, null, "Item qu\x00e1 hạn trả về thư.", eMailType.ItemOverdue);
                    base.m_player.Out.SendMailResponse(base.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
                }
            }
            finally
            {
                base.CommitChanges();
            }
        }

        public override bool MoveItem(int fromSlot, int toSlot, int count)
        {
            if (base.m_items[fromSlot] == null)
            {
                return false;
            }
            if ((((base.m_items[fromSlot] != null) && (base.m_items[toSlot] != null)) && (Equip.isDress(base.m_items[fromSlot].Template) && Equip.isDress(base.m_items[toSlot].Template))) && base.m_items[toSlot].CanStackedTo(base.m_items[fromSlot]))
            {
                if (base.m_items[toSlot].ValidDate != 0)
                {
                    if (base.m_items[fromSlot].ValidDate != 0)
                    {
                        SqlDataProvider.Data.ItemInfo info1 = base.m_items[toSlot];
                        info1.ValidDate += base.m_items[fromSlot].ValidDate;
                    }
                    else
                    {
                        base.m_items[toSlot].ValidDate = 0;
                    }
                }
                base.RemoveItemAt(fromSlot);
                this.UpdateItem(base.m_items[toSlot]);
                return true;
            }
            if (((base.IsEquipSlot(fromSlot) && !base.IsEquipSlot(toSlot)) && (base.m_items[toSlot] != null)) && (base.m_items[toSlot].Template.CategoryID != base.m_items[fromSlot].Template.CategoryID))
            {
                if (!this.CanEquipSlotContains(fromSlot, base.m_items[toSlot].Template))
                {
                    if (Equip.isDress(base.m_items[fromSlot].Template))
                    {
                        toSlot = base.FindFirstEmptySlot(0x51);
                    }
                    else
                    {
                        toSlot = base.FindFirstEmptySlot(0x1f, 0x4f);
                    }
                }
            }
            else
            {
                if (base.IsEquipSlot(toSlot))
                {
                    if (!this.CanEquipSlotContains(toSlot, base.m_items[fromSlot].Template))
                    {
                        this.UpdateItem(base.m_items[fromSlot]);
                        return false;
                    }
                    if (!(base.m_player.CanEquip(base.m_items[fromSlot].Template) && base.m_items[fromSlot].IsValidItem()))
                    {
                        this.UpdateItem(base.m_items[fromSlot]);
                        return false;
                    }
                }
                if (!((!base.IsEquipSlot(fromSlot) || (base.m_items[toSlot] == null)) || this.CanEquipSlotContains(fromSlot, base.m_items[toSlot].Template)))
                {
                    this.UpdateItem(base.m_items[toSlot]);
                    return false;
                }
            }
            if (!(Equip.isDress(base.m_items[fromSlot].Template) || (toSlot <= 80)))
            {
                toSlot = base.FindFirstEmptySlot(0x1f, 0x4f);
            }
            else if (!(!Equip.isDress(base.m_items[fromSlot].Template) || base.IsEquipSlot(toSlot)))
            {
                toSlot = base.FindFirstEmptySlot(0x51);
            }
            return base.MoveItem(fromSlot, toSlot, count);
        }

        public override void UpdateChangedPlaces()
        {
            int[] numArray = base.m_changedPlaces.ToArray();
            bool flag = false;
            foreach (int num2 in numArray)
            {
                if (base.IsEquipSlot(num2))
                {
                    SqlDataProvider.Data.ItemInfo itemAt = this.GetItemAt(num2);
                    if (itemAt != null)
                    {
                        base.m_player.OnUsingItem(itemAt.TemplateID);
                        itemAt.IsBinds = true;
                        if (!itemAt.IsUsed)
                        {
                            itemAt.IsUsed = true;
                            itemAt.BeginDate = DateTime.Now;
                        }
                    }
                    flag = true;
                    break;
                }
            }
            base.UpdateChangedPlaces();
            if (flag)
            {
                this.UpdatePlayerProperties();
            }
        }

        public void UpdatePlayerProperties()
        {
            base.m_player.BeginChanges();
            try
            {
                int attack = 0;
                int defence = 0;
                int agility = 0;
                int lucky = 0;
                int magicAttack = 0;
                int magicDefence = 0;
                int hp = 0;
                int level = 0;
                string style = "";
                string colors = "";
                string skin = "";
                int num9 = 0;
                int num10 = 0;
                int num11 = 0;
                int num12 = 0;
                int num13 = 0;
                int num14 = 0;
                int num15 = 0;
                int num16 = 0;
                int num17 = 0;
                int num18 = 0;
                int num19 = 0;
                int num20 = 0;
                int num21 = 0;
                int num22 = 0;
                int num23 = 0;
                int num24 = 0;
                int num25 = 0;
                int num26 = 0;
                int num27 = 0;
                int num28 = 0;
                int num29 = 0;
                int num30 = 0;
                int num31 = 0;
                int num32 = 0;
                int num33 = 0;
                int num34 = 0;
                int num35 = 0;
                int num36 = 0;
                int num37 = 0;
                int num38 = 0;
                int num39 = 0;
                int num40 = 0;
                int num41 = 0;
                int num43 = 0;
                int num44 = 0;
                base.m_player.UpdatePet(base.m_player.PetBag.GetPetIsEquip());
                List<UsersCardInfo> cards = base.m_player.CardBag.GetCards(0, 5);
                lock (base.m_lock)
                {
                    style = (base.m_items[0] == null) ? "" : (base.m_items[0].TemplateID.ToString() + "|" + base.m_items[0].Template.Pic);
                    colors = (base.m_items[0] == null) ? "" : base.m_items[0].Color;
                    skin = (base.m_items[5] == null) ? "" : base.m_items[5].Skin;
                    for (int i = 0; i < 0x1f; i++)
                    {
                        SqlDataProvider.Data.ItemInfo item = base.m_items[i];
                        if (item != null)
                        {
                            if (item.LianGrade > 0)
                            {
                                attack += item.Attack + ((int) base.m_player.getLianAddition((double) item.Attack, (double) item.LianGrade));
                                defence += item.Defence + ((int) base.m_player.getLianAddition((double) item.Defence, (double) item.LianGrade));
                                agility += item.Agility + ((int) base.m_player.getLianAddition((double) item.Agility, (double) item.LianGrade));
                                lucky += item.Luck + ((int) base.m_player.getLianAddition((double) item.Luck, (double) item.LianGrade));
                            }
                            else
                            {
                                attack += item.Attack;
                                defence += item.Defence;
                                agility += item.Agility;
                                lucky += item.Luck;
                            }
                            level = (level > (item.StrengthenLevel + item.LianGrade)) ? level : (item.StrengthenLevel + item.LianGrade);
                            this.AddBaseLatentProperty(item, ref attack, ref defence, ref agility, ref lucky);
                            this.AddBaseGemstoneProperty(item, ref num9, ref num10, ref num11, ref num12, ref num13);
                            SubActiveConditionInfo subActiveInfo = SubActiveMgr.GetSubActiveInfo(item);
                            if (subActiveInfo != null)
                            {
                                attack += subActiveInfo.GetValue("1");
                                defence += subActiveInfo.GetValue("2");
                                agility += subActiveInfo.GetValue("3");
                                lucky += subActiveInfo.GetValue("4");
                                hp += subActiveInfo.GetValue("5");
                            }
                        }
                        this.AddBeadProperty(i, ref num28, ref num29, ref num30, ref num31, ref num32);
                    }
                    UserRankInfo rank = base.m_player.Rank.GetRank(base.m_player.PlayerCharacter.Honor);
                    if (rank != null)
                    {
                        attack += rank.Attack;
                        defence += rank.Defence;
                        agility += rank.Agility;
                        lucky += rank.Luck;
                        hp += rank.HP;
                    }
                    foreach (UsersCardInfo info4 in cards)
                    {
                        if (info4.Count != -1)
                        {
                            if (info4.Place < 5)
                            {
                                num14 += CardMgr.GetProp(info4, 0);
                                num15 += CardMgr.GetProp(info4, 1);
                                num16 += CardMgr.GetProp(info4, 2);
                                num17 += CardMgr.GetProp(info4, 3);
                                num14 += info4.Attack;
                                num15 += info4.Defence;
                                num16 += info4.Agility;
                                num17 += info4.Luck;
                            }
                            if (info4.TemplateID > 0)
                            {
                                ItemTemplateInfo info5 = ItemMgr.FindItemTemplate(info4.TemplateID);
                                if (info5 != null)
                                {
                                    num14 += info5.Attack;
                                    num15 += info5.Defence;
                                    num16 += info5.Agility;
                                    num17 += info5.Luck;
                                }
                            }
                        }
                    }
                    num23 += ExerciseMgr.GetExercise(base.m_player.PlayerCharacter.Texp.attTexpExp, "A");
                    num24 += ExerciseMgr.GetExercise(base.m_player.PlayerCharacter.Texp.defTexpExp, "D");
                    num25 += ExerciseMgr.GetExercise(base.m_player.PlayerCharacter.Texp.spdTexpExp, "AG");
                    num26 += ExerciseMgr.GetExercise(base.m_player.PlayerCharacter.Texp.lukTexpExp, "L");
                    num27 += ExerciseMgr.GetExercise(base.m_player.PlayerCharacter.Texp.hpTexpExp, "H");
                    for (int j = 0; j < StyleIndex.Length; j++)
                    {
                        style = style + ",";
                        colors = colors + ",";
                        if (base.m_items[StyleIndex[j]] != null)
                        {
                            object obj3 = style;
                            style = string.Concat(new object[] { obj3, base.m_items[StyleIndex[j]].TemplateID, "|", base.m_items[StyleIndex[j]].Pic });
                            colors = colors + base.m_items[StyleIndex[j]].Color;
                        }
                    }
                    this.EquipBuffer();
                }
                attack += (((((num9 + num14) + num18) + num23) + num28) + num33) + num38;
                defence += (((((num10 + num15) + num19) + num24) + num29) + num34) + num39;
                agility += (((((num11 + num16) + num20) + num25) + num30) + num35) + num40;
                lucky += (((((num12 + num17) + num21) + num26) + num31) + num36) + num41;
                magicAttack += num43;
                magicDefence += num44;
                hp += (((num13 + num22) + num27) + num32) + num37;
                base.m_player.UpdateBaseProperties(attack, defence, agility, lucky, magicAttack, magicDefence, hp);
                base.m_player.UpdateStyle(style, colors, skin);
                base.m_player.ApertureEquip(level);
                base.m_player.UpdateWeapon(base.m_items[6]);
                base.m_player.UpdateSecondWeapon(base.m_items[15]);
                base.m_player.UpdateReduceDame(base.m_items[0x11]);
                base.m_player.UpdateHealstone(base.m_items[0x12]);
                base.m_player.PlayerProp.CreateProp(true, "Texp", num23, num24, num25, num26, num27);
                base.m_player.PlayerProp.CreateProp(true, "Card", num14, num15, num16, num17, 0);
                base.m_player.PlayerProp.CreateProp(true, "Gem", num9, num10, num11, num12, num13);
                base.m_player.PlayerProp.CreateProp(true, "Bead", num28, num29, num30, num31, num32);
                base.m_player.UpdateFightPower();
                this.GetUserNimbus();
                base.m_player.PlayerProp.ViewCurrent();
            }
            finally
            {
                base.m_player.CommitChanges();
            }
        }

        public int SlotAvatarBegin
        {
            get
            {
                return 0x1f;
            }
        }

        public int SlotAvatarEnd
        {
            get
            {
                return 0x181;
            }
        }
    }
}


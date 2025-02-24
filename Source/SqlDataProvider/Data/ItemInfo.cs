namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;

    public class ItemInfo : DataObject
    {
        private DateTime _advanceDate;
        private int _agilityCompose;
        private int _attackCompose;
        private int _bagType;
        private int _beadExp;
        private bool _beadIsLock;
        private int _beadLevel;
        private DateTime _beginDate;
        private int _Bless;
        private int _Blood;
        private string _color;
        private int _count;
        private int _Damage;
        private int _defendCompose;
        private DateTime _goldBeginTime;
        private ItemTemplateInfo _goldEquip;
        private int _goldValidDate;
        private bool _GoodsLock;
        private int _Guard;
        private int _hole1;
        private int _hole2;
        private int _hole3;
        private int _hole4;
        private int _hole5;
        private int _hole5Exp;
        private int _hole5Level;
        private int _hole6;
        private int _hole6Exp;
        private int _hole6Level;
        private bool _isBinds;
        private bool _isExist;
        private bool _isGold;
        private bool _isJudage;
        private bool _isLogs;
        private bool _isShowBind;
        private bool _isTips;
        private bool _isUsed;
        private int _itemID;
        private string _latentEnergyCurStr;
        private DateTime _latentEnergyEndTime;
        private string _latentEnergyNewStr;
        private int _LianExp;
        private int _LianGrade;
        private int _luckCompose;
        private int _magicAttack;
        private int _magicDefence;
        private int _place;
        private DateTime _removeDate;
        private int _removeType;
        private string _skin;
        private int _strengthenExp;
        private int _strengthenLevel;
        private int _strengthenTimes;
        private ItemTemplateInfo _template;
        private int _templateId;
        private int _userID;
        private int _validDate;

        public ItemInfo(ItemTemplateInfo temp)
        {
            this._template = temp;
        }

        public bool CanEquip()
        {
            return ((this._template.CategoryID < 10) || ((this._template.CategoryID >= 13) && (this._template.CategoryID <= 0x10)));
        }

        public bool CanLatentEnergy()
        {
            switch (this.Template.CategoryID)
            {
                case 2:
                case 3:
                case 4:
                case 6:
                case 13:
                case 15:
                    return true;

                case 5:
                    return false;

                case 14:
                    return false;
            }
            return false;
        }

        public bool CanStackedTo(SqlDataProvider.Data.ItemInfo to)
        {
            if ((((this._templateId == to.TemplateID) && (this.Template.MaxCount > 1)) && (this._isBinds == to.IsBinds)) && (this._isUsed == to._isUsed))
            {
                if ((this.ValidDate == 0) || ((this.BeginDate.Date == to.BeginDate.Date) && (this.ValidDate == this.ValidDate)))
                {
                    return true;
                }
            }
            else if ((((this._templateId == to.TemplateID) && Equip.isDress(this.Template)) && Equip.isDress(to.Template)) && (to.StrengthenLevel <= 0))
            {
                return true;
            }
            return false;
        }

        public SqlDataProvider.Data.ItemInfo Clone()
        {
            return new SqlDataProvider.Data.ItemInfo(this._template) { 
                _userID = this._userID, _validDate = this._validDate, _templateId = this._templateId, _goldEquip = this._goldEquip, _strengthenLevel = this._strengthenLevel, _strengthenExp = this._strengthenExp, _luckCompose = this._luckCompose, _itemID = 0, _isJudage = this._isJudage, _isExist = this._isExist, _isBinds = this._isBinds, _isUsed = this._isUsed, _defendCompose = this._defendCompose, _count = this._count, _color = this._color, Skin = this._skin, 
                _beginDate = this._beginDate, _attackCompose = this._attackCompose, _agilityCompose = this._agilityCompose, _bagType = this._bagType, _isDirty = true, _removeDate = this._removeDate, _removeType = this._removeType, _hole1 = this._hole1, _hole2 = this._hole2, _hole3 = this._hole3, _hole4 = this._hole4, _hole5 = this._hole5, _hole6 = this._hole6, _hole5Exp = this._hole5Exp, _hole5Level = this._hole5Level, _hole6Exp = this._hole6Exp, 
                _hole6Level = this._hole6Level, _isGold = this._isGold, _goldBeginTime = this._goldBeginTime, _goldValidDate = this._goldValidDate, _latentEnergyCurStr = this._latentEnergyCurStr, _latentEnergyNewStr = this._latentEnergyNewStr, _latentEnergyEndTime = this._latentEnergyEndTime, _beadExp = this._beadExp, _beadLevel = this._beadLevel, _beadIsLock = this._beadIsLock, _isShowBind = this._isShowBind, _Damage = this._Damage, _Guard = this._Guard, _Bless = this._Bless, _Blood = this._Blood, AdvanceDate = this._advanceDate
             };
        }

        public static SqlDataProvider.Data.ItemInfo CloneFromTemplate(ItemTemplateInfo goods, SqlDataProvider.Data.ItemInfo item)
        {
            if (goods == null)
            {
                return null;
            }
            SqlDataProvider.Data.ItemInfo info = new SqlDataProvider.Data.ItemInfo(goods) {
                AgilityCompose = item.AgilityCompose,
                AttackCompose = item.AttackCompose,
                BeginDate = item.BeginDate,
                Color = item.Color,
                Skin = item.Skin,
                DefendCompose = item.DefendCompose,
                IsBinds = item.IsBinds,
                Place = item.Place,
                BagType = item.BagType,
                IsUsed = item.IsUsed,
                IsDirty = item.IsDirty,
                IsExist = item.IsExist,
                IsJudge = item.IsJudge,
                LuckCompose = item.LuckCompose,
                StrengthenExp = item.StrengthenExp,
                StrengthenLevel = item.StrengthenLevel,
                TemplateID = goods.TemplateID,
                ValidDate = item.ValidDate,
                _template = goods,
                Count = item.Count,
                _removeDate = item._removeDate,
                _removeType = item._removeType,
                Hole1 = item.Hole1,
                Hole2 = item.Hole2,
                Hole3 = item.Hole3,
                Hole4 = item.Hole4,
                Hole5 = item.Hole5,
                Hole6 = item.Hole6,
                Hole5Level = item.Hole5Level,
                Hole5Exp = item.Hole5Exp,
                Hole6Level = item.Hole6Level,
                Hole6Exp = item.Hole6Exp,
                goldBeginTime = item.goldBeginTime,
                goldValidDate = item.goldValidDate,
                latentEnergyEndTime = item.latentEnergyEndTime,
                latentEnergyCurStr = item.latentEnergyCurStr,
                latentEnergyNewStr = item.latentEnergyNewStr,
                AdvanceDate = item.AdvanceDate
            };
            OpenHole(ref info);
            return info;
        }

        public void Copy(SqlDataProvider.Data.ItemInfo item)
        {
            this._userID = item.UserID;
            this._validDate = item.ValidDate;
            this._templateId = item.TemplateID;
            this._strengthenLevel = item.StrengthenLevel;
            this._strengthenExp = item.StrengthenExp;
            this._luckCompose = item.LuckCompose;
            this._itemID = 0;
            this._isJudage = item.IsJudge;
            this._isExist = item.IsExist;
            this._isBinds = item.IsBinds;
            this._isUsed = item.IsUsed;
            this._defendCompose = item.DefendCompose;
            this._count = item.Count;
            this._color = item.Color;
            this._skin = item.Skin;
            this._beginDate = item.BeginDate;
            this._attackCompose = item.AttackCompose;
            this._agilityCompose = item.AgilityCompose;
            this._bagType = item.BagType;
            base._isDirty = item.IsDirty;
            this._removeDate = item.RemoveDate;
            this._removeType = item.RemoveType;
            this._hole1 = item.Hole1;
            this._hole2 = item.Hole2;
            this._hole3 = item.Hole3;
            this._hole4 = item.Hole4;
            this._hole5 = item.Hole5;
            this._hole6 = item.Hole6;
            this._hole5Exp = item.Hole5Exp;
            this._hole5Level = item.Hole5Level;
            this._hole6Exp = item.Hole6Exp;
            this._hole6Level = item.Hole6Level;
            this._isGold = item.IsGold;
            this._goldBeginTime = item.goldBeginTime;
            this._goldValidDate = item.goldValidDate;
            this._strengthenExp = item.StrengthenExp;
            this._latentEnergyCurStr = item.latentEnergyCurStr;
            this._latentEnergyNewStr = item._latentEnergyNewStr;
            this._latentEnergyEndTime = item._latentEnergyEndTime;
            this._beadExp = item.beadExp;
            this._beadLevel = item.beadLevel;
            this._beadIsLock = item.beadIsLock;
            this._isShowBind = item.isShowBind;
            this._Damage = item.Damage;
            this._Guard = item.Guard;
            this._Bless = item.Bless;
            this._Blood = item.Blood;
            this._advanceDate = item.AdvanceDate;
        }

        public static SqlDataProvider.Data.ItemInfo CreateFromTemplate(ItemTemplateInfo goods, int count, int type)
        {
            if (goods == null)
            {
                return null;
            }
            return new SqlDataProvider.Data.ItemInfo(goods) { 
                AgilityCompose = 0, AttackCompose = 0, BeginDate = DateTime.Now, Color = "", Skin = "", DefendCompose = 0, IsUsed = false, IsDirty = false, IsExist = true, IsJudge = true, LuckCompose = 0, StrengthenLevel = 0, TemplateID = goods.TemplateID, ValidDate = 0, Count = count, IsBinds = goods.BindType == 1, 
                _removeDate = DateTime.Now, _removeType = type, Hole1 = -1, Hole2 = -1, Hole3 = -1, Hole4 = -1, Hole5 = -1, Hole6 = -1, Hole5Exp = 0, Hole5Level = 0, Hole6Exp = 0, Hole6Level = 0, goldValidDate = 0, goldBeginTime = DateTime.Now, StrengthenExp = 0, latentEnergyCurStr = "0,0,0,0", 
                latentEnergyNewStr = "0,0,0,0", latentEnergyEndTime = DateTime.Now, beadExp = 0, beadLevel = 0, beadIsLock = false, isShowBind = false, Damage = 0, Guard = 0, Bless = 0, Blood = 0, MagicAttack = 0, MagicDefence = 0, AdvanceDate = DateTime.Now
             };
        }

        public int eqType()
        {
            switch (this._template.CategoryID)
            {
                case 0x33:
                    return 1;

                case 0x34:
                    return 2;
            }
            return 0;
        }

        public static void FindSpecialItemInfo(SqlDataProvider.Data.ItemInfo info, ref int gold, ref int money, ref int giftToken)
        {
            switch (info.TemplateID)
            {
                case -300:
                    giftToken += info.Count;
                    info = null;
                    break;

                case -200:
                    money += info.Count;
                    info = null;
                    break;

                case -100:
                    gold += info.Count;
                    info = null;
                    break;
            }
        }

        public string GetBagName()
        {
            switch (this._template.CategoryID)
            {
                case 10:
                case 11:
                    return "Game.Server.GameObjects.Prop";

                case 12:
                    return "Game.Server.GameObjects.Task";
            }
            return "Game.Server.GameObjects.Equip";
        }

        public static void GetItemPrice(int Prices, int Values, decimal beat, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int iTemplateID, ref int iCount)
        {
            iTemplateID = 0;
            iCount = 0;
            switch (Prices)
            {
                case -4:
                    gifttoken += (int) (Values * beat);
                    break;

                case -3:
                    offer += (int) (Values * beat);
                    break;

                case -2:
                    gold += (int) (Values * beat);
                    break;

                case -1:
                    money += (int) (Values * beat);
                    break;

                default:
                    if (Prices > 0)
                    {
                        iTemplateID = Prices;
                        iCount = Values;
                    }
                    break;
            }
        }

        public int GetRemainDate()
        {
            if (this.ValidDate == 0)
            {
                return 0x7fffffff;
            }
            if (!this._isUsed)
            {
                return this.ValidDate;
            }
            int num = DateTime.Compare(this._beginDate.AddDays((double) this._validDate), DateTime.Now);
            return ((num < 0) ? 0 : num);
        }

        public bool IsAdvanceDate()
        {
            return (this._advanceDate.Date < DateTime.Now.Date);
        }

        public bool IsBead()
        {
            return ((this._template.Property1 == 0x1f) && (this._template.CategoryID == 11));
        }

        public bool IsCard()
        {
            int categoryID = this._template.CategoryID;
            if (categoryID != 11)
            {
                return (categoryID == 0x12);
            }
            return ((this._template.TemplateID == 0x1b5ec) || (this._template.TemplateID == 0x1b616));
        }

        public bool isDress()
        {
            switch (this._template.CategoryID)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 13:
                case 15:
                    return true;
            }
            return false;
        }

        public bool isDrill(int holelv)
        {
            switch (this._template.TemplateID)
            {
                case 0x2b12:
                    return (holelv == 2);

                case 0x2b13:
                    return (holelv == 3);

                case 0x2b1a:
                    return (holelv == 4);

                case 0x2b1b:
                    return (holelv == 0);

                case 0x2b1c:
                    return (holelv == 1);
            }
            return false;
        }

        public bool IsEquipPet()
        {
            return (((this._template.CategoryID == 50) || (this._template.CategoryID == 0x33)) || (this._template.CategoryID == 0x34));
        }

        public bool isGemStone()
        {
            return (this._template.TemplateID == 0x18704);
        }

        public bool IsProp()
        {
            int categoryID = this._template.CategoryID;
            if (categoryID <= 0x12)
            {
                if ((categoryID != 11) && (categoryID != 0x12))
                {
                    return false;
                }
            }
            else
            {
                switch (categoryID)
                {
                    case 0x20:
                    case 0x22:
                    case 0x23:
                    case 40:
                        goto Label_0066;

                    case 0x21:
                        return false;
                }
                return false;
            }
        Label_0066:
            return true;
        }

        public bool isTexp()
        {
            return (this._template.CategoryID == 20);
        }

        public bool IsValidGoldItem()
        {
            return ((this._goldValidDate > 0) && (DateTime.Compare(this._goldBeginTime.AddDays((double) this._goldValidDate), DateTime.Now) > 0));
        }

        public bool IsValidItem()
        {
            return (((this._validDate == 0) || !this._isUsed) || (DateTime.Compare(this._beginDate.AddDays((double) this._validDate), DateTime.Now) > 0));
        }

        public bool IsValidLatentEnergy()
        {
            return (this._latentEnergyEndTime.Date < DateTime.Now.Date);
        }

        public static void OpenHole(ref SqlDataProvider.Data.ItemInfo item)
        {
            string[] strArray = item.Template.Hole.Split(new char[] { '|' });
            for (int i = 0; i < strArray.Length; i++)
            {
                string[] strArray2 = strArray[i].Split(new char[] { ',' });
                if ((item.StrengthenLevel >= Convert.ToInt32(strArray2[0])) && (Convert.ToInt32(strArray2[1]) != -1))
                {
                    switch (i)
                    {
                        case 0:
                            if (item.Hole1 < 0)
                            {
                                item.Hole1 = 0;
                            }
                            break;

                        case 1:
                            if (item.Hole2 < 0)
                            {
                                item.Hole2 = 0;
                            }
                            break;

                        case 2:
                            if (item.Hole3 < 0)
                            {
                                item.Hole3 = 0;
                            }
                            break;

                        case 3:
                            if (item.Hole4 < 0)
                            {
                                item.Hole4 = 0;
                            }
                            break;

                        case 4:
                            if (item.Hole5 < 0)
                            {
                                item.Hole5 = 0;
                            }
                            break;

                        case 5:
                            if (item.Hole6 < 0)
                            {
                                item.Hole6 = 0;
                            }
                            break;
                    }
                }
            }
        }

        public void ResetLatentEnergy()
        {
            this._latentEnergyCurStr = "0,0,0,0";
            this._latentEnergyNewStr = "0,0,0,0";
        }

        public static List<int> SetItemType(ShopItemInfo shop, int type, ref int gold, ref int money, ref int offer, ref int gifttoken)
        {
            int iTemplateID = 0;
            int iCount = 0;
            List<int> list = new List<int>();
            if (type == 1)
            {
                GetItemPrice(shop.APrice1, shop.AValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
                GetItemPrice(shop.APrice2, shop.AValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
                GetItemPrice(shop.APrice3, shop.AValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
            }
            if (type == 2)
            {
                GetItemPrice(shop.BPrice1, shop.BValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
                GetItemPrice(shop.BPrice2, shop.BValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
                GetItemPrice(shop.BPrice3, shop.BValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
            }
            if (type == 3)
            {
                GetItemPrice(shop.CPrice1, shop.CValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
                GetItemPrice(shop.CPrice2, shop.CValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
                GetItemPrice(shop.CPrice3, shop.CValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
                if (iTemplateID > 0)
                {
                    list.Add(iTemplateID);
                    list.Add(iCount);
                }
            }
            return list;
        }

        public DateTime AdvanceDate
        {
            get
            {
                return this._advanceDate;
            }
            set
            {
                this._advanceDate = value;
                base._isDirty = true;
            }
        }

        public int Agility
        {
            get
            {
                int agility = this._template.Agility;
                if (this.IsGold && (this.GoldEquip != null))
                {
                    agility = this.GoldEquip.Agility;
                }
                return (this._agilityCompose + agility);
            }
        }

        public int AgilityCompose
        {
            get
            {
                return this._agilityCompose;
            }
            set
            {
                this._agilityCompose = value;
                base._isDirty = true;
            }
        }

        public int Attack
        {
            get
            {
                int attack = this._template.Attack;
                if (this.IsGold && (this.GoldEquip != null))
                {
                    attack = this.GoldEquip.Attack;
                }
                return (this._attackCompose + attack);
            }
        }

        public int AttackCompose
        {
            get
            {
                return this._attackCompose;
            }
            set
            {
                this._attackCompose = value;
                base._isDirty = true;
            }
        }

        public int BagType
        {
            get
            {
                return this._bagType;
            }
            set
            {
                this._bagType = value;
                base._isDirty = true;
            }
        }

        public int beadExp
        {
            get
            {
                return this._beadExp;
            }
            set
            {
                this._beadExp = value;
                base._isDirty = true;
            }
        }

        public bool beadIsLock
        {
            get
            {
                return this._beadIsLock;
            }
            set
            {
                this._beadIsLock = value;
                base._isDirty = true;
            }
        }

        public int beadLevel
        {
            get
            {
                return this._beadLevel;
            }
            set
            {
                this._beadLevel = value;
                base._isDirty = true;
            }
        }

        public DateTime BeginDate
        {
            get
            {
                return this._beginDate;
            }
            set
            {
                this._beginDate = value;
                base._isDirty = true;
            }
        }

        public int Bless
        {
            get
            {
                return this._Bless;
            }
            set
            {
                this._Bless = value;
                base._isDirty = true;
            }
        }

        public int Blood
        {
            get
            {
                return this._Blood;
            }
            set
            {
                this._Blood = value;
                base._isDirty = true;
            }
        }

        public string Color
        {
            get
            {
                return this._color;
            }
            set
            {
                this._color = value;
                base._isDirty = true;
            }
        }

        public int Count
        {
            get
            {
                return this._count;
            }
            set
            {
                this._count = value;
                base._isDirty = true;
            }
        }

        public int Damage
        {
            get
            {
                return this._Damage;
            }
            set
            {
                this._Damage = value;
                base._isDirty = true;
            }
        }

        public int Defence
        {
            get
            {
                int defence = this._template.Defence;
                if (this.IsGold && (this.GoldEquip != null))
                {
                    defence = this.GoldEquip.Defence;
                }
                return (this._defendCompose + defence);
            }
        }

        public int DefendCompose
        {
            get
            {
                return this._defendCompose;
            }
            set
            {
                this._defendCompose = value;
                base._isDirty = true;
            }
        }

        public int GetBagType
        {
            get
            {
                return (int) this._template.BagType;
            }
        }

        public DateTime goldBeginTime
        {
            get
            {
                return this._goldBeginTime;
            }
            set
            {
                this._goldBeginTime = value;
                base._isDirty = true;
            }
        }

        public ItemTemplateInfo GoldEquip
        {
            get
            {
                return this._goldEquip;
            }
            set
            {
                this._goldEquip = value;
                base._isDirty = true;
            }
        }

        public int goldValidDate
        {
            get
            {
                return this._goldValidDate;
            }
            set
            {
                this._goldValidDate = value;
                base._isDirty = true;
            }
        }

        public bool GoodsLock
        {
            get
            {
                return this._GoodsLock;
            }
            set
            {
                this._GoodsLock = value;
                base._isDirty = true;
            }
        }

        public int Guard
        {
            get
            {
                return this._Guard;
            }
            set
            {
                this._Guard = value;
                base._isDirty = true;
            }
        }

        public int Hole1
        {
            get
            {
                return this._hole1;
            }
            set
            {
                this._hole1 = value;
                base._isDirty = true;
            }
        }

        public int Hole2
        {
            get
            {
                return this._hole2;
            }
            set
            {
                this._hole2 = value;
                base._isDirty = true;
            }
        }

        public int Hole3
        {
            get
            {
                return this._hole3;
            }
            set
            {
                this._hole3 = value;
                base._isDirty = true;
            }
        }

        public int Hole4
        {
            get
            {
                return this._hole4;
            }
            set
            {
                this._hole4 = value;
                base._isDirty = true;
            }
        }

        public int Hole5
        {
            get
            {
                return this._hole5;
            }
            set
            {
                this._hole5 = value;
                base._isDirty = true;
            }
        }

        public int Hole5Exp
        {
            get
            {
                return this._hole5Exp;
            }
            set
            {
                this._hole5Exp = value;
                base._isDirty = true;
            }
        }

        public int Hole5Level
        {
            get
            {
                return this._hole5Level;
            }
            set
            {
                this._hole5Level = value;
                base._isDirty = true;
            }
        }

        public int Hole6
        {
            get
            {
                return this._hole6;
            }
            set
            {
                this._hole6 = value;
                base._isDirty = true;
            }
        }

        public int Hole6Exp
        {
            get
            {
                return this._hole6Exp;
            }
            set
            {
                this._hole6Exp = value;
                base._isDirty = true;
            }
        }

        public int Hole6Level
        {
            get
            {
                return this._hole6Level;
            }
            set
            {
                this._hole6Level = value;
                base._isDirty = true;
            }
        }

        public bool IsBinds
        {
            get
            {
                return this._isBinds;
            }
            set
            {
                this._isBinds = value;
                base._isDirty = true;
            }
        }

        public bool IsExist
        {
            get
            {
                return this._isExist;
            }
            set
            {
                this._isExist = value;
                base._isDirty = true;
            }
        }

        public bool IsGold
        {
            get
            {
                return this.IsValidGoldItem();
            }
        }

        public bool IsJudge
        {
            get
            {
                return this._isJudage;
            }
            set
            {
                this._isJudage = value;
                base._isDirty = true;
            }
        }

        public bool IsLogs
        {
            get
            {
                return this._isLogs;
            }
            set
            {
                this._isLogs = value;
            }
        }

        public bool isShowBind
        {
            get
            {
                return this._isShowBind;
            }
            set
            {
                this._isShowBind = value;
                base._isDirty = true;
            }
        }

        public bool IsTips
        {
            get
            {
                return this._isTips;
            }
            set
            {
                this._isTips = value;
            }
        }

        public bool IsUsed
        {
            get
            {
                return this._isUsed;
            }
            set
            {
                if (this._isUsed != value)
                {
                    this._isUsed = value;
                    base._isDirty = true;
                }
            }
        }

        public int ItemID
        {
            get
            {
                return this._itemID;
            }
            set
            {
                this._itemID = value;
                base._isDirty = true;
            }
        }

        public string latentEnergyCurStr
        {
            get
            {
                return this._latentEnergyCurStr;
            }
            set
            {
                this._latentEnergyCurStr = value;
                base._isDirty = true;
            }
        }

        public DateTime latentEnergyEndTime
        {
            get
            {
                return this._latentEnergyEndTime;
            }
            set
            {
                this._latentEnergyEndTime = value;
                base._isDirty = true;
            }
        }

        public string latentEnergyNewStr
        {
            get
            {
                return this._latentEnergyNewStr;
            }
            set
            {
                this._latentEnergyNewStr = value;
                base._isDirty = true;
            }
        }

        public int LianExp
        {
            get
            {
                return this._LianExp;
            }
            set
            {
                this._LianExp = value;
            }
        }

        public int LianGrade
        {
            get
            {
                return this._LianGrade;
            }
            set
            {
                this._LianGrade = value;
            }
        }

        public int Luck
        {
            get
            {
                int luck = this._template.Luck;
                if (this.IsGold && (this.GoldEquip != null))
                {
                    luck = this.GoldEquip.Luck;
                }
                return (this._luckCompose + luck);
            }
        }

        public int LuckCompose
        {
            get
            {
                return this._luckCompose;
            }
            set
            {
                this._luckCompose = value;
                base._isDirty = true;
            }
        }

        public int MagicAttack
        {
            get
            {
                return this._magicAttack;
            }
            set
            {
                this._magicAttack = value;
                base._isDirty = true;
            }
        }

        public int MagicDefence
        {
            get
            {
                return this._magicDefence;
            }
            set
            {
                this._magicDefence = value;
                base._isDirty = true;
            }
        }

        public string Pic
        {
            get
            {
                if (this.IsGold && (this.GoldEquip != null))
                {
                    return this.GoldEquip.Pic;
                }
                return this._template.Pic;
            }
        }

        public int Place
        {
            get
            {
                return this._place;
            }
            set
            {
                this._place = value;
                base._isDirty = true;
            }
        }

        public int RefineryLevel
        {
            get
            {
                if (this.IsGold && (this.GoldEquip != null))
                {
                    return this.GoldEquip.RefineryLevel;
                }
                return this._template.RefineryLevel;
            }
        }

        public DateTime RemoveDate
        {
            get
            {
                return this._removeDate;
            }
            set
            {
                this._removeDate = value;
                base._isDirty = true;
            }
        }

        public int RemoveType
        {
            get
            {
                return this._removeType;
            }
            set
            {
                this._removeType = value;
                this._removeDate = DateTime.Now;
                base._isDirty = true;
            }
        }

        public string Skin
        {
            get
            {
                return this._skin;
            }
            set
            {
                this._skin = value;
                base._isDirty = true;
            }
        }

        public int StrengthenExp
        {
            get
            {
                return this._strengthenExp;
            }
            set
            {
                this._strengthenExp = value;
                base._isDirty = true;
            }
        }

        public int StrengthenLevel
        {
            get
            {
                return this._strengthenLevel;
            }
            set
            {
                this._strengthenLevel = value;
                base._isDirty = true;
            }
        }

        public int StrengthenTimes
        {
            get
            {
                return this._strengthenTimes;
            }
            set
            {
                this._strengthenTimes = value;
                base._isDirty = true;
            }
        }

        public ItemTemplateInfo Template
        {
            get
            {
                return this._template;
            }
        }

        public int TemplateID
        {
            get
            {
                return this._templateId;
            }
            set
            {
                this._templateId = value;
                base._isDirty = true;
            }
        }

        public int UserID
        {
            get
            {
                return this._userID;
            }
            set
            {
                this._userID = value;
                base._isDirty = true;
            }
        }

        public int ValidDate
        {
            get
            {
                return this._validDate;
            }
            set
            {
                this._validDate = (value > 0x3e7) ? 0x16d : value;
                base._isDirty = true;
            }
        }
    }
}


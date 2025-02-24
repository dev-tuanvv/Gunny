namespace Game.Server.GameUtils
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server.Buffer;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Reflection;

    public class PlayerExtra
    {
        protected Timer _hotSpringTimer;
        public const int BEAD_MASTER = 3;
        private int[] buffData = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        private Dictionary<int, EventRewardProcessInfo> dictionary_0;
        public const int DUNGEON_HERO = 5;
        public const int HELP_STRAW = 4;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int[] m_buriedQuests;
        private List<FlopCardInfo> m_flopCard;
        private UsersExtraInfo m_Info;
        private Dictionary<int, int> m_kingBlessInfo;
        protected object m_lock = new object();
        protected GamePlayer m_player;
        private bool m_saveToDb;
        private List<EventAwardInfo> m_searchGoodItems;
        public int MapId = 1;
        public const int PET_REFRESH = 2;
        private int[] positions = new int[0x22];
        private static ThreadSafeRandom rand = new ThreadSafeRandom();
        public readonly DateTime reChangeEnd;
        public readonly DateTime reChangeStart;
        public const int STRENGTH_ENCHANCE = 1;
        public readonly DateTime strengthenEnd;
        public readonly DateTime strengthenStart;
        public int takeCardLimit = 3;
        public const int TASK_SPIRIT = 6;
        public const int TIME_DEITY = 7;
        public readonly DateTime upGradeEnd;
        public readonly DateTime upGradeStart;
        public readonly DateTime useMoneyEnd;
        public readonly DateTime useMoneyStart;

        public PlayerExtra(GamePlayer player, bool saveTodb)
        {
            this.m_player = player;
            this.m_kingBlessInfo = new Dictionary<int, int>();
            this.m_searchGoodItems = new List<EventAwardInfo>();
            this.m_flopCard = new List<FlopCardInfo>();
            this.m_saveToDb = saveTodb;
        }

        private void AddBuriedQuest()
        {
            string str;
            int index = rand.Next(this.m_buriedQuests.Length);
            QuestInfo singleQuest = QuestMgr.GetSingleQuest(this.m_buriedQuests[index]);
            this.Player.QuestInventory.AddQuest(singleQuest, out str);
            if (!string.IsNullOrEmpty(str))
            {
                log.InfoFormat("{0} AddBuriedQuest: {1}", this.Player.PlayerCharacter.NickName, str);
            }
        }

        private void AddGoods(int goodId, int count)
        {
            if (goodId > 0)
            {
                ItemTemplateInfo goods = ItemMgr.FindItemTemplate(goodId);
                if (goods != null)
                {
                    SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x69);
                    cloneItem.IsBinds = true;
                    cloneItem.Count = count;
                    this.Player.AddTemplate(cloneItem, cloneItem.Template.BagType, count, eGameView.OtherTypeGet, "Bản đồ kho b\x00e1u");
                }
            }
        }

        public void BeginHotSpringTimer()
        {
            int dueTime = 0xea60;
            if (this._hotSpringTimer == null)
            {
                this._hotSpringTimer = new Timer(new TimerCallback(this.HotSpringCheck), null, dueTime, dueTime);
            }
            else
            {
                this._hotSpringTimer.Change(dueTime, dueTime);
            }
        }

        public void ConvertKingBless()
        {
            if (this.m_Info != null)
            {
                string str = "";
                using (Dictionary<int, int>.KeyCollection.Enumerator enumerator = this.m_kingBlessInfo.Keys.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        switch (enumerator.Current)
                        {
                            case 1:
                                str = str + "STRENGTH_ENCHANCE," + this.m_kingBlessInfo[1];
                                break;

                            case 2:
                                str = str + "|PET_REFRESH," + this.m_kingBlessInfo[2];
                                break;

                            case 3:
                                str = str + "|BEAD_MASTER," + this.m_kingBlessInfo[3];
                                break;

                            case 4:
                                str = str + "|HELP_STRAW," + this.m_kingBlessInfo[4];
                                break;

                            case 5:
                                str = str + "|DUNGEON_HERO," + this.m_kingBlessInfo[5];
                                break;

                            case 6:
                                str = str + "|TASK_SPIRIT," + this.m_kingBlessInfo[6];
                                break;

                            case 7:
                                str = str + "|TIME_DEITY," + this.m_kingBlessInfo[7];
                                break;
                        }
                    }
                }
                this.m_Info.KingBleesInfo = str;
            }
        }

        public void ConvertSearchGoodItems()
        {
            if (this.SearchGoodItems.Count > 0)
            {
                string str = "";
                foreach (EventAwardInfo info in this.SearchGoodItems)
                {
                    str = str + string.Format("{0},{1},{2}|", info.TemplateID, info.Position, info.Count);
                }
                str = str.Substring(0, str.Length - 1);
                this.m_Info.SearchGoodItems = str;
            }
        }

        public void CreateRandomPos()
        {
            for (int i = 0; i < this.positions.Length; i++)
            {
                this.positions[i] = i + 1;
            }
            rand.Shuffer<int>(this.positions);
        }

        public void CreateSaveLifeBuff()
        {
            if ((this.m_player.PlayerCharacter.VIPLevel >= 4) && !this.m_player.PlayerCharacter.IsVIPExpire())
            {
                AbstractBuffer buffer = BufferList.CreateSaveLifeBuffer(3);
                if (buffer != null)
                {
                    buffer.Start(this.Player);
                }
            }
            else
            {
                AbstractBuffer buffer2 = BufferList.CreateSaveLifeBuffer(0);
                if (buffer2 != null)
                {
                    buffer2.Start(this.Player);
                }
            }
        }

        public void CreateSearchGoodItems()
        {
            this.m_searchGoodItems.Clear();
            List<EventAwardInfo> list = this.CreateSearchGoodsAward();
            lock (this.m_lock)
            {
                foreach (EventAwardInfo info in list)
                {
                    this.m_searchGoodItems.Add(info);
                }
            }
        }

        public List<EventAwardInfo> CreateSearchGoodsAward()
        {
            SearchGoodsTempInfo searchGoodsTempInfo = AwardMgr.GetSearchGoodsTempInfo(this.m_Info.starlevel);
            new Dictionary<int, EventAwardInfo>();
            string number = (searchGoodsTempInfo == null) ? "10,2,3,1" : searchGoodsTempInfo.ExtractNumber;
            int[] array = new int[] { -1, -2, -3, -4 };
            rand.Shuffer<int>(array);
            int num = int.Parse(number.Split(new char[] { ',' })[0]);
            int num2 = int.Parse(number.Split(new char[] { ',' })[1]);
            int num3 = int.Parse(number.Split(new char[] { ',' })[2]);
            int num4 = int.Parse(number.Split(new char[] { ',' })[3]);
            this.GetTotal(number);
            string[] strArray = number.Split(new char[] { ',' });
            this.CreateRandomPos();
            List<EventAwardInfo> list = new List<EventAwardInfo>();
            int num5 = 1;
            for (int i = 0; i < strArray.Length; i++)
            {
                int num7;
                EventAwardInfo info2;
                int num8;
                EventAwardInfo info3;
                int num9;
                EventAwardInfo info4;
                int num10;
                int num11;
                switch (i)
                {
                    case 0:
                        num7 = 0;
                        goto Label_0162;

                    case 1:
                        num8 = 0;
                        goto Label_01B7;

                    case 2:
                        num9 = 0;
                        goto Label_020D;

                    case 3:
                        num10 = 0;
                        goto Label_0271;

                    default:
                    {
                        continue;
                    }
                }
            Label_0122:
                info2 = EventAwardMgr.CreateSearchGoodsAward(eEventType.SEARCH_GOODS);
                if (info2 != null)
                {
                    info2.Position = this.positions[num5 - 1];
                    list.Add(info2);
                    num5++;
                }
                num7++;
            Label_0162:
                if (num7 < num)
                {
                    goto Label_0122;
                }
                continue;
            Label_0177:
                info3 = this.GetSpecialTem(-5, this.positions[num5 - 1]);
                info3.Position = this.positions[num5 - 1];
                list.Add(info3);
                num5++;
                num8++;
            Label_01B7:
                if (num8 < num2)
                {
                    goto Label_0177;
                }
                continue;
            Label_01CD:
                info4 = this.GetSpecialTem(-6, this.positions[num5 - 1]);
                info4.Position = this.positions[num5 - 1];
                list.Add(info4);
                num5++;
                num9++;
            Label_020D:
                if (num9 < num3)
                {
                    goto Label_01CD;
                }
                continue;
            Label_0220:
                num11 = rand.Next(array.Length);
                EventAwardInfo specialTem = this.GetSpecialTem(array[num11], this.positions[num5 - 1]);
                specialTem.Position = this.positions[num5 - 1];
                list.Add(specialTem);
                num5++;
                num10++;
            Label_0271:
                if (num10 < num4)
                {
                    goto Label_0220;
                }
            }
            return list;
        }

        private void CreateTakeCard()
        {
            this.m_flopCard.Clear();
            for (int i = 0; i < 5; i++)
            {
                int num2 = rand.Next(5, 0x4b);
                FlopCardInfo item = new FlopCardInfo {
                    Count = num2,
                    TemplateID = 0x2da0
                };
                this.m_flopCard.Add(item);
            }
            this.takeCardLimit = 3;
            rand.ShufferList<FlopCardInfo>(this.m_flopCard);
            GSPacketIn pkg = new GSPacketIn(0x62);
            pkg.WriteByte(0x18);
            pkg.WriteInt(this.takeCardLimit);
            pkg.WriteInt(this.m_flopCard.Count);
            foreach (FlopCardInfo info2 in this.m_flopCard)
            {
                pkg.WriteInt(info2.TemplateID);
                pkg.WriteInt(info2.Count);
            }
            this.Player.SendTCP(pkg);
        }

        public UsersExtraInfo CreateUsersExtra(int UserID)
        {
            return new UsersExtraInfo { UserID = UserID, starlevel = 1, nowPosition = 0, FreeCount = GameProperties.SearchGoodsFreeCount, SearchGoodItems = "", FreeAddAutionCount = 0, FreeSendMailCount = 0, KingBleesInfo = "", KingBlessEnddate = DateTime.Now, MissionEnergy = GameProperties.MaxMissionEnergy, buyEnergyCount = 1, KingBlessIndex = 0, LastTimeHotSpring = DateTime.Now, LastFreeTimeHotSpring = DateTime.Now, MinHotSpring = 60 };
        }

        public bool CheckNoviceActiveOpen(NoviceActiveType activeType)
        {
            bool flag = false;
            switch (activeType)
            {
                case NoviceActiveType.GRADE_UP_ACTIVE:
                    if ((this.upGradeStart <= DateTime.Now) && (this.upGradeEnd >= DateTime.Now))
                    {
                        flag = true;
                    }
                    return flag;

                case NoviceActiveType.STRENGTHEN_WEAPON_ACTIVE:
                    if ((this.strengthenStart <= DateTime.Now) && (this.strengthenEnd >= DateTime.Now))
                    {
                        flag = true;
                    }
                    return flag;

                case NoviceActiveType.USE_MONEY_ACTIVE:
                    if ((this.useMoneyStart <= DateTime.Now) && (this.useMoneyEnd >= DateTime.Now))
                    {
                        flag = true;
                    }
                    return flag;

                case NoviceActiveType.RECHANGE_MONEY_ACTIVE:
                    if ((this.reChangeStart <= DateTime.Now) && (this.reChangeEnd >= DateTime.Now))
                    {
                        flag = true;
                    }
                    return flag;
            }
            return flag;
        }

        private EventAwardInfo GetAwardByPos()
        {
            lock (this.m_lock)
            {
                foreach (EventAwardInfo info in this.m_searchGoodItems)
                {
                    if (info.Position == this.m_Info.nowPosition)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        public EventRewardProcessInfo GetEventProcess(int activeType)
        {
            object @lock = this.m_lock;
            lock (@lock)
            {
                if (!this.dictionary_0.ContainsKey(activeType))
                {
                    this.dictionary_0.Add(activeType, this.method_0(activeType));
                }
                return this.dictionary_0[activeType];
            }
        }

        public void GetSearchGoodItemsDb()
        {
            if (string.IsNullOrEmpty(this.m_Info.SearchGoodItems))
            {
                this.CreateSearchGoodItems();
            }
            else
            {
                foreach (string str in this.m_Info.SearchGoodItems.Split(new char[] { '|' }))
                {
                    EventAwardInfo item = new EventAwardInfo {
                        TemplateID = int.Parse(str.Split(new char[] { ',' })[0]),
                        Position = int.Parse(str.Split(new char[] { ',' })[1]),
                        Count = int.Parse(str.Split(new char[] { ',' })[2])
                    };
                    this.m_searchGoodItems.Add(item);
                }
            }
        }

        public EventAwardInfo GetSpecialTem(int type, int pos)
        {
            return new EventAwardInfo { TemplateID = type, Position = pos, Count = 1 };
        }

        public int GetTotal(string number)
        {
            string[] strArray = number.Split(new char[] { ',' });
            int num = 0;
            foreach (string str in strArray)
            {
                num += int.Parse(str);
            }
            return num;
        }

        protected void HotSpringCheck(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                if (this.m_player.CurrentHotSpringRoom == null)
                {
                    this.StopHotSpringTimer();
                }
                else if (this.Info.MinHotSpring <= 0)
                {
                    this.m_player.SendMessage("Bạn đ\x00e3 hết giờ tham gia suối nước n\x00f3ng.");
                    this.m_player.CurrentHotSpringRoom.RemovePlayer(this.m_player);
                }
                else
                {
                    int expWithLevel = HotSpringMgr.GetExpWithLevel(this.m_player.PlayerCharacter.Grade);
                    if (expWithLevel > 0)
                    {
                        UsersExtraInfo info = this.Info;
                        info.MinHotSpring--;
                        if (this.Info.MinHotSpring <= 5)
                        {
                            this.m_player.SendMessage("Bạn chỉ c\x00f2n " + this.Info.MinHotSpring + " ph\x00fat tham gia Suối nước n\x00f3ng. H\x00e3y gia hạn th\x00eam để tham gia l\x00e2u hơn.");
                        }
                        this.m_player.AddGP(expWithLevel);
                        this.m_player.Out.SendHotSpringUpdateTime(this.m_player, expWithLevel);
                        this.m_player.OnHotSpingExpAdd(this.Info.MinHotSpring, expWithLevel);
                    }
                    Thread.CurrentThread.Priority = priority;
                    tickCount = Environment.TickCount - tickCount;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("HotSpringCheck: " + exception);
            }
        }

        public void KingBlessStrengthEnchance(bool isUp)
        {
            if (isUp)
            {
                this.Player.PlayerCharacter.StrengthEnchance = this.KingBlessValue(1);
            }
            else
            {
                this.Player.PlayerCharacter.StrengthEnchance = 0;
            }
        }

        public int KingBlessValue(int key)
        {
            lock (this.m_lock)
            {
                if (this.m_kingBlessInfo.ContainsKey(key))
                {
                    return this.m_kingBlessInfo[key];
                }
            }
            return 0;
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    this.m_Info = bussiness.GetSingleUsersExtra(this.Player.PlayerCharacter.ID);
                    if (this.m_Info == null)
                    {
                        this.m_Info = this.CreateUsersExtra(this.Player.PlayerCharacter.ID);
                    }
                    else
                    {
                        this.SetupKingBlessFromData();
                    }
                    this.dictionary_0 = new Dictionary<int, EventRewardProcessInfo>();
                    foreach (EventRewardProcessInfo info in bussiness.GetUserEventProcess(this.m_player.PlayerCharacter.ID))
                    {
                        if (!this.dictionary_0.ContainsKey(info.ActiveType))
                        {
                            this.dictionary_0.Add(info.ActiveType, info);
                        }
                    }
                }
                this.m_buriedQuests = QuestMgr.GetAllBuriedQuest();
            }
        }

        private EventRewardProcessInfo method_0(int int_0)
        {
            return new EventRewardProcessInfo { UserID = this.m_player.PlayerCharacter.ID, ActiveType = int_0, Conditions = 0, AwardGot = 0 };
        }

        public void PlayNowPosition(int templateID)
        {
            if ((templateID == -2) || (templateID == -3))
            {
                EventAwardInfo awardByPos = this.GetAwardByPos();
                if (awardByPos != null)
                {
                    this.UpdateGoodItems(awardByPos);
                }
            }
            GSPacketIn pkg = new GSPacketIn(0x62);
            pkg.WriteByte(0x19);
            pkg.WriteInt(this.m_Info.nowPosition);
            this.Player.SendTCP(pkg);
        }

        public void RefreshKingBless()
        {
            lock (this.m_lock)
            {
                foreach (int num in this.m_kingBlessInfo.Keys)
                {
                    this.Player.Out.SendKingBlessUpdateBuffData(this.Player.PlayerCharacter.ID, num, this.m_kingBlessInfo[num]);
                }
            }
        }

        public void ResetUsersExtra()
        {
            if (this.m_Info.KingBlessEnddate > DateTime.Now)
            {
                this.SetupKingBless(false, this.Info.KingBlessIndex);
            }
            this.m_Info.starlevel = 1;
            this.m_Info.nowPosition = 0;
            this.m_Info.FreeCount = GameProperties.SearchGoodsFreeCount;
            this.m_Info.FreeAddAutionCount = 0;
            this.m_Info.FreeSendMailCount = 0;
            this.m_Info.MissionEnergy = GameProperties.MaxMissionEnergy;
            this.m_Info.buyEnergyCount = 1;
            this.CreateSaveLifeBuff();
            this.CreateSearchGoodItems();
        }

        public void RollDiceCallBack(bool isRemindRollBind)
        {
            int val = rand.Next(1, 6);
            bool flag = false;
            if (this.Info.FreeCount > 0)
            {
                UsersExtraInfo info = this.Info;
                info.FreeCount--;
                flag = true;
            }
            else if (this.Player.MoneyDirect(GameProperties.SearchGoodsPayMoney))
            {
                flag = true;
            }
            if (flag)
            {
                this.m_Info.nowPosition += val;
                if (this.m_Info.nowPosition > 0x23)
                {
                    this.m_Info.nowPosition = 0x23;
                }
                GSPacketIn pkg = new GSPacketIn(0x62);
                pkg.WriteByte(0x11);
                pkg.WriteInt(this.m_Info.FreeCount);
                pkg.WriteInt(val);
                pkg.WriteInt(this.m_Info.nowPosition);
                this.Player.SendTCP(pkg);
            }
            EventAwardInfo awardByPos = this.GetAwardByPos();
            if (awardByPos != null)
            {
                int templateID = awardByPos.TemplateID;
                switch (templateID)
                {
                    case -7:
                    case 0:
                        goto Label_01DB;

                    case -6:
                        this.UpdateGoodItems(awardByPos);
                        goto Label_01DB;

                    case -5:
                        this.UpdateGoodItems(awardByPos);
                        goto Label_01DB;

                    case -4:
                        this.m_Info.nowPosition = 0x23;
                        this.PlayNowPosition(templateID);
                        goto Label_01DB;

                    case -3:
                        this.m_Info.nowPosition++;
                        this.PlayNowPosition(templateID);
                        goto Label_01DB;

                    case -2:
                        this.m_Info.nowPosition--;
                        this.PlayNowPosition(templateID);
                        goto Label_01DB;

                    case -1:
                        this.m_Info.nowPosition = 0;
                        this.PlayNowPosition(templateID);
                        goto Label_01DB;
                }
                this.UpdateGoodItems(awardByPos);
            }
        Label_01DB:
            if (this.m_Info.nowPosition == 0x23)
            {
                SearchGoodsTempInfo searchGoodsTempInfo = AwardMgr.GetSearchGoodsTempInfo(this.m_Info.starlevel);
                if (searchGoodsTempInfo != null)
                {
                    this.AddGoods(searchGoodsTempInfo.DestinationReward, 1);
                }
            }
        }

        public virtual void SaveToDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    lock (this.m_lock)
                    {
                        this.ConvertKingBless();
                        this.ConvertSearchGoodItems();
                        if ((this.m_Info != null) && this.m_Info.IsDirty)
                        {
                            if (this.m_Info.ID > 0)
                            {
                                bussiness.UpdateUsersExtra(this.m_Info);
                            }
                            else
                            {
                                bussiness.AddUsersExtra(this.m_Info);
                            }
                        }
                    }
                }
            }
        }

        public string SetupKingBless(bool isFriend, int kingBlessIndex)
        {
            int num = 100;
            int num2 = 1;
            int num3 = 3;
            int num4 = 1;
            int num5 = 1;
            int num6 = 0;
            int num7 = 1;
            switch (kingBlessIndex)
            {
                case 2:
                    num2 = 2;
                    num3 = 4;
                    break;

                case 3:
                    num = 200;
                    num2 = 3;
                    num3 = 6;
                    num4 = 2;
                    num5 = 2;
                    break;
            }
            if (isFriend)
            {
                return ((((((("STRENGTH_ENCHANCE," + num) + "|PET_REFRESH," + num2) + "|BEAD_MASTER," + num3) + "|HELP_STRAW," + num4) + "|DUNGEON_HERO," + num5) + "|TASK_SPIRIT," + num6) + "|TIME_DEITY," + num7);
            }
            this.m_kingBlessInfo.Clear();
            foreach (int num9 in this.buffData)
            {
                switch (num9)
                {
                    case 1:
                        this.m_kingBlessInfo.Add(num9, num);
                        break;

                    case 2:
                        this.m_kingBlessInfo.Add(num9, num2);
                        break;

                    case 3:
                        this.m_kingBlessInfo.Add(num9, num3);
                        break;

                    case 4:
                        this.m_kingBlessInfo.Add(num9, num4);
                        break;

                    case 5:
                        this.m_kingBlessInfo.Add(num9, num5);
                        break;

                    case 6:
                        this.m_kingBlessInfo.Add(num9, num6);
                        break;

                    case 7:
                        this.m_kingBlessInfo.Add(num9, num7);
                        break;
                }
            }
            this.ConvertKingBless();
            return "";
        }

        public void SetupKingBlessFromData()
        {
            if (!string.IsNullOrEmpty(this.m_Info.KingBleesInfo))
            {
                this.m_kingBlessInfo.Clear();
                foreach (string str in this.m_Info.KingBleesInfo.Split(new char[] { '|' }))
                {
                    int num2 = int.Parse(str.Split(new char[] { ',' })[1]);
                    switch (str.Split(new char[] { ',' })[0])
                    {
                        case "STRENGTH_ENCHANCE":
                            this.m_kingBlessInfo.Add(1, num2);
                            break;

                        case "PET_REFRESH":
                            this.m_kingBlessInfo.Add(2, num2);
                            break;

                        case "BEAD_MASTER":
                            this.m_kingBlessInfo.Add(3, num2);
                            break;

                        case "HELP_STRAW":
                            this.m_kingBlessInfo.Add(4, num2);
                            break;

                        case "DUNGEON_HERO":
                            this.m_kingBlessInfo.Add(5, num2);
                            break;

                        case "TASK_SPIRIT":
                            this.m_kingBlessInfo.Add(6, num2);
                            break;

                        case "TIME_DEITY":
                            this.m_kingBlessInfo.Add(7, num2);
                            break;
                    }
                }
            }
        }

        public void StopHotSpringTimer()
        {
            if (this._hotSpringTimer != null)
            {
                this._hotSpringTimer.Dispose();
                this._hotSpringTimer = null;
            }
        }

        public void TakeCard(bool UseMoney)
        {
            string[] strArray = GameProperties.SearchGoodsTakeCardMoney.Split(new char[] { '|' });
            if (this.takeCardLimit > 0)
            {
                if (this.Player.MoneyDirect(int.Parse(strArray[3 - this.takeCardLimit])))
                {
                    rand.ShufferList<FlopCardInfo>(this.m_flopCard);
                    FlopCardInfo info = this.m_flopCard[this.takeCardLimit];
                    this.AddGoods(info.TemplateID, info.Count);
                    this.takeCardLimit--;
                    if (info != null)
                    {
                        GSPacketIn pkg = new GSPacketIn(0x62);
                        pkg.WriteByte(0x20);
                        pkg.WriteInt(this.takeCardLimit);
                        pkg.WriteInt(info.TemplateID);
                        pkg.WriteInt(info.Count);
                        this.Player.SendTCP(pkg);
                    }
                }
            }
            else
            {
                this.Player.SendMessage("Lượt lật thẻ đ\x00e3 hết!");
            }
        }

        public void UpdateEventCondition(int activeType, int value)
        {
            this.UpdateEventCondition(activeType, value, false);
        }

        public void UpdateEventCondition(int activeType, int value, bool isPlus)
        {
            EventRewardProcessInfo eventProcess = this.GetEventProcess(activeType);
            if (isPlus)
            {
                eventProcess.Conditions += value;
            }
            else if (eventProcess.Conditions < value)
            {
                eventProcess.Conditions = value;
            }
            DateTime now = DateTime.Now;
            DateTime endTime = DateTime.Now;
            switch (activeType)
            {
                case 1:
                    now = this.upGradeStart;
                    endTime = this.upGradeEnd;
                    break;

                case 2:
                    now = this.strengthenStart;
                    endTime = this.strengthenEnd;
                    break;

                case 3:
                    now = this.useMoneyStart;
                    endTime = this.useMoneyEnd;
                    break;

                case 4:
                    now = this.reChangeStart;
                    endTime = this.reChangeEnd;
                    break;
            }
            this.m_player.Out.SendOpenNoviceActive(0, activeType, eventProcess.Conditions, eventProcess.AwardGot, now, endTime);
        }

        public void UpdateGoodItems(EventAwardInfo good)
        {
            if ((((good.TemplateID != -1) && (good.TemplateID != -2)) && (good.TemplateID != -3)) && (good.TemplateID != -4))
            {
                if (good.TemplateID == -5)
                {
                    this.CreateTakeCard();
                }
                if (good.TemplateID == -6)
                {
                    this.AddBuriedQuest();
                }
                this.AddGoods(good.TemplateID, good.Count);
                for (int i = 0; i < this.m_searchGoodItems.Count; i++)
                {
                    if (this.m_searchGoodItems[i].Position == good.Position)
                    {
                        this.m_searchGoodItems[i].TemplateID = 0;
                        break;
                    }
                }
                GSPacketIn pkg = new GSPacketIn(0x62);
                pkg.WriteByte(0x17);
                pkg.WriteInt((good.TemplateID < 0) ? 0 : good.TemplateID);
                this.Player.SendTCP(pkg);
            }
        }

        public bool UseKingBless(int key)
        {
            lock (this.m_lock)
            {
                if (this.m_kingBlessInfo.ContainsKey(key) && (this.m_kingBlessInfo[key] > 0))
                {
                    Dictionary<int, int> dictionary;
                    (dictionary = this.m_kingBlessInfo)[key] = dictionary[key] - 1;
                    this.Player.Out.SendKingBlessUpdateBuffData(this.Player.PlayerCharacter.ID, key, this.m_kingBlessInfo[key]);
                    return true;
                }
            }
            return false;
        }

        public UsersExtraInfo Info
        {
            get
            {
                return this.m_Info;
            }
            set
            {
                this.m_Info = value;
            }
        }

        public Dictionary<int, int> KingBlessInfo
        {
            get
            {
                return this.m_kingBlessInfo;
            }
            set
            {
                this.m_kingBlessInfo = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
        }

        public List<EventAwardInfo> SearchGoodItems
        {
            get
            {
                return this.m_searchGoodItems;
            }
            set
            {
                this.m_searchGoodItems = value;
            }
        }

        public class FlopCardInfo
        {
            public int Count { get; set; }

            public int TemplateID { get; set; }
        }
    }
}


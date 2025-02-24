namespace Game.Server.GameUtils
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class PlayerActives
    {
        protected Timer _christmasTimer;
        protected Timer _labyrinthTimer;
        private int _lightriddleColdown = 15;
        protected Timer _lightriddleTimer;
        public readonly int coinTemplateID = 0x311e9;
        public readonly int countBoguReset = 5;
        private readonly int countBox1GoguAward = 10;
        private readonly int countBox2GoguAward = 0x19;
        private readonly int countBox3GoguAward = 50;
        private readonly int ChikenBoxCount = 0x12;
        private readonly int defaultCoins = 0x3e8;
        private readonly int flushCoins = 15;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly int LuckyStartBoxCount = 14;
        private ActiveSystemInfo m_activeInfo;
        private NewChickenBoxItemInfo m_award;
        private UserBoguAdventureInfo m_boguAdventure;
        private int[] m_boguAdventureMoney;
        private NewChickenBoxItemInfo[] m_ChickenBoxRewards;
        private UserChristmasInfo m_christmas;
        private int[] m_eagleEyePrice;
        private int m_flushPrice;
        private int m_freeEyeCount;
        private int m_freeFlushTime;
        private int m_freeOpenCardCount;
        private int m_freeRefreshBoxCount;
        private int m_labyrinthCountDown = GameProperties.WarriorFamRaidTimeRemain;
        protected object m_lock = new object();
        private DateTime m_luckyBegindate;
        private DateTime m_luckyEnddate;
        private NewChickenBoxItemInfo[] m_LuckyStartRewards;
        private int m_minUseNum;
        private int[] m_openCardPrice;
        protected GamePlayer m_player;
        private PyramidInfo m_pyramid;
        private PyramidConfigInfo m_pyramidConfig;
        private List<NewChickenBoxItemInfo> m_RemoveChickenBoxRewards;
        private bool m_saveToDb;
        private ThreadSafeRandom rand = new ThreadSafeRandom();

        public PlayerActives(GamePlayer player, bool saveTodb)
        {
            this.m_player = player;
            this.m_saveToDb = saveTodb;
            this.m_eagleEyePrice = GameProperties.ConvertStringArrayToIntArray("NewChickenEagleEyePrice");
            this.m_openCardPrice = GameProperties.ConvertStringArrayToIntArray("NewChickenOpenCardPrice");
            this.m_boguAdventureMoney = GameProperties.ConvertStringArrayToIntArray("BoguAdventurePrice");
            this.m_flushPrice = GameProperties.NewChickenFlushPrice;
            this.m_freeFlushTime = 120;
            this.m_RemoveChickenBoxRewards = new List<NewChickenBoxItemInfo>();
            this.m_freeEyeCount = 0;
            this.m_freeOpenCardCount = 0;
            this.m_freeRefreshBoxCount = 0;
            this.SetupPyramidConfig();
            this.SetupLuckyStart();
        }

        public void AddTime(int min)
        {
            lock (this.m_christmas)
            {
                this.m_christmas.AvailTime += min;
            }
        }

        public bool AvailTime()
        {
            DateTime gameBeginTime = this.Christmas.gameBeginTime;
            DateTime gameEndTime = this.Christmas.gameEndTime;
            TimeSpan span = (TimeSpan) (DateTime.Now - gameBeginTime);
            TimeSpan span2 = (TimeSpan) (gameEndTime - gameBeginTime);
            double num = span2.TotalMinutes - span.TotalMinutes;
            return (num > 0.0);
        }

        public void BeginChristmasTimer()
        {
            int dueTime = 0xea60;
            if (this._christmasTimer == null)
            {
                this._christmasTimer = new Timer(new TimerCallback(this.ChristmasTimeCheck), null, dueTime, dueTime);
            }
            else
            {
                this._christmasTimer.Change(dueTime, dueTime);
            }
        }

        private void BeginLabyrinthTimer()
        {
            int dueTime = 0x3e8;
            if (this._labyrinthTimer == null)
            {
                this._labyrinthTimer = new Timer(new TimerCallback(this.LabyrinthCheck), null, dueTime, dueTime);
            }
            else
            {
                this._labyrinthTimer.Change(dueTime, dueTime);
            }
        }

        public void BeginLightriddleTimer()
        {
            int dueTime = 0x3e8;
            if (this._lightriddleTimer == null)
            {
                this._lightriddleTimer = new Timer(new TimerCallback(this.LightriddleCheck), null, dueTime, dueTime);
            }
            else
            {
                this._lightriddleTimer.Change(dueTime, dueTime);
            }
        }

        private bool CanGetBigAward()
        {
            for (int i = 0; i <= this.m_player.Labyrinth.myProgress; i += 2)
            {
                if (i == this.m_player.Labyrinth.currentFloor)
                {
                    return true;
                }
            }
            return false;
        }

        public void CleantOutLabyrinth()
        {
            this.BeginLabyrinthTimer();
        }

        public int CountOpenCanTakeBoxGoguAdventure(int type)
        {
            switch (type)
            {
                case 0:
                    return this.countBox1GoguAward;

                case 1:
                    return this.countBox2GoguAward;

                case 2:
                    return this.countBox3GoguAward;
            }
            return 0;
        }

        public List<BoguCeilInfo> CovertBoguMapToArray(string boguMap)
        {
            List<BoguCeilInfo> list = new List<BoguCeilInfo>();
            foreach (string str in boguMap.Split(new char[] { '|' }))
            {
                string[] strArray3 = str.Split(new char[] { ',' });
                BoguCeilInfo item = new BoguCeilInfo {
                    Index = int.Parse(strArray3[0]),
                    State = int.Parse(strArray3[1]),
                    Result = int.Parse(strArray3[2])
                };
                list.Add(item);
            }
            return list;
        }

        public string CovertBoguMapToString(BoguCeilInfo[] boguMap)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < boguMap.Length; i++)
            {
                BoguCeilInfo info = boguMap[i];
                string item = string.Concat(new object[] { info.Index, ",", info.State, ",", info.Result });
                list.Add(item);
            }
            return string.Join("|", list.ToArray());
        }

        public void CreateActiveSystemInfo(int UserID, string name)
        {
            lock (this.m_lock)
            {
                this.m_activeInfo = new ActiveSystemInfo();
                this.m_activeInfo.ID = 0;
                this.m_activeInfo.UserID = UserID;
                this.m_activeInfo.useableScore = 0;
                this.m_activeInfo.totalScore = 0;
                this.m_activeInfo.AvailTime = 0;
                this.m_activeInfo.NickName = name;
                this.m_activeInfo.dayScore = 0;
                this.m_activeInfo.CanGetGift = true;
                this.m_activeInfo.canOpenCounts = 5;
                this.m_activeInfo.canEagleEyeCounts = 5;
                this.m_activeInfo.lastFlushTime = DateTime.Now;
                this.m_activeInfo.isShowAll = true;
                this.m_activeInfo.ActiveMoney = 0;
                this.m_activeInfo.activityTanabataNum = 0;
                this.m_activeInfo.LuckystarCoins = this.defaultCoins;
                this.m_activeInfo.ChallengeNum = GameProperties.YearMonsterFightNum;
                this.m_activeInfo.BuyBuffNum = GameProperties.YearMonsterFightNum;
                this.m_activeInfo.lastEnterYearMonter = DateTime.Now;
                this.m_activeInfo.DamageNum = 0;
                this.CreateYearMonterBoxState();
            }
        }

        public void CreateBoguAdventureInfo()
        {
            lock (this.m_lock)
            {
                BoguCeilInfo[] boguMap = this.CreateRandomBoguMap();
                this.m_boguAdventure = new UserBoguAdventureInfo();
                this.m_boguAdventure.UserID = this.Player.PlayerCharacter.ID;
                this.m_boguAdventure.CurrentPostion = 0;
                this.m_boguAdventure.OpenCount = 0;
                this.m_boguAdventure.HP = 2;
                this.m_boguAdventure.ResetCount = this.countBoguReset;
                this.m_boguAdventure.Map = this.CovertBoguMapToString(boguMap);
                this.m_boguAdventure.MapData = boguMap.ToList<BoguCeilInfo>();
                this.m_boguAdventure.Award = "0,0,0";
                this.SaveToDatabase();
            }
        }

        public NewChickenBoxItemInfo[] CreateChickenBoxAward(int count, eEventType DataId)
        {
            List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
            Dictionary<int, NewChickenBoxItemInfo> dictionary = new Dictionary<int, NewChickenBoxItemInfo>();
            int num = 0;
            for (int i = 0; list.Count < count; i++)
            {
                List<NewChickenBoxItemInfo> newChickenBoxAward = EventAwardMgr.GetNewChickenBoxAward(DataId);
                if (newChickenBoxAward.Count > 0)
                {
                    NewChickenBoxItemInfo info = newChickenBoxAward[0];
                    if (!dictionary.Keys.Contains<int>(info.TemplateID))
                    {
                        dictionary.Add(info.TemplateID, info);
                        info.Position = num;
                        list.Add(info);
                        num++;
                    }
                }
            }
            return list.ToArray();
        }

        public void CreateChristmasInfo(int UserID)
        {
            lock (this.m_lock)
            {
                this.m_christmas = new UserChristmasInfo();
                this.m_christmas.ID = 0;
                this.m_christmas.UserID = UserID;
                this.m_christmas.count = 0;
                this.m_christmas.exp = 0;
                this.m_christmas.awardState = 0;
                this.m_christmas.lastPacks = 0x44c;
                this.m_christmas.packsNumber = -1;
                this.m_christmas.gameBeginTime = DateTime.Now;
                this.m_christmas.gameEndTime = DateTime.Now.AddMinutes(60.0);
                this.m_christmas.isEnter = false;
                this.m_christmas.dayPacks = 0;
                this.m_christmas.AvailTime = 0;
            }
        }

        public void CreateLuckyStartAward()
        {
            this.m_LuckyStartRewards = this.CreateChickenBoxAward(this.LuckyStartBoxCount, eEventType.LUCKY_STAR);
            NewChickenBoxItemInfo info = new NewChickenBoxItemInfo {
                TemplateID = this.coinTemplateID,
                StrengthenLevel = 0,
                Count = 1,
                IsBinds = true,
                Quality = 1
            };
            this.m_LuckyStartRewards[0] = info;
        }

        public void CreatePyramidInfo()
        {
            lock (this.m_lock)
            {
                this.m_pyramid = new PyramidInfo();
                this.m_pyramid.ID = 0;
                this.m_pyramid.UserID = this.Player.PlayerCharacter.ID;
                this.m_pyramid.currentLayer = 1;
                this.m_pyramid.maxLayer = 1;
                this.m_pyramid.totalPoint = 0;
                this.m_pyramid.turnPoint = 0;
                this.m_pyramid.pointRatio = 0;
                this.m_pyramid.currentFreeCount = 0;
                this.m_pyramid.currentReviveCount = 0;
                this.m_pyramid.isPyramidStart = false;
                this.m_pyramid.LayerItems = "";
            }
        }

        private BoguCeilInfo[] CreateRandomBoguMap()
        {
            BoguCeilInfo info = new BoguCeilInfo();
            BoguCeilInfo[] infoArray = new BoguCeilInfo[70];
            int[] source = this.RandomMine();
            for (int i = 0; i < 70; i++)
            {
                infoArray[i] = new BoguCeilInfo { Index = i + 1, State = 3, Result = source.Contains<int>((i + 1)) ? -1 : -2, AroundCount = 0 };
            }
            return infoArray;
        }

        public void CreateYearMonterBoxState()
        {
            string[] strArray = GameProperties.YearMonsterBoxInfo.Split(new char[] { '|' });
            int length = strArray.Length;
            string[] strArray2 = new string[length];
            for (int i = 0; i < length; i++)
            {
                int num3 = int.Parse(strArray[i].Split(new char[] { ',' })[1]) * 0x2710;
                if (num3 <= this.m_activeInfo.DamageNum)
                {
                    strArray2[i] = "2";
                }
                else
                {
                    strArray2[i] = "1";
                }
            }
            this.m_activeInfo.BoxState = string.Join("-", strArray2);
        }

        public void ChangeLuckyStartAwardPlace()
        {
        }

        protected void ChristmasTimeCheck(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                this.UpdateChristmasTime();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
            }
            catch (Exception exception)
            {
                Console.WriteLine("ChristmasTimeCheck: " + exception);
            }
        }

        public void EnterChickenBox()
        {
            if (this.m_ChickenBoxRewards == null)
            {
                this.LoadChickenBox();
            }
        }

        public BoguCeilInfo FindCeilBoguMap(int index)
        {
            lock (this.m_lock)
            {
                if (this.m_boguAdventure.MapData == null)
                {
                    this.m_boguAdventure.MapData = this.CovertBoguMapToArray(this.m_boguAdventure.Map);
                }
                foreach (BoguCeilInfo info2 in this.m_boguAdventure.MapData)
                {
                    if (info2.Index == index)
                    {
                        return info2;
                    }
                }
                return null;
            }
        }

        private void GetAward()
        {
            int index = this.rand.Next(this.m_LuckyStartRewards.Length);
            this.m_award = this.m_LuckyStartRewards[index];
        }

        public NewChickenBoxItemInfo GetAward(int pos)
        {
            foreach (NewChickenBoxItemInfo info in this.m_ChickenBoxRewards)
            {
                if (!((info.Position != pos) || info.IsSelected))
                {
                    return info;
                }
            }
            return null;
        }

        public int[] GetCountAroundIndex(int index)
        {
            List<int> list = new List<int>();
            int[] source = new int[] { 1, 11, 0x15, 0x1f, 0x29, 0x33, 0x3d };
            int[] numArray2 = new int[] { 10, 20, 30, 40, 50, 60, 70 };
            if ((index > 0) && (index <= 70))
            {
                int item = index - 1;
                if ((item >= 1) && (item <= 70))
                {
                    list.Add(item);
                }
                int num2 = index - 9;
                if ((num2 >= 1) && (num2 <= 70))
                {
                    list.Add(num2);
                }
                int num3 = index - 10;
                if ((num3 >= 1) && (num3 <= 70))
                {
                    list.Add(num3);
                }
                int num4 = index - 11;
                if ((num4 >= 1) && (num4 <= 70))
                {
                    list.Add(num4);
                }
                int num5 = index + 1;
                if ((num5 >= 1) && (num5 <= 70))
                {
                    list.Add(num5);
                }
                int num6 = index + 9;
                if ((num6 >= 1) && (num6 <= 70))
                {
                    list.Add(num6);
                }
                int num7 = index + 10;
                if ((num7 >= 1) && (num7 <= 70))
                {
                    list.Add(num7);
                }
                int num8 = index + 11;
                if ((num8 >= 1) && (num8 <= 70))
                {
                    list.Add(num8);
                }
                if (source.Contains<int>(index))
                {
                    list.Remove(item);
                    list.Remove(num6);
                    list.Remove(num4);
                }
                if (numArray2.Contains<int>(index))
                {
                    list.Remove(num5);
                    list.Remove(num2);
                    list.Remove(num8);
                }
            }
            return list.ToArray();
        }

        private void GetLabyrinthAward()
        {
            int index = this.m_player.Labyrinth.currentFloor - 1;
            int gp = this.m_player.CreateExps()[index];
            string str = this.m_player.labyrinthGolds[index];
            int count = int.Parse(str.Split(new char[] { '|' })[0]);
            int num4 = int.Parse(str.Split(new char[] { '|' })[1]);
            if (!((this.m_player.PropBag.GetItemByTemplateID(0, 0x2e8c) != null) && this.m_player.RemoveTemplate(0x2e8c, 1)))
            {
                this.m_player.Labyrinth.isDoubleAward = false;
            }
            if (this.m_player.Labyrinth.isDoubleAward)
            {
                int num5 = 2;
                gp *= num5;
                count *= num5;
                num4 *= num5;
            }
            UserLabyrinthInfo labyrinth = this.m_player.Labyrinth;
            labyrinth.accumulateExp += gp;
            List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
            if (this.CanGetBigAward())
            {
                infos = this.m_player.CopyDrop(2, 0x9c42);
                this.m_player.AddTemplate(infos, count, eGameView.dungeonTypeGet);
                this.m_player.AddHardCurrency(num4);
            }
            this.m_player.AddGP(gp);
            this.PlusCleantOutInfo(this.m_player.Labyrinth.currentFloor, gp, num4, infos);
        }

        public BoguCeilInfo[] GetTotalMineAround(int index)
        {
            lock (this.m_lock)
            {
                List<BoguCeilInfo> list = new List<BoguCeilInfo>();
                foreach (int num2 in this.GetCountAroundIndex(index))
                {
                    BoguCeilInfo item = this.FindCeilBoguMap(num2);
                    if ((item != null) && (item.Result == -1))
                    {
                        list.Add(item);
                    }
                }
                return list.ToArray();
            }
        }

        public BoguCeilInfo[] GetTotalMineAroundNotOpen(int index)
        {
            lock (this.m_lock)
            {
                List<BoguCeilInfo> list = new List<BoguCeilInfo>();
                foreach (int num2 in this.GetCountAroundIndex(index))
                {
                    BoguCeilInfo item = this.FindCeilBoguMap(num2);
                    if (((item != null) && (item.Result == -1)) && (item.State == 3))
                    {
                        list.Add(item);
                    }
                }
                return list.ToArray();
            }
        }

        public bool IsChickenBoxOpen()
        {
            Convert.ToDateTime(GameProperties.NewChickenBeginTime);
            DateTime time = Convert.ToDateTime(GameProperties.NewChickenEndTime);
            return (DateTime.Now.Date < time.Date);
        }

        public bool IsChristmasOpen()
        {
            Convert.ToDateTime(GameProperties.ChristmasBeginDate);
            DateTime time = Convert.ToDateTime(GameProperties.ChristmasEndDate);
            return (DateTime.Now.Date < time.Date);
        }

        public bool IsDragonBoatOpen()
        {
            Convert.ToDateTime(GameProperties.DragonBoatBeginDate);
            DateTime time = Convert.ToDateTime(GameProperties.DragonBoatEndDate);
            return (DateTime.Now.Date < time.Date);
        }

        public bool IsFreeFlushTime()
        {
            DateTime lastFlushTime = this.Info.lastFlushTime;
            DateTime time2 = lastFlushTime.AddMinutes((double) this.freeFlushTime);
            TimeSpan span = (TimeSpan) (DateTime.Now - this.Info.lastFlushTime);
            TimeSpan span2 = (TimeSpan) (time2 - lastFlushTime);
            double num = span2.TotalMinutes - span.TotalMinutes;
            return (num > 0.0);
        }

        public bool IsLuckStarActivityOpen()
        {
            Convert.ToDateTime(GameProperties.LuckStarActivityBeginDate);
            DateTime time = Convert.ToDateTime(GameProperties.LuckStarActivityEndDate);
            return (DateTime.Now.Date < time.Date);
        }

        public bool IsPyramidOpen()
        {
            Convert.ToDateTime(GameProperties.PyramidBeginTime);
            DateTime time = Convert.ToDateTime(GameProperties.PyramidEndTime);
            return (DateTime.Now.Date < time.Date);
        }

        public bool IsYearMonsterOpen()
        {
            Convert.ToDateTime(GameProperties.YearMonsterBeginDate);
            DateTime time = Convert.ToDateTime(GameProperties.YearMonsterEndDate);
            return (DateTime.Now.Date < time.Date);
        }

        protected void LabyrinthCheck(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                this.UpdateLabyrinthTime();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
            }
            catch (Exception exception)
            {
                Console.WriteLine("LabyrinthCheck: " + exception);
            }
        }

        protected void LightriddleCheck(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                if (this._lightriddleColdown > 0)
                {
                    this._lightriddleColdown--;
                    if (this._lightriddleColdown == 1)
                    {
                        LanternriddlesInfo lanternriddles = ActiveSystemMgr.GetLanternriddles(this.m_player.PlayerId);
                        if (lanternriddles == null)
                        {
                            this.StopLightriddleTimer();
                            return;
                        }
                        LightriddleQuestInfo getCurrentQuestion = lanternriddles.GetCurrentQuestion;
                        string str = "5 Chiến Hồn Đơn, 10.000 EXP v\x00e0 29 điểm t\x00edch lũy";
                        string str2 = "1 Chiến Hồn Đơn v\x00e0 1.000 EXP.";
                        string award = "Hệ thống Nguy\x00ean Ti\x00eau lổi.";
                        int gp = 0x3e8;
                        SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(0x18704), 1, 0x69);
                        bool iscorrect = false;
                        if (getCurrentQuestion != null)
                        {
                            if (lanternriddles.IsHint)
                            {
                                lanternriddles.Option = getCurrentQuestion.OptionTrue;
                            }
                            if (lanternriddles.Option == getCurrentQuestion.OptionTrue)
                            {
                                cloneItem.Count = 5;
                                gp = 0x2710;
                                lanternriddles.MyInteger += 0x1d;
                                lanternriddles.QuestionNum++;
                                if (lanternriddles.IsDouble)
                                {
                                    str = "5 Chiến Hồn Đơn, 10.000 EXP v\x00e0 58 điểm t\x00edch lũy";
                                    lanternriddles.MyInteger += 0x1d;
                                }
                                award = str;
                                iscorrect = true;
                            }
                            else
                            {
                                cloneItem.Count = 1;
                                award = str2;
                            }
                        }
                        if (lanternriddles.Option > 0)
                        {
                            this.m_player.AddGP(gp);
                            cloneItem.IsBinds = true;
                            this.m_player.AddTemplate(cloneItem);
                            this.SendLightriddleAnswerResult(iscorrect, lanternriddles.Option, award);
                        }
                    }
                }
                else
                {
                    LanternriddlesInfo info5 = ActiveSystemMgr.GetLanternriddles(this.m_player.PlayerId);
                    if (info5 == null)
                    {
                        this.StopLightriddleTimer();
                        return;
                    }
                    if (info5.CanNextQuest)
                    {
                        info5.QuestionIndex++;
                        info5.Option = -1;
                        info5.IsHint = false;
                        info5.IsDouble = false;
                        info5.EndDate = ActiveSystemMgr.EndDate;
                        this.SendLightriddleQuestion(info5);
                        this._lightriddleColdown = 15;
                    }
                    else
                    {
                        info5.QuestionIndex = info5.QuestionView;
                        info5.IsHint = true;
                        info5.IsDouble = true;
                        info5.EndDate = DateTime.Now;
                        this.SendLightriddleQuestion(info5);
                        this.StopLightriddleTimer();
                    }
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
            }
            catch (Exception exception)
            {
                Console.WriteLine("LabyrinthCheck: " + exception);
            }
        }

        public void LoadChickenBox()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                this.m_ChickenBoxRewards = bussiness.GetSingleNewChickenBox(this.Player.PlayerCharacter.ID);
                if (this.m_ChickenBoxRewards.Length == 0)
                {
                    this.PayFlushView();
                }
            }
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    if (this.IsChristmasOpen())
                    {
                        this.m_christmas = bussiness.GetSingleUserChristmas(this.Player.PlayerCharacter.ID);
                        if (this.m_christmas == null)
                        {
                            this.CreateChristmasInfo(this.Player.PlayerCharacter.ID);
                        }
                    }
                    this.m_activeInfo = bussiness.GetSingleActiveSystem(this.Player.PlayerCharacter.ID);
                    if (this.m_activeInfo == null)
                    {
                        this.CreateActiveSystemInfo(this.Player.PlayerCharacter.ID, this.Player.PlayerCharacter.NickName);
                    }
                    this.m_boguAdventure = bussiness.GetSingleBoguAdventure(this.Player.PlayerCharacter.ID);
                    if (this.m_boguAdventure == null)
                    {
                        this.CreateBoguAdventureInfo();
                    }
                    else
                    {
                        this.m_boguAdventure.MapData = this.CovertBoguMapToArray(this.m_boguAdventure.Map);
                    }
                }
            }
        }

        public bool LoadPyramid()
        {
            if (this.m_pyramid == null)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    this.m_pyramid = bussiness.GetSinglePyramid(this.Player.PlayerCharacter.ID);
                    if (this.m_pyramid == null)
                    {
                        this.CreatePyramidInfo();
                    }
                }
            }
            return true;
        }

        public void PayFlushView()
        {
            this.m_activeInfo.lastFlushTime = DateTime.Now;
            this.m_activeInfo.isShowAll = true;
            this.m_activeInfo.canOpenCounts = 5;
            this.m_activeInfo.canEagleEyeCounts = 5;
            this.RemoveChickenBoxRewards();
            this.m_ChickenBoxRewards = this.CreateChickenBoxAward(this.ChikenBoxCount, eEventType.CHICKEN_BOX);
            for (int i = 0; i < this.m_ChickenBoxRewards.Length; i++)
            {
                this.m_ChickenBoxRewards[i].UserID = this.Player.PlayerCharacter.ID;
            }
        }

        private void PlusCleantOutInfo(int FamRaidLevel, int exp, int HardCurrency, List<SqlDataProvider.Data.ItemInfo> lists)
        {
            GSPacketIn pkg = new GSPacketIn(0x83, this.m_player.PlayerId);
            pkg.WriteByte(7);
            pkg.WriteInt(FamRaidLevel);
            pkg.WriteInt(exp);
            pkg.WriteInt(lists.Count);
            foreach (SqlDataProvider.Data.ItemInfo info in lists)
            {
                pkg.WriteInt(info.TemplateID);
                pkg.WriteInt(info.Count);
            }
            pkg.WriteInt(HardCurrency);
            this.m_player.SendTCP(pkg);
        }

        private int[] RandomMine()
        {
            List<int> list = new List<int>();
            int num = 20;
            List<int> list2 = new List<int>();
            for (int i = 1; i <= 70; i++)
            {
                list2.Add(i);
            }
            while (num > 0)
            {
                int index = this.rand.Next(0, list2.Count - 1);
                list.Add(list2[index]);
                list2.RemoveAt(index);
                num--;
            }
            return list.ToArray();
        }

        private int[] RandomMine2()
        {
            List<int> list = new List<int>();
            List<int> list2 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<int> list3 = new List<int> { 11, 12, 13, 14, 15, 0x10, 0x11, 0x12, 0x13, 20 };
            List<int> list4 = new List<int> { 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 30 };
            List<int> list5 = new List<int> { 0x1f, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 40 };
            List<int> list6 = new List<int> { 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, 0x31, 50 };
            List<int> list7 = new List<int> { 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 60 };
            List<int> list8 = new List<int> { 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 70 };
            int num = this.rand.Next(2, 4);
            int num2 = this.rand.Next(2, 4);
            int num3 = this.rand.Next(2, 4);
            int num4 = this.rand.Next(2, 4);
            int num5 = this.rand.Next(2, 4);
            int num6 = this.rand.Next(2, 4);
            int num7 = this.rand.Next(2, 4);
            for (int i = 0; i < 7; i++)
            {
                int num9;
                switch (i)
                {
                    case 0:
                        goto Label_03DB;

                    case 1:
                    {
                        while (num2 > 0)
                        {
                            num9 = this.rand.Next(0, list3.Count - 1);
                            list.Add(list3[num9]);
                            list3.RemoveAt(num9);
                            num2--;
                        }
                        continue;
                    }
                    case 2:
                    {
                        while (num3 > 0)
                        {
                            num9 = this.rand.Next(0, list4.Count - 1);
                            list.Add(list4[num9]);
                            list4.RemoveAt(num9);
                            num3--;
                        }
                        continue;
                    }
                    case 3:
                    {
                        while (num4 > 0)
                        {
                            num9 = this.rand.Next(0, list5.Count - 1);
                            list.Add(list5[num9]);
                            list5.RemoveAt(num9);
                            num4--;
                        }
                        continue;
                    }
                    case 4:
                    {
                        while (num5 > 0)
                        {
                            num9 = this.rand.Next(0, list6.Count - 1);
                            list.Add(list6[num9]);
                            list6.RemoveAt(num9);
                            num5--;
                        }
                        continue;
                    }
                    case 5:
                    {
                        while (num6 > 0)
                        {
                            num9 = this.rand.Next(0, list7.Count - 1);
                            list.Add(list7[num9]);
                            list7.RemoveAt(num9);
                            num6--;
                        }
                        continue;
                    }
                    case 6:
                    {
                        while (num7 > 0)
                        {
                            num9 = this.rand.Next(0, list8.Count - 1);
                            list.Add(list8[num9]);
                            list8.RemoveAt(num9);
                            num7--;
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
            Label_03A5:
                num9 = this.rand.Next(0, list2.Count - 1);
                list.Add(list2[num9]);
                list2.RemoveAt(num9);
                num--;
            Label_03DB:
                if (num > 0)
                {
                    goto Label_03A5;
                }
            }
            return list.ToArray();
        }

        public void RandomPosition()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < this.m_ChickenBoxRewards.Length; i++)
            {
                list.Add(this.m_ChickenBoxRewards[i].Position);
            }
            this.rand.Shuffer<NewChickenBoxItemInfo>(this.m_ChickenBoxRewards);
            for (int j = 0; j < list.Count; j++)
            {
                this.m_ChickenBoxRewards[j].Position = list[j];
            }
        }

        public void RemoveChickenBoxRewards()
        {
            for (int i = 0; i < this.m_ChickenBoxRewards.Length; i++)
            {
                NewChickenBoxItemInfo item = this.m_ChickenBoxRewards[i];
                if ((item != null) && (item.ID > 0))
                {
                    item.Position = -1;
                    this.m_RemoveChickenBoxRewards.Add(item);
                }
            }
        }

        public void ResetBoguAdventureInfo()
        {
            lock (this.m_lock)
            {
                BoguCeilInfo[] boguMap = this.CreateRandomBoguMap();
                this.m_boguAdventure.CurrentPostion = 0;
                this.m_boguAdventure.OpenCount = 0;
                this.m_boguAdventure.HP = 2;
                this.m_boguAdventure.Map = this.CovertBoguMapToString(boguMap);
                this.m_boguAdventure.MapData = boguMap.ToList<BoguCeilInfo>();
            }
        }

        public void ResetChristmas()
        {
            lock (this.m_lock)
            {
                if (this.m_christmas != null)
                {
                    this.m_christmas.dayPacks = 0;
                    this.m_christmas.AvailTime = 0;
                    this.m_christmas.isEnter = false;
                    this.m_activeInfo.dayScore = 0;
                }
            }
        }

        public void ResetDragonBoat()
        {
            lock (this.m_lock)
            {
                this.m_activeInfo.useableScore = 0;
                this.m_activeInfo.totalScore = 0;
                this.m_activeInfo.dayScore = 0;
                this.m_activeInfo.CanGetGift = true;
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
                        if ((this.m_pyramid != null) && this.m_pyramid.IsDirty)
                        {
                            if (this.m_pyramid.ID > 0)
                            {
                                bussiness.UpdatePyramid(this.m_pyramid);
                            }
                            else
                            {
                                bussiness.AddPyramid(this.m_pyramid);
                            }
                        }
                        if ((this.m_boguAdventure != null) && this.m_boguAdventure.IsDirty)
                        {
                            bussiness.UpdateBoguAdventure(this.m_boguAdventure);
                        }
                        if ((this.m_christmas != null) && this.m_christmas.IsDirty)
                        {
                            if (this.m_christmas.ID > 0)
                            {
                                bussiness.UpdateUserChristmas(this.m_christmas);
                            }
                            else
                            {
                                bussiness.AddUserChristmas(this.m_christmas);
                            }
                        }
                        if ((this.m_activeInfo != null) && this.m_activeInfo.IsDirty)
                        {
                            if (this.m_activeInfo.ID > 0)
                            {
                                bussiness.UpdateActiveSystem(this.m_activeInfo);
                            }
                            else
                            {
                                bussiness.AddActiveSystem(this.m_activeInfo);
                            }
                        }
                        if (this.m_ChickenBoxRewards != null)
                        {
                            foreach (NewChickenBoxItemInfo info in this.m_ChickenBoxRewards)
                            {
                                if ((info != null) && info.IsDirty)
                                {
                                    if (info.ID > 0)
                                    {
                                        bussiness.UpdateNewChickenBox(info);
                                    }
                                    else
                                    {
                                        bussiness.AddNewChickenBox(info);
                                    }
                                }
                            }
                        }
                        if (this.m_RemoveChickenBoxRewards.Count > 0)
                        {
                            foreach (NewChickenBoxItemInfo info2 in this.m_RemoveChickenBoxRewards)
                            {
                                bussiness.UpdateNewChickenBox(info2);
                            }
                        }
                    }
                }
            }
            LanternriddlesInfo lanternriddles = ActiveSystemMgr.GetLanternriddles(this.m_player.PlayerCharacter.ID);
        }

        public void SendChickenBoxItemList()
        {
            GSPacketIn pkg = new GSPacketIn(0x57);
            pkg.WriteInt(3);
            pkg.WriteDateTime(this.Info.lastFlushTime);
            pkg.WriteInt(this.freeFlushTime);
            pkg.WriteInt(this.freeRefreshBoxCount);
            pkg.WriteInt(this.freeEyeCount);
            pkg.WriteInt(this.freeOpenCardCount);
            pkg.WriteBoolean(this.Info.isShowAll);
            pkg.WriteInt(this.ChickenBoxRewards.Length);
            foreach (NewChickenBoxItemInfo info in this.ChickenBoxRewards)
            {
                pkg.WriteInt(info.TemplateID);
                pkg.WriteInt(info.StrengthenLevel);
                pkg.WriteInt(info.Count);
                pkg.WriteInt(info.ValidDate);
                pkg.WriteInt(info.AttackCompose);
                pkg.WriteInt(info.DefendCompose);
                pkg.WriteInt(info.AgilityCompose);
                pkg.WriteInt(info.LuckCompose);
                pkg.WriteInt(info.Position);
                pkg.WriteBoolean(info.IsSelected);
                pkg.WriteBoolean(info.IsSeeded);
                pkg.WriteBoolean(info.IsBinds);
            }
            this.m_player.SendTCP(pkg);
        }

        public void SendDragonBoatAward()
        {
            if ((DateTime.Now.DayOfWeek == DayOfWeek.Sunday) && this.IsDragonBoatOpen())
            {
                int myRank;
                int dragonBoatMinScore = GameProperties.DragonBoatMinScore;
                int dragonBoatAreaMinScore = GameProperties.DragonBoatAreaMinScore;
                List<ActiveSystemInfo> list = ActiveSystemMgr.SelectTopTenCurrenServer(dragonBoatMinScore);
                int num3 = 0;
                List<SqlDataProvider.Data.ItemInfo> list2 = new List<SqlDataProvider.Data.ItemInfo>();
                string format = "Phần thưởng Thuyền rồng hạng {0}";
                foreach (ActiveSystemInfo info in list)
                {
                    if ((info.UserID == this.Player.PlayerCharacter.ID) && this.m_activeInfo.CanGetGift)
                    {
                        myRank = info.myRank;
                        if (myRank <= 10)
                        {
                            format = string.Format(format, myRank);
                            WorldEventMgr.SendItemsToMail(CommunalActiveMgr.GetAwardInfos(1, myRank), info.UserID, this.Player.PlayerCharacter.NickName, format);
                            num3++;
                        }
                        break;
                    }
                }
                list = ActiveSystemMgr.SelectTopTenAllServer(dragonBoatAreaMinScore);
                foreach (ActiveSystemInfo info2 in list)
                {
                    if ((info2.UserID == this.Player.PlayerCharacter.ID) && this.m_activeInfo.CanGetGift)
                    {
                        myRank = info2.myRank;
                        if (myRank <= 10)
                        {
                            format = string.Format(format, myRank) + " li\x00ean server";
                            WorldEventMgr.SendItemsToMail(CommunalActiveMgr.GetAwardInfos(2, myRank), info2.UserID, this.Player.PlayerCharacter.NickName, format);
                            num3++;
                        }
                        break;
                    }
                }
                if (num3 > 0)
                {
                    this.m_activeInfo.CanGetGift = false;
                }
            }
        }

        public void SendEvent()
        {
            if (this.IsChickenBoxOpen())
            {
                this.m_player.Out.SendChickenBoxOpen(this.m_player.PlayerId, this.flushPrice, this.openCardPrice, this.eagleEyePrice);
            }
            if (this.IsLuckStarActivityOpen())
            {
                this.m_player.Out.SendLuckStarOpen(this.m_player.PlayerId);
            }
        }

        public GSPacketIn SendLightriddleAnswerResult(bool Iscorrect, int option, string award)
        {
            GSPacketIn pkg = new GSPacketIn(0x91, this.m_player.PlayerId);
            pkg.WriteByte(0x27);
            pkg.WriteBoolean(Iscorrect);
            pkg.WriteBoolean(Iscorrect);
            pkg.WriteInt(option);
            pkg.WriteString(award);
            this.m_player.SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn SendLightriddleQuestion(LanternriddlesInfo Lanternriddles)
        {
            GSPacketIn pkg = new GSPacketIn(0x91, this.m_player.PlayerId);
            pkg.WriteByte(0x26);
            pkg.WriteInt(Lanternriddles.QuestionIndex);
            pkg.WriteInt(Lanternriddles.GetQuestionID);
            pkg.WriteInt(Lanternriddles.QuestionView);
            pkg.WriteDateTime(Lanternriddles.EndDate);
            pkg.WriteInt(Lanternriddles.DoubleFreeCount);
            pkg.WriteInt(Lanternriddles.DoublePrice);
            pkg.WriteInt(Lanternriddles.HitFreeCount);
            pkg.WriteInt(Lanternriddles.HitPrice);
            pkg.WriteInt(Lanternriddles.MyInteger);
            pkg.WriteInt(Lanternriddles.QuestionNum);
            pkg.WriteInt(Lanternriddles.Option);
            pkg.WriteBoolean(Lanternriddles.IsHint);
            pkg.WriteBoolean(Lanternriddles.IsDouble);
            this.m_player.SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn SendLightriddleRank(int myRank, List<RankingLightriddleInfo> list)
        {
            GSPacketIn pkg = new GSPacketIn(0x91, this.m_player.PlayerId);
            pkg.WriteByte(0x2a);
            pkg.WriteInt(myRank);
            pkg.WriteInt(list.Count);
            foreach (RankingLightriddleInfo info in list)
            {
                pkg.WriteInt(info.Rank);
                pkg.WriteString(info.NickName);
                pkg.WriteByte((byte) info.TypeVIP);
                pkg.WriteInt(info.Integer);
                List<LuckyStartToptenAwardInfo> lanternriddlesAwardByRank = WorldEventMgr.GetLanternriddlesAwardByRank(info.Rank);
                pkg.WriteInt(lanternriddlesAwardByRank.Count);
                foreach (LuckyStartToptenAwardInfo info2 in lanternriddlesAwardByRank)
                {
                    pkg.WriteInt(info2.TemplateID);
                    pkg.WriteInt(info2.Count);
                    pkg.WriteBoolean(info2.IsBinds);
                    pkg.WriteInt(info2.Validate);
                }
            }
            this.m_player.SendTCP(pkg);
            return pkg;
        }

        public void SendLuckStarAllGoodsInfo()
        {
            GSPacketIn pkg = new GSPacketIn(0x57, this.m_player.PlayerId);
            pkg.WriteInt(0x15);
            pkg.WriteInt(this.m_activeInfo.LuckystarCoins);
            pkg.WriteDateTime(this.LuckyBegindate);
            pkg.WriteDateTime(this.LuckyEnddate);
            pkg.WriteInt(this.minUseNum);
            int length = this.m_LuckyStartRewards.Length;
            int index = 0;
            pkg.WriteInt(length);
            while (index < length)
            {
                pkg.WriteInt(this.m_LuckyStartRewards[index].TemplateID);
                pkg.WriteInt(this.m_LuckyStartRewards[index].StrengthenLevel);
                pkg.WriteInt(this.m_LuckyStartRewards[index].Count);
                pkg.WriteInt(this.m_LuckyStartRewards[index].ValidDate);
                pkg.WriteInt(this.m_LuckyStartRewards[index].AttackCompose);
                pkg.WriteInt(this.m_LuckyStartRewards[index].DefendCompose);
                pkg.WriteInt(this.m_LuckyStartRewards[index].AgilityCompose);
                pkg.WriteInt(this.m_LuckyStartRewards[index].LuckCompose);
                pkg.WriteBoolean(this.m_LuckyStartRewards[index].IsBinds);
                pkg.WriteInt(this.m_LuckyStartRewards[index].Quality);
                index++;
            }
            this.m_player.SendTCP(pkg);
        }

        public GSPacketIn SendLuckStarClose()
        {
            GSPacketIn pkg = new GSPacketIn(0x57, this.m_player.PlayerId);
            pkg.WriteInt(0x1a);
            this.m_player.SendTCP(pkg);
            return pkg;
        }

        public void SendLuckStarRewardRank()
        {
            GSPacketIn pkg = new GSPacketIn(0x57, this.m_player.PlayerId);
            pkg.WriteInt(0x1b);
            List<LuckyStartToptenAwardInfo> luckyStartToptenAward = WorldEventMgr.GetLuckyStartToptenAward();
            pkg.WriteInt(luckyStartToptenAward.Count);
            foreach (LuckyStartToptenAwardInfo info in luckyStartToptenAward)
            {
                pkg.WriteInt(info.TemplateID);
                pkg.WriteInt(info.StrengthenLevel);
                pkg.WriteInt(info.Count);
                pkg.WriteInt(info.Validate);
                pkg.WriteInt(info.AttackCompose);
                pkg.WriteInt(info.DefendCompose);
                pkg.WriteInt(info.AgilityCompose);
                pkg.WriteInt(info.LuckCompose);
                pkg.WriteBoolean(info.IsBinds);
                pkg.WriteInt(info.Type);
            }
            this.m_player.SendTCP(pkg);
        }

        public void SendLuckStarRewardRecord()
        {
            List<LuckStarRewardRecordInfo> recordList = ActiveSystemMgr.RecordList;
            GSPacketIn pkg = new GSPacketIn(0x57, this.m_player.PlayerId);
            pkg.WriteInt(0x16);
            pkg.WriteInt(recordList.Count);
            foreach (LuckStarRewardRecordInfo info in recordList)
            {
                pkg.WriteInt(info.TemplateID);
                pkg.WriteInt(info.Count);
                pkg.WriteString(info.nickName);
            }
            this.m_player.SendTCP(pkg);
        }

        public void SendLuckStarTurnGoodsInfo()
        {
            this.GetAward();
            this.m_activeInfo.LuckystarCoins += this.flushCoins;
            GSPacketIn pkg = new GSPacketIn(0x57, this.m_player.PlayerId);
            pkg.WriteInt(0x17);
            pkg.WriteInt(this.m_activeInfo.LuckystarCoins);
            pkg.WriteInt(this.Award.TemplateID);
            pkg.WriteInt(this.Award.StrengthenLevel);
            pkg.WriteInt(this.Award.Count);
            pkg.WriteInt(this.Award.ValidDate);
            pkg.WriteInt(this.Award.AttackCompose);
            pkg.WriteInt(this.Award.DefendCompose);
            pkg.WriteInt(this.Award.AgilityCompose);
            pkg.WriteInt(this.Award.LuckCompose);
            pkg.WriteBoolean(this.Award.IsBinds);
            this.m_player.SendTCP(pkg);
            if (this.Award.TemplateID == this.coinTemplateID)
            {
                if (GameProperties.IsActiveMoney)
                {
                    this.m_player.AddActiveMoney(this.m_activeInfo.LuckystarCoins);
                }
                else
                {
                    this.m_player.AddMoney(this.m_activeInfo.LuckystarCoins);
                }
                this.m_activeInfo.LuckystarCoins = this.defaultCoins;
            }
            ActiveSystemMgr.UpdateLuckStarRewardRecord(this.m_player.PlayerCharacter.ID, this.m_player.PlayerCharacter.NickName, this.Award.TemplateID, this.Award.Count, this.m_player.PlayerCharacter.typeVIP);
        }

        public void SendUpdateReward()
        {
            if (this.Award.TemplateID != this.coinTemplateID)
            {
                GSPacketIn pkg = new GSPacketIn(0x57, this.m_player.PlayerId);
                pkg.WriteInt(0x18);
                pkg.WriteInt(this.Award.TemplateID);
                pkg.WriteInt(this.Award.Count);
                pkg.WriteString(this.m_player.PlayerCharacter.NickName);
                this.m_player.SendTCP(pkg);
            }
        }

        private void SetupLuckyStart()
        {
            this.m_luckyBegindate = DateTime.Parse(GameProperties.LuckStarActivityBeginDate);
            this.m_luckyEnddate = DateTime.Parse(GameProperties.LuckStarActivityEndDate);
            this.m_minUseNum = GameProperties.MinUseNum;
        }

        private void SetupPyramidConfig()
        {
            lock (this.m_lock)
            {
                this.m_pyramidConfig = new PyramidConfigInfo();
                this.m_pyramidConfig.isOpen = this.IsPyramidOpen();
                this.m_pyramidConfig.isScoreExchange = !this.IsPyramidOpen();
                this.m_pyramidConfig.beginTime = Convert.ToDateTime(GameProperties.PyramidBeginTime);
                this.m_pyramidConfig.endTime = Convert.ToDateTime(GameProperties.PyramidEndTime);
                this.m_pyramidConfig.freeCount = 3;
                this.m_pyramidConfig.revivePrice = GameProperties.ConvertStringArrayToIntArray("PyramidRevivePrice");
                this.m_pyramidConfig.turnCardPrice = GameProperties.PyramydTurnCardPrice;
            }
        }

        public void SetYearMonterBoxState(int id)
        {
            string[] strArray = this.m_activeInfo.BoxState.Split(new char[] { '-' });
            int length = strArray.Length;
            string[] strArray2 = new string[length];
            for (int i = 0; i < length; i++)
            {
                if (i == id)
                {
                    strArray2[i] = "3";
                }
                else
                {
                    strArray2[i] = strArray[i];
                }
            }
            this.m_activeInfo.BoxState = string.Join("-", strArray2);
        }

        public void SpeededUpCleantOutLabyrinth()
        {
            UserLabyrinthInfo labyrinth = this.Player.Labyrinth;
            labyrinth.isCleanOut = false;
            labyrinth.isInGame = false;
            labyrinth.completeChallenge = false;
            labyrinth.remainTime = 0;
            labyrinth.currentRemainTime = 0;
            labyrinth.cleanOutAllTime = 0;
            for (int i = labyrinth.currentFloor; i <= labyrinth.myProgress; i++)
            {
                this.GetLabyrinthAward();
                labyrinth.currentFloor++;
            }
            labyrinth.currentFloor = labyrinth.myProgress;
            this.Player.Out.SendLabyrinthUpdataInfo(this.Player.PlayerId, labyrinth);
            this.StopLabyrinthTimer();
        }

        public void StopCleantOutLabyrinth()
        {
            UserLabyrinthInfo labyrinth = this.Player.Labyrinth;
            labyrinth.isCleanOut = false;
            this.Player.Out.SendLabyrinthUpdataInfo(this.Player.PlayerId, labyrinth);
            this.StopLabyrinthTimer();
        }

        public void StopChristmasTimer()
        {
            if (this._christmasTimer != null)
            {
                this._christmasTimer.Dispose();
                this._christmasTimer = null;
            }
        }

        public void StopLabyrinthTimer()
        {
            if (this._labyrinthTimer != null)
            {
                this._labyrinthTimer.Dispose();
                this._labyrinthTimer = null;
            }
        }

        public void StopLightriddleTimer()
        {
            if (this._lightriddleTimer != null)
            {
                this._lightriddleColdown = 15;
                this._lightriddleTimer.Dispose();
                this._lightriddleTimer = null;
            }
        }

        public bool UpdateCeilBoguMap(BoguCeilInfo ceilInfo)
        {
            lock (this.m_lock)
            {
                bool flag2 = false;
                if (this.m_boguAdventure.MapData == null)
                {
                    this.m_boguAdventure.MapData = this.CovertBoguMapToArray(this.m_boguAdventure.Map);
                }
                if (this.FindCeilBoguMap(ceilInfo.Index) != null)
                {
                    flag2 = true;
                }
                if (flag2)
                {
                    this.m_boguAdventure.Map = this.CovertBoguMapToString(this.m_boguAdventure.MapData.ToArray());
                }
                return flag2;
            }
        }

        public bool UpdateChickenBoxAward(NewChickenBoxItemInfo box)
        {
            for (int i = 0; i < this.m_ChickenBoxRewards.Length; i++)
            {
                if (this.m_ChickenBoxRewards[i].Position == box.Position)
                {
                    this.m_ChickenBoxRewards[i] = box;
                    return true;
                }
            }
            return false;
        }

        public void UpdateChristmasTime()
        {
            DateTime gameBeginTime = this.Christmas.gameBeginTime;
            DateTime gameEndTime = this.Christmas.gameEndTime;
            TimeSpan span = (TimeSpan) (DateTime.Now - gameBeginTime);
            TimeSpan span2 = (TimeSpan) (gameEndTime - gameBeginTime);
            double num = span2.TotalMinutes - span.TotalMinutes;
            lock (this.m_christmas)
            {
                this.m_christmas.AvailTime = (((int) num) < 0) ? 0 : ((int) num);
            }
        }

        public void UpdateLabyrinthTime()
        {
            UserLabyrinthInfo labyrinth = this.Player.Labyrinth;
            labyrinth.isCleanOut = true;
            labyrinth.isInGame = true;
            if ((labyrinth.remainTime > 0) && (labyrinth.currentRemainTime > 0))
            {
                labyrinth.remainTime--;
                labyrinth.currentRemainTime--;
                this.m_labyrinthCountDown--;
            }
            if (this.m_labyrinthCountDown == 0)
            {
                this.GetLabyrinthAward();
                this.m_labyrinthCountDown = 120;
                labyrinth.currentFloor++;
                if (labyrinth.currentFloor > labyrinth.myProgress)
                {
                    labyrinth.currentFloor = labyrinth.myProgress;
                    this.StopLabyrinthTimer();
                }
            }
            this.Player.Out.SendLabyrinthUpdataInfo(this.Player.PlayerId, labyrinth);
        }

        public NewChickenBoxItemInfo ViewAward(int pos)
        {
            foreach (NewChickenBoxItemInfo info in this.m_ChickenBoxRewards)
            {
                if (!((info.Position != pos) || info.IsSeeded))
                {
                    return info;
                }
            }
            return null;
        }

        public void YearMonterValidate()
        {
            lock (this.m_lock)
            {
                if (this.m_activeInfo.lastEnterYearMonter.Date < DateTime.Now.Date)
                {
                    this.m_activeInfo.ChallengeNum = GameProperties.YearMonsterFightNum;
                    this.m_activeInfo.BuyBuffNum = GameProperties.YearMonsterFightNum;
                    this.m_activeInfo.lastEnterYearMonter = DateTime.Now;
                    this.m_activeInfo.DamageNum = 0;
                    this.CreateYearMonterBoxState();
                }
            }
        }

        public NewChickenBoxItemInfo Award
        {
            get
            {
                return this.m_award;
            }
            set
            {
                this.m_award = value;
            }
        }

        public UserBoguAdventureInfo BoguAdventure
        {
            get
            {
                return this.m_boguAdventure;
            }
            set
            {
                this.m_boguAdventure = value;
            }
        }

        public int[] BoguAdventureMoney
        {
            get
            {
                return this.m_boguAdventureMoney;
            }
            set
            {
                this.m_boguAdventureMoney = value;
            }
        }

        public NewChickenBoxItemInfo[] ChickenBoxRewards
        {
            get
            {
                return this.m_ChickenBoxRewards;
            }
            set
            {
                this.m_ChickenBoxRewards = value;
            }
        }

        public UserChristmasInfo Christmas
        {
            get
            {
                return this.m_christmas;
            }
            set
            {
                this.m_christmas = value;
            }
        }

        public int[] eagleEyePrice
        {
            get
            {
                return this.m_eagleEyePrice;
            }
            set
            {
                this.m_eagleEyePrice = value;
            }
        }

        public int flushPrice
        {
            get
            {
                return this.m_flushPrice;
            }
            set
            {
                this.m_flushPrice = value;
            }
        }

        public int freeEyeCount
        {
            get
            {
                return this.m_freeEyeCount;
            }
            set
            {
                this.m_freeEyeCount = value;
            }
        }

        public int freeFlushTime
        {
            get
            {
                return this.m_freeFlushTime;
            }
            set
            {
                this.m_freeFlushTime = value;
            }
        }

        public int freeOpenCardCount
        {
            get
            {
                return this.m_freeOpenCardCount;
            }
            set
            {
                this.m_freeOpenCardCount = value;
            }
        }

        public int freeRefreshBoxCount
        {
            get
            {
                return this.m_freeRefreshBoxCount;
            }
            set
            {
                this.m_freeRefreshBoxCount = value;
            }
        }

        public ActiveSystemInfo Info
        {
            get
            {
                return this.m_activeInfo;
            }
            set
            {
                this.m_activeInfo = value;
            }
        }

        public DateTime LuckyBegindate
        {
            get
            {
                return this.m_luckyBegindate;
            }
            set
            {
                this.m_luckyBegindate = value;
            }
        }

        public DateTime LuckyEnddate
        {
            get
            {
                return this.m_luckyEnddate;
            }
            set
            {
                this.m_luckyEnddate = value;
            }
        }

        public int minUseNum
        {
            get
            {
                return this.m_minUseNum;
            }
            set
            {
                this.m_minUseNum = value;
            }
        }

        public int[] openCardPrice
        {
            get
            {
                return this.m_openCardPrice;
            }
            set
            {
                this.m_openCardPrice = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
        }

        public PyramidInfo Pyramid
        {
            get
            {
                return this.m_pyramid;
            }
            set
            {
                this.m_pyramid = value;
            }
        }

        public PyramidConfigInfo PyramidConfig
        {
            get
            {
                return this.m_pyramidConfig;
            }
            set
            {
                this.m_pyramidConfig = value;
            }
        }
    }
}


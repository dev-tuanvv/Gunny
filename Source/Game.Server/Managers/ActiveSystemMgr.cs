namespace Game.Server.Managers
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class ActiveSystemMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, ActiveSystemInfo> m_activeSystem = new Dictionary<int, ActiveSystemInfo>();
        private static Dictionary<int, ActivitySystemItemInfo> m_activySystemItem = new Dictionary<int, ActivitySystemItemInfo>();
        private static int m_boatCompleteExp;
        private static bool m_IsBattleGoundOpen;
        private static bool m_IsFightFootballTime;
        private static bool m_IsLeagueOpen;
        private static bool m_IsReset;
        private static bool m_IsSendAward;
        private static Dictionary<int, LanternriddlesInfo> m_lanternriddlesInfo = new Dictionary<int, LanternriddlesInfo>();
        private static bool m_lanternriddlesOpen;
        protected static Timer m_lanternriddlesScanTimer;
        private static ReaderWriterLock m_lock;
        private static int m_luckStarCountDown;
        private static int m_periodType;
        private static List<LuckStarRewardRecordInfo> m_recordList = new List<LuckStarRewardRecordInfo>();
        private static int m_reduceToemUpGrace;
        protected static Timer m_scanRank;
        private static bool m_sendCloseToClient;
        private static bool m_sendOpenToClient;
        protected static Timer m_statusScanTimer;
        private static bool m_x2Exp;
        private static bool m_x3Exp;
        private static ThreadSafeRandom rand;

        public static void AddOrUpdateLanternriddles(int playerID, LanternriddlesInfo Lanternriddles)
        {
            Lanternriddles.QuestViews = LightriddleQuestMgr.Get30LightriddleQuest();
            if (!m_lanternriddlesInfo.ContainsKey(playerID))
            {
                m_lanternriddlesInfo.Add(playerID, Lanternriddles);
            }
            else
            {
                m_lanternriddlesInfo[playerID] = Lanternriddles;
            }
        }

        public static void AddRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
        {
            if (m_recordList.Count > 10)
            {
                m_recordList.Clear();
            }
            LuckStarRewardRecordInfo item = new LuckStarRewardRecordInfo {
                PlayerID = PlayerID,
                nickName = nickName,
                useStarNum = 1,
                TemplateID = TemplateID,
                Count = Count,
                isVip = isVip
            };
            m_recordList.Add(item);
        }

        public static void BeginTimer()
        {
            int dueTime = 0x1b7740;
            if (m_scanRank == null)
            {
                m_scanRank = new Timer(new TimerCallback(ActiveSystemMgr.TimeCheck), null, dueTime, dueTime);
            }
            else
            {
                m_scanRank.Change(dueTime, dueTime);
            }
            dueTime = 0xea60;
            if (m_statusScanTimer == null)
            {
                m_statusScanTimer = new Timer(new TimerCallback(ActiveSystemMgr.StatusScan), null, dueTime, dueTime);
            }
            else
            {
                m_statusScanTimer.Change(dueTime, dueTime);
            }
            dueTime = 0xea60;
            if (m_lanternriddlesScanTimer == null)
            {
                m_lanternriddlesScanTimer = new Timer(new TimerCallback(ActiveSystemMgr.LanternriddlesScan), null, dueTime, dueTime);
            }
            else
            {
                m_lanternriddlesScanTimer.Change(dueTime, dueTime);
            }
        }

        public static bool CanExchange()
        {
            return ((DateTime.Now.DayOfWeek == DayOfWeek.Sunday) && (m_periodType == 2));
        }

        private static bool CanOpenLanternriddles()
        {
            Convert.ToDateTime(GameProperties.LightRiddleBeginDate);
            DateTime time = Convert.ToDateTime(GameProperties.LightRiddleEndDate);
            return (DateTime.Now.Date < time.Date);
        }

        public static bool CanX2Exp()
        {
            int gP = CommunalActiveMgr.GetGP(5);
            return ((DateTime.Now.DayOfWeek == DayOfWeek.Friday) && (boatCompleteExp >= gP));
        }

        public static bool CanX3Exp()
        {
            int gP = CommunalActiveMgr.GetGP(6);
            return ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday) && (boatCompleteExp >= gP));
        }

        public static LanternriddlesInfo CreateNullLanternriddlesInfo(int playerID)
        {
            return new LanternriddlesInfo { PlayerID = playerID, QuestionIndex = 30, QuestionView = 30, DoubleFreeCount = GameProperties.LightRiddleFreeComboNum, DoublePrice = GameProperties.LightRiddleComboMoney, HitFreeCount = GameProperties.LightRiddleFreeHitNum, HitPrice = GameProperties.LightRiddleHitMoney, MyInteger = 0, QuestionNum = 0, Option = -1, IsHint = true, IsDouble = true, EndDate = DateTime.Now };
        }

        public static void CheckPeriod()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                bool flag = false;
                if (m_IsReset)
                {
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        bussiness.ResetCommunalActive(1, false);
                    }
                    m_IsReset = false;
                    flag = LoadSystermInfo();
                }
                if (flag)
                {
                    foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                    {
                        if (player != null)
                        {
                            player.Actives.SendDragonBoatAward();
                        }
                    }
                }
                int gP = CommunalActiveMgr.GetGP(6);
                if (boatCompleteExp >= gP)
                {
                    m_periodType = 2;
                    m_reduceToemUpGrace = 40;
                }
                m_x3Exp = false;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                if (!m_IsReset)
                {
                    foreach (GamePlayer player2 in WorldMgr.GetAllPlayers())
                    {
                        if (player2 != null)
                        {
                            player2.Actives.ResetDragonBoat();
                        }
                    }
                    bool flag2 = false;
                    using (PlayerBussiness bussiness2 = new PlayerBussiness())
                    {
                        flag2 = bussiness2.ResetDragonBoat();
                    }
                    if (flag2)
                    {
                        LoadSystermInfo();
                        m_IsReset = true;
                    }
                }
                m_periodType = 1;
                m_reduceToemUpGrace = 0;
            }
            else if ((DateTime.Now.DayOfWeek == DayOfWeek.Friday) && !m_x2Exp)
            {
                foreach (GamePlayer player3 in WorldMgr.GetAllPlayers())
                {
                    if (player3 != null)
                    {
                        player3.CanX2Exp = true;
                    }
                }
                m_x2Exp = true;
            }
            else if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday) && !m_x3Exp)
            {
                foreach (GamePlayer player4 in WorldMgr.GetAllPlayers())
                {
                    if (player4 != null)
                    {
                        player4.CanX3Exp = true;
                    }
                }
                m_x3Exp = true;
                m_x2Exp = false;
            }
        }

        public static LanternriddlesInfo EnterLanternriddles(int playerID)
        {
            if (!m_lanternriddlesInfo.ContainsKey(playerID))
            {
                LanternriddlesInfo info = new LanternriddlesInfo {
                    PlayerID = playerID,
                    QuestionIndex = 1,
                    QuestionView = 30,
                    DoubleFreeCount = GameProperties.LightRiddleFreeComboNum,
                    DoublePrice = GameProperties.LightRiddleComboMoney,
                    HitFreeCount = GameProperties.LightRiddleFreeHitNum,
                    HitPrice = GameProperties.LightRiddleHitMoney,
                    QuestViews = LightriddleQuestMgr.Get30LightriddleQuest(),
                    MyInteger = 0,
                    QuestionNum = 0,
                    Option = -1,
                    IsHint = false,
                    IsDouble = false,
                    EndDate = EndDate
                };
                m_lanternriddlesInfo.Add(playerID, info);
                return info;
            }
            return GetLanternriddlesInfo(playerID);
        }

        public static List<ActivitySystemItemInfo> FindActivitySystemItemByLayer(int layer)
        {
            List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
            if (m_activySystemItem != null)
            {
                foreach (ActivitySystemItemInfo info in m_activySystemItem.Values)
                {
                    if ((info.Quality == layer) && (info.ActivityType == 8))
                    {
                        list.Add(info);
                    }
                }
            }
            return list;
        }

        public static int FindAreaMyRank(int ID)
        {
            int dragonBoatAreaMinScore = GameProperties.DragonBoatAreaMinScore;
            if (m_activeSystem.ContainsKey(ID) && (m_activeSystem[ID].totalScore >= dragonBoatAreaMinScore))
            {
                return m_activeSystem[ID].myRank;
            }
            return -1;
        }

        public static List<ActivitySystemItemInfo> FindGrowthPackage(int layer)
        {
            List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
            if (m_activySystemItem != null)
            {
                foreach (ActivitySystemItemInfo info in m_activySystemItem.Values)
                {
                    if ((info.Quality == layer) && (info.ActivityType == 20))
                    {
                        list.Add(info);
                    }
                }
            }
            return list;
        }

        public static int FindMyRank(int ID)
        {
            int dragonBoatMinScore = GameProperties.DragonBoatMinScore;
            if (m_activeSystem.ContainsKey(ID) && (m_activeSystem[ID].totalScore >= dragonBoatMinScore))
            {
                return m_activeSystem[ID].myRank;
            }
            return -1;
        }

        public static LanternriddlesInfo GetLanternriddles(int playerID)
        {
            if (m_lanternriddlesInfo.ContainsKey(playerID))
            {
                return m_lanternriddlesInfo[playerID];
            }
            return null;
        }

        public static LanternriddlesInfo GetLanternriddlesInfo(int playerID)
        {
            if (m_lanternriddlesInfo.ContainsKey(playerID))
            {
                if (m_lanternriddlesInfo[playerID].CanNextQuest)
                {
                    m_lanternriddlesInfo[playerID].EndDate = EndDate;
                    if (m_lanternriddlesInfo[playerID].QuestionIndex > 1)
                    {
                        LanternriddlesInfo local1 = m_lanternriddlesInfo[playerID];
                        local1.QuestionIndex++;
                    }
                }
                if (m_lanternriddlesInfo[playerID].EndDate.Date < DateTime.Now.Date)
                {
                    m_lanternriddlesInfo[playerID].QuestionIndex = 1;
                    m_lanternriddlesInfo[playerID].QuestViews = LightriddleQuestMgr.Get30LightriddleQuest();
                    m_lanternriddlesInfo[playerID].DoubleFreeCount = GameProperties.LightRiddleFreeComboNum;
                    m_lanternriddlesInfo[playerID].DoublePrice = GameProperties.LightRiddleComboMoney;
                    m_lanternriddlesInfo[playerID].HitFreeCount = GameProperties.LightRiddleFreeHitNum;
                    m_lanternriddlesInfo[playerID].HitPrice = GameProperties.LightRiddleHitMoney;
                    m_lanternriddlesInfo[playerID].MyInteger = 0;
                    m_lanternriddlesInfo[playerID].QuestionNum = 0;
                    m_lanternriddlesInfo[playerID].Option = -1;
                    m_lanternriddlesInfo[playerID].IsHint = false;
                    m_lanternriddlesInfo[playerID].IsDouble = false;
                    m_lanternriddlesInfo[playerID].EndDate = EndDate;
                }
                return m_lanternriddlesInfo[playerID];
            }
            return CreateNullLanternriddlesInfo(playerID);
        }

        public static List<SqlDataProvider.Data.ItemInfo> GetPyramidAward(int layer)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            List<ActivitySystemItemInfo> list2 = new List<ActivitySystemItemInfo>();
            List<ActivitySystemItemInfo> list3 = FindActivitySystemItemByLayer(layer);
            int count = 1;
            int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>) (from s in list3 select s.Random)).Max());
            List<ActivitySystemItemInfo> source = (from s in list3
                where s.Random >= maxRound
                select s).ToList<ActivitySystemItemInfo>();
            int num2 = source.Count<ActivitySystemItemInfo>();
            if (num2 > 0)
            {
                count = (count > num2) ? num2 : count;
                foreach (int num4 in GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    ActivitySystemItemInfo item = source[num4];
                    list2.Add(item);
                }
            }
            foreach (ActivitySystemItemInfo info2 in list2)
            {
                SqlDataProvider.Data.ItemInfo info3 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info2.TemplateID), info2.Count, 0x66);
                info3.TemplateID = info2.TemplateID;
                info3.IsBinds = info2.IsBinds;
                info3.ValidDate = info2.ValidDate;
                info3.Count = info2.Count;
                info3.StrengthenLevel = info2.StrengthenLevel;
                info3.AttackCompose = 0;
                info3.DefendCompose = 0;
                info3.AgilityCompose = 0;
                info3.LuckCompose = 0;
                list.Add(info3);
            }
            return list;
        }

        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int[] numArray = new int[count];
            for (int i = 0; i < count; i++)
            {
                int num2 = ThreadSafeRandom.NextStatic(minValue, maxValue + 1);
                int num3 = 0;
                for (int j = 0; j < i; j++)
                {
                    if (numArray[j] == num2)
                    {
                        num3++;
                    }
                }
                if (num3 == 0)
                {
                    numArray[i] = num2;
                }
                else
                {
                    i--;
                }
            }
            return numArray;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                rand = new ThreadSafeRandom();
                m_IsBattleGoundOpen = false;
                m_IsLeagueOpen = false;
                m_IsFightFootballTime = false;
                m_lanternriddlesOpen = false;
                m_sendOpenToClient = true;
                m_sendCloseToClient = true;
                m_luckStarCountDown = Math.Abs((int) ((60 - DateTime.Now.Minute) - 30));
                Setup();
                return LoadSystermInfo();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ActiveSystemMgr", exception);
                }
                return false;
            }
        }

        public static void LanternriddlesAnswer(int playerID, int option)
        {
            if (m_lanternriddlesInfo.ContainsKey(playerID))
            {
                m_lanternriddlesInfo[playerID].Option = option;
            }
        }

        public static void LanternriddlesOpenClose()
        {
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player != null)
                {
                    player.Out.SendLanternriddlesOpen(player.PlayerId, m_lanternriddlesOpen);
                }
            }
        }

        protected static void LanternriddlesScan(object sender)
        {
            try
            {
                log.Info("Begin Lanternriddles CheckPeriod....");
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                if (CanOpenLanternriddles())
                {
                    int hour = DateTime.Now.Hour;
                    DateTime time = Convert.ToDateTime(GameProperties.LightRiddleBeginTime);
                    DateTime time2 = Convert.ToDateTime(GameProperties.LightRiddleEndTime);
                    int num3 = time.Hour;
                    int num4 = time2.Hour;
                    if ((hour >= num3) && (hour < num4))
                    {
                        m_lanternriddlesOpen = true;
                        if (m_sendOpenToClient)
                        {
                            LanternriddlesOpenClose();
                            m_sendOpenToClient = false;
                        }
                    }
                    else
                    {
                        m_lanternriddlesOpen = false;
                        if ((hour >= num4) && m_sendCloseToClient)
                        {
                            LanternriddlesOpenClose();
                            m_sendCloseToClient = false;
                        }
                    }
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                log.Info("End Lanternriddles CheckPeriod....");
            }
            catch (Exception exception)
            {
                log.Error("lanternriddlesScan ", exception);
            }
        }

        private static bool LoadSystermInfo()
        {
            try
            {
                m_activeSystem = new Dictionary<int, ActiveSystemInfo>();
                m_activySystemItem = new Dictionary<int, ActivitySystemItemInfo>();
                long num = 0L;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    foreach (ActiveSystemInfo info in bussiness.GetAllActiveSystemData())
                    {
                        if (!m_activeSystem.ContainsKey(info.UserID))
                        {
                            num += info.totalScore;
                            m_activeSystem.Add(info.UserID, info);
                        }
                    }
                    foreach (ActivitySystemItemInfo info2 in bussiness.GetAllActivitySystemItem())
                    {
                        if (!m_activySystemItem.ContainsKey(info2.ID))
                        {
                            m_activySystemItem.Add(info2.ID, info2);
                        }
                    }
                    if (num > 0x7fffffffL)
                    {
                        num = 0x7fffffffL;
                    }
                    UpdateBoatExpFromDB((int) num);
                    return true;
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ActiveSystemMgr", exception);
                }
            }
            return false;
        }

        public static List<ActiveSystemInfo> SelectTopTenAllServer(int condition)
        {
            List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
            IOrderedEnumerable<KeyValuePair<int, ActiveSystemInfo>> enumerable = from pair in m_activeSystem
                where pair.Value.totalScore >= condition
                orderby pair.Value.totalScore descending
                select pair;
            foreach (KeyValuePair<int, ActiveSystemInfo> pair in enumerable)
            {
                if (list.Count == 10)
                {
                    return list;
                }
                list.Add(pair.Value);
            }
            return list;
        }

        public static List<ActiveSystemInfo> SelectTopTenCurrenServer(int condition)
        {
            List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
            IOrderedEnumerable<KeyValuePair<int, ActiveSystemInfo>> enumerable = from pair in m_activeSystem
                where pair.Value.totalScore >= condition
                orderby pair.Value.totalScore descending
                select pair;
            foreach (KeyValuePair<int, ActiveSystemInfo> pair in enumerable)
            {
                if (list.Count == 10)
                {
                    return list;
                }
                list.Add(pair.Value);
            }
            return list;
        }

        public static void SendTCP(GSPacketIn pkg, int playerID)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(playerID);
            if (playerById != null)
            {
                playerById.SendTCP(pkg);
            }
        }

        public static void Setup()
        {
            try
            {
                m_periodType = 1;
                m_boatCompleteExp = 0;
                m_reduceToemUpGrace = 0;
                CommunalActiveInfo info = CommunalActiveMgr.FindCommunalActive(1);
                if (info != null)
                {
                    m_IsSendAward = info.IsSendAward;
                    m_IsReset = info.IsReset;
                }
                else
                {
                    m_IsSendAward = true;
                    m_IsReset = true;
                }
                CheckPeriod();
                BeginTimer();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ActiveSystemMgr Setup", exception);
                }
            }
        }

        protected static void StatusScan(object sender)
        {
            try
            {
                log.Info("Begin ActiveSystem CheckPeriod....");
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                CheckPeriod();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                log.Info("End ActiveSystem CheckPeriod....");
            }
            catch (Exception exception)
            {
                log.Error("StatusScan ", exception);
            }
        }

        public static void StopAllTimer()
        {
            if (m_scanRank != null)
            {
                m_scanRank.Dispose();
                m_scanRank = null;
            }
            if (m_lanternriddlesScanTimer != null)
            {
                m_lanternriddlesScanTimer.Dispose();
                m_lanternriddlesScanTimer = null;
            }
            if (m_statusScanTimer != null)
            {
                m_statusScanTimer.Dispose();
                m_statusScanTimer = null;
            }
        }

        protected static void TimeCheck(object sender)
        {
            try
            {
                log.Info("Begin ActiveSystem TimeCheck....");
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                LoadSystermInfo();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                log.Info("End ActiveSystem TimeCheck....");
            }
            catch (Exception exception)
            {
                log.Error("StatusScan ", exception);
            }
        }

        public static void UpdateBoatExp(int exp)
        {
            m_boatCompleteExp += exp;
        }

        public static void UpdateBoatExpFromDB(int exp)
        {
            m_boatCompleteExp = 0;
            m_boatCompleteExp += exp;
        }

        public static void UpdateIsBattleGoundOpen(bool open)
        {
            m_IsBattleGoundOpen = open;
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player != null)
                {
                    player.Out.SendBattleGoundOpen(player.PlayerCharacter.ID);
                }
            }
        }

        public static void UpdateIsFightFootballTime(bool open)
        {
            m_IsFightFootballTime = open;
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player != null)
                {
                    player.Out.SendFightFootballTimeOpenClose(player.PlayerCharacter.ID, open);
                }
            }
        }

        public static void UpdateIsLeagueOpen(bool open)
        {
            m_IsLeagueOpen = open;
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player != null)
                {
                    if (open)
                    {
                        player.Out.SendLeagueNotice(player.PlayerCharacter.ID, player.BattleData.MatchInfo.restCount, player.BattleData.maxCount, 1);
                    }
                    else
                    {
                        player.Out.SendLeagueNotice(player.PlayerCharacter.ID, player.BattleData.MatchInfo.restCount, player.BattleData.maxCount, 2);
                    }
                }
            }
        }

        public static void UpdateLuckStarRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
        {
            AddRewardRecord(PlayerID, nickName, TemplateID, Count, isVip);
        }

        public static int boatCompleteExp
        {
            get
            {
                return m_boatCompleteExp;
            }
        }

        public static DateTime EndDate
        {
            get
            {
                return DateTime.Now.AddMilliseconds((double) (GameProperties.LightRiddleAnswerTime * 0x3e8));
            }
        }

        public static bool IsBattleGoundOpen
        {
            get
            {
                return m_IsBattleGoundOpen;
            }
            set
            {
                m_IsBattleGoundOpen = value;
            }
        }

        public static bool IsFightFootballTime
        {
            get
            {
                return m_IsFightFootballTime;
            }
            set
            {
                m_IsFightFootballTime = value;
            }
        }

        public static bool IsLeagueOpen
        {
            get
            {
                return m_IsLeagueOpen;
            }
            set
            {
                m_IsLeagueOpen = value;
            }
        }

        public static bool LanternriddlesOpen
        {
            get
            {
                return m_lanternriddlesOpen;
            }
        }

        public static int periodType
        {
            get
            {
                return m_periodType;
            }
        }

        public static List<LuckStarRewardRecordInfo> RecordList
        {
            get
            {
                return m_recordList;
            }
        }

        public static int ReduceToemUpGrace
        {
            get
            {
                return m_reduceToemUpGrace;
            }
        }
    }
}


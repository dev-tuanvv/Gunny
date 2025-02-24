namespace Bussiness.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class EventAwardMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static EventAwardInfo[] m_eventAward;
        private static Dictionary<int, List<EventAwardInfo>> m_EventAwards;
        private static ThreadSafeRandom rand = new ThreadSafeRandom();

        public static void CreateEventAward(eEventType DateId)
        {
        }

        public static EventAwardInfo CreateSearchGoodsAward(eEventType DataId)
        {
            List<EventAwardInfo> list = new List<EventAwardInfo>();
            List<EventAwardInfo> list2 = FindEventAward(DataId);
            int count = 1;
            int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>) (from s in list2 select s.Random)).Max());
            List<EventAwardInfo> source = (from s in list2
                where s.Random >= maxRound
                select s).ToList<EventAwardInfo>();
            int num2 = source.Count<EventAwardInfo>();
            if (num2 > 0)
            {
                count = (count > num2) ? num2 : count;
                foreach (int num4 in GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    EventAwardInfo item = source[num4];
                    list.Add(item);
                }
            }
            foreach (EventAwardInfo info2 in list)
            {
                if (ItemMgr.FindItemTemplate(info2.TemplateID) != null)
                {
                    return info2;
                }
            }
            return null;
        }

        public static List<EventAwardInfo> FindEventAward(eEventType DataId)
        {
            if (m_EventAwards.ContainsKey((int) DataId))
            {
                return m_EventAwards[(int) DataId];
            }
            return null;
        }

        public static List<SqlDataProvider.Data.ItemInfo> GetAllEventAwardAward(eEventType DataId)
        {
            List<EventAwardInfo> list = FindEventAward(DataId);
            List<SqlDataProvider.Data.ItemInfo> list2 = new List<SqlDataProvider.Data.ItemInfo>();
            foreach (EventAwardInfo info in list)
            {
                SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info.TemplateID), info.Count, 0x69);
                item.IsBinds = info.IsBinds;
                item.ValidDate = info.ValidDate;
                list2.Add(item);
            }
            return list2;
        }

        public static List<EventAwardInfo> GetBoGuBoxAward(int type)
        {
            List<EventAwardInfo> list = FindEventAward(eEventType.BOGU_AVEDTURE);
            List<EventAwardInfo> list2 = new List<EventAwardInfo>();
            foreach (EventAwardInfo info in list)
            {
                if (info.Random == type)
                {
                    list2.Add(info);
                }
            }
            return list2;
        }

        public static List<EventAwardInfo> GetDiceAward(eEventType DataId)
        {
            List<EventAwardInfo> list = new List<EventAwardInfo>();
            List<EventAwardInfo> list2 = FindEventAward(DataId);
            int count = 1;
            int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>) (from s in list2 select s.Random)).Max());
            List<EventAwardInfo> source = (from s in list2
                where s.Random >= maxRound
                select s).ToList<EventAwardInfo>();
            int num2 = source.Count<EventAwardInfo>();
            if (num2 > 0)
            {
                count = (count > num2) ? num2 : count;
                foreach (int num4 in GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    EventAwardInfo item = source[num4];
                    list.Add(item);
                }
            }
            return list;
        }

        public static List<CardInfo> GetFightFootballTimeAward(eEventType DataId)
        {
            List<CardInfo> list = new List<CardInfo>();
            List<EventAwardInfo> list2 = new List<EventAwardInfo>();
            List<EventAwardInfo> list3 = FindEventAward(DataId);
            int count = 1;
            int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>) (from s in list3 select s.Random)).Max());
            List<EventAwardInfo> source = (from s in list3
                where s.Random >= maxRound
                select s).ToList<EventAwardInfo>();
            int num2 = source.Count<EventAwardInfo>();
            if (num2 > 0)
            {
                count = (count > num2) ? num2 : count;
                foreach (int num4 in GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    EventAwardInfo item = source[num4];
                    list2.Add(item);
                }
            }
            foreach (EventAwardInfo info2 in list2)
            {
                CardInfo info3 = new CardInfo {
                    templateID = info2.TemplateID,
                    count = info2.Count
                };
                list.Add(info3);
            }
            return list;
        }

        public static List<NewChickenBoxItemInfo> GetNewChickenBoxAward(eEventType DataId)
        {
            List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
            List<EventAwardInfo> list2 = new List<EventAwardInfo>();
            List<EventAwardInfo> list3 = FindEventAward(DataId);
            int count = 1;
            int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>) (from s in list3 select s.Random)).Max());
            List<EventAwardInfo> source = (from s in list3
                where s.Random >= maxRound
                select s).ToList<EventAwardInfo>();
            int num2 = source.Count<EventAwardInfo>();
            if (num2 > 0)
            {
                count = (count > num2) ? num2 : count;
                foreach (int num4 in GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    EventAwardInfo item = source[num4];
                    list2.Add(item);
                }
            }
            foreach (EventAwardInfo info2 in list2)
            {
                NewChickenBoxItemInfo info3 = new NewChickenBoxItemInfo {
                    TemplateID = info2.TemplateID,
                    IsBinds = info2.IsBinds,
                    ValidDate = info2.ValidDate,
                    Count = info2.Count,
                    StrengthenLevel = info2.StrengthenLevel,
                    AttackCompose = 0,
                    DefendCompose = 0,
                    AgilityCompose = 0,
                    LuckCompose = 0
                };
                ItemTemplateInfo info4 = ItemMgr.FindItemTemplate(info2.TemplateID);
                info3.Quality = (info4 == null) ? 2 : info4.Quality;
                info3.IsSelected = false;
                info3.IsSeeded = false;
                list.Add(info3);
            }
            return list;
        }

        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int[] numArray = new int[count];
            for (int i = 0; i < count; i++)
            {
                int num2 = rand.Next(minValue, maxValue + 1);
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
            return ReLoad();
        }

        public static EventAwardInfo[] LoadEventAwardDb()
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                return bussiness.GetEventAwardInfos();
            }
        }

        public static Dictionary<int, List<EventAwardInfo>> LoadEventAwards(EventAwardInfo[] EventAwards)
        {
            Dictionary<int, List<EventAwardInfo>> dictionary = new Dictionary<int, List<EventAwardInfo>>();
            for (int i = 0; i < EventAwards.Length; i++)
            {
                Func<EventAwardInfo, bool> predicate = null;
                EventAwardInfo info = EventAwards[i];
                if (!dictionary.Keys.Contains<int>(info.ActivityType))
                {
                    if (predicate == null)
                    {
                        predicate = s => s.ActivityType == info.ActivityType;
                    }
                    IEnumerable<EventAwardInfo> source = EventAwards.Where<EventAwardInfo>(predicate);
                    dictionary.Add(info.ActivityType, source.ToList<EventAwardInfo>());
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                EventAwardInfo[] eventAwards = LoadEventAwardDb();
                Dictionary<int, List<EventAwardInfo>> dictionary = LoadEventAwards(eventAwards);
                if (eventAwards != null)
                {
                    Interlocked.Exchange<EventAwardInfo[]>(ref m_eventAward, eventAwards);
                    Interlocked.Exchange<Dictionary<int, List<EventAwardInfo>>>(ref m_EventAwards, dictionary);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ReLoad", exception);
                }
                return false;
            }
            return true;
        }
    }
}


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

    public class QuestMgr
    {
        private static Dictionary<int, AchievementInfo> dictionary_3 = new Dictionary<int, AchievementInfo>();
        private static Dictionary<int, List<AchievementCondictionInfo>> dictionary_4 = new Dictionary<int, List<AchievementCondictionInfo>>();
        private static Dictionary<int, List<AchievementGoodsInfo>> dictionary_5 = new Dictionary<int, List<AchievementGoodsInfo>>();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, List<QuestConditionInfo>> m_questcondiction = new Dictionary<int, List<QuestConditionInfo>>();
        private static Dictionary<int, List<QuestAwardInfo>> m_questgoods = new Dictionary<int, List<QuestAwardInfo>>();
        private static Dictionary<int, QuestInfo> m_questinfo = new Dictionary<int, QuestInfo>();

        public static List<AchievementCondictionInfo> GetAchievementCondiction(AchievementInfo info)
        {
            if (dictionary_4.ContainsKey(info.ID))
            {
                return dictionary_4[info.ID];
            }
            return null;
        }

        public static List<AchievementGoodsInfo> GetAchievementGoods(AchievementInfo info)
        {
            if (dictionary_5.ContainsKey(info.ID))
            {
                return dictionary_5[info.ID];
            }
            return null;
        }

        public static List<AchievementInfo> GetAllAchievements()
        {
            return dictionary_3.Values.ToList<AchievementInfo>();
        }

        public static int[] GetAllBuriedQuest()
        {
            List<int> list = new List<int>();
            foreach (QuestInfo info in m_questinfo.Values)
            {
                if (info.QuestID == 10)
                {
                    list.Add(info.ID);
                }
            }
            return list.ToArray();
        }

        public static List<QuestConditionInfo> GetQuestCondiction(QuestInfo info)
        {
            if (m_questcondiction.ContainsKey(info.ID))
            {
                return m_questcondiction[info.ID];
            }
            return null;
        }

        public static List<QuestAwardInfo> GetQuestGoods(QuestInfo info)
        {
            if (m_questgoods.ContainsKey(info.ID))
            {
                return m_questgoods[info.ID];
            }
            return null;
        }

        public static AchievementInfo GetSingleAchievement(int id)
        {
            if (dictionary_3.ContainsKey(id))
            {
                return dictionary_3[id];
            }
            return null;
        }

        public static QuestInfo GetSingleQuest(int id)
        {
            if (m_questinfo.ContainsKey(id))
            {
                return m_questinfo[id];
            }
            return null;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static Dictionary<int, List<AchievementCondictionInfo>> LoadAchievementCondictionDb(Dictionary<int, AchievementInfo> achs)
        {
            Dictionary<int, List<AchievementCondictionInfo>> dictionary = new Dictionary<int, List<AchievementCondictionInfo>>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                AchievementCondictionInfo[] allAchievementCondiction = bussiness.GetAllAchievementCondiction();
                using (Dictionary<int, AchievementInfo>.ValueCollection.Enumerator enumerator = achs.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        AchievementInfo ach = enumerator.Current;
                        IEnumerable<AchievementCondictionInfo> source = from s in allAchievementCondiction
                            where s.AchievementID == ach.ID
                            select s;
                        dictionary.Add(ach.ID, source.ToList<AchievementCondictionInfo>());
                    }
                    return dictionary;
                }
            }
        }

        public static Dictionary<int, List<AchievementGoodsInfo>> LoadAchievementGoodDb(Dictionary<int, AchievementInfo> achs)
        {
            Dictionary<int, List<AchievementGoodsInfo>> dictionary = new Dictionary<int, List<AchievementGoodsInfo>>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                AchievementGoodsInfo[] allAchievementGoods = bussiness.GetAllAchievementGoods();
                using (Dictionary<int, AchievementInfo>.ValueCollection.Enumerator enumerator = achs.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        AchievementInfo ach = enumerator.Current;
                        IEnumerable<AchievementGoodsInfo> source = from s in allAchievementGoods
                            where s.AchievementID == ach.ID
                            select s;
                        dictionary.Add(ach.ID, source.ToList<AchievementGoodsInfo>());
                    }
                    return dictionary;
                }
            }
        }

        public static Dictionary<int, AchievementInfo> LoadAchievementInfoDb()
        {
            Dictionary<int, AchievementInfo> dictionary = new Dictionary<int, AchievementInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (AchievementInfo info in bussiness.GetAllAchievement())
                {
                    if (!dictionary.ContainsKey(info.ID))
                    {
                        dictionary.Add(info.ID, info);
                    }
                }
            }
            return dictionary;
        }

        public static Dictionary<int, List<QuestConditionInfo>> LoadQuestCondictionDb(Dictionary<int, QuestInfo> quests)
        {
            Dictionary<int, List<QuestConditionInfo>> dictionary = new Dictionary<int, List<QuestConditionInfo>>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                QuestConditionInfo[] allQuestCondiction = bussiness.GetAllQuestCondiction();
                using (Dictionary<int, QuestInfo>.ValueCollection.Enumerator enumerator = quests.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        QuestInfo quest = enumerator.Current;
                        IEnumerable<QuestConditionInfo> source = from s in allQuestCondiction
                            where s.QuestID == quest.ID
                            select s;
                        dictionary.Add(quest.ID, source.ToList<QuestConditionInfo>());
                    }
                }
            }
            return dictionary;
        }

        public static Dictionary<int, List<QuestAwardInfo>> LoadQuestGoodDb(Dictionary<int, QuestInfo> quests)
        {
            Dictionary<int, List<QuestAwardInfo>> dictionary = new Dictionary<int, List<QuestAwardInfo>>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                QuestAwardInfo[] allQuestGoods = bussiness.GetAllQuestGoods();
                using (Dictionary<int, QuestInfo>.ValueCollection.Enumerator enumerator = quests.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        QuestInfo quest = enumerator.Current;
                        IEnumerable<QuestAwardInfo> source = from s in allQuestGoods
                            where s.QuestID == quest.ID
                            select s;
                        dictionary.Add(quest.ID, source.ToList<QuestAwardInfo>());
                    }
                }
            }
            return dictionary;
        }

        public static Dictionary<int, QuestInfo> LoadQuestInfoDb()
        {
            Dictionary<int, QuestInfo> dictionary = new Dictionary<int, QuestInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (QuestInfo info in bussiness.GetALlQuest())
                {
                    if (!dictionary.ContainsKey(info.ID))
                    {
                        dictionary.Add(info.ID, info);
                    }
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, QuestInfo> quests = LoadQuestInfoDb();
                Dictionary<int, List<QuestConditionInfo>> dictionary2 = LoadQuestCondictionDb(quests);
                Dictionary<int, List<QuestAwardInfo>> dictionary3 = LoadQuestGoodDb(quests);
                Dictionary<int, AchievementInfo> achs = LoadAchievementInfoDb();
                Dictionary<int, List<AchievementCondictionInfo>> dictionary5 = LoadAchievementCondictionDb(achs);
                Dictionary<int, List<AchievementGoodsInfo>> dictionary6 = LoadAchievementGoodDb(achs);
                if (quests.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, QuestInfo>>(ref m_questinfo, quests);
                    Interlocked.Exchange<Dictionary<int, List<QuestConditionInfo>>>(ref m_questcondiction, dictionary2);
                    Interlocked.Exchange<Dictionary<int, List<QuestAwardInfo>>>(ref m_questgoods, dictionary3);
                }
                if (achs.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, List<AchievementCondictionInfo>>>(ref dictionary_4, dictionary5);
                    Interlocked.Exchange<Dictionary<int, AchievementInfo>>(ref dictionary_3, achs);
                    Interlocked.Exchange<Dictionary<int, List<AchievementGoodsInfo>>>(ref dictionary_5, dictionary6);
                }
                return true;
            }
            catch (Exception exception)
            {
                log.Error("QuestMgr", exception);
            }
            return false;
        }
    }
}


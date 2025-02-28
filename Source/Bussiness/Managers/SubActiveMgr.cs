﻿namespace Bussiness.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class SubActiveMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static Dictionary<int, SubActiveConditionInfo> m_SubActiveConditionInfo = new Dictionary<int, SubActiveConditionInfo>();
        public static Dictionary<int, List<SubActiveInfo>> m_SubActiveInfo = new Dictionary<int, List<SubActiveInfo>>();

        public static SubActiveConditionInfo GetSubActiveInfo(SqlDataProvider.Data.ItemInfo item)
        {
            foreach (List<SubActiveInfo> list in m_SubActiveInfo.Values)
            {
                foreach (SubActiveInfo info in list)
                {
                    if (IsValid(info))
                    {
                        foreach (SubActiveConditionInfo info2 in m_SubActiveConditionInfo.Values)
                        {
                            if (((info.ActiveID == info2.ActiveID) && (info.SubID == info2.SubID)) && (info2.ConditionID == item.TemplateID))
                            {
                                switch (item.Template.CategoryID)
                                {
                                    case 5:
                                    case 7:
                                    case 1:
                                        if (item.StrengthenLevel == info2.Type)
                                        {
                                            return info2;
                                        }
                                        if (item.IsGold && ((item.StrengthenLevel + 100) == info2.Type))
                                        {
                                            return info2;
                                        }
                                        return null;

                                    case 6:
                                        return info2;
                                }
                                return info2;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool IsValid(SubActiveInfo info)
        {
            DateTime startTime = info.StartTime;
            DateTime endTime = info.EndTime;
            return ((info.StartTime.Ticks <= DateTime.Now.Ticks) && (info.EndTime.Ticks >= DateTime.Now.Ticks));
        }

        public static Dictionary<int, SubActiveConditionInfo> LoadSubActiveConditionDb(Dictionary<int, List<SubActiveInfo>> conditions)
        {
            Dictionary<int, SubActiveConditionInfo> dictionary = new Dictionary<int, SubActiveConditionInfo>();
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (int num in conditions.Keys)
                {
                    foreach (SubActiveConditionInfo info in bussiness.GetAllSubActiveCondition(num))
                    {
                        if (!((num != info.ActiveID) || dictionary.ContainsKey(info.ID)))
                        {
                            dictionary.Add(info.ID, info);
                        }
                    }
                }
            }
            return dictionary;
        }

        public static Dictionary<int, List<SubActiveInfo>> LoadSubActiveDb()
        {
            Dictionary<int, List<SubActiveInfo>> dictionary = new Dictionary<int, List<SubActiveInfo>>();
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (SubActiveInfo info in bussiness.GetAllSubActive())
                {
                    List<SubActiveInfo> list = new List<SubActiveInfo>();
                    if (!dictionary.ContainsKey(info.ActiveID))
                    {
                        list.Add(info);
                        dictionary.Add(info.ActiveID, list);
                    }
                    else
                    {
                        dictionary[info.ActiveID].Add(info);
                    }
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, List<SubActiveInfo>> conditions = LoadSubActiveDb();
                Dictionary<int, SubActiveConditionInfo> dictionary2 = LoadSubActiveConditionDb(conditions);
                if (conditions.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, List<SubActiveInfo>>>(ref m_SubActiveInfo, conditions);
                    Interlocked.Exchange<Dictionary<int, SubActiveConditionInfo>>(ref m_SubActiveConditionInfo, dictionary2);
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


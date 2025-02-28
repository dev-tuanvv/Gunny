﻿namespace Game.Logic
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public static class MissionInfoMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock = new ReaderWriterLock();
        private static Dictionary<int, MissionInfo> m_missionInfos = new Dictionary<int, MissionInfo>();
        private static ThreadSafeRandom m_rand = new ThreadSafeRandom();

        public static MissionInfo GetMissionInfo(int id)
        {
            if (m_missionInfos.ContainsKey(id))
            {
                return m_missionInfos[id];
            }
            return null;
        }

        public static bool Init()
        {
            return Reload();
        }

        private static Dictionary<int, MissionInfo> LoadFromDatabase()
        {
            Dictionary<int, MissionInfo> dictionary = new Dictionary<int, MissionInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (MissionInfo info in bussiness.GetAllMissionInfo())
                {
                    if (!dictionary.ContainsKey(info.Id))
                    {
                        dictionary.Add(info.Id, info);
                    }
                }
            }
            return dictionary;
        }

        public static bool Reload()
        {
            try
            {
                Dictionary<int, MissionInfo> dictionary = LoadFromDatabase();
                if (dictionary.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, MissionInfo>>(ref m_missionInfos, dictionary);
                }
                return true;
            }
            catch (Exception exception)
            {
                log.Error("MissionInfoMgr", exception);
            }
            return false;
        }
    }
}


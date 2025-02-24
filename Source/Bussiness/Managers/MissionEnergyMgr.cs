namespace Bussiness.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public static class MissionEnergyMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock = new ReaderWriterLock();
        private static Dictionary<int, MissionEnergyInfo> m_missionEnergyInfos = new Dictionary<int, MissionEnergyInfo>();
        private static ThreadSafeRandom m_rand = new ThreadSafeRandom();

        public static MissionEnergyInfo GetMissionEnergyInfo(int id)
        {
            if (m_missionEnergyInfos.ContainsKey(id))
            {
                return m_missionEnergyInfos[id];
            }
            return null;
        }

        public static bool Init()
        {
            return Reload();
        }

        private static Dictionary<int, MissionEnergyInfo> LoadFromDatabase()
        {
            Dictionary<int, MissionEnergyInfo> dictionary = new Dictionary<int, MissionEnergyInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (MissionEnergyInfo info in bussiness.GetAllMissionEnergyInfo())
                {
                    if (!dictionary.ContainsKey(info.Count))
                    {
                        dictionary.Add(info.Count, info);
                    }
                }
            }
            return dictionary;
        }

        public static bool Reload()
        {
            try
            {
                Dictionary<int, MissionEnergyInfo> dictionary = LoadFromDatabase();
                if (dictionary.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, MissionEnergyInfo>>(ref m_missionEnergyInfos, dictionary);
                }
                return true;
            }
            catch (Exception exception)
            {
                log.Error("MissionEnergyInfoMgr", exception);
            }
            return false;
        }
    }
}


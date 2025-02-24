namespace Game.Logic
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public static class PveInfoMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock = new ReaderWriterLock();
        private static Dictionary<int, PveInfo> m_pveInfos = new Dictionary<int, PveInfo>();
        private static ThreadSafeRandom m_rand = new ThreadSafeRandom();

        public static PveInfo[] GetPveInfo()
        {
            if (m_pveInfos == null)
            {
                Init();
            }
            return m_pveInfos.Values.ToArray<PveInfo>();
        }

        public static PveInfo GetPveInfoById(int id)
        {
            if (m_pveInfos.ContainsKey(id))
            {
                return m_pveInfos[id];
            }
            return null;
        }

        public static PveInfo GetPveInfoByType(eRoomType roomType, int levelLimits)
        {
            if (roomType <= eRoomType.AcademyDungeon)
            {
                switch (roomType)
                {
                    case eRoomType.Exploration:
                        foreach (PveInfo info3 in m_pveInfos.Values)
                        {
                            if ((info3.Type == (int) roomType) && (info3.LevelLimits == levelLimits))
                            {
                                return info3;
                            }
                        }
                        break;

                    case eRoomType.Dungeon:
                    case eRoomType.FightLib:
                    case eRoomType.Freshman:
                    case eRoomType.AcademyDungeon:
                        goto Label_00A7;
                }
            }
            else
            {
                switch (roomType)
                {
                    case eRoomType.Lanbyrinth:
                    case eRoomType.ConsortiaBoss:
                    case eRoomType.ActivityDungeon:
                    case eRoomType.SpecialActivityDungeon:
                        goto Label_00A7;
                }
            }
            goto Label_0163;
        Label_00A7:
            foreach (PveInfo info in m_pveInfos.Values)
            {
                if (info.Type == (int) roomType)
                {
                    return info;
                }
            }
        Label_0163:
            return null;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static Dictionary<int, PveInfo> LoadFromDatabase()
        {
            Dictionary<int, PveInfo> dictionary = new Dictionary<int, PveInfo>();
            using (PveBussiness bussiness = new PveBussiness())
            {
                foreach (PveInfo info in bussiness.GetAllPveInfos())
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
                Dictionary<int, PveInfo> dictionary = LoadFromDatabase();
                if (dictionary.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, PveInfo>>(ref m_pveInfos, dictionary);
                }
                return true;
            }
            catch (Exception exception)
            {
                log.Error("PveInfoMgr", exception);
            }
            return false;
        }
    }
}


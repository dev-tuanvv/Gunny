namespace Game.Server.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class LevelMgr
    {
        private static Dictionary<int, LevelInfo> _levels;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        public static LevelInfo FindLevel(int Grade)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_levels.ContainsKey(Grade))
                {
                    return _levels[Grade];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static int GetGP(int level)
        {
            if ((MaxLevel > level) && (level > 0))
            {
                return FindLevel(level - 1).GP;
            }
            return 0;
        }

        public static int GetLevel(int GP)
        {
            if (GP >= FindLevel(MaxLevel).GP)
            {
                return MaxLevel;
            }
            for (int i = 1; i <= MaxLevel; i++)
            {
                if (GP < FindLevel(i).GP)
                {
                    if ((i - 1) != 0)
                    {
                        return (i - 1);
                    }
                    return 1;
                }
            }
            return 1;
        }

        public static int IncreaseGP(int level, int totalGP)
        {
            if ((MaxLevel > level) && (level > 0))
            {
                return (level * 12);
            }
            return 0;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _levels = new Dictionary<int, LevelInfo>();
                rand = new ThreadSafeRandom();
                return LoadLevel(_levels);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("LevelMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadLevel(Dictionary<int, LevelInfo> Level)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (LevelInfo info in bussiness.GetAllLevel())
                {
                    if (!Level.ContainsKey(info.Grade))
                    {
                        Level.Add(info.Grade, info);
                    }
                }
            }
            return true;
        }

        public static int ReduceGP(int level, int totalGP)
        {
            if ((MaxLevel > level) && (level > 0))
            {
                totalGP -= FindLevel(level - 1).GP;
                if (totalGP >= (level * 12))
                {
                    return (level * 12);
                }
                if (totalGP >= 0)
                {
                    return totalGP;
                }
            }
            return 0;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, LevelInfo> level = new Dictionary<int, LevelInfo>();
                if (LoadLevel(level))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _levels = level;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("LevelMgr", exception);
                }
            }
            return false;
        }

        public static int MaxLevel
        {
            get
            {
                return _levels.Count;
            }
        }
    }
}


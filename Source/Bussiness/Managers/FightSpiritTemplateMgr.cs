namespace Bussiness.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class FightSpiritTemplateMgr
    {
        private static Dictionary<int, FightSpiritTemplateInfo> _fightSpiritTemplate;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        public static FightSpiritTemplateInfo FindFightSpiritTemplateInfo(int FigSpiritId, int lv)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (FightSpiritTemplateInfo info in _fightSpiritTemplate.Values)
                {
                    if ((info.FightSpiritID == FigSpiritId) && (info.Level == lv))
                    {
                        return info;
                    }
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

        public static int getProp(int FigSpiritId, int lv, int place)
        {
            FightSpiritTemplateInfo info = FindFightSpiritTemplateInfo(FigSpiritId, lv);
            switch (place)
            {
                case 2:
                    return info.Attack;

                case 3:
                    return info.Lucky;

                case 5:
                    return info.Agility;

                case 11:
                    return info.Defence;

                case 13:
                    return info.Blood;
            }
            return 0;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _fightSpiritTemplate = new Dictionary<int, FightSpiritTemplateInfo>();
                rand = new ThreadSafeRandom();
                return Load(_fightSpiritTemplate);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ConsortiaLevelMgr", exception);
                }
                return false;
            }
        }

        private static bool Load(Dictionary<int, FightSpiritTemplateInfo> consortiaLevel)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (FightSpiritTemplateInfo info in bussiness.GetAllFightSpiritTemplate())
                {
                    if (!consortiaLevel.ContainsKey(info.ID))
                    {
                        consortiaLevel.Add(info.ID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, FightSpiritTemplateInfo> consortiaLevel = new Dictionary<int, FightSpiritTemplateInfo>();
                if (Load(consortiaLevel))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _fightSpiritTemplate = consortiaLevel;
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
                    log.Error("ConsortiaLevelMgr", exception);
                }
            }
            return false;
        }
    }
}


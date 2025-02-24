namespace Game.Server.Managers
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Rooms;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class FightRateMgr
    {
        protected static Dictionary<int, FightRateInfo> _fightRate;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;

        public static bool CanChangeStyle(BaseRoom game, GSPacketIn pkg)
        {
            FightRateInfo[] allFightRateInfo = GetAllFightRateInfo();
            try
            {
                foreach (FightRateInfo info in allFightRateInfo)
                {
                    if (((((info.BeginDay.Year <= DateTime.Now.Year) && (DateTime.Now.Year <= info.EndDay.Year)) && ((info.BeginDay.DayOfYear <= DateTime.Now.DayOfYear) && (DateTime.Now.DayOfYear <= info.EndDay.DayOfYear))) && ((info.BeginTime.TimeOfDay <= DateTime.Now.TimeOfDay) && (DateTime.Now.TimeOfDay <= info.EndTime.TimeOfDay))) && (ThreadSafeRandom.NextStatic(0xf4240) < info.Rate))
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            pkg.WriteBoolean(false);
            return false;
        }

        public static FightRateInfo[] GetAllFightRateInfo()
        {
            FightRateInfo[] infoArray = null;
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                infoArray = _fightRate.Values.ToArray<FightRateInfo>();
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            if (infoArray != null)
            {
                return infoArray;
            }
            return new FightRateInfo[0];
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _fightRate = new Dictionary<int, FightRateInfo>();
                return LoadFightRate(_fightRate);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("AwardMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadFightRate(Dictionary<int, FightRateInfo> fighRate)
        {
            using (ServiceBussiness bussiness = new ServiceBussiness())
            {
                foreach (FightRateInfo info in bussiness.GetFightRate(GameServer.Instance.Configuration.ServerID))
                {
                    if (!fighRate.ContainsKey(info.ID))
                    {
                        fighRate.Add(info.ID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, FightRateInfo> fighRate = new Dictionary<int, FightRateInfo>();
                if (LoadFightRate(fighRate))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _fightRate = fighRate;
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
                    log.Error("AwardMgr", exception);
                }
            }
            return false;
        }
    }
}


namespace Game.Server.Managers
{
    using Bussiness;
    using Game.Server;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Threading;
    using System.Reflection;

    public class RateMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock = new ReaderWriterLock();
        private static ArrayList m_RateInfos = new ArrayList();

        public static float GetRate(eRateType eType)
        {
            float rate = 1f;
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                RateInfo rateInfoWithType = GetRateInfoWithType((int) eType);
                if (rateInfoWithType == null)
                {
                    return rate;
                }
                if (rateInfoWithType.Rate == 0f)
                {
                    return 1f;
                }
                if (IsValid(rateInfoWithType))
                {
                    rate = rateInfoWithType.Rate;
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return rate;
        }

        private static RateInfo GetRateInfoWithType(int type)
        {
            foreach (RateInfo info in m_RateInfos)
            {
                if (info.Type == type)
                {
                    return info;
                }
            }
            return null;
        }

        public static bool Init(GameServerConfig config)
        {
            bool flag;
            m_lock.AcquireWriterLock(-1);
            try
            {
                using (ServiceBussiness bussiness = new ServiceBussiness())
                {
                    m_RateInfos = bussiness.GetRate(config.ServerID);
                }
                flag = true;
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("RateMgr", exception);
                }
                flag = false;
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            return flag;
        }

        private static bool IsValid(RateInfo _RateInfo)
        {
            DateTime beginDay = _RateInfo.BeginDay;
            DateTime endDay = _RateInfo.EndDay;
            return (((((_RateInfo.BeginDay.Year <= DateTime.Now.Year) && (DateTime.Now.Year <= _RateInfo.EndDay.Year)) && ((_RateInfo.BeginDay.DayOfYear <= DateTime.Now.DayOfYear) && (DateTime.Now.DayOfYear <= _RateInfo.EndDay.DayOfYear))) && (_RateInfo.BeginTime.TimeOfDay <= DateTime.Now.TimeOfDay)) && (DateTime.Now.TimeOfDay <= _RateInfo.EndTime.TimeOfDay));
        }

        public static bool ReLoad()
        {
            return Init(GameServer.Instance.Configuration);
        }
    }
}


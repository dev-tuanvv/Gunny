namespace Bussiness.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class TotemHonorMgr
    {
        private static Dictionary<int, TotemHonorTemplateInfo> _totemHonorTemplate;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        public static TotemHonorTemplateInfo FindTotemHonorTemplateInfo(int ID)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_totemHonorTemplate.ContainsKey(ID))
                {
                    return _totemHonorTemplate[ID];
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

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
                rand = new ThreadSafeRandom();
                return Load(_totemHonorTemplate);
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

        private static bool Load(Dictionary<int, TotemHonorTemplateInfo> TotemHonorTemplate)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (TotemHonorTemplateInfo info in bussiness.GetAllTotemHonorTemplate())
                {
                    if (!TotemHonorTemplate.ContainsKey(info.ID))
                    {
                        TotemHonorTemplate.Add(info.ID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, TotemHonorTemplateInfo> totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
                if (Load(totemHonorTemplate))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _totemHonorTemplate = totemHonorTemplate;
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


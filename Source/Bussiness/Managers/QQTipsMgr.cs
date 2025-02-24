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

    public class QQTipsMgr
    {
        private static Dictionary<int, QQtipsMessagesInfo> _qqtips;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static int qqtipSelectIndex = 0;

        public static QQtipsMessagesInfo GetQQtipsMessages()
        {
            QQtipsMessagesInfo info;
            if (_qqtips == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (qqtipSelectIndex >= _qqtips.Count)
                {
                    qqtipSelectIndex = 1;
                }
                else
                {
                    qqtipSelectIndex++;
                }
                info = _qqtips[qqtipSelectIndex];
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return info;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _qqtips = new Dictionary<int, QQtipsMessagesInfo>();
                return LoadItem(_qqtips);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Init", exception);
                }
                return false;
            }
        }

        public static bool LoadItem(Dictionary<int, QQtipsMessagesInfo> infos)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (QQtipsMessagesInfo info in bussiness.GetAllQQtipsMessagesLoad())
                {
                    if (!infos.Keys.Contains<int>(info.ID))
                    {
                        infos.Add(info.ID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, QQtipsMessagesInfo> infos = new Dictionary<int, QQtipsMessagesInfo>();
                if (LoadItem(infos))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _qqtips = infos;
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
                    log.Error("ReLoad", exception);
                }
            }
            return false;
        }
    }
}


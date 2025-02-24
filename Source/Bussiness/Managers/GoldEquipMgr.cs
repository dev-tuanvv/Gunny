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

    public class GoldEquipMgr
    {
        private static Dictionary<int, GoldEquipTemplateLoadInfo> _items;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;

        public static GoldEquipTemplateLoadInfo FindGoldEquipCategoryID(int CategoryID)
        {
            if (_items == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (GoldEquipTemplateLoadInfo info in _items.Values)
                {
                    if (info.CategoryID == CategoryID)
                    {
                        return info;
                    }
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static GoldEquipTemplateLoadInfo FindGoldEquipNewTemplate(int TemplateId)
        {
            if (_items == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (GoldEquipTemplateLoadInfo info in _items.Values)
                {
                    if (info.OldTemplateId == TemplateId)
                    {
                        return info;
                    }
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static GoldEquipTemplateLoadInfo FindGoldEquipOldTemplate(int TemplateId)
        {
            if (_items == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (GoldEquipTemplateLoadInfo info in _items.Values)
                {
                    if ((info.NewTemplateId == TemplateId) && (info.OldTemplateId.ToString().Substring(4) != "4"))
                    {
                        return info;
                    }
                }
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
                _items = new Dictionary<int, GoldEquipTemplateLoadInfo>();
                return LoadItem(_items);
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

        public static bool LoadItem(Dictionary<int, GoldEquipTemplateLoadInfo> infos)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (GoldEquipTemplateLoadInfo info in bussiness.GetAllGoldEquipTemplateLoad())
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
                Dictionary<int, GoldEquipTemplateLoadInfo> infos = new Dictionary<int, GoldEquipTemplateLoadInfo>();
                if (LoadItem(infos))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _items = infos;
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


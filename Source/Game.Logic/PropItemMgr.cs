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

    public class PropItemMgr
    {
        private static Dictionary<int, ItemTemplateInfo> _allProp;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        public static int[] PropBag = new int[] { 0x2711, 0x2712, 0x2713, 0x2714, 0x2715, 0x2716, 0x2717, 0x2718 };
        public static int[] PropFightBag = new int[] { 0x2719, 0x271a, 0x271b, 0x271c, 0x271f, 0x2720, 0x2721, 0x2722 };
        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static ItemTemplateInfo FindAllProp(int id)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_allProp.ContainsKey(id))
                {
                    return _allProp[id];
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

        public static ItemTemplateInfo FindFightingProp(int id)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (!PropBag.Contains<int>(id))
                {
                    return null;
                }
                if (_allProp.ContainsKey(id))
                {
                    return _allProp[id];
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
                _allProp = new Dictionary<int, ItemTemplateInfo>();
                return LoadProps(_allProp);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("InitProps", exception);
                }
                return false;
            }
        }

        private static bool LoadProps(Dictionary<int, ItemTemplateInfo> allProp)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (ItemTemplateInfo info in bussiness.GetSingleCategory(10))
                {
                    allProp.Add(info.TemplateID, info);
                }
            }
            return true;
        }

        public static bool Reload()
        {
            try
            {
                Dictionary<int, ItemTemplateInfo> allProp = new Dictionary<int, ItemTemplateInfo>();
                if (LoadProps(allProp))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _allProp = allProp;
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
                    log.Error("ReloadProps", exception);
                }
            }
            return false;
        }
    }
}


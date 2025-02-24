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

    public class ItemMgr
    {
        private static Dictionary<int, ItemTemplateInfo> _items;
        private static Dictionary<int, LoadUserBoxInfo> _timeBoxs;
        private static List<ItemTemplateInfo> Lists;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;

        public static LoadUserBoxInfo FindItemBoxTemplate(int Id)
        {
            if (_timeBoxs == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_timeBoxs.Keys.Contains<int>(Id))
                {
                    return _timeBoxs[Id];
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static LoadUserBoxInfo FindItemBoxTypeAndLv(int type, int lv)
        {
            if (_timeBoxs == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (LoadUserBoxInfo info in _timeBoxs.Values)
                {
                    if ((info.Type == type) && (info.Level == lv))
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

        public static ItemTemplateInfo FindItemTemplate(int templateId)
        {
            if (_items == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_items.Keys.Contains<int>(templateId))
                {
                    return _items[templateId];
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static ItemTemplateInfo GetGoodsbyFusionTypeandLevel(int fusionType, int level)
        {
            if (_items == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(-1);
            try
            {
                foreach (ItemTemplateInfo info in _items.Values)
                {
                    if ((info.FusionType == fusionType) && (info.Level == level))
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

        public static ItemTemplateInfo GetGoodsbyFusionTypeandQuality(int fusionType, int quality)
        {
            if (_items == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (ItemTemplateInfo info in _items.Values)
                {
                    if ((info.FusionType == fusionType) && (info.Quality == quality))
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

        public static List<ItemTemplateInfo> GetMagicStoneByQuality(int quality)
        {
            return GetMagicStoneByQuality(quality, 0);
        }

        public static List<ItemTemplateInfo> GetMagicStoneByQuality(int minQuality, int maxQuality)
        {
            if (_items == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            if (minQuality <= 0)
            {
                minQuality = 1;
            }
            if (maxQuality > 4)
            {
                maxQuality = 4;
            }
            try
            {
                foreach (ItemTemplateInfo info in _items.Values)
                {
                    if (maxQuality <= 0)
                    {
                        if (Equip.isMagicStone(info) && (info.Property3 == minQuality))
                        {
                            list.Add(info);
                        }
                    }
                    else if ((Equip.isMagicStone(info) && (info.Property3 >= minQuality)) && (info.Property3 <= maxQuality))
                    {
                        list.Add(info);
                    }
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return list;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _items = new Dictionary<int, ItemTemplateInfo>();
                _timeBoxs = new Dictionary<int, LoadUserBoxInfo>();
                Lists = new List<ItemTemplateInfo>();
                return LoadItem(_items, _timeBoxs);
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

        public static bool LoadItem(Dictionary<int, ItemTemplateInfo> infos, Dictionary<int, LoadUserBoxInfo> userBoxs)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (ItemTemplateInfo info in bussiness.GetAllGoods())
                {
                    if (!infos.Keys.Contains<int>(info.TemplateID))
                    {
                        infos.Add(info.TemplateID, info);
                    }
                }
                foreach (LoadUserBoxInfo info2 in bussiness.GetAllTimeBoxAward())
                {
                    if (!userBoxs.Keys.Contains<int>(info2.ID))
                    {
                        userBoxs.Add(info2.ID, info2);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ItemTemplateInfo> infos = new Dictionary<int, ItemTemplateInfo>();
                Dictionary<int, LoadUserBoxInfo> userBoxs = new Dictionary<int, LoadUserBoxInfo>();
                if (LoadItem(infos, userBoxs))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _items = infos;
                        _timeBoxs = userBoxs;
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

        public static List<SqlDataProvider.Data.ItemInfo> SpiltGoodsMaxCount(SqlDataProvider.Data.ItemInfo itemInfo)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            for (int i = 0; i < itemInfo.Count; i += itemInfo.Template.MaxCount)
            {
                int num2 = (itemInfo.Count < itemInfo.Template.MaxCount) ? itemInfo.Count : itemInfo.Template.MaxCount;
                SqlDataProvider.Data.ItemInfo item = itemInfo.Clone();
                item.Count = num2;
                list.Add(item);
            }
            return list;
        }
    }
}


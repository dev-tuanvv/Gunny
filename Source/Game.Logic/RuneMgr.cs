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

    public class RuneMgr
    {
        private static Dictionary<int, RuneTemplateInfo> _items;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;

        public static RuneTemplateInfo FindRuneByTemplateID(int templateID)
        {
            if (_items == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (RuneTemplateInfo info in _items.Values)
                {
                    if (info.TemplateID == templateID)
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

        public static int FindRuneExp(int lv)
        {
            if (lv < 0)
            {
                return 1;
            }
            return GameProperties.RuneExp()[lv];
        }

        public static RuneTemplateInfo FindRuneTemplateID(int templateID, int lv)
        {
            List<RuneTemplateInfo> listRuneByTemplate = GetListRuneByTemplate(templateID);
            foreach (RuneTemplateInfo info in listRuneByTemplate)
            {
                if (info.BaseLevel >= lv)
                {
                    return info;
                }
            }
            return null;
        }

        public static List<RuneTemplateInfo> GetListRuneByTemplate(int templateID)
        {
            if (_items == null)
            {
                Init();
            }
            List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                int nextTemplateID = templateID;
                foreach (RuneTemplateInfo info in _items.Values)
                {
                    if (info.TemplateID == nextTemplateID)
                    {
                        list.Add(info);
                        nextTemplateID = info.NextTemplateID;
                    }
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return list;
        }

        public static int GetRuneLevel(int GP)
        {
            List<int> list = GameProperties.RuneExp();
            if (GP >= list[MaxLv() - 1])
            {
                return MaxLv();
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (GP < list[i])
                {
                    return i;
                }
            }
            return 1;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _items = new Dictionary<int, RuneTemplateInfo>();
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

        public static bool LoadItem(Dictionary<int, RuneTemplateInfo> infos)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (RuneTemplateInfo info in bussiness.GetAllRuneTemplate())
                {
                    if (!infos.Keys.Contains<int>(info.TemplateID))
                    {
                        infos.Add(info.TemplateID, info);
                    }
                }
            }
            return true;
        }

        public static int MaxExp()
        {
            List<int> list = GameProperties.RuneExp();
            int num = ((MaxLv() - 1) < 0) ? 0 : (MaxLv() - 1);
            return list[num];
        }

        public static int MaxLv()
        {
            return GameProperties.RuneExp().Count;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, RuneTemplateInfo> infos = new Dictionary<int, RuneTemplateInfo>();
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


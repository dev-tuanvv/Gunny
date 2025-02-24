namespace Game.Server.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class ClothPropertyTemplateInfoMgr
    {
        private static Dictionary<int, ClothPropertyTemplateInfo> _clothProperty;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;

        public static ClothPropertyTemplateInfo GetClothPropertyWithID(int ID)
        {
            lock (m_lock)
            {
                if (_clothProperty.Count > 0)
                {
                    foreach (ClothPropertyTemplateInfo info2 in _clothProperty.Values)
                    {
                        if (info2.ID == ID)
                        {
                            return info2;
                        }
                    }
                }
                return null;
            }
        }

        public static ClothPropertyTemplateInfo GetClothPropertyWithID(int ID, int Sex)
        {
            lock (m_lock)
            {
                if (_clothProperty.Count > 0)
                {
                    foreach (ClothPropertyTemplateInfo info2 in _clothProperty.Values)
                    {
                        if ((info2.ID == ID) && (info2.Sex == Sex))
                        {
                            return info2;
                        }
                    }
                }
                return null;
            }
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _clothProperty = new Dictionary<int, ClothPropertyTemplateInfo>();
                return LoadClothProperty(_clothProperty);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ClothPropertyMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadClothProperty(Dictionary<int, ClothPropertyTemplateInfo> clothProperty)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (ClothPropertyTemplateInfo info in bussiness.GetAllClothProperty())
                {
                    if (!clothProperty.ContainsKey(info.ID))
                    {
                        clothProperty.Add(info.ID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ClothPropertyTemplateInfo> clothProperty = new Dictionary<int, ClothPropertyTemplateInfo>();
                if (LoadClothProperty(clothProperty))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _clothProperty = clothProperty;
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
                    log.Error("ClothPropertyMgr", exception);
                }
            }
            return false;
        }
    }
}


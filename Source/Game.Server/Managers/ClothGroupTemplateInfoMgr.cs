namespace Game.Server.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class ClothGroupTemplateInfoMgr
    {
        private static Dictionary<int, ClothGroupTemplateInfo> _clothGroup;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;

        public static int CountClothGroupWithID(int ID)
        {
            lock (m_lock)
            {
                return GetClothGroupWithID(ID).Count;
            }
        }

        public static ClothGroupTemplateInfo GetClothGroup(int ID, int TemplateID, int Sex)
        {
            lock (m_lock)
            {
                if (_clothGroup.Count > 0)
                {
                    foreach (ClothGroupTemplateInfo info2 in _clothGroup.Values)
                    {
                        if (((info2.TemplateID == TemplateID) && (info2.ID == ID)) && (info2.Sex == Sex))
                        {
                            return info2;
                        }
                    }
                }
                return null;
            }
        }

        public static List<ClothGroupTemplateInfo> GetClothGroupWithID(int ID)
        {
            lock (m_lock)
            {
                List<ClothGroupTemplateInfo> list2 = new List<ClothGroupTemplateInfo>();
                if (_clothGroup.Count > 0)
                {
                    foreach (ClothGroupTemplateInfo info in _clothGroup.Values)
                    {
                        if (info.ID == ID)
                        {
                            list2.Add(info);
                        }
                    }
                }
                return list2;
            }
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _clothGroup = new Dictionary<int, ClothGroupTemplateInfo>();
                return LoadClothGroup(_clothGroup);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ClothGroupMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadClothGroup(Dictionary<int, ClothGroupTemplateInfo> clothGroup)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (ClothGroupTemplateInfo info in bussiness.GetAllClothGroup())
                {
                    if (!clothGroup.ContainsKey(info.ItemID))
                    {
                        clothGroup.Add(info.ItemID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ClothGroupTemplateInfo> clothGroup = new Dictionary<int, ClothGroupTemplateInfo>();
                if (LoadClothGroup(clothGroup))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _clothGroup = clothGroup;
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
                    log.Error("ClothGroupMgr", exception);
                }
            }
            return false;
        }
    }
}


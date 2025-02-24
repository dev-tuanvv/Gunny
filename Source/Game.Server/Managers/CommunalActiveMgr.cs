namespace Game.Server.Managers
{
    using Bussiness;
    using Bussiness.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class CommunalActiveMgr
    {
        private static Dictionary<int, CommunalActiveAwardInfo> _communalActiveAwards;
        private static Dictionary<int, CommunalActiveExpInfo> _communalActiveExps;
        private static Dictionary<int, CommunalActiveInfo> _communalActives;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        public static CommunalActiveInfo FindCommunalActive(int ActiveID)
        {
            if (_communalActives == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_communalActives.ContainsKey(ActiveID))
                {
                    return _communalActives[ActiveID];
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

        public static List<CommunalActiveAwardInfo> FindCommunalAwards(int isArea)
        {
            if (_communalActiveAwards == null)
            {
                Init();
            }
            List<CommunalActiveAwardInfo> list = new List<CommunalActiveAwardInfo>();
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (CommunalActiveAwardInfo info in _communalActiveAwards.Values)
                {
                    if (info.IsArea == isArea)
                    {
                        list.Add(info);
                    }
                }
                return list;
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return list;
        }

        public static List<SqlDataProvider.Data.ItemInfo> GetAwardInfos(int type, int rank)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            List<CommunalActiveAwardInfo> list2 = FindCommunalAwards(type);
            foreach (CommunalActiveAwardInfo info in list2)
            {
                if (info.RandID == rank)
                {
                    SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info.TemplateID), info.Count, 0x66);
                    if (item != null)
                    {
                        item.Count = info.Count;
                        item.IsBinds = info.IsBind;
                        item.ValidDate = info.ValidDate;
                        item.StrengthenLevel = info.StrengthenLevel;
                        item.AttackCompose = info.AttackCompose;
                        item.DefendCompose = info.DefendCompose;
                        item.AgilityCompose = info.AgilityCompose;
                        item.LuckCompose = info.LuckCompose;
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public static int GetGP(int level)
        {
            if (_communalActiveExps == null)
            {
                Init();
            }
            if (_communalActiveExps.ContainsKey(level))
            {
                return _communalActiveExps[level].Exp;
            }
            return 0;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _communalActives = new Dictionary<int, CommunalActiveInfo>();
                _communalActiveAwards = new Dictionary<int, CommunalActiveAwardInfo>();
                _communalActiveExps = new Dictionary<int, CommunalActiveExpInfo>();
                rand = new ThreadSafeRandom();
                return LoadData(_communalActives, _communalActiveAwards, _communalActiveExps);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CommunalActiveMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadData(Dictionary<int, CommunalActiveInfo> CommunalActives, Dictionary<int, CommunalActiveAwardInfo> CommunalActiveAwards, Dictionary<int, CommunalActiveExpInfo> CommunalActiveExps)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (CommunalActiveInfo info in bussiness.GetAllCommunalActive())
                {
                    if (!CommunalActives.ContainsKey(info.ActiveID))
                    {
                        CommunalActives.Add(info.ActiveID, info);
                    }
                }
                foreach (CommunalActiveAwardInfo info2 in bussiness.GetAllCommunalActiveAward())
                {
                    if (!CommunalActiveAwards.ContainsKey(info2.ID))
                    {
                        CommunalActiveAwards.Add(info2.ID, info2);
                    }
                }
                foreach (CommunalActiveExpInfo info3 in bussiness.GetAllCommunalActiveExp())
                {
                    if (!CommunalActiveExps.ContainsKey(info3.Grade))
                    {
                        CommunalActiveExps.Add(info3.Grade, info3);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, CommunalActiveInfo> communalActives = new Dictionary<int, CommunalActiveInfo>();
                Dictionary<int, CommunalActiveAwardInfo> communalActiveAwards = new Dictionary<int, CommunalActiveAwardInfo>();
                Dictionary<int, CommunalActiveExpInfo> communalActiveExps = new Dictionary<int, CommunalActiveExpInfo>();
                if (LoadData(communalActives, communalActiveAwards, communalActiveExps))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _communalActives = communalActives;
                        _communalActiveAwards = communalActiveAwards;
                        _communalActiveExps = communalActiveExps;
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
                    log.Error("CommunalActiveMgr", exception);
                }
            }
            return false;
        }

        public static void ResetEvent()
        {
        }
    }
}


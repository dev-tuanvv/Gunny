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

    public class TreasureAwardMgr
    {
        private static Dictionary<int, TreasureAwardInfo> _treasureAward;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        public static List<TreasureDataInfo> CreateTreasureData(int UserID)
        {
            List<TreasureDataInfo> list = new List<TreasureDataInfo>();
            Dictionary<int, TreasureDataInfo> dictionary = new Dictionary<int, TreasureDataInfo>();
            for (int i = 0; list.Count < 0x10; i++)
            {
                List<TreasureDataInfo> treasureData = GetTreasureData();
                int num2 = rand.Next(treasureData.Count);
                TreasureDataInfo info = treasureData[num2];
                info.UserID = UserID;
                if (!dictionary.Keys.Contains<int>(info.TemplateID))
                {
                    dictionary.Add(info.TemplateID, info);
                    list.Add(info);
                }
            }
            return list;
        }

        public static TreasureAwardInfo FindTreasureAwardInfo(int ID)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_treasureAward.ContainsKey(ID))
                {
                    return _treasureAward[ID];
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

        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int[] numArray = new int[count];
            for (int i = 0; i < count; i++)
            {
                int num2 = ThreadSafeRandom.NextStatic(minValue, maxValue + 1);
                int num3 = 0;
                for (int j = 0; j < i; j++)
                {
                    if (numArray[j] == num2)
                    {
                        num3++;
                    }
                }
                if (num3 == 0)
                {
                    numArray[i] = num2;
                }
                else
                {
                    i--;
                }
            }
            return numArray;
        }

        public static List<TreasureDataInfo> GetTreasureData()
        {
            List<TreasureDataInfo> list = new List<TreasureDataInfo>();
            List<TreasureAwardInfo> list2 = new List<TreasureAwardInfo>();
            List<TreasureAwardInfo> treasureInfos = GetTreasureInfos();
            int count = 1;
            int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>) (from s in treasureInfos select s.Random)).Max());
            List<TreasureAwardInfo> source = (from s in treasureInfos
                where s.Random >= maxRound
                select s).ToList<TreasureAwardInfo>();
            int num2 = source.Count<TreasureAwardInfo>();
            if (num2 > 0)
            {
                count = (count > num2) ? num2 : count;
                foreach (int num4 in GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    TreasureAwardInfo item = source[num4];
                    list2.Add(item);
                }
            }
            foreach (TreasureAwardInfo info2 in list2)
            {
                TreasureDataInfo info3 = new TreasureDataInfo {
                    ID = 0,
                    UserID = 0,
                    TemplateID = info2.TemplateID,
                    Count = info2.Count,
                    ValidDate = info2.Validate,
                    pos = -1,
                    BeginDate = DateTime.Now,
                    IsExit = true
                };
                list.Add(info3);
            }
            return list;
        }

        public static List<TreasureAwardInfo> GetTreasureInfos()
        {
            if (_treasureAward == null)
            {
                Init();
            }
            List<TreasureAwardInfo> list = new List<TreasureAwardInfo>();
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (TreasureAwardInfo info in _treasureAward.Values)
                {
                    list.Add(info);
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
                _treasureAward = new Dictionary<int, TreasureAwardInfo>();
                rand = new ThreadSafeRandom();
                return Load(_treasureAward);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("TreasureAwardMgr", exception);
                }
                return false;
            }
        }

        private static bool Load(Dictionary<int, TreasureAwardInfo> treasureAward)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (TreasureAwardInfo info in bussiness.GetAllTreasureAward())
                {
                    if (!treasureAward.ContainsKey(info.ID))
                    {
                        treasureAward.Add(info.ID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, TreasureAwardInfo> treasureAward = new Dictionary<int, TreasureAwardInfo>();
                if (Load(treasureAward))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _treasureAward = treasureAward;
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
                    log.Error("TreasureAwardMgr", exception);
                }
            }
            return false;
        }
    }
}


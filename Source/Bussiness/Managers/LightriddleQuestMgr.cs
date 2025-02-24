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

    public class LightriddleQuestMgr
    {
        private static Dictionary<int, LightriddleQuestInfo> _lightriddleQuests;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static Dictionary<int, LightriddleQuestInfo> Get30LightriddleQuest()
        {
            Dictionary<int, LightriddleQuestInfo> dictionary3;
            if (_lightriddleQuests == null)
            {
                Init();
            }
            Dictionary<int, LightriddleQuestInfo> dictionary = new Dictionary<int, LightriddleQuestInfo>();
            Dictionary<int, LightriddleQuestInfo> dictionary2 = new Dictionary<int, LightriddleQuestInfo>();
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                int count = _lightriddleQuests.Count;
                int key = 1;
                for (int i = 0; dictionary.Count < 30; i++)
                {
                    int num4 = random.Next(1, count);
                    LightriddleQuestInfo info = _lightriddleQuests[num4];
                    if (!dictionary2.Keys.Contains<int>(info.QuestionID))
                    {
                        dictionary.Add(key, info);
                        dictionary2.Add(info.QuestionID, info);
                        key++;
                    }
                }
                dictionary3 = dictionary;
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return dictionary3;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _lightriddleQuests = new Dictionary<int, LightriddleQuestInfo>();
                return LoadData(_lightriddleQuests);
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

        public static bool LoadData(Dictionary<int, LightriddleQuestInfo> infos)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (LightriddleQuestInfo info in bussiness.GetAllLightriddleQuestInfo())
                {
                    if (!infos.Keys.Contains<int>(info.QuestionID))
                    {
                        infos.Add(info.QuestionID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, LightriddleQuestInfo> infos = new Dictionary<int, LightriddleQuestInfo>();
                if (LoadData(infos))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _lightriddleQuests = infos;
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


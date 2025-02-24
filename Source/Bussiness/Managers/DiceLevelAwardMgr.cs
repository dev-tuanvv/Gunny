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

    public class DiceLevelAwardMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static DiceLevelAwardInfo[] m_diceLevelAward;
        private static Dictionary<int, List<DiceLevelAwardInfo>> m_DiceLevelAwards;
        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static List<DiceLevelAwardInfo> FindDiceLevelAward(int DataId)
        {
            if (m_DiceLevelAwards.ContainsKey(DataId))
            {
                return m_DiceLevelAwards[DataId];
            }
            return null;
        }

        public static List<DiceLevelAwardInfo> GetAllDiceLevelAwardAward(int DataId)
        {
            return FindDiceLevelAward(DataId);
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static DiceLevelAwardInfo[] LoadDiceLevelAwardDb()
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                return bussiness.GetDiceLevelAwardInfos();
            }
        }

        public static Dictionary<int, List<DiceLevelAwardInfo>> LoadDiceLevelAwards(DiceLevelAwardInfo[] DiceLevelAwards)
        {
            Dictionary<int, List<DiceLevelAwardInfo>> dictionary = new Dictionary<int, List<DiceLevelAwardInfo>>();
            for (int i = 0; i < DiceLevelAwards.Length; i++)
            {
                Func<DiceLevelAwardInfo, bool> predicate = null;
                DiceLevelAwardInfo info = DiceLevelAwards[i];
                if (!dictionary.Keys.Contains<int>(info.DiceLevel))
                {
                    if (predicate == null)
                    {
                        predicate = s => s.DiceLevel == info.DiceLevel;
                    }
                    IEnumerable<DiceLevelAwardInfo> source = DiceLevelAwards.Where<DiceLevelAwardInfo>(predicate);
                    dictionary.Add(info.DiceLevel, source.ToList<DiceLevelAwardInfo>());
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                DiceLevelAwardInfo[] diceLevelAwards = LoadDiceLevelAwardDb();
                Dictionary<int, List<DiceLevelAwardInfo>> dictionary = LoadDiceLevelAwards(diceLevelAwards);
                if (diceLevelAwards != null)
                {
                    Interlocked.Exchange<DiceLevelAwardInfo[]>(ref m_diceLevelAward, diceLevelAwards);
                    Interlocked.Exchange<Dictionary<int, List<DiceLevelAwardInfo>>>(ref m_DiceLevelAwards, dictionary);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ReLoad", exception);
                }
                return false;
            }
            return true;
        }
    }
}


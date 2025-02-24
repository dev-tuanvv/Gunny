namespace Bussiness.Managers
{
    using Bussiness;
    using Bussiness.Protocol;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class DropMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static List<DropCondiction> m_dropcondiction = new List<DropCondiction>();
        private static Dictionary<int, List<DropItem>> m_dropitem = new Dictionary<int, List<DropItem>>();
        private static string[] m_DropTypes = Enum.GetNames(typeof(eDropType));

        public static int FindCondiction(eDropType type, string para1, string para2)
        {
            string str = "," + para1 + ",";
            string str2 = "," + para2 + ",";
            foreach (DropCondiction condiction in m_dropcondiction)
            {
                if (((condiction.CondictionType == (int) type) && (condiction.Para1.IndexOf(str) != -1)) && (condiction.Para2.IndexOf(str2) != -1))
                {
                    return condiction.DropId;
                }
            }
            return 0;
        }

        public static List<DropItem> FindDropItem(int dropId)
        {
            if (m_dropitem.ContainsKey(dropId))
            {
                return m_dropitem[dropId];
            }
            return null;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static List<DropCondiction> LoadDropConditionDb()
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                DropCondiction[] allDropCondictions = bussiness.GetAllDropCondictions();
                return ((allDropCondictions != null) ? allDropCondictions.ToList<DropCondiction>() : null);
            }
        }

        public static Dictionary<int, List<DropItem>> LoadDropItemDb()
        {
            Dictionary<int, List<DropItem>> dictionary = new Dictionary<int, List<DropItem>>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                DropItem[] allDropItems = bussiness.GetAllDropItems();
                using (List<DropCondiction>.Enumerator enumerator = m_dropcondiction.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        DropCondiction info = enumerator.Current;
                        IEnumerable<DropItem> source = from s in allDropItems
                            where s.DropId == info.DropId
                            select s;
                        dictionary.Add(info.DropId, source.ToList<DropItem>());
                    }
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                List<DropCondiction> list = LoadDropConditionDb();
                Interlocked.Exchange<List<DropCondiction>>(ref m_dropcondiction, list);
                Dictionary<int, List<DropItem>> dictionary = LoadDropItemDb();
                Interlocked.Exchange<Dictionary<int, List<DropItem>>>(ref m_dropitem, dictionary);
                return true;
            }
            catch (Exception exception)
            {
                log.Error("DropMgr", exception);
            }
            return false;
        }
    }
}


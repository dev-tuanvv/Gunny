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

    public class ItemBoxMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ItemBoxInfo[] m_itemBox;
        private static Dictionary<int, List<ItemBoxInfo>> m_itemBoxs;
        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static bool CreateItemBox(int DateId, List<SqlDataProvider.Data.ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int exp)
        {
            return CreateItemBox(DateId, null, itemInfos, ref gold, ref point, ref giftToken, ref exp);
        }

        public static bool CreateItemBox(int DateId, List<ItemBoxInfo> tempBox, List<SqlDataProvider.Data.ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int exp)
        {
            int templateId;
            List<ItemBoxInfo> list = new List<ItemBoxInfo>();
            List<ItemBoxInfo> list2 = FindItemBox(DateId);
            if ((tempBox != null) && (tempBox.Count > 0))
            {
                list2 = tempBox;
            }
            if (list2 == null)
            {
                return false;
            }
            list = (from s in list2
                where s.IsSelect
                select s).ToList<ItemBoxInfo>();
            int count = 1;
            int maxRound = 0;
            if (list.Count < list2.Count)
            {
                maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>) (from s in list2
                    where !s.IsSelect
                    select s.Random)).Max());
            }
            List<ItemBoxInfo> source = (from s in list2
                where !s.IsSelect && (s.Random >= maxRound)
                select s).ToList<ItemBoxInfo>();
            int num3 = source.Count<ItemBoxInfo>();
            if (num3 > 0)
            {
                count = (count > num3) ? num3 : count;
                int[] numArray = GetRandomUnrepeatArray(0, num3 - 1, count);
                templateId = 0;
                while (templateId < numArray.Length)
                {
                    int num4 = numArray[templateId];
                    ItemBoxInfo item = source[num4];
                    if (list == null)
                    {
                        list = new List<ItemBoxInfo>();
                    }
                    list.Add(item);
                    templateId++;
                }
            }
            foreach (ItemBoxInfo info2 in list)
            {
                SqlDataProvider.Data.ItemInfo info3;
                if (info2 == null)
                {
                    return false;
                }
                templateId = info2.TemplateId;
                if (templateId <= -200)
                {
                    if (templateId != -300)
                    {
                        if (templateId != -200)
                        {
                            goto Label_0245;
                        }
                        point += info2.ItemCount;
                    }
                    else
                    {
                        giftToken += info2.ItemCount;
                    }
                    continue;
                }
                switch (templateId)
                {
                    case -100:
                    {
                        gold += info2.ItemCount;
                        continue;
                    }
                    case 0x2b63:
                    {
                        exp += info2.ItemCount;
                        continue;
                    }
                }
            Label_0245:
                info3 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info2.TemplateId), info2.ItemCount, 0x65);
                if (info3 != null)
                {
                    info3.IsBinds = info2.IsBind;
                    info3.ValidDate = info2.ItemValid;
                    info3.StrengthenLevel = info2.StrengthenLevel;
                    info3.AttackCompose = info2.AttackCompose;
                    info3.DefendCompose = info2.DefendCompose;
                    info3.AgilityCompose = info2.AgilityCompose;
                    info3.LuckCompose = info2.LuckCompose;
                    if (itemInfos == null)
                    {
                        itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                    }
                    itemInfos.Add(info3);
                }
            }
            return true;
        }

        public static bool CreateItemBox(int DateId, List<SqlDataProvider.Data.ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int medal, ref int exp, ref int honor, ref int hardCurrency, ref int leagueMoney, ref int useableScore, ref int prestge, ref int magicStonePoint)
        {
            List<ItemBoxInfo> list = new List<ItemBoxInfo>();
            List<ItemBoxInfo> list2 = FindItemBox(DateId);
            if (list2 == null)
            {
                return false;
            }
            list = (from s in list2
                where s.IsSelect
                select s).ToList<ItemBoxInfo>();
            int count = 1;
            int maxRound = 0;
            if (list.Count < list2.Count)
            {
                maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>) (from s in list2
                    where !s.IsSelect
                    select s.Random)).Max());
            }
            List<ItemBoxInfo> source = (from s in list2
                where !s.IsSelect && (s.Random >= maxRound)
                select s).ToList<ItemBoxInfo>();
            int num2 = source.Count<ItemBoxInfo>();
            if (num2 > 0)
            {
                count = (count > num2) ? num2 : count;
                foreach (int num4 in GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    ItemBoxInfo item = source[num4];
                    if (list == null)
                    {
                        list = new List<ItemBoxInfo>();
                    }
                    list.Add(item);
                }
            }
            foreach (ItemBoxInfo info2 in list)
            {
                if (info2 == null)
                {
                    return false;
                }
                switch (info2.TemplateId)
                {
                    case -1200:
                        useableScore += info2.ItemCount;
                        break;

                    case -1100:
                        giftToken += info2.ItemCount;
                        break;

                    case -1000:
                        leagueMoney += info2.ItemCount;
                        break;

                    case -1400:
                        magicStonePoint += info2.ItemCount;
                        break;

                    case -1300:
                        prestge += info2.ItemCount;
                        break;

                    case -900:
                        hardCurrency += info2.ItemCount;
                        break;

                    case -800:
                        honor += info2.ItemCount;
                        break;

                    case -300:
                        medal += info2.ItemCount;
                        break;

                    case -200:
                        point += info2.ItemCount;
                        break;

                    case -100:
                        gold += info2.ItemCount;
                        break;

                    case 0x2b63:
                        exp += info2.ItemCount;
                        break;

                    default:
                    {
                        SqlDataProvider.Data.ItemInfo info4 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info2.TemplateId), info2.ItemCount, 0x65);
                        if (info4 != null)
                        {
                            info4.Count = info2.ItemCount;
                            info4.IsBinds = info2.IsBind;
                            info4.ValidDate = info2.ItemValid;
                            info4.StrengthenLevel = info2.StrengthenLevel;
                            info4.AttackCompose = info2.AttackCompose;
                            info4.DefendCompose = info2.DefendCompose;
                            info4.AgilityCompose = info2.AgilityCompose;
                            info4.LuckCompose = info2.LuckCompose;
                            info4.IsTips = info2.IsTips != 0;
                            info4.IsLogs = info2.IsLogs;
                            if (itemInfos == null)
                            {
                                itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                            }
                            itemInfos.Add(info4);
                        }
                        break;
                    }
                }
            }
            return true;
        }

        public static List<ItemBoxInfo> FindItemBox(int DataId)
        {
            if (m_itemBoxs.ContainsKey(DataId))
            {
                return m_itemBoxs[DataId];
            }
            return null;
        }

        public static ItemBoxInfo FindSpecialItemBox(int DataId)
        {
            ItemBoxInfo info = new ItemBoxInfo();
            if (DataId <= -300)
            {
                if (DataId != -1100)
                {
                    if (DataId == -300)
                    {
                        info.TemplateId = 0x2c9c;
                        info.ItemCount = 1;
                    }
                    return info;
                }
                info.TemplateId = 0x2bcd;
                info.ItemCount = 1;
                return info;
            }
            if (DataId != -200)
            {
                if (DataId != -100)
                {
                    if (DataId == 0x2c90)
                    {
                        info.TemplateId = 0x2c9c;
                        info.ItemCount = 1;
                    }
                    return info;
                }
                info.TemplateId = 0x2be1;
                info.ItemCount = 1;
                return info;
            }
            info.TemplateId = 0x1b674;
            info.ItemCount = 1;
            return info;
        }

        public static List<SqlDataProvider.Data.ItemInfo> GetAllItemBoxAward(int DataId)
        {
            List<ItemBoxInfo> list = FindItemBox(DataId);
            List<SqlDataProvider.Data.ItemInfo> list2 = new List<SqlDataProvider.Data.ItemInfo>();
            foreach (ItemBoxInfo info in list)
            {
                SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info.TemplateId), info.ItemCount, 0x69);
                item.IsBinds = info.IsBind;
                item.ValidDate = info.ItemValid;
                list2.Add(item);
            }
            return list2;
        }

        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int[] numArray = new int[count];
            for (int i = 0; i < count; i++)
            {
                int num2 = random.Next(minValue, maxValue + 1);
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

        public static bool Init()
        {
            return ReLoad();
        }

        public static ItemBoxInfo[] LoadItemBoxDb()
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                return bussiness.GetItemBoxInfos();
            }
        }

        public static Dictionary<int, List<ItemBoxInfo>> LoadItemBoxs(ItemBoxInfo[] itemBoxs)
        {
            Dictionary<int, List<ItemBoxInfo>> dictionary = new Dictionary<int, List<ItemBoxInfo>>();
            for (int i = 0; i < itemBoxs.Length; i++)
            {
                Func<ItemBoxInfo, bool> predicate = null;
                ItemBoxInfo info = itemBoxs[i];
                if (!dictionary.Keys.Contains<int>(info.DataId))
                {
                    if (predicate == null)
                    {
                        predicate = s => s.DataId == info.DataId;
                    }
                    IEnumerable<ItemBoxInfo> source = itemBoxs.Where<ItemBoxInfo>(predicate);
                    dictionary.Add(info.DataId, source.ToList<ItemBoxInfo>());
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                ItemBoxInfo[] itemBoxs = LoadItemBoxDb();
                Dictionary<int, List<ItemBoxInfo>> dictionary = LoadItemBoxs(itemBoxs);
                if (itemBoxs != null)
                {
                    Interlocked.Exchange<ItemBoxInfo[]>(ref m_itemBox, itemBoxs);
                    Interlocked.Exchange<Dictionary<int, List<ItemBoxInfo>>>(ref m_itemBoxs, dictionary);
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


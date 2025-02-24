namespace Game.Server.Managers
{
    using Bussiness;
    using Bussiness.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class FusionMgr
    {
        private static Dictionary<string, FusionInfo> dictionary_0;
        private static readonly ILog ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Items_Fusion_List_Info[] m_itemsfusionlist = null;
        private static Random random_0;
        private static ReaderWriterLock readerWriterLock_0;

        public static ItemTemplateInfo Fusion(List<SqlDataProvider.Data.ItemInfo> Items, List<SqlDataProvider.Data.ItemInfo> AppendItems, ref bool isBind, ref bool result)
        {
            Func<ItemTemplateInfo, bool> predicate = null;
            Func<ItemTemplateInfo, double> keySelector = null;
            Func<ItemTemplateInfo, bool> func3 = null;
            Func<ItemTemplateInfo, double> func4 = null;
            List<int> list = new List<int>();
            int level = 0;
            int TotalRate = 0;
            int maxValue = 0;
            if (Items != null)
            {
                ItemTemplateInfo info = null;
                foreach (SqlDataProvider.Data.ItemInfo info2 in Items)
                {
                    if (info2 != null)
                    {
                        list.Add(info2.Template.FusionType);
                        if (info2.Template.Level > level)
                        {
                            level = info2.Template.Level;
                        }
                        TotalRate += info2.Template.FusionRate;
                        maxValue += info2.Template.FusionNeedRate;
                        if (info2.IsBinds)
                        {
                            isBind = true;
                        }
                    }
                }
                foreach (SqlDataProvider.Data.ItemInfo info3 in AppendItems)
                {
                    TotalRate += info3.Template.FusionRate / 2;
                    maxValue += info3.Template.FusionNeedRate / 2;
                    if (info3.IsBinds)
                    {
                        isBind = true;
                    }
                }
                list.Sort();
                StringBuilder builder = new StringBuilder();
                foreach (int num3 in list)
                {
                    builder.Append(num3);
                }
                string key = builder.ToString();
                readerWriterLock_0.AcquireReaderLock(-1);
                try
                {
                    if (dictionary_0.ContainsKey(key))
                    {
                        FusionInfo info4 = dictionary_0[key];
                        ItemTemplateInfo goodsbyFusionTypeandLevel = ItemMgr.GetGoodsbyFusionTypeandLevel(info4.Reward, level);
                        ItemTemplateInfo item = ItemMgr.GetGoodsbyFusionTypeandLevel(info4.Reward, level + 1);
                        ItemTemplateInfo info7 = ItemMgr.GetGoodsbyFusionTypeandLevel(info4.Reward, level + 2);
                        List<ItemTemplateInfo> source = new List<ItemTemplateInfo>();
                        if (info7 != null)
                        {
                            source.Add(info7);
                        }
                        if (item != null)
                        {
                            source.Add(item);
                        }
                        if (goodsbyFusionTypeandLevel != null)
                        {
                            source.Add(goodsbyFusionTypeandLevel);
                        }
                        if (predicate == null)
                        {
                            predicate = s => (((double) TotalRate) / ((double) s.FusionNeedRate)) <= 1.1;
                        }
                        if (keySelector == null)
                        {
                            keySelector = s => ((double) TotalRate) / ((double) s.FusionNeedRate);
                        }
                        ItemTemplateInfo info8 = source.Where<ItemTemplateInfo>(predicate).OrderByDescending<ItemTemplateInfo, double>(keySelector).FirstOrDefault<ItemTemplateInfo>();
                        if (func3 == null)
                        {
                            func3 = s => (((double) TotalRate) / ((double) s.FusionNeedRate)) > 1.1;
                        }
                        if (func4 == null)
                        {
                            func4 = s => ((double) TotalRate) / ((double) s.FusionNeedRate);
                        }
                        ItemTemplateInfo info9 = source.Where<ItemTemplateInfo>(func3).OrderBy<ItemTemplateInfo, double>(func4).FirstOrDefault<ItemTemplateInfo>();
                        if ((info8 != null) && (info9 == null))
                        {
                            info = info8;
                            Items_Fusion_List_Info info10 = null;
                            for (int i = 0; i < m_itemsfusionlist.Count<Items_Fusion_List_Info>(); i++)
                            {
                                if (m_itemsfusionlist[i].TemplateID == info8.TemplateID)
                                {
                                    info10 = m_itemsfusionlist[i];
                                    break;
                                }
                            }
                            if (info10 != null)
                            {
                                Random random = new Random();
                                if (random.Next(1, 100) < info10.Real)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                            else
                            {
                                result = true;
                            }
                        }
                        if ((info8 != null) && (info9 != null))
                        {
                            if ((info8.Level - info9.Level) == 2)
                            {
                                double num6 = ((100 * TotalRate) * 0.6) / ((double) info8.FusionNeedRate);
                            }
                            else
                            {
                                double num7 = ((double) (100 * TotalRate)) / ((double) info8.FusionNeedRate);
                            }
                            if ((((double) (100 * TotalRate)) / ((double) info8.FusionNeedRate)) > random_0.Next(100))
                            {
                                info = info8;
                                result = true;
                            }
                            else
                            {
                                info = info9;
                                result = true;
                            }
                        }
                        if ((info8 == null) && (info9 != null))
                        {
                            info = info9;
                            if (random_0.Next(maxValue) < TotalRate)
                            {
                                result = true;
                            }
                        }
                        if (result)
                        {
                            using (List<SqlDataProvider.Data.ItemInfo>.Enumerator enumerator = Items.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    if (enumerator.Current.Template.TemplateID == info.TemplateID)
                                    {
                                        goto Label_04E2;
                                    }
                                }
                                return info;
                            Label_04E2:
                                result = false;
                            }
                        }
                        return info;
                    }
                }
                catch
                {
                }
                finally
                {
                    readerWriterLock_0.ReleaseReaderLock();
                }
            }
            return null;
        }

        public static Dictionary<int, double> FusionPreview(List<SqlDataProvider.Data.ItemInfo> Items, List<SqlDataProvider.Data.ItemInfo> AppendItems, ref bool isBind)
        {
            Func<ItemTemplateInfo, bool> predicate = null;
            Func<ItemTemplateInfo, double> keySelector = null;
            Func<ItemTemplateInfo, bool> func3 = null;
            Func<ItemTemplateInfo, double> func4 = null;
            List<int> list = new List<int>();
            int level = 0;
            int TotalRate = 0;
            int num2 = 0;
            Dictionary<int, double> dictionary = new Dictionary<int, double>();
            dictionary.Clear();
            foreach (SqlDataProvider.Data.ItemInfo info in Items)
            {
                list.Add(info.Template.FusionType);
                if (info.Template.Level > level)
                {
                    level = info.Template.Level;
                }
                TotalRate += info.Template.FusionRate;
                num2 += info.Template.FusionNeedRate;
                if (info.IsBinds)
                {
                    isBind = true;
                }
            }
            foreach (SqlDataProvider.Data.ItemInfo info2 in AppendItems)
            {
                TotalRate += info2.Template.FusionRate / 2;
                num2 += info2.Template.FusionRate / 2;
                if (info2.IsBinds)
                {
                    isBind = true;
                }
            }
            list.Sort();
            StringBuilder builder = new StringBuilder();
            foreach (int num3 in list)
            {
                builder.Append(num3);
            }
            string key = builder.ToString().Trim();
            readerWriterLock_0.AcquireReaderLock(-1);
            try
            {
                if (dictionary_0.ContainsKey(key))
                {
                    Items_Fusion_List_Info info9;
                    int num6;
                    double num4 = 0.0;
                    double num5 = 0.0;
                    FusionInfo info3 = dictionary_0[key];
                    ItemTemplateInfo goodsbyFusionTypeandLevel = ItemMgr.GetGoodsbyFusionTypeandLevel(info3.Reward, level);
                    ItemTemplateInfo item = ItemMgr.GetGoodsbyFusionTypeandLevel(info3.Reward, level + 1);
                    ItemTemplateInfo info6 = ItemMgr.GetGoodsbyFusionTypeandLevel(info3.Reward, level + 2);
                    List<ItemTemplateInfo> source = new List<ItemTemplateInfo>();
                    if (info6 != null)
                    {
                        source.Add(info6);
                    }
                    if (item != null)
                    {
                        source.Add(item);
                    }
                    if (goodsbyFusionTypeandLevel != null)
                    {
                        source.Add(goodsbyFusionTypeandLevel);
                    }
                    if (predicate == null)
                    {
                        predicate = s => (((double) TotalRate) / ((double) s.FusionNeedRate)) <= 1.1;
                    }
                    if (keySelector == null)
                    {
                        keySelector = s => ((double) TotalRate) / ((double) s.FusionNeedRate);
                    }
                    ItemTemplateInfo info7 = source.Where<ItemTemplateInfo>(predicate).OrderByDescending<ItemTemplateInfo, double>(keySelector).FirstOrDefault<ItemTemplateInfo>();
                    if (func3 == null)
                    {
                        func3 = s => (((double) TotalRate) / ((double) s.FusionNeedRate)) > 1.1;
                    }
                    if (func4 == null)
                    {
                        func4 = s => ((double) TotalRate) / ((double) s.FusionNeedRate);
                    }
                    ItemTemplateInfo info8 = source.Where<ItemTemplateInfo>(func3).OrderBy<ItemTemplateInfo, double>(func4).FirstOrDefault<ItemTemplateInfo>();
                    if ((info7 != null) && (info8 == null))
                    {
                        info9 = null;
                        for (num6 = 0; num6 < kethop.m_itemsfusionlist.Count<Items_Fusion_List_Info>(); num6++)
                        {
                            if (kethop.m_itemsfusionlist[num6].TemplateID == info7.TemplateID)
                            {
                                info9 = kethop.m_itemsfusionlist[num6];
                                break;
                            }
                        }
                        if (info9 != null)
                        {
                            dictionary.Add(info7.TemplateID, (double) info9.Show);
                        }
                        else
                        {
                            dictionary.Add(info7.TemplateID, 100.0);
                        }
                    }
                    if ((info7 != null) && (info8 != null))
                    {
                        if ((info7.Level - info8.Level) == 2)
                        {
                            num4 = ((100 * TotalRate) * 0.6) / ((double) info7.FusionNeedRate);
                            num5 = 100.0 - num4;
                        }
                        else
                        {
                            num4 = ((double) (100 * TotalRate)) / ((double) info7.FusionNeedRate);
                            num5 = 100.0 - num4;
                        }
                        dictionary.Add(info7.TemplateID, num4);
                        dictionary.Add(info8.TemplateID, num5);
                    }
                    if ((info7 == null) && (info8 != null))
                    {
                        info9 = null;
                        for (num6 = 0; num6 < kethop.m_itemsfusionlist.Count<Items_Fusion_List_Info>(); num6++)
                        {
                            if (kethop.m_itemsfusionlist[num6].TemplateID == info8.TemplateID)
                            {
                                info9 = kethop.m_itemsfusionlist[num6];
                                break;
                            }
                        }
                        if (info9 != null)
                        {
                            dictionary.Add(info8.TemplateID, (double) info9.Show);
                        }
                        else
                        {
                            dictionary.Add(info8.TemplateID, 100.0);
                        }
                    }
                    foreach (int num7 in dictionary.Keys.ToArray<int>())
                    {
                        foreach (SqlDataProvider.Data.ItemInfo info10 in Items)
                        {
                            if ((num7 == info10.Template.TemplateID) && dictionary.ContainsKey(num7))
                            {
                                dictionary.Remove(num7);
                            }
                        }
                    }
                }
                return dictionary;
            }
            catch
            {
            }
            finally
            {
                readerWriterLock_0.ReleaseReaderLock();
            }
            return null;
        }

        public static bool Init()
        {
            try
            {
                readerWriterLock_0 = new ReaderWriterLock();
                dictionary_0 = new Dictionary<string, FusionInfo>();
                random_0 = new Random();
                return smethod_0(dictionary_0);
            }
            catch (Exception exception)
            {
                if (ilog_0.IsErrorEnabled)
                {
                    ilog_0.Error("FusionMgr", exception);
                }
                return false;
            }
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<string, FusionInfo> dictionary = new Dictionary<string, FusionInfo>();
                if (smethod_0(dictionary))
                {
                    readerWriterLock_0.AcquireWriterLock(-1);
                    try
                    {
                        dictionary_0 = dictionary;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        readerWriterLock_0.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception exception)
            {
                if (ilog_0.IsErrorEnabled)
                {
                    ilog_0.Error("FusionMgr", exception);
                }
            }
            return false;
        }

        private static bool smethod_0(Dictionary<string, FusionInfo> PVQImtJIaVCSTenHlR1)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (FusionInfo info in bussiness.GetAllFusion())
                {
                    List<int> list = new List<int> {
                        info.Item1,
                        info.Item2,
                        info.Item3,
                        info.Item4
                    };
                    list.Sort();
                    StringBuilder builder = new StringBuilder();
                    foreach (int num in list)
                    {
                        if (num != 0)
                        {
                            builder.Append(num);
                        }
                    }
                    string key = builder.ToString();
                    if (!PVQImtJIaVCSTenHlR1.ContainsKey(key))
                    {
                        PVQImtJIaVCSTenHlR1.Add(key, info);
                    }
                }
                m_itemsfusionlist = bussiness.GetAllFusionList();
            }
            return true;
        }
    }
}


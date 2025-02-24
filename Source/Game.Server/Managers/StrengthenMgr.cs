namespace Game.Server.Managers
{
    using Bussiness;
    using Bussiness.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    public class StrengthenMgr
    {
        private static Dictionary<int, StrengthenInfo> _Refinery_Strengthens;
        private static Dictionary<int, StrengthenInfo> _strengthens;
        private static Dictionary<int, StrengThenExpInfo> _Strengthens_Exps;
        private static Dictionary<int, StrengthenGoodsInfo> _Strengthens_Goods;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;
        public static readonly List<int> RingRefineryTemplate;

        static StrengthenMgr()
        {
            log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            List<int> list = new List<int> { 0x233e, 0x23a2, 0x2406, 0x246a, 0x24ce, 0x2532, 0x2596, 0x25fa, 0x265e, 0x26c2 };
            RingRefineryTemplate = list;
        }

        public static bool canUpLv(int exp, int level)
        {
            StrengThenExpInfo info = FindStrengthenExpInfo(level + 1);
            return ((info != null) && (exp >= info.Exp));
        }

        public static StrengthenInfo FindRefineryStrengthenInfo(int level)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_Refinery_Strengthens.ContainsKey(level))
                {
                    return _Refinery_Strengthens[level];
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

        public static StrengThenExpInfo FindStrengthenExpInfo(int level)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_Strengthens_Exps.ContainsKey(level))
                {
                    return _Strengthens_Exps[level];
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

        public static StrengthenGoodsInfo FindStrengthenGoodsInfo(int level, int templateId)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (StrengthenGoodsInfo info in _Strengthens_Goods.Values)
                {
                    if ((info.Level == level) && (templateId == info.CurrentEquip))
                    {
                        return info;
                    }
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

        public static StrengthenInfo FindStrengthenInfo(int level)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_strengthens.ContainsKey(level))
                {
                    return _strengthens[level];
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

        public static StrengthenGoodsInfo FindTransferInfo(int templateId)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (StrengthenGoodsInfo info in _Strengthens_Goods.Values)
                {
                    if ((templateId == info.GainEquip) || (templateId == info.CurrentEquip))
                    {
                        return info;
                    }
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

        public static StrengthenGoodsInfo FindTransferInfo(int level, int templateId)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                foreach (StrengthenGoodsInfo info in _Strengthens_Goods.Values)
                {
                    if ((info.Level == level) && (templateId == info.CurrentEquip))
                    {
                        return info;
                    }
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

        public static int GetNecklaceLevel(int exp)
        {
            foreach (StrengThenExpInfo info in _Strengthens_Exps.Values)
            {
                if (exp < info.NecklaceStrengthExp)
                {
                    return (((info.Level - 1) < 0) ? 0 : (info.Level - 1));
                }
            }
            return 0;
        }

        public static int GetNecklaceMaxExp(int lv)
        {
            StrengThenExpInfo info = FindStrengthenExpInfo(lv);
            if (info == null)
            {
                return 0;
            }
            return info.NecklaceStrengthExp;
        }

        public static int GetNecklaceMaxPlus(int lv)
        {
            StrengThenExpInfo info = FindStrengthenExpInfo(lv);
            if (info == null)
            {
                return 0;
            }
            return info.NecklaceStrengthPlus;
        }

        public static int GetNecklacePlus(int exp, int currentPlus)
        {
            foreach (StrengThenExpInfo info in _Strengthens_Exps.Values)
            {
                if (exp < info.NecklaceStrengthExp)
                {
                    StrengThenExpInfo info2 = FindStrengthenExpInfo(GetNecklaceLevel(exp));
                    if (info2 == null)
                    {
                        return currentPlus;
                    }
                    return info2.NecklaceStrengthPlus;
                }
            }
            return currentPlus;
        }

        public static int getNeedExp(int Exp, int level)
        {
            StrengThenExpInfo info = FindStrengthenExpInfo(level + 1);
            if (info == null)
            {
                return 0;
            }
            return (info.Exp - Exp);
        }

        public static ItemTemplateInfo GetRealWeaponTemplate(SqlDataProvider.Data.ItemInfo item)
        {
            ItemTemplateInfo info = null;
            if (item.Template.CategoryID != 7)
            {
                return info;
            }
            string str = "";
            if ((item.LianGrade <= 5) && (item.StrengthenLevel >= 9))
            {
                switch (item.LianGrade)
                {
                    case 1:
                        str = "1";
                        break;

                    case 2:
                    case 3:
                    case 4:
                        str = "2";
                        break;

                    case 5:
                        str = "3";
                        break;
                }
            }
            else if ((item.StrengthenLevel >= 10) && (item.StrengthenLevel <= 12))
            {
                switch (item.StrengthenLevel)
                {
                    case 10:
                    case 11:
                        str = "1";
                        break;

                    case 12:
                        str = "2";
                        break;
                }
            }
            return ItemMgr.FindItemTemplate(int.Parse(item.TemplateID.ToString().Substring(0, 4) + str));
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _strengthens = new Dictionary<int, StrengthenInfo>();
                _Refinery_Strengthens = new Dictionary<int, StrengthenInfo>();
                _Strengthens_Goods = new Dictionary<int, StrengthenGoodsInfo>();
                _Strengthens_Exps = new Dictionary<int, StrengThenExpInfo>();
                rand = new ThreadSafeRandom();
                return LoadStrengthen(_strengthens, _Refinery_Strengthens, _Strengthens_Exps, _Strengthens_Goods);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("StrengthenMgr", exception);
                }
                return false;
            }
        }

        public static void InheritProperty(SqlDataProvider.Data.ItemInfo Item, ref SqlDataProvider.Data.ItemInfo item)
        {
            if (Item.Hole1 >= 0)
            {
                item.Hole1 = Item.Hole1;
            }
            if (Item.Hole2 >= 0)
            {
                item.Hole2 = Item.Hole2;
            }
            if (Item.Hole3 >= 0)
            {
                item.Hole3 = Item.Hole3;
            }
            if (Item.Hole4 >= 0)
            {
                item.Hole4 = Item.Hole4;
            }
            if (Item.Hole5 >= 0)
            {
                item.Hole5 = Item.Hole5;
            }
            if (Item.Hole6 >= 0)
            {
                item.Hole6 = Item.Hole6;
            }
            item.AttackCompose = Item.AttackCompose;
            item.DefendCompose = Item.DefendCompose;
            item.LuckCompose = Item.LuckCompose;
            item.AgilityCompose = Item.AgilityCompose;
            item.IsBinds = Item.IsBinds;
            item.ValidDate = Item.ValidDate;
        }

        public static void InheritTransferProperty(ref SqlDataProvider.Data.ItemInfo itemZero, ref SqlDataProvider.Data.ItemInfo itemOne, bool tranHole, bool tranHoleFivSix)
        {
            int num = itemZero.Hole1;
            int num2 = itemZero.Hole2;
            int num3 = itemZero.Hole3;
            int num4 = itemZero.Hole4;
            int num5 = itemZero.Hole5;
            int num6 = itemZero.Hole6;
            int num7 = itemZero.Hole5Exp;
            int num8 = itemZero.Hole5Level;
            int num9 = itemZero.Hole6Exp;
            int num10 = itemZero.Hole6Level;
            int attackCompose = itemZero.AttackCompose;
            int defendCompose = itemZero.DefendCompose;
            int agilityCompose = itemZero.AgilityCompose;
            int luckCompose = itemZero.LuckCompose;
            int strengthenLevel = itemZero.StrengthenLevel;
            int strengthenExp = itemZero.StrengthenExp;
            bool isGold = itemZero.IsGold;
            int goldValidDate = itemZero.goldValidDate;
            DateTime goldBeginTime = itemZero.goldBeginTime;
            string latentEnergyCurStr = itemZero.latentEnergyCurStr;
            string latentEnergyNewStr = itemZero.latentEnergyNewStr;
            DateTime latentEnergyEndTime = itemZero.latentEnergyEndTime;
            int num18 = itemOne.Hole1;
            int num19 = itemOne.Hole2;
            int num20 = itemOne.Hole3;
            int num21 = itemOne.Hole4;
            int num22 = itemOne.Hole5;
            int num23 = itemOne.Hole6;
            int num24 = itemOne.Hole5Exp;
            int num25 = itemOne.Hole5Level;
            int num26 = itemOne.Hole6Exp;
            int num27 = itemOne.Hole6Level;
            int num28 = itemOne.AttackCompose;
            int num29 = itemOne.DefendCompose;
            int num30 = itemOne.AgilityCompose;
            int num31 = itemOne.LuckCompose;
            int num32 = itemOne.StrengthenLevel;
            int num33 = itemOne.StrengthenExp;
            bool flag2 = itemOne.IsGold;
            int num34 = itemOne.goldValidDate;
            DateTime time3 = itemOne.goldBeginTime;
            string str3 = itemOne.latentEnergyCurStr;
            string str4 = itemOne.latentEnergyNewStr;
            DateTime time4 = itemOne.latentEnergyEndTime;
            if (tranHole)
            {
                itemOne.Hole1 = num;
                itemZero.Hole1 = num18;
                itemOne.Hole2 = num2;
                itemZero.Hole2 = num19;
                itemOne.Hole3 = num3;
                itemZero.Hole3 = num20;
                itemOne.Hole4 = num4;
                itemZero.Hole4 = num21;
            }
            if (tranHoleFivSix)
            {
                itemOne.Hole5 = num5;
                itemZero.Hole5 = num22;
                itemOne.Hole6 = num6;
                itemZero.Hole6 = num23;
            }
            itemOne.Hole5Exp = num7;
            itemZero.Hole5Exp = num24;
            itemOne.Hole5Level = num8;
            itemZero.Hole5Level = num25;
            itemOne.Hole6Exp = num9;
            itemZero.Hole6Exp = num26;
            itemOne.Hole6Level = num10;
            itemZero.Hole6Level = num27;
            itemZero.StrengthenLevel = num32;
            itemOne.StrengthenLevel = strengthenLevel;
            itemZero.StrengthenExp = num33;
            itemOne.StrengthenExp = strengthenExp;
            itemZero.AttackCompose = num28;
            itemOne.AttackCompose = attackCompose;
            itemZero.DefendCompose = num29;
            itemOne.DefendCompose = defendCompose;
            itemZero.LuckCompose = num31;
            itemOne.LuckCompose = luckCompose;
            itemZero.AgilityCompose = num30;
            itemOne.AgilityCompose = agilityCompose;
            if (itemZero.IsBinds || itemOne.IsBinds)
            {
                itemOne.IsBinds = true;
                itemZero.IsBinds = true;
            }
            itemZero.goldBeginTime = time3;
            itemOne.goldBeginTime = goldBeginTime;
            itemZero.goldValidDate = num34;
            itemOne.goldValidDate = goldValidDate;
            itemZero.latentEnergyCurStr = str3;
            itemOne.latentEnergyCurStr = latentEnergyCurStr;
            itemZero.latentEnergyNewStr = str4;
            itemOne.latentEnergyNewStr = latentEnergyNewStr;
            itemZero.latentEnergyEndTime = time4;
            itemOne.latentEnergyEndTime = latentEnergyEndTime;
        }

        private static bool LoadStrengthen(Dictionary<int, StrengthenInfo> strengthen, Dictionary<int, StrengthenInfo> RefineryStrengthen, Dictionary<int, StrengThenExpInfo> StrengthenExp, Dictionary<int, StrengthenGoodsInfo> StrengthensGoods)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                StrengthenInfo[] allStrengthen = bussiness.GetAllStrengthen();
                StrengthenInfo[] allRefineryStrengthen = bussiness.GetAllRefineryStrengthen();
                StrengThenExpInfo[] allStrengThenExp = bussiness.GetAllStrengThenExp();
                StrengthenGoodsInfo[] allStrengthenGoodsInfo = bussiness.GetAllStrengthenGoodsInfo();
                foreach (StrengthenInfo info in allStrengthen)
                {
                    if (!strengthen.ContainsKey(info.StrengthenLevel))
                    {
                        strengthen.Add(info.StrengthenLevel, info);
                    }
                }
                foreach (StrengthenInfo info2 in allRefineryStrengthen)
                {
                    if (!RefineryStrengthen.ContainsKey(info2.StrengthenLevel))
                    {
                        RefineryStrengthen.Add(info2.StrengthenLevel, info2);
                    }
                }
                foreach (StrengThenExpInfo info3 in allStrengThenExp)
                {
                    if (!StrengthenExp.ContainsKey(info3.Level))
                    {
                        StrengthenExp.Add(info3.Level, info3);
                    }
                }
                foreach (StrengthenGoodsInfo info4 in allStrengthenGoodsInfo)
                {
                    if (!StrengthensGoods.ContainsKey(info4.ID))
                    {
                        StrengthensGoods.Add(info4.ID, info4);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, StrengthenInfo> strengthen = new Dictionary<int, StrengthenInfo>();
                Dictionary<int, StrengthenInfo> refineryStrengthen = new Dictionary<int, StrengthenInfo>();
                Dictionary<int, StrengThenExpInfo> strengthenExp = new Dictionary<int, StrengThenExpInfo>();
                Dictionary<int, StrengthenGoodsInfo> strengthensGoods = new Dictionary<int, StrengthenGoodsInfo>();
                if (LoadStrengthen(strengthen, refineryStrengthen, strengthenExp, strengthensGoods))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _strengthens = strengthen;
                        _Refinery_Strengthens = refineryStrengthen;
                        _Strengthens_Exps = strengthenExp;
                        _Strengthens_Goods = strengthensGoods;
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
                    log.Error("StrengthenMgr", exception);
                }
            }
            return false;
        }

        public static bool TransferCondition(SqlDataProvider.Data.ItemInfo itemAtZero, SqlDataProvider.Data.ItemInfo itemAtOne)
        {
            return (((itemAtZero.Template.CategoryID == 7) || (itemAtOne.Template.CategoryID == 7)) && ((itemAtZero.StrengthenLevel >= 10) || (itemAtOne.StrengthenLevel >= 10)));
        }
    }
}


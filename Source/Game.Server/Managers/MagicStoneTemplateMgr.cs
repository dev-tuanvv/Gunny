namespace Game.Server.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class MagicStoneTemplateMgr
    {
        private static Dictionary<int, MagicStoneTemplateInfo> _magicTemp;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom random = new ThreadSafeRandom();

        private static void ChangePropDefault(ref SqlDataProvider.Data.ItemInfo item, MagicStoneTemplateInfo magicTemp)
        {
            item.AttackCompose = (item.AttackCompose > 0) ? magicTemp.Attack : item.AttackCompose;
            item.DefendCompose = (item.DefendCompose > 0) ? magicTemp.Defence : item.DefendCompose;
            item.LuckCompose = (item.LuckCompose > 0) ? magicTemp.Luck : item.LuckCompose;
            item.AgilityCompose = (item.AgilityCompose > 0) ? magicTemp.Agility : item.AgilityCompose;
            item.MagicAttack = (item.MagicAttack > 0) ? magicTemp.MagicAttack : item.MagicAttack;
            item.MagicDefence = (item.MagicDefence > 0) ? magicTemp.MagicDefence : item.MagicDefence;
        }

        private static void ChangeWithPropID(ref SqlDataProvider.Data.ItemInfo item, int idchoise, MagicStoneTemplateInfo magicTemp)
        {
            switch (idchoise)
            {
                case 1:
                    item.AttackCompose = magicTemp.Attack;
                    break;

                case 2:
                    item.DefendCompose = magicTemp.Defence;
                    break;

                case 3:
                    item.LuckCompose = magicTemp.Luck;
                    break;

                case 4:
                    item.AgilityCompose = magicTemp.Agility;
                    break;

                case 5:
                    item.MagicAttack = magicTemp.MagicAttack;
                    break;

                case 6:
                    item.MagicDefence = magicTemp.MagicDefence;
                    break;
            }
        }

        public static List<MagicStoneTemplateInfo> GetMagicStoneTemplate(int templateid)
        {
            lock (m_lock)
            {
                List<MagicStoneTemplateInfo> list2 = new List<MagicStoneTemplateInfo>();
                foreach (MagicStoneTemplateInfo info in _magicTemp.Values)
                {
                    if (info.TemplateID == templateid)
                    {
                        list2.Add(info);
                    }
                }
                return list2;
            }
        }

        public static MagicStoneTemplateInfo GetMagicStoneTemplate(int templateid, int level)
        {
            lock (m_lock)
            {
                foreach (MagicStoneTemplateInfo info2 in _magicTemp.Values)
                {
                    if ((info2.TemplateID == templateid) && (info2.Level == level))
                    {
                        return info2;
                    }
                }
                return null;
            }
        }

        public static MagicStoneTemplateInfo GetMagicStoneTemplateWithExp(int templateid, int exp)
        {
            lock (m_lock)
            {
                List<MagicStoneTemplateInfo> magicStoneTemplate = GetMagicStoneTemplate(templateid);
                int num = 0;
                IOrderedEnumerable<MagicStoneTemplateInfo> enumerable = from pair in magicStoneTemplate
                    orderby pair.Exp descending
                    select pair;
                foreach (MagicStoneTemplateInfo info2 in enumerable)
                {
                    if (num == 0)
                    {
                        num = info2.Exp;
                    }
                    if ((info2.Exp <= exp) || (exp > num))
                    {
                        return info2;
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
                _magicTemp = new Dictionary<int, MagicStoneTemplateInfo>();
                return LoadMagicStoneTemplate(_magicTemp);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("MagicStoneTemplateMgr", exception);
                }
                return false;
            }
        }

        public static bool IsNormalStone(int type)
        {
            return (type == 1);
        }

        private static bool LoadMagicStoneTemplate(Dictionary<int, MagicStoneTemplateInfo> magicStone)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (MagicStoneTemplateInfo info in bussiness.GetAllMagicStoneTemplate())
                {
                    if (!magicStone.ContainsKey(info.ID))
                    {
                        magicStone.Add(info.ID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, MagicStoneTemplateInfo> magicStone = new Dictionary<int, MagicStoneTemplateInfo>();
                if (LoadMagicStoneTemplate(magicStone))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _magicTemp = magicStone;
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
                    log.Error("MagicStoneTemplateMgr", exception);
                }
            }
            return false;
        }

        public static void SetupMagicStoneWithLevel(SqlDataProvider.Data.ItemInfo item)
        {
            lock (m_lock)
            {
                MagicStoneTemplateInfo magicStoneTemplate;
                List<int> list2;
                int num;
                int num2;
                int num3;
                int num4;
                int num5;
                int num6;
                if ((item != null) && Equip.isMagicStone(item.Template))
                {
                    if (item.StrengthenLevel == 0)
                    {
                        item.StrengthenLevel = 1;
                        magicStoneTemplate = GetMagicStoneTemplate(item.TemplateID, item.StrengthenLevel);
                    }
                    else
                    {
                        magicStoneTemplate = GetMagicStoneTemplateWithExp(item.TemplateID, item.StrengthenExp);
                    }
                    if (magicStoneTemplate != null)
                    {
                        item.StrengthenLevel = magicStoneTemplate.Level;
                        if (item.StrengthenExp <= 0)
                        {
                            item.StrengthenExp = magicStoneTemplate.Exp;
                        }
                        List<int> list = new List<int> { 1, 2, 3, 4, 5, 6 };
                        switch (item.Template.Property3)
                        {
                            case 1:
                                item.AttackCompose = magicStoneTemplate.Attack;
                                item.DefendCompose = magicStoneTemplate.Defence;
                                item.LuckCompose = magicStoneTemplate.Luck;
                                item.AgilityCompose = magicStoneTemplate.Agility;
                                item.MagicAttack = magicStoneTemplate.MagicAttack;
                                item.MagicDefence = magicStoneTemplate.MagicDefence;
                                break;

                            case 2:
                                if (((((item.AttackCompose != 0) || (item.DefendCompose != 0)) || ((item.LuckCompose != 0) || (item.AgilityCompose != 0))) || (item.MagicAttack != 0)) || (item.MagicDefence != 0))
                                {
                                    goto Label_022B;
                                }
                                list2 = new List<int> { 1, 2, 3, 4, 5, 6 };
                                num = 0;
                                goto Label_021D;

                            case 3:
                                if (((((item.AttackCompose != 0) || (item.DefendCompose != 0)) || ((item.LuckCompose != 0) || (item.AgilityCompose != 0))) || (item.MagicAttack != 0)) || (item.MagicDefence != 0))
                                {
                                    goto Label_0307;
                                }
                                list2 = new List<int> { 1, 2, 3, 4, 5, 6 };
                                num3 = 0;
                                goto Label_02F9;

                            case 4:
                                if (((((item.AttackCompose != 0) || (item.DefendCompose != 0)) || ((item.LuckCompose != 0) || (item.AgilityCompose != 0))) || (item.MagicAttack != 0)) || (item.MagicDefence != 0))
                                {
                                    goto Label_03E3;
                                }
                                list2 = new List<int> { 1, 2, 3, 4, 5, 6 };
                                num5 = 0;
                                goto Label_03D5;
                        }
                    }
                }
                goto Label_03FF;
            Label_01E6:
                num2 = random.Next(0, list2.Count - 1);
                ChangeWithPropID(ref item, list2[num2], magicStoneTemplate);
                list2.RemoveAt(num2);
                num++;
            Label_021D:
                if (num < 2)
                {
                    goto Label_01E6;
                }
                goto Label_03FF;
            Label_022B:
                ChangePropDefault(ref item, magicStoneTemplate);
                goto Label_03FF;
            Label_02C2:
                num4 = random.Next(0, list2.Count - 1);
                ChangeWithPropID(ref item, list2[num4], magicStoneTemplate);
                list2.RemoveAt(num4);
                num3++;
            Label_02F9:
                if (num3 < 3)
                {
                    goto Label_02C2;
                }
                goto Label_03FF;
            Label_0307:
                ChangePropDefault(ref item, magicStoneTemplate);
                goto Label_03FF;
            Label_039E:
                num6 = random.Next(0, list2.Count - 1);
                ChangeWithPropID(ref item, list2[num6], magicStoneTemplate);
                list2.RemoveAt(num6);
                num5++;
            Label_03D5:
                if (num5 < 4)
                {
                    goto Label_039E;
                }
                goto Label_03FF;
            Label_03E3:
                ChangePropDefault(ref item, magicStoneTemplate);
            Label_03FF:;
            }
        }

        public static bool StoneNormalSame(SqlDataProvider.Data.ItemInfo item1, SqlDataProvider.Data.ItemInfo item2)
        {
            bool flag = false;
            if (IsNormalStone(item1.Template.Property3) && IsNormalStone(item2.Template.Property3))
            {
                int num = 0;
                int num2 = 0;
                if (item1.AttackCompose > 0)
                {
                    num = 1;
                }
                else if (item1.DefendCompose > 0)
                {
                    num = 2;
                }
                else if (item1.AgilityCompose > 0)
                {
                    num = 3;
                }
                else if (item1.LuckCompose > 0)
                {
                    num = 4;
                }
                else if (item1.MagicAttack > 0)
                {
                    num = 5;
                }
                else if (item1.MagicDefence > 0)
                {
                    num = 6;
                }
                if (item2.AttackCompose > 0)
                {
                    num2 = 1;
                }
                else if (item2.DefendCompose > 0)
                {
                    num2 = 2;
                }
                else if (item2.AgilityCompose > 0)
                {
                    num2 = 3;
                }
                else if (item2.LuckCompose > 0)
                {
                    num2 = 4;
                }
                else if (item2.MagicAttack > 0)
                {
                    num2 = 5;
                }
                else if (item2.MagicDefence > 0)
                {
                    num2 = 6;
                }
                if (((num > 0) && (num2 > 0)) && (num == num2))
                {
                    flag = true;
                }
            }
            return flag;
        }
    }
}


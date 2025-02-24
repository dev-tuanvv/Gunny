namespace Game.Logic
{
    using Bussiness;
    using Bussiness.Managers;
    using Bussiness.Protocol;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DropInventory
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static ThreadSafeRandom random = new ThreadSafeRandom();
        public static int roundDate = 0;

        public static bool AnswerDrop(int answerId, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Answer, answerId.ToString(), "0");
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Answer, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static bool BossDrop(int missionId, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Boss, missionId.ToString(), "0");
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Boss, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static bool BoxDrop(eRoomType e, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Box, ((int)e).ToString(), "0");
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Box, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static bool CardDrop(eRoomType e, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Cards, ((int)e).ToString(), "0");
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Cards, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static bool CopyAllDrop(int copyId, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Copy, copyId.ToString(), "0");
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetAllDropItems(eDropType.Copy, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static bool CopyDrop(int copyId, int user, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Copy, copyId.ToString(), user.ToString());
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Copy, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static List<SqlDataProvider.Data.ItemInfo> CopySystemDrop(int copyId, int OpenCount)
        {
            int num;
            int num2 = Convert.ToInt32((double)(OpenCount * 0.1));
            int num3 = Convert.ToInt32((double)(OpenCount * 0.3));
            int num4 = (OpenCount - num2) - num3;
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
            int dropId = GetDropCondiction(eDropType.Copy, copyId.ToString(), "2");
            if (dropId > 0)
            {
                for (num = 0; num < num2; num++)
                {
                    if (GetDropItems(eDropType.Copy, dropId, ref itemInfos))
                    {
                        list.Add(itemInfos[0]);
                        itemInfos = null;
                    }
                }
            }
            int num6 = GetDropCondiction(eDropType.Copy, copyId.ToString(), "3");
            if (num6 > 0)
            {
                for (num = 0; num < num3; num++)
                {
                    if (GetDropItems(eDropType.Copy, num6, ref itemInfos))
                    {
                        list.Add(itemInfos[0]);
                        itemInfos = null;
                    }
                }
            }
            int num7 = GetDropCondiction(eDropType.Copy, copyId.ToString(), "4");
            if (num7 > 0)
            {
                for (num = 0; num < num4; num++)
                {
                    if (GetDropItems(eDropType.Copy, num7, ref itemInfos))
                    {
                        list.Add(itemInfos[0]);
                        itemInfos = null;
                    }
                }
            }
            return RandomSortList(list);
        }

        public static bool FireDrop(eRoomType e, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Fire, ((int)e).ToString(), "0");
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Fire, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        private static bool GetAllDropItems(eDropType type, int dropId, ref List<SqlDataProvider.Data.ItemInfo> itemInfos)
        {
            if (dropId != 0)
            {
                try
                {
                    int count = 1;
                    List<DropItem> list = DropMgr.FindDropItem(dropId);
                    int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>)(from s in list select s.Random)).Max());
                    int num2 = (from s in list
                                where s.Random >= maxRound
                                select s).ToList<DropItem>().Count<DropItem>();
                    if (num2 == 0)
                    {
                        return false;
                    }
                    count = (count > num2) ? num2 : count;
                    GetRandomUnrepeatArray(0, num2 - 1, count);
                    foreach (DropItem item in list)
                    {
                        int num3 = ThreadSafeRandom.NextStatic(item.BeginData, item.EndData);
                        ItemTemplateInfo goods = ItemMgr.FindItemTemplate(item.ItemId);
                        SqlDataProvider.Data.ItemInfo info2 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, num3, 0x65);
                        if (info2 != null)
                        {
                            info2.IsBinds = item.IsBind;
                            info2.ValidDate = item.ValueDate;
                            info2.IsTips = item.IsTips;
                            info2.IsLogs = item.IsLogs;
                            if (itemInfos == null)
                            {
                                itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                            }
                            if (DropInfoMgr.CanDrop(goods.TemplateID))
                            {
                                itemInfos.Add(info2);
                            }
                        }
                    }
                    return true;
                }
                catch
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(string.Concat(new object[] { "Drop Error：", type, " dropId ", dropId }));
                    }
                }
            }
            return false;
        }

        public static bool GetDrop(int copyId, int user, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Trminhpc, copyId.ToString(), user.ToString());
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Trminhpc, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        private static int GetDropCondiction(eDropType type, string para1, string para2)
        {
            try
            {
                return DropMgr.FindCondiction(type, para1, para2);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Concat(new object[] { "Drop Error：", type, " @ ", exception }));
                }
            }
            return 0;
        }

        private static bool GetDropItems(eDropType type, int dropId, ref List<SqlDataProvider.Data.ItemInfo> itemInfos)
        {
            if (dropId != 0)
            {
                try
                {
                    int count = 1;
                    List<DropItem> list = DropMgr.FindDropItem(dropId);
                    int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>)(from s in list select s.Random)).Max());
                    List<DropItem> source = (from s in list
                                             where s.Random >= maxRound
                                             select s).ToList<DropItem>();
                    int num2 = source.Count<DropItem>();
                    if (num2 == 0)
                    {
                        return false;
                    }
                    count = (count > num2) ? num2 : count;
                    foreach (int num3 in GetRandomUnrepeatArray(0, num2 - 1, count))
                    {
                        int num4 = ThreadSafeRandom.NextStatic(source[num3].BeginData, source[num3].EndData);
                        ItemTemplateInfo goods = ItemMgr.FindItemTemplate(source[num3].ItemId);
                        SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, num4, 0x65);
                        if (item != null)
                        {
                            item.IsBinds = source[num3].IsBind;
                            item.ValidDate = source[num3].ValueDate;
                            item.IsTips = source[num3].IsTips;
                            item.IsLogs = source[num3].IsLogs;
                            if (itemInfos == null)
                            {
                                itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                            }
                            if (DropInfoMgr.CanDrop(goods.TemplateID))
                            {
                                itemInfos.Add(item);
                            }
                        }
                    }
                    return true;
                }
                catch
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(string.Concat(new object[] { "Drop Error：", type, " dropId ", dropId }));
                    }
                }
            }
            return false;
        }

        private static bool GetDropPets(eDropType type, int dropId, ref List<PetTemplateInfo> petInfos)
        {
            if (dropId != 0)
            {
                try
                {
                    int count = 1;
                    List<DropItem> list = DropMgr.FindDropItem(dropId);
                    int maxRound = ThreadSafeRandom.NextStatic(((IEnumerable<int>)(from s in list select s.Random)).Max());
                    List<DropItem> source = (from s in list
                                             where s.Random >= maxRound
                                             select s).ToList<DropItem>();
                    int num2 = source.Count<DropItem>();
                    if (num2 == 0)
                    {
                        return false;
                    }
                    count = (count > num2) ? num2 : count;
                    foreach (int num3 in GetRandomUnrepeatArray(0, num2 - 1, count))
                    {
                        ThreadSafeRandom.NextStatic(source[num3].BeginData, source[num3].EndData);
                        PetTemplateInfo item = PetMgr.FindPetTemplate(source[num3].ItemId);
                        if (item != null)
                        {
                            if (petInfos == null)
                            {
                                petInfos = new List<PetTemplateInfo>();
                            }
                            if (DropInfoMgr.CanDrop(item.TemplateID))
                            {
                                petInfos.Add(item);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception exception)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(string.Concat(new object[] { "Drop Error：", type, " @ ", exception }));
                    }
                }
            }
            return false;
        }

        public static bool GetPetDrop(int copyId, int user, ref List<PetTemplateInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Trminhpc, copyId.ToString(), user.ToString());
            if (dropId > 0)
            {
                List<PetTemplateInfo> petInfos = null;
                if (GetDropPets(eDropType.Trminhpc, dropId, ref petInfos))
                {
                    info = (petInfos != null) ? petInfos : null;
                    return true;
                }
            }
            return false;
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

        public static bool NPCDrop(int dropId, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.NPC, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static bool PvEQuestsDrop(int npcId, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.PveQuests, npcId.ToString(), "0");
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.PveQuests, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static bool PvPQuestsDrop(eRoomType e, bool playResult, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.PvpQuests, ((int)e).ToString(), Convert.ToInt16(playResult).ToString());
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.PvpQuests, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static List<SqlDataProvider.Data.ItemInfo> RandomSortList(List<SqlDataProvider.Data.ItemInfo> list)
        {
            return (from key in list
                    orderby random.Next()
                    select key).ToList<SqlDataProvider.Data.ItemInfo>();
        }

        public static bool RetrieveDrop(int user, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Retrieve, user.ToString(), "0");
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Retrieve, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }

        public static bool SpecialDrop(int missionId, int boxType, ref List<SqlDataProvider.Data.ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Special, missionId.ToString(), boxType.ToString());
            if (dropId > 0)
            {
                List<SqlDataProvider.Data.ItemInfo> itemInfos = null;
                if (GetDropItems(eDropType.Special, dropId, ref itemInfos))
                {
                    info = (itemInfos != null) ? itemInfos : null;
                    return true;
                }
            }
            return false;
        }
    }
}


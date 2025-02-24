namespace Bussiness.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class WorldEventMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, LuckyStartToptenAwardInfo> m_lanternriddlesToptenAward;
        private static ReaderWriterLock m_lock;
        private static Dictionary<int, LuckyStartToptenAwardInfo> m_luckyStartToptenAward;
        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static List<LuckyStartToptenAwardInfo> GetLanternriddlesAwardByRank(int rank)
        {
            List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
            foreach (LuckyStartToptenAwardInfo info in m_lanternriddlesToptenAward.Values)
            {
                if (info.Type == rank)
                {
                    list.Add(info);
                }
            }
            return list;
        }

        public static List<LuckyStartToptenAwardInfo> GetLuckyStartAwardByRank(int rank)
        {
            int num = 0;
            switch (rank)
            {
                case 1:
                    num = 11;
                    break;

                case 2:
                    num = 12;
                    break;

                case 3:
                    num = 13;
                    break;

                case 4:
                case 5:
                    num = 14;
                    break;

                case 6:
                case 7:
                    num = 15;
                    break;

                case 8:
                case 9:
                case 10:
                    num = 0x10;
                    break;
            }
            List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
            foreach (LuckyStartToptenAwardInfo info in m_luckyStartToptenAward.Values)
            {
                if (info.Type == num)
                {
                    list.Add(info);
                }
            }
            return list;
        }

        public static List<LuckyStartToptenAwardInfo> GetLuckyStartToptenAward()
        {
            List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
            foreach (LuckyStartToptenAwardInfo info in m_luckyStartToptenAward.Values)
            {
                list.Add(info);
            }
            return list;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                m_luckyStartToptenAward = new Dictionary<int, LuckyStartToptenAwardInfo>();
                m_lanternriddlesToptenAward = new Dictionary<int, LuckyStartToptenAwardInfo>();
                return LoadData(m_luckyStartToptenAward, m_lanternriddlesToptenAward);
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

        public static bool LoadData(Dictionary<int, LuckyStartToptenAwardInfo> luckyStarts, Dictionary<int, LuckyStartToptenAwardInfo> lanternriddles)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (LuckyStartToptenAwardInfo info in bussiness.GetAllLuckyStartToptenAward())
                {
                    if (!luckyStarts.Keys.Contains<int>(info.ID))
                    {
                        luckyStarts.Add(info.ID, info);
                    }
                }
                foreach (LuckyStartToptenAwardInfo info2 in bussiness.GetAllLanternriddlesTopTenAward())
                {
                    if (!lanternriddles.Keys.Contains<int>(info2.ID))
                    {
                        lanternriddles.Add(info2.ID, info2);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, LuckyStartToptenAwardInfo> luckyStarts = new Dictionary<int, LuckyStartToptenAwardInfo>();
                Dictionary<int, LuckyStartToptenAwardInfo> lanternriddles = new Dictionary<int, LuckyStartToptenAwardInfo>();
                if (LoadData(luckyStarts, lanternriddles))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        m_luckyStartToptenAward = luckyStarts;
                        m_lanternriddlesToptenAward = lanternriddles;
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

        public static bool SendItemsToMail(List<SqlDataProvider.Data.ItemInfo> infos, int PlayerId, string Nickname, string title)
        {
            bool flag = false;
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
                foreach (SqlDataProvider.Data.ItemInfo info in infos)
                {
                    if (info.Template.MaxCount == 1)
                    {
                        for (int j = 0; j < info.Count; j++)
                        {
                            SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info.Template, info);
                            item.Count = 1;
                            list.Add(item);
                        }
                    }
                    else
                    {
                        list.Add(info);
                    }
                }
                for (int i = 0; i < list.Count; i += 5)
                {
                    SqlDataProvider.Data.ItemInfo info4;
                    MailInfo mail = new MailInfo {
                        Title = title,
                        Gold = 0,
                        IsExist = true,
                        Money = 0,
                        Receiver = Nickname,
                        ReceiverID = PlayerId,
                        Sender = "Hệ thống",
                        SenderID = 0,
                        Type = 9,
                        GiftToken = 0
                    };
                    StringBuilder builder = new StringBuilder();
                    StringBuilder builder2 = new StringBuilder();
                    builder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark", new object[0]));
                    int num3 = i;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex1 = info4.ItemID.ToString();
                        mail.Annex1Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "1、", mail.Annex1Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "1、", mail.Annex1Name, "x", info4.Count, ";" }));
                    }
                    num3 = i + 1;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex2 = info4.ItemID.ToString();
                        mail.Annex2Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "2、", mail.Annex2Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "2、", mail.Annex2Name, "x", info4.Count, ";" }));
                    }
                    num3 = i + 2;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex3 = info4.ItemID.ToString();
                        mail.Annex3Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "3、", mail.Annex3Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "3、", mail.Annex3Name, "x", info4.Count, ";" }));
                    }
                    num3 = i + 3;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex4 = info4.ItemID.ToString();
                        mail.Annex4Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "4、", mail.Annex4Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "4、", mail.Annex4Name, "x", info4.Count, ";" }));
                    }
                    num3 = i + 4;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex5 = info4.ItemID.ToString();
                        mail.Annex5Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "5、", mail.Annex5Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "5、", mail.Annex5Name, "x", info4.Count, ";" }));
                    }
                    mail.AnnexRemark = builder.ToString();
                    mail.Content = builder2.ToString();
                    flag = bussiness.SendMail(mail);
                }
            }
            return flag;
        }
    }
}


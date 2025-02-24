namespace Game.Server.Managers
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Server.Buffer;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class AwardMgr
    {
        private static Dictionary<int, DailyAwardInfo> _dailyAward;
        private static bool _dailyAwardState;
        private static Dictionary<int, SearchGoodsTempInfo> _searchGoodsTemp;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;

        public static bool AddDailyAward(GamePlayer player)
        {
            if (DateTime.Now.Date != player.PlayerCharacter.LastAward.Date)
            {
                PlayerInfo playerCharacter = player.PlayerCharacter;
                playerCharacter.DayLoginCount++;
                player.PlayerCharacter.LastAward = DateTime.Now;
                foreach (DailyAwardInfo info in GetAllAwardInfo())
                {
                    if (info.Type == 0)
                    {
                        ItemTemplateInfo template = ItemMgr.FindItemTemplate(info.TemplateID);
                        if (template != null)
                        {
                            BufferList.CreateBufferHour(template, info.ValidDate).Start(player);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool AddSignAwards(GamePlayer player, int DailyLog)
        {
            DailyAwardInfo[] allAwardInfo = GetAllAwardInfo();
            new StringBuilder();
            string translation = string.Empty;
            bool flag = false;
            int templateId = 0;
            int count = 1;
            int validDate = 0;
            bool isBinds = true;
            bool flag3 = false;
            foreach (DailyAwardInfo info in allAwardInfo)
            {
                flag = true;
                if (DailyLog <= 9)
                {
                    if (DailyLog != 3)
                    {
                        if ((DailyLog == 9) && (info.Type == DailyLog))
                        {
                            templateId = info.TemplateID;
                            count = info.Count;
                            validDate = info.ValidDate;
                            isBinds = info.IsBinds;
                            flag3 = true;
                        }
                    }
                    else if (info.Type == DailyLog)
                    {
                        count = info.Count;
                        player.AddGiftToken(count);
                        flag3 = true;
                    }
                }
                else if (DailyLog != 0x11)
                {
                    if ((DailyLog == 0x1a) && (info.Type == DailyLog))
                    {
                        templateId = info.TemplateID;
                        count = info.Count;
                        validDate = info.ValidDate;
                        isBinds = info.IsBinds;
                        flag3 = true;
                    }
                }
                else if (info.Type == DailyLog)
                {
                    templateId = info.TemplateID;
                    count = info.Count;
                    validDate = info.ValidDate;
                    isBinds = info.IsBinds;
                    flag3 = true;
                }
            }
            ItemTemplateInfo goods = ItemMgr.FindItemTemplate(templateId);
            if (goods != null)
            {
                int num5 = count;
                for (int i = 0; i < num5; i += goods.MaxCount)
                {
                    int num7 = ((i + goods.MaxCount) > num5) ? (num5 - i) : goods.MaxCount;
                    SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, num7, 0x71);
                    cloneItem.ValidDate = validDate;
                    cloneItem.IsBinds = isBinds;
                    if (!player.AddTemplate(cloneItem, cloneItem.Template.BagType, cloneItem.Count, eGameView.CaddyTypeGet))
                    {
                        flag = true;
                        using (PlayerBussiness bussiness = new PlayerBussiness())
                        {
                            MailInfo info4;
                            cloneItem.UserID = 0;
                            bussiness.AddGoods(cloneItem);
                            info4 = new MailInfo {
                                Annex1 = cloneItem.ItemID.ToString(),
                                Content = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Content", new object[] { cloneItem.Template.Name }),
                                Gold = 0,
                                Money = 0,
                                Receiver = player.PlayerCharacter.NickName,
                                ReceiverID = player.PlayerCharacter.ID,
                                //Sender = info4.Receiver,
                                //SenderID = info4.ReceiverID,
                                Title = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Title", new object[] { cloneItem.Template.Name }),
                                Type = 15
                            };
                            bussiness.SendMail(info4);
                            translation = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Mail", new object[0]);
                        }
                    }
                }
            }
            if (!(!flag || string.IsNullOrEmpty(translation)))
            {
                player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
            return flag3;
        }

        public static DailyAwardInfo[] GetAllAwardInfo()
        {
            DailyAwardInfo[] infoArray = null;
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                infoArray = _dailyAward.Values.ToArray<DailyAwardInfo>();
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            if (infoArray != null)
            {
                return infoArray;
            }
            return new DailyAwardInfo[0];
        }

        public static SearchGoodsTempInfo GetSearchGoodsTempInfo(int starId)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_searchGoodsTemp.ContainsKey(starId))
                {
                    return _searchGoodsTemp[starId];
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

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _dailyAward = new Dictionary<int, DailyAwardInfo>();
                _searchGoodsTemp = new Dictionary<int, SearchGoodsTempInfo>();
                _dailyAwardState = false;
                return LoadDailyAward(_dailyAward, _searchGoodsTemp);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("AwardMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadDailyAward(Dictionary<int, DailyAwardInfo> awards, Dictionary<int, SearchGoodsTempInfo> searchGoodsTemp)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (DailyAwardInfo info in bussiness.GetAllDailyAward())
                {
                    if (!awards.ContainsKey(info.ID))
                    {
                        awards.Add(info.ID, info);
                    }
                }
                foreach (SearchGoodsTempInfo info2 in bussiness.GetAllSearchGoodsTemp())
                {
                    if (!searchGoodsTemp.ContainsKey(info2.StarID))
                    {
                        searchGoodsTemp.Add(info2.StarID, info2);
                    }
                }
            }
            return true;
        }

        public static int MaxStar()
        {
            return _searchGoodsTemp.Count;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, DailyAwardInfo> awards = new Dictionary<int, DailyAwardInfo>();
                Dictionary<int, SearchGoodsTempInfo> searchGoodsTemp = new Dictionary<int, SearchGoodsTempInfo>();
                if (LoadDailyAward(awards, searchGoodsTemp))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _dailyAward = awards;
                        _searchGoodsTemp = searchGoodsTemp;
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
                    log.Error("AwardMgr", exception);
                }
            }
            return false;
        }

        public static bool DailyAwardState
        {
            get
            {
                return _dailyAwardState;
            }
            set
            {
                _dailyAwardState = value;
            }
        }
    }
}


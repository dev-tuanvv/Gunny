namespace Center.Server
{
    using Bussiness;
    using Bussiness.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Reflection;

    public class WorldMgr
    {
        private static object _syncStop = new object();
        public static DateTime BattleGoundOpenTime;
        public static DateTime begin_time;
        public static string[] bossResourceId = new string[] { "1", "2", "2", "4" };
        public static bool CanSendLightriddleAward;
        public static bool CanSendLuckyStarAward;
        public static long current_blood = 0L;
        public static int currentPVE_ID;
        public static DateTime end_time;
        public static int fight_time;
        public static DateTime FightFootballTime;
        public static bool fightOver;
        public static bool IsBattleGoundOpen;
        public static bool IsFightFootballTime;
        public static bool IsLeagueOpen;
        public static DateTime LeagueOpenTime;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static int LuckStarCountDown;
        private static Dictionary<int, LanternriddlesInfo> m_lanternriddlesInfo;
        private static Dictionary<string, RankingLightriddleInfo> m_lightriddleRankList;
        private static Dictionary<int, LuckStarRewardRecordInfo> m_luckStarRewardRecordInfo;
        private static Dictionary<string, RankingPersonInfo> m_rankList;
        public static readonly long MAX_BLOOD = 0x7d2b7500L;
        public static string[] name = new string[] { "Cuồng long", "B\x00e1 tước hắc \x00e1m", "B\x00e1 tước hắc \x00e1m", "Đội trưởng" };
        public static List<string> NotceList = new List<string>();
        public static int[] Pve_Id = new int[] { 0x4db, 0x7531, 0x7532, 0x7534 };
        public static bool roomClose;
        private static readonly int worldbossTime = 60;
        public static bool worldOpen;

        public static void AddOrUpdateLanternriddles(int playerID, LanternriddlesInfo Lanternriddles)
        {
            if (!m_lanternriddlesInfo.ContainsKey(playerID))
            {
                m_lanternriddlesInfo.Add(playerID, Lanternriddles);
            }
            else
            {
                m_lanternriddlesInfo[playerID] = Lanternriddles;
            }
        }

        public static bool CheckName(string NickName)
        {
            return m_rankList.Keys.Contains<string>(NickName);
        }

        public static List<LuckStarRewardRecordInfo> GetAllLuckyStarRank()
        {
            List<LuckStarRewardRecordInfo> list = new List<LuckStarRewardRecordInfo>();
            foreach (LuckStarRewardRecordInfo info in m_luckStarRewardRecordInfo.Values)
            {
                list.Add(info);
            }
            return list;
        }

        public static LanternriddlesInfo GetLanternriddles(int playerID)
        {
            if (m_lanternriddlesInfo.ContainsKey(playerID))
            {
                return m_lanternriddlesInfo[playerID];
            }
            return null;
        }

        public static RankingPersonInfo GetSingleRank(string name)
        {
            return m_rankList[name];
        }

        public static bool LoadNotice(string path)
        {
            string str = path + SystemNoticeFile;
            if (!File.Exists(str))
            {
                log.Error("SystemNotice file : " + str + " not found !");
            }
            else
            {
                try
                {
                    foreach (XElement element in XDocument.Load(str).Root.Nodes())
                    {
                        try
                        {
                            int.Parse(element.Attribute("id").Value);
                            string item = element.Attribute("notice").Value;
                            NotceList.Add(item);
                        }
                        catch (Exception exception)
                        {
                            log.Error("BattleMgr setup error:", exception);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    log.Error("BattleMgr setup error:", exception2);
                }
            }
            log.InfoFormat("Total {0} syterm notice loaded.", NotceList.Count);
            return true;
        }

        public static void ReduceBlood(int value)
        {
            if (current_blood > 0L)
            {
                current_blood -= value;
            }
        }

        public static void ResetLightriddleRank()
        {
            m_lightriddleRankList.Clear();
        }

        public static void ResetLuckStar()
        {
            m_luckStarRewardRecordInfo.Clear();
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                bussiness.ResetLuckStarRank();
            }
            LuckStarCountDown = Math.Abs((int) ((60 - DateTime.Now.Minute) - 30));
        }

        public static void SavekyStarToDatabase()
        {
            List<LuckStarRewardRecordInfo> allLuckyStarRank = GetAllLuckyStarRank();
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (LuckStarRewardRecordInfo info in allLuckyStarRank)
                {
                    bussiness.SaveLuckStarRankInfo(info);
                }
            }
        }

        public static void SaveLuckyStarRewardRecord()
        {
            int hour = DateTime.Now.Hour;
            if (LuckStarCountDown > 0)
            {
                LuckStarCountDown--;
            }
            if (LuckStarCountDown == 0)
            {
                if (CanSendLuckyStarAward)
                {
                    SavekyStarToDatabase();
                    LuckStarCountDown = 30;
                }
                else
                {
                    LuckStarCountDown = -1;
                }
            }
        }

        public static List<RankingLightriddleInfo> SelectTopEight()
        {
            List<RankingLightriddleInfo> list = new List<RankingLightriddleInfo>();
            IOrderedEnumerable<KeyValuePair<string, RankingLightriddleInfo>> enumerable = from pair in m_lightriddleRankList
                where pair.Value.Integer > 1
                orderby pair.Value.Integer descending
                select pair;
            foreach (KeyValuePair<string, RankingLightriddleInfo> pair in enumerable)
            {
                if (list.Count == 8)
                {
                    return list;
                }
                list.Add(pair.Value);
            }
            return list;
        }

        public static List<RankingPersonInfo> SelectTopTen()
        {
            List<RankingPersonInfo> list = new List<RankingPersonInfo>();
            IOrderedEnumerable<KeyValuePair<string, RankingPersonInfo>> enumerable = from pair in m_rankList
                orderby pair.Value.Damage descending
                select pair;
            foreach (KeyValuePair<string, RankingPersonInfo> pair in enumerable)
            {
                if (list.Count == 10)
                {
                    return list;
                }
                list.Add(pair.Value);
            }
            return list;
        }

        public static void SendLightriddleTopEightAward()
        {
            List<RankingLightriddleInfo> list = SelectTopEight();
            foreach (RankingLightriddleInfo info in list)
            {
                string format = "Phần thưởng hạng {0} hoạt động nguy\x00ean ti\x00eau";
                List<LuckyStartToptenAwardInfo> lanternriddlesAwardByRank = WorldEventMgr.GetLanternriddlesAwardByRank(info.Rank);
                List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
                foreach (LuckyStartToptenAwardInfo info2 in lanternriddlesAwardByRank)
                {
                    SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info2.TemplateID), 1, 0x69);
                    item.IsBinds = info2.IsBinds;
                    item.ValidDate = info2.Validate;
                    item.Count = info2.Count;
                    infos.Add(item);
                }
                format = string.Format(format, info.Rank);
                WorldEventMgr.SendItemsToMail(infos, info.PlayerId, info.NickName, format);
            }
            CanSendLightriddleAward = false;
        }

        public static void SendLuckyStarTopTenAward()
        {
            int minUseNum = GameProperties.MinUseNum;
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (LuckStarRewardRecordInfo info in bussiness.GetLuckStarTopTenRank(minUseNum))
                {
                    string format = "Phần thưởng hạng {0} hoạt động Sao may mắn";
                    List<LuckyStartToptenAwardInfo> luckyStartAwardByRank = WorldEventMgr.GetLuckyStartAwardByRank(info.rank);
                    List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
                    foreach (LuckyStartToptenAwardInfo info2 in luckyStartAwardByRank)
                    {
                        SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info2.TemplateID), 1, 0x69);
                        item.IsBinds = info2.IsBinds;
                        item.ValidDate = info2.Validate;
                        item.Count = info2.Count;
                        infos.Add(item);
                    }
                    format = string.Format(format, info.rank);
                    WorldEventMgr.SendItemsToMail(infos, info.PlayerID, info.nickName, format);
                }
            }
            CanSendLuckyStarAward = false;
        }

        public static void SetupWorldBoss(int id)
        {
            current_blood = MAX_BLOOD;
            begin_time = DateTime.Now;
            end_time = begin_time.AddDays(1.0);
            fight_time = worldbossTime - begin_time.Minute;
            fightOver = false;
            roomClose = false;
            currentPVE_ID = id;
            worldOpen = true;
        }

        public static void SortRank()
        {
            IOrderedEnumerable<KeyValuePair<string, RankingLightriddleInfo>> enumerable = from pair in m_lightriddleRankList
                orderby pair.Value.Integer descending
                select pair;
            int num = 1;
            Dictionary<string, RankingLightriddleInfo> dictionary = new Dictionary<string, RankingLightriddleInfo>();
            foreach (KeyValuePair<string, RankingLightriddleInfo> pair in enumerable)
            {
                pair.Value.Rank = num;
                dictionary.Add(pair.Key, pair.Value);
                num++;
            }
            m_lightriddleRankList = dictionary;
        }

        public static bool Start()
        {
            try
            {
                CanSendLightriddleAward = true;
                CanSendLuckyStarAward = true;
                m_rankList = new Dictionary<string, RankingPersonInfo>();
                m_lightriddleRankList = new Dictionary<string, RankingLightriddleInfo>();
                m_lanternriddlesInfo = new Dictionary<int, LanternriddlesInfo>();
                m_luckStarRewardRecordInfo = new Dictionary<int, LuckStarRewardRecordInfo>();
                current_blood = MAX_BLOOD;
                begin_time = DateTime.Now;
                LeagueOpenTime = DateTime.Now;
                BattleGoundOpenTime = DateTime.Now;
                FightFootballTime = DateTime.Now;
                ResetLuckStar();
                end_time = begin_time.AddDays(1.0);
                fightOver = true;
                roomClose = true;
                worldOpen = false;
                IsLeagueOpen = false;
                IsBattleGoundOpen = false;
                IsFightFootballTime = false;
                return LoadNotice("");
            }
            catch (Exception exception)
            {
                log.ErrorFormat("Load server list from db failed:{0}", exception);
                return false;
            }
        }

        public static void UpdateFightTime()
        {
            if (!fightOver)
            {
                fight_time = worldbossTime - begin_time.Minute;
            }
        }

        public static void UpdateLightriddleRank(int integer, int typeVip, string nickName, int playerId)
        {
            if (m_lightriddleRankList.Keys.Contains<string>(nickName))
            {
                m_lightriddleRankList[nickName].Integer = integer;
            }
            else
            {
                RankingLightriddleInfo info = new RankingLightriddleInfo {
                    NickName = nickName,
                    Integer = integer,
                    TypeVIP = typeVip,
                    PlayerId = playerId
                };
                m_lightriddleRankList.Add(nickName, info);
            }
            SortRank();
        }

        public static void UpdateLuckStarRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
        {
            if (m_luckStarRewardRecordInfo.Keys.Contains<int>(PlayerID))
            {
                LuckStarRewardRecordInfo local1 = m_luckStarRewardRecordInfo[PlayerID];
                local1.useStarNum++;
            }
            else
            {
                LuckStarRewardRecordInfo info = new LuckStarRewardRecordInfo {
                    PlayerID = PlayerID,
                    nickName = nickName,
                    useStarNum = 1,
                    TemplateID = TemplateID,
                    Count = Count,
                    isVip = isVip
                };
                m_luckStarRewardRecordInfo.Add(PlayerID, info);
            }
        }

        public static void UpdateRank(int damage, int honor, string nickName)
        {
            if (m_rankList.Keys.Contains<string>(nickName))
            {
                RankingPersonInfo local1 = m_rankList[nickName];
                local1.Damage += damage;
                RankingPersonInfo local2 = m_rankList[nickName];
                local2.Honor += honor;
            }
            else
            {
                RankingPersonInfo info = new RankingPersonInfo {
                    ID = m_rankList.Count + 1,
                    Name = nickName,
                    Damage = damage,
                    Honor = honor
                };
                m_rankList.Add(nickName, info);
            }
        }

        public static void WorldBossClearRank()
        {
            m_rankList.Clear();
        }

        public static void WorldBossClose()
        {
            worldOpen = false;
        }

        public static void WorldBossFightOver()
        {
            fightOver = true;
        }

        public static void WorldBossRoomClose()
        {
            roomClose = true;
        }

        private static string SystemNoticeFile
        {
            get
            {
                return ConfigurationManager.AppSettings["SystemNoticePath"];
            }
        }
    }
}


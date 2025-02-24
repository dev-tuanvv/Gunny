namespace Game.Server.RingStation
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Server.Battle;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using Game.Server.RingStation.Battle;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public sealed class RingStationMgr
    {
        private static RingstationConfigInfo _ringstationConfigInfo;
        private static VirtualPlayerInfo _virtualPlayerInfo = new VirtualPlayerInfo();
        private static Dictionary<int, UserRingStationInfo> dictionary_1 = new Dictionary<int, UserRingStationInfo>();
        private static Dictionary<int, List<RingstationBattleFieldInfo>> dictionary_2 = new Dictionary<int, List<RingstationBattleFieldInfo>>();
        private static Func<RingstationBattleFieldInfo, DateTime> func_0;
        private static Func<UserRingStationInfo, bool> func_1;
        private static Func<UserRingStationInfo, int> func_2;
        private static Func<UserRingStationInfo, int> func_3;
        private static Func<UserRingStationInfo, bool> func_4;
        private static Func<UserRingStationInfo, int> func_5;
        private static int int_0 = 10;
        private static List<VirtualPlayerInfo> list_0 = new List<VirtualPlayerInfo>();
        private static List<UserRingStationInfo> list_1 = new List<UserRingStationInfo>();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();
        protected static object m_lock = new object();
        private static Dictionary<int, RingStationGamePlayer> m_players = new Dictionary<int, RingStationGamePlayer>();
        private static RingStationBattleServer m_server = null;
        protected static Timer m_statusScanTimer;
        private static string[] NickName;
        private static ThreadSafeRandom rand = new ThreadSafeRandom();
        private static readonly string weaklessGuildProgressStr = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
        private static readonly string[] weapons = new string[] { "7017|Sdart", "7018|Selectricbar", "7019|Sbrick", "7016|Sbomb", "7008|dart", "7020|Sfruit", "7021|SOPSbox", "70631|maya2", "70461|Scar2", "70591|Ssushi2", "70581|wand2", "70561|bazooka2", "70511|Bslingshot2", "70501|Snihontou2", "70451|Sicecream2" };

        public static bool AddPlayer(int playerId, RingStationGamePlayer player)
        {
            m_clientLocker.AcquireWriterLock(-1);
            try
            {
                if (m_players.ContainsKey(playerId))
                {
                    return false;
                }
                m_players.Add(playerId, player);
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
            return true;
        }

        public static UserRingStationInfo BaseRingStationChallenges(int id)
        {
            UserRingStationInfo info = new UserRingStationInfo {
                Rank = 0,
                WeaponID = _virtualPlayerInfo.Weapon,
                signMsg = LanguageMgr.GetTranslation("BaseRingStationChallenges.Msg2", new object[0]),
                BaseDamage = 0xf2,
                BaseGuard = 120,
                BaseEnergy = 240
            };
            PlayerInfo info2 = new PlayerInfo {
                ID = (id == 0) ? RingStationConfiguration.NextPlayerID() : id,
                UserName = "NormalInfo",
                NickName = LanguageMgr.GetTranslation("BaseRingStationChallenges.Msg1", new object[0]),
                typeVIP = 1,
                VIPLevel = 1,
                Grade = 0x19,
                Sex = false,
                Style = _virtualPlayerInfo.Style,
                Colors = ",,,,,,,,,,,,,,,",
                Skin = "",
                ConsortiaName = "",
                Hide = 0x423a35c7,
                Offer = 0,
                Win = 0,
                Total = 0,
                Escape = 0,
                Repute = 0,
                Nimbus = 0,
                GP = 0x15ed7d,
                FightPower = 0x3822,
                AchievementPoint = 0,
                Attack = 0xe1,
                Defence = 160,
                Agility = 50,
                Luck = 60,
                hp = 0xdac
            };
            info.Info = info2;
            return info;
        }

        public static void BeginTimer()
        {
            int dueTime = 0xea60;
            if (m_statusScanTimer == null)
            {
                m_statusScanTimer = new Timer(new TimerCallback(RingStationMgr.StatusScan), null, dueTime, dueTime);
            }
            else
            {
                m_statusScanTimer.Change(dueTime, dueTime);
            }
        }

        public static void CreateAutoBot(int roomtype, int gametype, int npcId)
        {
            BaseRoomRingStation room = new BaseRoomRingStation(RingStationConfiguration.NextRoomId()) {
                RoomType = roomtype,
                GameType = gametype,
                PickUpNpcId = npcId,
                IsAutoBot = true,
                IsFreedom = true
            };
            RingStationGamePlayer player = new RingStationGamePlayer();
            string str = RingStationConfiguration.RandomName[rand.Next(0, RingStationConfiguration.RandomName.Length - 1)];
            player.NickName = str;
            player.GP = 0x503;
            player.Grade = 5;
            player.Attack = 100;
            player.Defence = 100;
            player.Luck = 100;
            player.Agility = 100;
            player.hp = 0x3e8;
            player.FightPower = 0x4b0;
            player.BaseAttack = 100.0;
            player.BaseDefence = 50.0;
            player.BaseAgility = 1.0 - (player.Agility * 0.001);
            player.BaseBlood = 1000.0;
            string str2 = weapons[rand.Next(weapons.Length)];
            player.Style = string.Format("1214|head13,,3244|hair44,,5276|cloth76,6204|face3,{0},,,,,,,,,", str2);
            player.Colors = ",,,,,,,,,,,,,,,";
            player.Hide = 0x423a35c7;
            player.TemplateID = int.Parse(str2.Split(new char[] { '|' })[0]);
            player.StrengthLevel = 0;
            player.WeaklessGuildProgressStr = "R/O/DeABAtgWdWsIAAAAAAAAgCAECwAAAAAAABgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
            player.ID = npcId;
            if (m_server != null)
            {
                AddPlayer(player.ID, player);
                room.AddPlayer(player);
                m_server.AddRoom(room);
            }
        }

        public static void CreateAutoBot(GamePlayer player, int roomtype, int gametype, int npcId)
        {
            BaseRoomRingStation room = new BaseRoomRingStation(RingStationConfiguration.NextRoomId()) {
                RoomType = roomtype,
                GameType = gametype,
                PickUpNpcId = npcId,
                IsAutoBot = true,
                IsFreedom = true
            };
            RingStationGamePlayer player2 = new RingStationGamePlayer();
            string str = RingStationConfiguration.RandomName[rand.Next(0, RingStationConfiguration.RandomName.Length - 1)];
            player2.NickName = str;
            player2.GP = player.PlayerCharacter.GP;
            player2.Grade = player.PlayerCharacter.Grade;
            player2.Attack = player.PlayerCharacter.Attack;
            player2.Defence = player.PlayerCharacter.Defence;
            player2.Luck = player.PlayerCharacter.Luck;
            player2.Agility = player.PlayerCharacter.Agility;
            player2.hp = player.PlayerCharacter.hp;
            player2.FightPower = player.PlayerCharacter.FightPower;
            player2.BaseAttack = player.GetBaseAttack();
            player2.BaseDefence = player.GetBaseDefence();
            player2.BaseAgility = player.GetBaseAgility();
            player2.BaseBlood = player.GetBaseBlood();
            string str2 = weapons[rand.Next(weapons.Length)];
            player2.Style = string.Format("1214|head13,,3244|hair44,,5276|cloth76,6204|face3,{0},,,,,,,,,", str2);
            player2.Colors = ",,,,,,,,,,,,,,,";
            player2.Hide = 0x423a35c7;
            player2.TemplateID = int.Parse(str2.Split(new char[] { '|' })[0]);
            player2.StrengthLevel = 0;
            player2.WeaklessGuildProgressStr = weaklessGuildProgressStr;
            player2.ID = npcId;
            if (m_server != null)
            {
                AddPlayer(player2.ID, player2);
                room.AddPlayer(player2);
                m_server.AddRoom(room);
            }
        }

        public static int CreateRingStationChallenge(UserRingStationInfo player, int roomtype, int gametype)
        {
            int iD = player.Info.ID;
            BaseRoomRingStation room = new BaseRoomRingStation(RingStationConfiguration.NextRoomId()) {
                RoomType = roomtype,
                GameType = gametype,
                PickUpNpcId = iD,
                IsAutoBot = true,
                IsFreedom = false
            };
            RingStationGamePlayer player2 = new RingStationGamePlayer {
                NickName = player.Info.NickName,
                GP = player.Info.GP,
                Grade = player.Info.Grade,
                Attack = player.Info.Attack,
                Defence = player.Info.Defence,
                Luck = player.Info.Luck,
                Agility = player.Info.Agility,
                hp = player.Info.hp,
                FightPower = player.Info.FightPower,
                BaseAttack = player.BaseDamage,
                BaseDefence = player.BaseGuard,
                BaseAgility = player.BaseEnergy,
                BaseBlood = player.Info.hp,
                Style = player.Info.Style,
                Colors = player.Info.Colors,
                Hide = player.Info.Hide,
                TemplateID = player.WeaponID,
                StrengthLevel = 1,
                WeaklessGuildProgressStr = weaklessGuildProgressStr,
                ID = iD
            };
            if (m_server != null)
            {
                AddPlayer(iD, player2);
                room.AddPlayer(player2);
                m_server.AddRoom(room);
            }
            return iD;
        }

        public static List<UserRingStationInfo> FindRingStationInfoByRank(int userId, int min, int max)
        {
            List<UserRingStationInfo> list = new List<UserRingStationInfo>();
            foreach (UserRingStationInfo info in dictionary_1.Values)
            {
                if (((info.UserID != userId) && (info.Rank >= min)) && (info.Rank <= max))
                {
                    list.Add(info);
                }
            }
            return list;
        }

        public static List<RingStationGamePlayer> GetAllPlayer()
        {
            List<RingStationGamePlayer> list = new List<RingStationGamePlayer>();
            m_clientLocker.AcquireReaderLock(-1);
            try
            {
                foreach (RingStationGamePlayer player in m_players.Values)
                {
                    list.Add(player);
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return list;
        }

        public static int GetAutoBot(GamePlayer player, int roomtype, int gametype)
        {
            int playerId = RingStationConfiguration.NextPlayerID();
            BaseRoomRingStation room = new BaseRoomRingStation(RingStationConfiguration.NextRoomId()) {
                RoomType = roomtype,
                GameType = gametype,
                PickUpNpcId = playerId,
                IsAutoBot = true,
                IsFreedom = false
            };
            RingStationGamePlayer player2 = new RingStationGamePlayer();
            string str = RingStationConfiguration.RandomName[rand.Next(0, RingStationConfiguration.RandomName.Length - 1)];
            player2.NickName = NickName[rand.Next(NickName.Length)] + playerId;
            player2.GP = player.PlayerCharacter.GP;
            player2.Grade = player.PlayerCharacter.Grade;
            player2.Attack = player.PlayerCharacter.Attack;
            player2.Defence = player.PlayerCharacter.Defence;
            player2.Luck = player.PlayerCharacter.Luck;
            player2.Agility = player.PlayerCharacter.Agility;
            player2.hp = player.PlayerCharacter.hp;
            player2.FightPower = player.PlayerCharacter.FightPower;
            player2.BaseAttack = player.GetBaseAttack();
            player2.BaseDefence = player.GetBaseDefence();
            player2.BaseAgility = player.GetBaseAgility();
            player2.BaseBlood = player.GetBaseBlood();
            string str2 = weapons[rand.Next(weapons.Length)];
            player2.Style = string.Format("1214|head13,,3244|hair44,,5276|cloth76,6204|face3,{0},,,,,,,,,", str2);
            player2.Colors = ",,,,,,,,,,,,,,,";
            player2.Hide = 0x423a35c7;
            player2.TemplateID = int.Parse(str2.Split(new char[] { '|' })[0]);
            player2.StrengthLevel = 0;
            player2.WeaklessGuildProgressStr = weaklessGuildProgressStr;
            player2.ID = playerId;
            if (m_server != null)
            {
                AddPlayer(playerId, player2);
                room.AddPlayer(player2);
                m_server.AddRoom(room);
            }
            return playerId;
        }

        public static RingStationGamePlayer GetPlayerById(int playerId)
        {
            RingStationGamePlayer player = null;
            m_clientLocker.AcquireReaderLock(-1);
            try
            {
                if (m_players.ContainsKey(playerId))
                {
                    player = m_players[playerId];
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return player;
        }

        public static RingstationBattleFieldInfo[] GetRingBattleFields(int playerId)
        {
            List<RingstationBattleFieldInfo> list = new List<RingstationBattleFieldInfo>();
            object @lock = m_lock;
            lock (@lock)
            {
                if (dictionary_2.ContainsKey(playerId))
                {
                    List<RingstationBattleFieldInfo> list2 = dictionary_2[playerId];
                    foreach (RingstationBattleFieldInfo info in list2)
                    {
                        list.Add(info);
                    }
                }
            }
            IEnumerable<RingstationBattleFieldInfo> source = list;
            if (func_0 == null)
            {
                func_0 = new Func<RingstationBattleFieldInfo, DateTime>(RingStationMgr.smethod_0);
            }
            return source.OrderByDescending<RingstationBattleFieldInfo, DateTime>(func_0).Take<RingstationBattleFieldInfo>(10).ToArray<RingstationBattleFieldInfo>();
        }

        public static UserRingStationInfo GetRingStationChallenge(int playerId, int rank, ref bool isAutoBot)
        {
            if (dictionary_1.ContainsKey(playerId) && (rank != 0))
            {
                return dictionary_1[playerId];
            }
            isAutoBot = true;
            return BaseRingStationChallenges(playerId);
        }

        public static UserRingStationInfo[] GetRingStationInfos(int userId, int rank)
        {
            NormalPlayer = GetVirtualPlayerInfo();
            Dictionary<int, UserRingStationInfo> dictionary = new Dictionary<int, UserRingStationInfo>();
            if (rank > 0)
            {
                int min = rank;
                int count = list_1.Count;
                int num3 = int_0 / 2;
                if ((min != count) && ((count - min) >= num3))
                {
                    if ((count - min) > int_0)
                    {
                        min = rank - num3;
                        count = rank + num3;
                    }
                }
                else
                {
                    min = count - int_0;
                }
                if (min < num3)
                {
                    count = min + int_0;
                    min = 0;
                }
                if (min < 0)
                {
                    min = 0;
                }
                List<UserRingStationInfo> list = FindRingStationInfoByRank(userId, min, count);
                if (list.Count > 4)
                {
                    int num5;
                    for (int i = 0; dictionary.Count < 4; i = num5 + 1)
                    {
                        UserRingStationInfo info = list[rand.Next(list.Count)];
                        if (info != null)
                        {
                            if (!dictionary.ContainsKey(info.UserID))
                            {
                                dictionary.Add(info.UserID, info);
                            }
                            list.Remove(info);
                        }
                        num5 = i;
                    }
                }
            }
            if (dictionary.Count == 0)
            {
                UserRingStationInfo info2 = BaseRingStationChallenges(0);
                dictionary.Add(info2.Info.ID, info2);
            }
            return dictionary.Values.ToArray<UserRingStationInfo>();
        }

        public static UserRingStationInfo[] GetRingStationRanks()
        {
            List<UserRingStationInfo> list = new List<UserRingStationInfo>();
            foreach (UserRingStationInfo info in list_1)
            {
                list.Add(info);
                if (list.Count >= 50)
                {
                    break;
                }
            }
            return list.ToArray();
        }

        public static UserRingStationInfo GetSingleRingStationInfos(int playerId)
        {
            if (dictionary_1.ContainsKey(playerId))
            {
                return dictionary_1[playerId];
            }
            return null;
        }

        public static VirtualPlayerInfo GetVirtualPlayerInfo()
        {
            int num = rand.Next(list_0.Count);
            return list_0[num];
        }

        public static int GetWeaponID(string style)
        {
            if (!string.IsNullOrEmpty(style))
            {
                string str = style.Split(new char[] { ',' })[6];
                if (str.IndexOf("|") != -1)
                {
                    return int.Parse(str.Split(new char[] { '|' })[0]);
                }
            }
            return 0x1b60;
        }

        public static bool Init()
        {
            bool flag = false;
            try
            {
                m_players.Clear();
                BattleServer server = BattleMgr.GetServer(1);
                if (server == null)
                {
                    return false;
                }
                m_server = new RingStationBattleServer(RingStationConfiguration.ServerID, server.Ip, server.Port, "1,7road");
                try
                {
                    NickName = GameProperties.VirtualName.Split(new char[] { ',' });
                    flag = m_server.Start();
                    if (!SetupVirtualPlayer())
                    {
                        return false;
                    }
                    using (new PlayerBussiness())
                    {
                        _ringstationConfigInfo = null;
                        if (_ringstationConfigInfo == null)
                        {
                            _ringstationConfigInfo = new RingstationConfigInfo();
                            _ringstationConfigInfo.buyCount = 10;
                            _ringstationConfigInfo.buyPrice = 0x1f40;
                            _ringstationConfigInfo.cdPrice = 0x2710;
                            _ringstationConfigInfo.AwardTime = DateTime.Now.AddDays(3.0);
                            _ringstationConfigInfo.AwardNum = 450;
                            _ringstationConfigInfo.AwardFightWin = "1-50,25|51-100,20|101-1000000,15";
                            _ringstationConfigInfo.AwardFightLost = "1-50,15|51-100,10|101-1000000,5";
                            _ringstationConfigInfo.ChampionText = "";
                            _ringstationConfigInfo.ChallengeNum = 10;
                            _ringstationConfigInfo.IsFirstUpdateRank = true;
                        }
                    }
                    BeginTimer();
                    ReLoadUserRingStation();
                    ReLoadBattleField();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            catch (Exception exception2)
            {
                log.Error("RingStationMgr Init", exception2);
            }
            return flag;
        }

        public static RingstationBattleFieldInfo[] LoadRingstationBattleFieldDb()
        {
            using (new PlayerBussiness())
            {
                return new List<RingstationBattleFieldInfo>().ToArray();
            }
        }

        public static Dictionary<int, List<RingstationBattleFieldInfo>> LoadRingstationBattleFields(RingstationBattleFieldInfo[] RingstationBattleField)
        {
            int num;
            Dictionary<int, List<RingstationBattleFieldInfo>> dictionary = new Dictionary<int, List<RingstationBattleFieldInfo>>();
            for (int i = 0; i < RingstationBattleField.Length; i = num + 1)
            {
                Func<RingstationBattleFieldInfo, bool> predicate = null;
                RingstationBattleFieldInfo info = RingstationBattleField[i];
                if (!dictionary.Keys.Contains<int>(info.UserID))
                {
                    if (predicate == null)
                    {
                        predicate = s => s.UserID == info.UserID;
                    }
                    IEnumerable<RingstationBattleFieldInfo> source = RingstationBattleField.Where<RingstationBattleFieldInfo>(predicate);
                    dictionary.Add(info.UserID, source.ToList<RingstationBattleFieldInfo>());
                }
                num = i;
            }
            return dictionary;
        }

        public static void LoadRingStationInfo(PlayerInfo player, int dame, int guard)
        {
            if (player != null)
            {
                using (new PlayerBussiness())
                {
                    if (dictionary_1.ContainsKey(player.ID))
                    {
                        bool flag3 = false;
                        UserRingStationInfo info = dictionary_1[player.ID];
                        if ((dame != info.BaseDamage) && (info.BaseGuard != guard))
                        {
                            info.BaseDamage = dame;
                            info.BaseGuard = guard;
                            info.BaseEnergy = (int) (1.0 - (player.Agility * 0.001));
                            flag3 = true;
                        }
                        int weaponID = GetWeaponID(player.Style);
                        if (info.WeaponID != weaponID)
                        {
                            info.WeaponID = weaponID;
                            flag3 = true;
                        }
                        bool flag6 = flag3;
                    }
                    else
                    {
                        UserRingStationInfo info2 = new UserRingStationInfo {
                            UserID = player.ID,
                            WeaponID = GetWeaponID(player.Style),
                            BaseDamage = dame,
                            BaseGuard = guard,
                            BaseEnergy = (int) (1.0 - (player.Agility * 0.001)),
                            signMsg = LanguageMgr.GetTranslation("RingStation.signMsg", new object[0]),
                            ChallengeNum = _ringstationConfigInfo.ChallengeNum,
                            buyCount = _ringstationConfigInfo.buyCount,
                            ChallengeTime = DateTime.Now,
                            LastDate = DateTime.Now,
                            Info = player
                        };
                        dictionary_1.Add(player.ID, info2);
                    }
                }
            }
        }

        public static UserRingStationInfo[] LoadUserRingStationDb()
        {
            using (new PlayerBussiness())
            {
                return new List<UserRingStationInfo>().ToArray();
            }
        }

        public static Dictionary<int, UserRingStationInfo> LoadUserRingStations(UserRingStationInfo[] UserRingStation)
        {
            Dictionary<int, UserRingStationInfo> dictionary = new Dictionary<int, UserRingStationInfo>();
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                int num;
                for (int i = 0; i < UserRingStation.Length; i = num + 1)
                {
                    UserRingStationInfo info = UserRingStation[i];
                    if (!dictionary.Keys.Contains<int>(info.UserID))
                    {
                        info.Info = bussiness.GetUserSingleByUserID(info.UserID);
                        if (info.Info != null)
                        {
                            info.WeaponID = GetWeaponID(info.Info.Style);
                            dictionary.Add(info.UserID, info);
                        }
                    }
                    num = i;
                }
            }
            return dictionary;
        }

        public static bool ReLoadBattleField()
        {
            bool flag2;
            try
            {
                RingstationBattleFieldInfo[] ringstationBattleField = LoadRingstationBattleFieldDb();
                Dictionary<int, List<RingstationBattleFieldInfo>> dictionary = LoadRingstationBattleFields(ringstationBattleField);
                if (ringstationBattleField.Length != 0)
                {
                    Interlocked.Exchange<Dictionary<int, List<RingstationBattleFieldInfo>>>(ref dictionary_2, dictionary);
                }
                return true;
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ReLoad RingstationBattleField", exception);
                }
                flag2 = false;
            }
            return flag2;
        }

        public static bool ReLoadUserRingStation()
        {
            bool flag2;
            try
            {
                UserRingStationInfo[] userRingStation = LoadUserRingStationDb();
                Dictionary<int, UserRingStationInfo> dictionary = LoadUserRingStations(userRingStation);
                if (userRingStation.Length != 0)
                {
                    Interlocked.Exchange<Dictionary<int, UserRingStationInfo>>(ref dictionary_1, dictionary);
                    IEnumerable<UserRingStationInfo> source = userRingStation;
                    if (func_1 == null)
                    {
                        func_1 = new Func<UserRingStationInfo, bool>(RingStationMgr.smethod_1);
                    }
                    IEnumerable<UserRingStationInfo> enumerable2 = source.Where<UserRingStationInfo>(func_1);
                    if (func_2 == null)
                    {
                        func_2 = new Func<UserRingStationInfo, int>(RingStationMgr.smethod_2);
                    }
                    list_1 = enumerable2.OrderBy<UserRingStationInfo, int>(func_2).ToList<UserRingStationInfo>();
                }
                return true;
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ReLoad All UserRingStation", exception);
                }
                flag2 = false;
            }
            return flag2;
        }

        public static bool SetupVirtualPlayer()
        {
            int num2;
            int[] numArray = new int[] { 
                0x1b67, 0x1b68, 0x1b69, 0x1b6a, 0x1b6b, 0x1b6c, 0x1b6d, 0x1b6e, 0x1b6f, 0x1b72, 0x1b73, 0x1b74, 0x1b77, 0x1b78, 0x1b77, 0x1b86, 
                0x1b87, 0x1b88, 0x1b89, 0x1b8a, 0x1b8b, 0x1b8d, 0x1b92, 0x1b93, 0x1b94
             };
            int[] numArray2 = new int[] { 
                0x5eb, 0x5ed, 0x5f2, 0x5f3, 0x5f4, 0x5f5, 0x5f8, 0x603, 0x606, 0x608, 0x609, 0x60e, 0x60f, 0x610, 0x613, 0x628, 
                0x629, 0x62a, 0x62e, 0x634, 0x638, 0x639, 0x63a, 0x63b, 0x63c, 0x63e
             };
            int[] numArray3 = new int[] { 
                0x914, 0x91c, 0x91e, 0x91f, 0x920, 0x921, 0x922, 0x929, 0x92c, 0x932, 0x935, 0x960, 0x961, 0x962, 0x963, 0x964, 
                0x965, 0x967, 0x968, 0x969, 0x96d, 0x970, 0x971, 0x974, 0x976, 0x977
             };
            int[] numArray4 = new int[] { 
                0xced, 0xcee, 0xcef, 0xcf0, 0xcf2, 0xcf3, 0xcf4, 0xcf5, 0xcf6, 0xcf7, 0xcf8, 0xcf9, 0xcfa, 0xcfb, 0xcfc, 0xcfd, 
                0xcfe, 0xd02, 0xd03, 0xd05, 0xd06, 0xd07, 0xd08, 0xd09, 0xd0a, 0xd0b
             };
            int[] numArray5 = new int[] { 
                0x10e1, 0x10e3, 0x10e7, 0x10e9, 0x10ec, 0x10f2, 0x10f3, 0x10fb, 0x10ff, 0x1101, 0x1103, 0x1131, 0x1132, 0x1133, 0x1134, 0x1135, 
                0x1136, 0x1137, 0x1138, 0x1139, 0x113a, 0x113c, 0x113d, 0x113e, 0x113f, 0x1140
             };
            int[] numArray6 = new int[] { 
                0x15c1, 0x15c2, 0x15c4, 0x15c7, 0x15c8, 0x15c9, 0x15ca, 0x15cb, 0x15cc, 0x15ce, 0x15cf, 0x15d0, 0x15d1, 0x15d2, 0x15d4, 0x15d5, 
                0x15d7, 0x15d8, 0x15da, 0x15dc, 0x15e0, 0x15e1, 0x15e2, 0x15e3, 0x15e4, 0x15e5
             };
            int[] numArray7 = new int[] { 
                0x186f, 0x1870, 0x1871, 0x1872, 0x1873, 0x1874, 0x1875, 0x1876, 0x1877, 0x1878, 0x1879, 0x187a, 0x187b, 0x187c, 0x187d, 0x187e, 
                0x187f, 0x1880, 0x1881, 0x1882, 0x1883, 0x1884, 0x1885, 0x1886, 0x1887, 0x1888
             };
            int[] numArray8 = new int[] { 
                0x3aa1, 0x3aaa, 0x3aab, 0x3aac, 0x3aad, 0x3aae, 0x3aaf, 0x3ab0, 0x3ab1, 0x3ab2, 0x3ab3, 0x3ab4, 0x3ab5, 0x3ac9, 0x3ab7, 0x3ab8, 
                0x3ab9, 0x3aba, 0x3abb, 0x3abc, 0x3abd, 0x3abe, 0x3abf, 0x3ac0, 0x3ac1, 0x3ac2
             };
            int length = numArray.Length;
            for (int i = 0; i < length; i = num2 + 1)
            {
                ItemTemplateInfo info = ItemMgr.FindItemTemplate(numArray[i]);
                ItemTemplateInfo info2 = ItemMgr.FindItemTemplate(numArray2[i]);
                ItemTemplateInfo info3 = ItemMgr.FindItemTemplate(numArray3[i]);
                ItemTemplateInfo info4 = ItemMgr.FindItemTemplate(numArray4[i]);
                ItemTemplateInfo info5 = ItemMgr.FindItemTemplate(numArray5[i]);
                ItemTemplateInfo info6 = ItemMgr.FindItemTemplate(numArray6[i]);
                ItemTemplateInfo info7 = ItemMgr.FindItemTemplate(numArray7[i]);
                ItemTemplateInfo info8 = ItemMgr.FindItemTemplate(numArray8[i]);
                if (((((info != null) && (info2 != null)) && ((info3 != null) && (info4 != null))) && (((info5 != null) && (info6 != null)) && (info7 != null))) && (info8 != null))
                {
                    string str = string.Format("{0}|{1}", numArray[i], info.Pic);
                    string str2 = string.Format("{0}|{1}", numArray2[i], info2.Pic);
                    string str3 = string.Format("{0}|{1}", numArray3[i], info3.Pic);
                    string str4 = string.Format("{0}|{1}", numArray4[i], info4.Pic);
                    string str5 = string.Format("{0}|{1}", numArray5[i], info5.Pic);
                    string str6 = string.Format("{0}|{1}", numArray6[i], info6.Pic);
                    string str7 = string.Format("{0}|{1}", numArray7[i], info7.Pic);
                    string str8 = string.Format("{0}|{1}", numArray8[i], info8.Pic);
                    string str9 = string.Format("{0},{1},{2},{3},{4},{5},{6},,{7},,,,,,,,,", new object[] { str2, str3, str4, str5, str6, str7, str, str8 });
                    VirtualPlayerInfo item = new VirtualPlayerInfo {
                        Style = str9,
                        Weapon = numArray[i]
                    };
                    list_0.Add(item);
                }
                num2 = i;
            }
            return (list_0.Count > Math.Abs((int) (length / 2)));
        }

        private static DateTime smethod_0(RingstationBattleFieldInfo ringstationBattleFieldInfo_0)
        {
            return ringstationBattleFieldInfo_0.BattleTime;
        }

        private static bool smethod_1(UserRingStationInfo userRingStationInfo_0)
        {
            return (userRingStationInfo_0.Rank != 0);
        }

        private static int smethod_2(UserRingStationInfo userRingStationInfo_0)
        {
            return userRingStationInfo_0.Rank;
        }

        private static int smethod_3(UserRingStationInfo userRingStationInfo_0)
        {
            return userRingStationInfo_0.Total;
        }

        private static bool smethod_4(UserRingStationInfo userRingStationInfo_0)
        {
            return (userRingStationInfo_0.Rank != 0);
        }

        private static int smethod_5(UserRingStationInfo userRingStationInfo_0)
        {
            return userRingStationInfo_0.Rank;
        }

        protected static void StatusScan(object sender)
        {
            try
            {
                log.Info("Begin Scan RingStation Info....");
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                bool flag = false;
                if (ReLoadUserRingStation())
                {
                    object obj4;
                    List<UserRingStationInfo> list = new List<UserRingStationInfo>();
                    foreach (UserRingStationInfo info in dictionary_1.Values)
                    {
                        list.Add(info);
                    }
                    if (_ringstationConfigInfo.IsFirstUpdateRank && (list.Count > int_0))
                    {
                        IEnumerable<UserRingStationInfo> enumerable = list;
                        if (func_3 == null)
                        {
                            func_3 = new Func<UserRingStationInfo, int>(RingStationMgr.smethod_3);
                        }
                        List<UserRingStationInfo> list2 = enumerable.OrderByDescending<UserRingStationInfo, int>(func_3).ToList<UserRingStationInfo>();
                        object @lock = m_lock;
                        lock ((obj4 = @lock))
                        {
                            int num2;
                            for (int i = 0; i < list2.Count; i = num2 + 1)
                            {
                                UserRingStationInfo ring = list2[i];
                                ring.Rank = i + 1;
                                UpdateRingStationInfo(ring);
                                num2 = i;
                            }
                        }
                        _ringstationConfigInfo.IsFirstUpdateRank = false;
                        flag = true;
                    }
                    IEnumerable<UserRingStationInfo> source = list;
                    if (func_4 == null)
                    {
                        func_4 = new Func<UserRingStationInfo, bool>(RingStationMgr.smethod_4);
                    }
                    IEnumerable<UserRingStationInfo> enumerable3 = source.Where<UserRingStationInfo>(func_4);
                    if (func_5 == null)
                    {
                        func_5 = new Func<UserRingStationInfo, int>(RingStationMgr.smethod_5);
                    }
                    list_1 = enumerable3.OrderBy<UserRingStationInfo, int>(func_5).ToList<UserRingStationInfo>();
                    if (list_1.Count > 0)
                    {
                        UserRingStationInfo info3 = list_1[0];
                        if (info3.Info != null)
                        {
                            _ringstationConfigInfo.ChampionText = info3.Info.NickName;
                            flag = true;
                        }
                    }
                    if (_ringstationConfigInfo.IsEndTime())
                    {
                        object obj3 = m_lock;
                        lock ((obj4 = obj3))
                        {
                            _ringstationConfigInfo.AwardTime = DateTime.Now;
                            _ringstationConfigInfo.AwardTime = DateTime.Now.AddDays(3.0);
                            flag = true;
                        }
                        if (list.Count > 0)
                        {
                            ItemTemplateInfo goods = ItemMgr.FindItemTemplate(-1000);
                            string translation = LanguageMgr.GetTranslation("RingStation.RankAward", new object[0]);
                            if (goods != null)
                            {
                                foreach (UserRingStationInfo info5 in list)
                                {
                                    int num4 = _ringstationConfigInfo.AwardNumByRank(info5.Rank);
                                    List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
                                    if (num4 > 0)
                                    {
                                        SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x66);
                                        item.Count = num4;
                                        item.ValidDate = 0;
                                        item.IsBinds = true;
                                        infos.Add(item);
                                        if (WorldEventMgr.SendItemsToMail(infos, info5.UserID, info5.Info.NickName, translation))
                                        {
                                            GamePlayer playerById = WorldMgr.GetPlayerById(info5.UserID);
                                            if (playerById != null)
                                            {
                                                playerById.Out.SendMailResponse(info5.UserID, eMailRespose.Receiver);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                    }
                }
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
                log.Info("End Scan RingStation Info....");
            }
            catch (Exception exception)
            {
                log.Error("StatusScan ", exception);
            }
        }

        public static bool UpdateRingStationFight(UserRingStationInfo ring)
        {
            bool flag2;
            if (ring == null)
            {
                flag2 = false;
            }
            else
            {
                object @lock = m_lock;
                lock (@lock)
                {
                    if (dictionary_1.ContainsKey(ring.UserID))
                    {
                        dictionary_1[ring.UserID] = ring;
                        return true;
                    }
                }
                flag2 = false;
            }
            return flag2;
        }

        public static bool UpdateRingStationInfo(UserRingStationInfo ring)
        {
            if (ring != null)
            {
                using (new PlayerBussiness())
                {
                    object @lock = m_lock;
                    lock (@lock)
                    {
                        if (dictionary_1.ContainsKey(ring.UserID))
                        {
                            dictionary_1[ring.UserID] = ring;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static RingstationConfigInfo ConfigInfo
        {
            get
            {
                return _ringstationConfigInfo;
            }
        }

        public static VirtualPlayerInfo NormalPlayer
        {
            get
            {
                return _virtualPlayerInfo;
            }
            set
            {
                _virtualPlayerInfo = value;
            }
        }
    }
}


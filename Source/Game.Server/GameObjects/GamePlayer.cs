namespace Game.Server.GameObjects
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using Game.Server;
    using Game.Server.Achievement;
    using Game.Server.Buffer;
    using Game.Server.GameUtils;
    using Game.Server.HotSpringRooms;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using Game.Server.Quests;
    using Game.Server.Rooms;
    using Game.Server.SceneMarryRooms;
    using Game.Server.Statics;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class GamePlayer : IGamePlayer
    {
        private Dictionary<int, int> _friends;
        private List<int> _viFarms;
        public DateTime BossBoxStartTime;
        public int canTakeOut;
        public Dictionary<int, CardInfo> Card = new Dictionary<int, CardInfo>();
        public CardInfo[] CardsTakeOut = new CardInfo[9];
        public int CurrentRoomIndex;
        public int CurrentRoomTeam;
        public int FightPower;
        public double GPAddPlus;
        public double GuildRichAddPlus = 1.0;
        public int Hot_Direction;
        public int Hot_X;
        public int Hot_Y;
        public int HotMap;
        private HotSpringRoom hotSpringRoom_0;
        public bool IsInChristmasRoom;
        public bool IsInWorldBossRoom;
        public bool isPowerFullUsed;
        public bool KickProtect;
        public readonly string[] labyrinthGolds = new string[] { 
            "0|0", "2|2", "0|0", "2|2", "0|0", "2|3", "0|0", "3|3", "0|0", "3|4", "0|0", "3|4", "0|0", "4|5", "0|0", "4|5", 
            "0|0", "4|6", "0|0", "5|6", "0|0", "5|7", "0|0", "5|7", "0|0", "6|8", "0|0", "6|8", "0|0", "6|10", "0|0", "8|10", 
            "0|0", "8|11", "0|0", "8|11", "0|0", "10|12", "0|0", "10|12"
         };
        public DateTime LastAttachMail;
        public DateTime LastChatTime;
        public DateTime LastDrillUpTime;
        public DateTime LastEnterWorldBoss;
        public DateTime LastFigUpTime;
        public DateTime LastOpenCard;
        public DateTime LastOpenChristmasPackage;
        public DateTime LastOpenGrowthPackage;
        public DateTime LastOpenPack;
        public DateTime LastOpenYearMonterPackage;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string m_account;
        private PlayerActives m_actives;
        private Game.Server.Achievement.AchievementInventory m_achievementInventory;
        private PlayerAvatarCollection m_avatarcollect;
        private PlayerBattle m_battle;
        private PlayerBeadInventory m_BeadBag;
        private Game.Server.Buffer.BufferList m_bufferList;
        private PlayerInventory m_caddyBag;
        private CardInventory m_cardBag;
        protected GameClient m_client;
        private PlayerInventory m_ConsortiaBag;
        private UTF8Encoding m_converter;
        private MarryRoom m_currentMarryRoom;
        private BaseRoom m_currentRoom;
        private SqlDataProvider.Data.ItemInfo m_currentSecondWeapon;
        private BaseSevenDoubleRoom m_currentSevenDoubleRoom;
        private int m_changed;
        private PlayerInfo m_character;
        private PlayerDice m_dice;
        private PlayerDressModel m_dressmodel;
        private PlayerEquipInventory m_equipBag;
        private List<SqlDataProvider.Data.ItemInfo> m_equipEffect;
        private PlayerExtra m_extra;
        private PlayerFarm m_farm;
        private PlayerInventory m_farmBag;
        private PlayerInventory m_fightBag;
        private List<BufferInfo> m_fightBuffInfo;
        private PlayerInventory m_food;
        protected BaseGame m_game;
        private List<UserGemStone> m_GemStone;
        private SqlDataProvider.Data.ItemInfo m_healstone;
        private int m_immunity = 0xff;
        private bool m_isAASInfo;
        private bool m_isMinor;
        private UserLabyrinthInfo m_Labyrinth;
        private PlayerMagicStoneInventory m_magicStoneBag;
        private SqlDataProvider.Data.ItemInfo m_MainWeapon;
        private UsersPetinfo m_pet;
        private PetInventory m_petBag;
        private PlayerInventory m_petEgg;
        private long m_pingTime;
        private int m_playerId;
        private PlayerProperty m_playerProp;
        protected Player m_players;
        private ePlayerState m_playerState;
        private PlayerInventory m_propBag;
        private char[] m_pvepermissions;
        private Game.Server.Quests.QuestInventory m_questInventory;
        private PlayerRank m_rank;
        private bool m_showPP;
        private PlayerInventory m_storeBag;
        private PlayerInventory m_tempBag;
        private Dictionary<string, object> m_tempProperties = new Dictionary<string, object>();
        public bool m_toemview;
        private PlayerTreasure m_treasure;
        private Dictionary<int, UserDrillInfo> m_userDrills;
        private PlayerInventory m_vegetable;
        public int MarryMap;
        public double OfferAddPlus = 1.0;
        private static char[] permissionChars = new char[] { '1', '3', '7', 'F' };
        public long PingStart;
        public byte States;
        private static readonly int[] StyleIndex = new int[] { 1, 2, 3, 4, 5, 6, 11, 13, 14, 15, 0x10, 0x11, 0x12, 0x13, 20 };
        public int takeoutCount;
        public int winningStreak;
        public int WorldBossMap;
        public int X;
        public int Y;

        public event PlayerAchievementFinish AchievementFinishEvent;

        public event PlayerAdoptPetEventHandle AdoptPetEvent;

        public event PlayerGameKillBossEventHandel AfterKillingBoss;

        public event PlayerGameKillEventHandel AfterKillingLiving;

        public event PlayerItemPropertyEventHandle AfterUsingItem;

        public event PlayerCropPrimaryEventHandle CropPrimaryEvent;

        public event PlayerEnterHotSpring EnterHotSpringEvent;

        public event PlayerVIPUpgrade Event_0;

        public event PlayerFightAddOffer FightAddOfferEvent;

        public event PlayerFightOneBloodIsWin FightOneBloodIsWin;

        public event GameKillDropEventHandel GameKillDrop;

        public event PlayerGameOverEventHandle GameOver;

        public event PlayerGoldCollection GoldCollect;

        public event PlayerOwnConsortiaEventHandle GuildChanged;

        public event PlayerGiftTokenCollection GiftTokenCollect;

        public event PlayerHotSpingExpAdd HotSpingExpAdd;

        public event PlayerItemComposeEventHandle ItemCompose;

        public event PlayerItemFusionEventHandle ItemFusion;

        public event PlayerItemInsertEventHandle ItemInsert;

        public event PlayerItemMeltEventHandle ItemMelt;

        public event PlayerItemStrengthenEventHandle ItemStrengthen;

        public event PlayerEventHandle LevelUp;

        public event PlayerMissionFullOverEventHandle MissionFullOver;

        public event PlayerMissionOverEventHandle MissionOver;

        public event PlayerMissionTurnOverEventHandle MissionTurnOver;

        public event PlayerNewGearEventHandle NewGearEvent;

        public event PlayerNewGearEventHandle2 NewGearEvent2;

        public event PlayerShopEventHandle Paid;

        public event PlayerEventHandle PingTimeOnline;

        public event PlayerPropertisChange PropertisChange;

        public event PlayerQuestFinish QuestFinishEvent;

        public event PlayerSeedFoodPetEventHandle SeedFoodPetEvent;

        public event PlayerUnknowQuestConditionEventHandle UnknowQuestConditionEvent;

        public event PlayerUpLevelPetEventHandle UpLevelPetEvent;

        public event PlayerEventHandle UseBuffer;

        public event PlayerUseBugle UseBugle;

        public event PlayerUserToemGemstoneEventHandle UserToemGemstonetEvent;

        public GamePlayer(int playerId, string account, GameClient client, PlayerInfo info)
        {
            this.m_playerId = playerId;
            this.m_account = account;
            this.m_client = client;
            this.m_character = info;
            this.m_equipBag = new PlayerEquipInventory(this);
            this.m_BeadBag = new PlayerBeadInventory(this);
            this.m_magicStoneBag = new PlayerMagicStoneInventory(this);
            this.m_propBag = new PlayerInventory(this, true, 0x31, 1, 0, true);
            this.m_ConsortiaBag = new PlayerInventory(this, true, 100, 11, 0, true);
            this.m_storeBag = new PlayerInventory(this, true, 20, 12, 0, true);
            this.m_fightBag = new PlayerInventory(this, false, 3, 3, 0, false);
            this.m_tempBag = new PlayerInventory(this, false, 100, 4, 0, true);
            this.m_caddyBag = new PlayerInventory(this, false, 30, 5, 0, true);
            this.m_farmBag = new PlayerInventory(this, true, 30, 13, 0, true);
            this.m_vegetable = new PlayerInventory(this, true, 30, 14, 0, true);
            this.m_food = new PlayerInventory(this, true, 30, 0x22, 0, true);
            this.m_petEgg = new PlayerInventory(this, true, 30, 0x23, 0, true);
            this.m_cardBag = new CardInventory(this, true, 100, 0);
            this.m_farm = new PlayerFarm(this, true, 30, 0);
            this.m_petBag = new PetInventory(this, true, 10, 8, 0);
            this.m_treasure = new PlayerTreasure(this, true);
            this.m_rank = new PlayerRank(this, true);
            this.m_dressmodel = new PlayerDressModel(this, true);
            this.m_avatarcollect = new PlayerAvatarCollection(this, true);
            this.m_playerProp = new PlayerProperty(this);
            this.m_dice = new PlayerDice(this, true);
            this.m_battle = new PlayerBattle(this, true);
            this.m_actives = new PlayerActives(this, true);
            this.m_extra = new PlayerExtra(this, true);
            this.m_questInventory = new Game.Server.Quests.QuestInventory(this);
            this.m_achievementInventory = new Game.Server.Achievement.AchievementInventory(this);
            this.m_bufferList = new Game.Server.Buffer.BufferList(this);
            this.m_fightBuffInfo = new List<BufferInfo>();
            this.m_equipEffect = new List<SqlDataProvider.Data.ItemInfo>();
            this.m_GemStone = new List<UserGemStone>();
            this.m_userDrills = new Dictionary<int, UserDrillInfo>();
            this.m_Labyrinth = null;
            this.GPAddPlus = 1.0;
            this.m_toemview = true;
            this.X = 0x286;
            this.Y = 0x4d9;
            this.MarryMap = 0;
            this.LastChatTime = DateTime.Today;
            this.LastFigUpTime = DateTime.Today;
            this.LastDrillUpTime = DateTime.Today;
            this.LastOpenPack = DateTime.Today;
            this.LastOpenGrowthPackage = DateTime.Now;
            this.LastOpenChristmasPackage = DateTime.Now;
            this.LastOpenYearMonterPackage = DateTime.Now;
            this.m_showPP = false;
            this.m_converter = new UTF8Encoding();
            this.BossBoxStartTime = DateTime.Now;
        }

        public bool ActiveMoneyEnable(int value)
        {
            if (!GameProperties.IsActiveMoney)
            {
                return this.MoneyDirect(value);
            }
            if ((value >= 1) && (value <= 0x7fffffff))
            {
                if (this.Actives.Info.ActiveMoney >= value)
                {
                    this.RemoveActiveMoney(value);
                    this.RemoveMoney(value);
                    return true;
                }
                this.SendMessage(string.Format("Xu năng động kh\x00f4ng đủ. Hiện tại bạn c\x00f3 {0} Xu năng động", this.Actives.Info.ActiveMoney));
            }
            return false;
        }

        public int AddActiveMoney(int value)
        {
            if ((value > 0) && GameProperties.IsActiveMoney)
            {
                ActiveSystemInfo info = this.Actives.Info;
                info.ActiveMoney += value;
                if (this.Actives.Info.ActiveMoney == -2147483648)
                {
                    this.Actives.Info.ActiveMoney = 0x7fffffff;
                    this.SendMessage("Xu năng động đ\x00e3 đạt gới hạn, kh\x00f4ng thể nhận th\x00eam.");
                    return value;
                }
                this.SendHideMessage(string.Format("Hệ thống vừa th\x00eam v\x00e0o {0} Xu năng động , n\x00e2ng Xu năng động l\x00ean {1} Xu", value, this.Actives.Info.ActiveMoney));
                return value;
            }
            return 0;
        }

        public int AddAchievementPoint(int value)
        {
            if (value > 0)
            {
                this.m_character.AchievementPoint += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public void AddBeadEffect(SqlDataProvider.Data.ItemInfo item)
        {
            this.m_equipEffect.Add(item);
        }

        public int AddCardSoul(int value)
        {
            if (value > 0)
            {
                this.m_character.CardSoul += value;
                if (this.m_character.CardSoul == -2147483648)
                {
                    this.m_character.CardSoul = 0x7fffffff;
                    this.SendMessage("Thẻ hồn đ\x00e3 đạt cảnh giới cao nhất, kh\x00f4ng thể nhận th\x00eam.");
                }
                return value;
            }
            return 0;
        }

        public int AddDamageScores(int value)
        {
            if (value > 0)
            {
                this.m_character.damageScores += value;
                if (this.m_character.damageScores == -2147483648)
                {
                    this.m_character.damageScores = 0x7fffffff;
                    this.SendMessage("T\x00edch lũy đ\x00e3 đạt cảnh giới cao nhất, kh\x00f4ng thể nhận th\x00eam.");
                }
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddDDPlayPoint(int value)
        {
            if (value > 0)
            {
                this.m_character.DDPlayPoint += value;
                return value;
            }
            return 0;
        }

        public void AddExpVip(int value)
        {
            List<int> list = GameProperties.VIPExp();
            if (value >= 100)
            {
                this.m_character.VIPExp += value / 100;
            }
            for (int i = 0; i < list.Count; i++)
            {
                int vIPExp = this.m_character.VIPExp;
                int vIPLevel = this.m_character.VIPLevel;
                if (vIPLevel == 12)
                {
                    this.m_character.VIPExp = list[11];
                    break;
                }
                if ((vIPLevel < 12) && this.canUpLv(vIPExp, vIPLevel))
                {
                    this.m_character.VIPLevel++;
                }
            }
            if (this.m_character.IsVIPExpire())
            {
                this.Out.SendOpenVIP(this.PlayerCharacter);
            }
        }

        public int AddGold(int value)
        {
            if (value > 0)
            {
                this.m_character.Gold += value;
                if (this.m_character.Gold == -2147483648)
                {
                    this.m_character.Gold = 0x7fffffff;
                    this.SendMessage("V\x00e0ng đ\x00e3 đạt gới hạn, kh\x00f4ng thể nhận th\x00eam.");
                }
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddGoXu(int value)
        {
            if (value > 0)
            {
                this.m_character.GoXu += value;
                this.SendMessage(string.Concat(new object[] { "Bạn được nhận th\x00eam ", value, " GoXu. Tổng GoXu: ", this.m_character.GoXu }));
                return value;
            }
            return 0;
        }

        public int AddGP(int gp)
        {
            if (gp >= 0)
            {
                if (AntiAddictionMgr.ISASSon)
                {
                    gp = (int) (gp * AntiAddictionMgr.GetAntiAddictionCoefficient(this.PlayerCharacter.AntiAddiction));
                }
                gp = (int) (gp * RateMgr.GetRate(eRateType.Experience_Rate));
                if (this.GPAddPlus > 0.0)
                {
                    gp = (int) (gp * this.GPAddPlus);
                }
                this.m_character.GP += gp;
                if (this.m_character.GP < 1)
                {
                    this.m_character.GP = 1;
                }
                this.Level = LevelMgr.GetLevel(this.m_character.GP);
                int maxLevel = LevelMgr.MaxLevel;
                LevelInfo info = LevelMgr.FindLevel(maxLevel);
                if ((this.Level == maxLevel) && (info != null))
                {
                    this.m_character.GP = info.GP;
                    int num2 = gp / 100;
                    if (num2 > 0)
                    {
                        this.AddOffer(num2);
                        this.SendHideMessage(string.Format("Max level kinh nghiệm quy đổi th\x00e0nh {0} c\x00f4ng trạng", num2));
                    }
                }
                this.UpdateFightPower();
                this.OnPropertiesChanged();
                return gp;
            }
            return 0;
        }

        public void AddGift(eGiftType type)
        {
            string[] strArray;
            ItemTemplateInfo info;
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            bool testActive = GameProperties.TestActive;
            switch (type)
            {
                case eGiftType.MONEY:
                    if (testActive)
                    {
                        this.AddMoney(GameProperties.FreeMoney);
                    }
                    break;

                case eGiftType.SMALL_EXP:
                    strArray = GameProperties.FreeExp.Split(new char[] { '|' });
                    info = ItemMgr.FindItemTemplate(Convert.ToInt32(strArray[0]));
                    if (info != null)
                    {
                        list.Add(SqlDataProvider.Data.ItemInfo.CreateFromTemplate(info, Convert.ToInt32(strArray[1]), 0x66));
                    }
                    break;

                case eGiftType.BIG_EXP:
                    strArray = GameProperties.BigExp.Split(new char[] { '|' });
                    info = ItemMgr.FindItemTemplate(Convert.ToInt32(strArray[0]));
                    if ((info != null) && testActive)
                    {
                        list.Add(SqlDataProvider.Data.ItemInfo.CreateFromTemplate(info, Convert.ToInt32(strArray[1]), 0x66));
                    }
                    break;

                case eGiftType.PET_EXP:
                    strArray = GameProperties.PetExp.Split(new char[] { '|' });
                    info = ItemMgr.FindItemTemplate(Convert.ToInt32(strArray[0]));
                    if ((info != null) && testActive)
                    {
                        list.Add(SqlDataProvider.Data.ItemInfo.CreateFromTemplate(info, Convert.ToInt32(strArray[1]), 0x66));
                    }
                    break;
            }
            foreach (SqlDataProvider.Data.ItemInfo info2 in list)
            {
                info2.IsBinds = true;
                this.AddTemplate(info2, info2.Template.BagType, info2.Count, eGameView.dungeonTypeGet);
            }
        }

        public int AddGiftToken(int value)
        {
            if (value > 0)
            {
                this.m_character.GiftToken += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddHardCurrency(int value)
        {
            if (value > 0)
            {
                this.m_character.hardCurrency += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddHonor(int value)
        {
            if (value > 0)
            {
                this.m_character.myHonor += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public bool AddItem(SqlDataProvider.Data.ItemInfo item)
        {
            AbstractInventory itemInventory = this.GetItemInventory(item.Template);
            return itemInventory.AddItem(item, itemInventory.BeginSlot);
        }

        public int AddLeagueMoney(int value)
        {
            if (value > 0)
            {
                this.m_character.LeagueMoney += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public void AddLog(string type, string content)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                bussiness.AddUserLogEvent(this.PlayerCharacter.ID, this.PlayerCharacter.UserName, this.PlayerCharacter.NickName, type, content);
            }
        }

        public int AddMagicStonePoint(int value)
        {
            if (value > 0)
            {
                this.m_character.MagicStonePoint += value;
                this.Out.SendMagicStonePoint(this.PlayerCharacter);
                return value;
            }
            return 0;
        }

        public int AddMaxHonor(int value)
        {
            if (value > 0)
            {
                this.m_character.MaxBuyHonor += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddMedal(int value)
        {
            if (value > 0)
            {
                this.m_character.medal += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddMissionEnergy(int value)
        {
            if (value > 0)
            {
                UsersExtraInfo info = this.Extra.Info;
                info.MissionEnergy += value;
                this.Out.SendMissionEnergy(this.Extra.Info);
                return value;
            }
            return 0;
        }

        public int AddMoney(int value)
        {
            if (value > 0)
            {
                this.m_character.Money += value;
                if (this.m_character.Money == -2147483648)
                {
                    this.m_character.Money = 0x7fffffff;
                    this.SendMessage("Xu đ\x00e3 đạt gới hạn, kh\x00f4ng thể nhận th\x00eam.");
                }
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddmyHonor(int value)
        {
            if (value > 0)
            {
                this.m_character.myHonor += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddOffer(int value)
        {
            return this.AddOffer(value, true);
        }

        public int AddOffer(int value, bool IsRate)
        {
            if (value > 0)
            {
                if (AntiAddictionMgr.ISASSon)
                {
                    value = (int) (value * AntiAddictionMgr.GetAntiAddictionCoefficient(this.PlayerCharacter.AntiAddiction));
                }
                if (IsRate)
                {
                    value *= (((int) this.OfferAddPlus) == 0) ? 1 : ((int) this.OfferAddPlus);
                }
                this.m_character.Offer += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddPetScore(int value)
        {
            if (value > 0)
            {
                this.m_character.petScore += value;
                if (this.m_character.petScore == -2147483648)
                {
                    this.m_character.petScore = 0x7fffffff;
                    this.SendMessage("T\x00edch lũy đ\x00e3 đạt cảnh giới cao nhất, kh\x00f4ng thể nhận th\x00eam.");
                }
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public void AddPrestige(bool isWin)
        {
            this.BattleData.AddPrestige(isWin);
        }

        public int AddRichesOffer(int value)
        {
            if (value > 0)
            {
                this.m_character.RichesOffer += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int AddRobRiches(int value)
        {
            if (value > 0)
            {
                if (AntiAddictionMgr.ISASSon)
                {
                    value = (int) (value * AntiAddictionMgr.GetAntiAddictionCoefficient(this.PlayerCharacter.AntiAddiction));
                }
                this.m_character.RichesRob += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public void AddRuneProperty(SqlDataProvider.Data.ItemInfo item, ref double defence, ref double attack)
        {
            RuneTemplateInfo info = RuneMgr.FindRuneByTemplateID(item.TemplateID);
            if (info != null)
            {
                string[] strArray = info.Attribute1.Split(new char[] { '|' });
                string[] strArray2 = info.Attribute2.Split(new char[] { '|' });
                int index = 0;
                int num2 = 0;
                if (item.Hole1 > info.BaseLevel)
                {
                    if (strArray.Length > 1)
                    {
                        index = 1;
                    }
                    if (strArray2.Length > 1)
                    {
                        num2 = 1;
                    }
                }
                int num3 = Convert.ToInt32(strArray[index]);
                Convert.ToInt32(strArray2[num2]);
                switch (info.Type1)
                {
                    case 0x23:
                        attack += num3;
                        break;

                    case 0x24:
                        defence += num3;
                        break;
                }
            }
        }

        public int AddScore(int value)
        {
            if (value > 0)
            {
                this.m_character.Score += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem)
        {
            return this.AddTemplate(cloneItem, cloneItem.Template.BagType, cloneItem.Count, eGameView.OtherTypeGet);
        }

        public bool AddTemplate(List<SqlDataProvider.Data.ItemInfo> infos)
        {
            return this.AddTemplate(infos, eGameView.OtherTypeGet);
        }

        public bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem, string name)
        {
            return this.AddTemplate(cloneItem, cloneItem.Template.BagType, cloneItem.Count, eGameView.OtherTypeGet, name);
        }

        public bool AddTemplate(List<SqlDataProvider.Data.ItemInfo> infos, eGameView typeGet)
        {
            if (infos != null)
            {
                List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
                foreach (SqlDataProvider.Data.ItemInfo info in infos)
                {
                    info.IsBinds = true;
                    if (!(this.StackItemToAnother(info) || this.AddItem(info)))
                    {
                        list.Add(info);
                    }
                }
                this.BagFullSendToMail(list);
                return true;
            }
            return false;
        }

        public bool AddTemplate(List<SqlDataProvider.Data.ItemInfo> infos, int count, eGameView gameView)
        {
            if (infos != null)
            {
                List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
                foreach (SqlDataProvider.Data.ItemInfo info in infos)
                {
                    info.IsBinds = true;
                    info.Count = count;
                    if (!(this.StackItemToAnother(info) || this.AddItem(info)))
                    {
                        list.Add(info);
                    }
                }
                this.BagFullSendToMail(list);
                return true;
            }
            return false;
        }

        public bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem, eBageType bagType, int count, eGameView gameView)
        {
            if (eBageType.FightBag == bagType)
            {
                return this.FightBag.AddItem(cloneItem);
            }
            return this.AddTemplate(cloneItem, bagType, count, gameView, "no");
        }

        public bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem, eBageType bagType, int count, eGameView gameView, string Name)
        {
            PlayerInventory inventory = this.GetInventory(bagType);
            if (cloneItem != null)
            {
                List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
                if (!(inventory.StackItemToAnother(cloneItem) || inventory.AddItem(cloneItem)))
                {
                    infos.Add(cloneItem);
                }
                this.BagFullSendToMail(infos);
                if (Name != "no")
                {
                    this.SendItemNotice(cloneItem, (int) gameView, Name);
                }
                return true;
            }
            return false;
        }

        public int AddTotem(int value)
        {
            if (value > 0)
            {
                this.m_character.totemId += value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public void ApertureEquip(int level)
        {
            this.EquipShowImp(0, (level < 5) ? 1 : ((level < 7) ? 2 : 3));
        }

        public void BagFullSendToMail(List<SqlDataProvider.Data.ItemInfo> infos)
        {
            if (infos.Count > 0)
            {
                if (GameProperties.BagMailEnable)
                {
                    WorldMgr.AddItemToMailBag(this.m_character.ID, infos);
                    this.SendMessage("H\x00e0nh trang đ\x00e3 đầy, vật phẩm chuyển v\x00e0o -T\x00fai ẩn- Vui l\x00f2ng chờ 15p để nhận lại vật phẩm qua thư.");
                }
                else
                {
                    bool flag = false;
                    using (new PlayerBussiness())
                    {
                        flag = this.SendItemsToMail(infos, "", "H\x00e0nh trang đ\x00e3 đầy. Gửi v\x00e0o thư.", eMailType.BuyItem);
                    }
                    if (flag)
                    {
                        this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                }
            }
        }

        public void BeginAllChanges()
        {
            this.BeginChanges();
            this.m_bufferList.BeginChanges();
            this.m_equipBag.BeginChanges();
            this.m_propBag.BeginChanges();
            this.m_BeadBag.BeginChanges();
            this.m_magicStoneBag.BeginChanges();
            this.m_farmBag.BeginChanges();
            this.m_vegetable.BeginChanges();
            this.m_cardBag.BeginChanges();
        }

        public void BeginChanges()
        {
            Interlocked.Increment(ref this.m_changed);
        }

        public void CalculatorClearnOutLabyrinth()
        {
            if (this.m_Labyrinth != null)
            {
                int num = 0;
                for (int i = this.m_Labyrinth.currentFloor; i <= this.m_Labyrinth.myProgress; i++)
                {
                    num += 2;
                }
                num *= 60;
                this.m_Labyrinth.remainTime = num;
                this.m_Labyrinth.currentRemainTime = num;
                this.m_Labyrinth.cleanOutAllTime = num;
            }
        }

        public bool CanEquip(ItemTemplateInfo item)
        {
            bool flag = true;
            string message = "";
            if (!item.CanEquip)
            {
                flag = false;
                message = LanguageMgr.GetTranslation("Game.Server.GameObjects.NoEquip", new object[0]);
            }
            else if (this.m_character.Grade < item.NeedLevel)
            {
                flag = false;
                message = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanLevel", new object[0]);
            }
            if (!flag)
            {
                this.Out.SendMessage(eMessageType.ERROR, message);
            }
            return flag;
        }

        public bool canUpLv(int exp, int _curLv)
        {
            List<int> list = GameProperties.VIPExp();
            return ((((((exp >= list[0]) && (_curLv == 0)) || ((exp >= list[1]) && (_curLv == 1))) || (((exp >= list[2]) && (_curLv == 2)) || ((exp >= list[3]) && (_curLv == 3)))) || ((((exp >= list[4]) && (_curLv == 4)) || ((exp >= list[5]) && (_curLv == 5))) || (((exp >= list[6]) && (_curLv == 6)) || ((exp >= list[7]) && (_curLv == 7))))) || ((exp >= list[8]) && (_curLv == 8)));
        }

        public void ClearCaddyBag()
        {
            List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
            for (int i = 0; i < this.CaddyBag.Capalility; i++)
            {
                SqlDataProvider.Data.ItemInfo itemAt = this.CaddyBag.GetItemAt(i);
                if (itemAt != null)
                {
                    SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(itemAt.Template, itemAt);
                    item.Count = 1;
                    infos.Add(item);
                }
            }
            this.CaddyBag.ClearBag();
            this.AddTemplate(infos);
        }

        public void ClearConsortia()
        {
            this.PlayerCharacter.ClearConsortia();
            this.OnPropertiesChanged();
            this.QuestInventory.ClearConsortiaQuest();
            string translation = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender", new object[0]);
            string title = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]);
            this.ConsortiaBag.SendAllItemsToMail(translation, title, eMailType.StoreCanel);
        }

        public bool ClearFightBag()
        {
            this.FightBag.ClearBag();
            return true;
        }

        public void ClearFightBuffOneMatch()
        {
            List<BufferInfo> list = new List<BufferInfo>();
            foreach (BufferInfo info in this.FightBuffs)
            {
                if (((info != null) && (info.Type >= 400)) && (info.Type <= 0x196))
                {
                    list.Add(info);
                }
            }
            foreach (BufferInfo info2 in list)
            {
                this.FightBuffs.Remove(info2);
            }
            list.Clear();
        }

        public void ClearFootballCard()
        {
            for (int i = 0; i < this.CardsTakeOut.Length; i++)
            {
                this.CardsTakeOut[i] = null;
            }
        }

        public void ClearStoreBag()
        {
            for (int i = 0; i < this.StoreBag.Capalility; i++)
            {
                SqlDataProvider.Data.ItemInfo itemAt = this.StoreBag.GetItemAt(i);
                if (itemAt != null)
                {
                    int num2;
                    if (itemAt.Template.BagType == eBageType.PropBag)
                    {
                        num2 = this.PropBag.FindFirstEmptySlot();
                        if (this.PropBag.StackItemToAnother(itemAt) || this.PropBag.AddItemTo(itemAt, num2))
                        {
                            this.StoreBag.TakeOutItem(itemAt);
                        }
                    }
                    else
                    {
                        num2 = this.EquipBag.FindFirstEmptySlot(0x1f);
                        if (this.EquipBag.StackItemToAnother(itemAt) || this.EquipBag.AddItemTo(itemAt, num2))
                        {
                            this.StoreBag.TakeOutItem(itemAt);
                        }
                    }
                }
            }
            if (this.StoreBag.GetItems().Count > 0)
            {
                this.StoreBag.SendAllItemsToMail("Hệ thống", "Vật phẩm trả về từ Tiệm r\x00e8n.", eMailType.StoreCanel);
            }
        }

        public bool ClearTempBag()
        {
            this.TempBag.ClearBag();
            return true;
        }

        public void CommitAllChanges()
        {
            this.CommitChanges();
            this.m_bufferList.CommitChanges();
            this.m_equipBag.CommitChanges();
            this.m_propBag.CommitChanges();
            this.m_BeadBag.BeginChanges();
            this.m_magicStoneBag.BeginChanges();
            this.m_farmBag.CommitChanges();
            this.m_vegetable.CommitChanges();
            this.m_cardBag.CommitChanges();
        }

        public void CommitChanges()
        {
            Interlocked.Decrement(ref this.m_changed);
            this.OnPropertiesChanged();
        }

        public string CompleteGetAward(int floor)
        {
            string[] strArray = new string[floor];
            for (int i = 0; i < floor; i++)
            {
                strArray[i] = "i";
            }
            string[] strArray2 = this.m_Labyrinth.ProcessAward.Split(new char[] { '-' });
            string str = string.Join("-", strArray);
            for (int j = floor; j < strArray2.Length; j++)
            {
                str = str + "-" + strArray2[j];
            }
            return str;
        }

        public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
        {
            return ConsortiaMgr.ConsortiaFight(consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth, count);
        }

        public void ContinousVIP(int thoigian, DateTime ExpireDayOut)
        {
            int vIPLevel = this.m_character.VIPLevel;
            if ((vIPLevel < 6) && (thoigian == 180))
            {
                this.m_character.VIPLevel = 6;
                this.m_character.VIPExp = 0xfa0;
                this.m_character.VIPExpireDay = ExpireDayOut;
            }
            else if ((vIPLevel < 4) && (thoigian == 90))
            {
                this.m_character.VIPLevel = 4;
                this.m_character.VIPExp = 800;
                this.m_character.VIPExpireDay = ExpireDayOut;
            }
            else
            {
                this.m_character.VIPExpireDay = ExpireDayOut;
            }
        }

        public string ConverterPvePermission(char[] chArray)
        {
            string str = "";
            for (int i = 0; i < chArray.Length; i++)
            {
                str = str + chArray[i].ToString();
            }
            return str;
        }

        public List<SqlDataProvider.Data.ItemInfo> CopyDrop(int SessionId, int m_missionInfoId)
        {
            List<SqlDataProvider.Data.ItemInfo> info = null;
            DropInventory.CopyDrop(m_missionInfoId, SessionId, ref info);
            return info;
        }

        public void CreateDefaultDressModel()
        {
            List<SqlDataProvider.Data.ItemInfo> allEquipItems = this.EquipBag.GetAllEquipItems();
            if ((allEquipItems.Count > 0) && (this.DressModel.GetDressModelWithSlotID(this.PlayerCharacter.CurrentDressModel).Count <= 0))
            {
                foreach (SqlDataProvider.Data.ItemInfo info in allEquipItems)
                {
                    UserDressModelInfo dress = new UserDressModelInfo {
                        UserID = this.PlayerCharacter.ID,
                        ItemID = info.ItemID,
                        CategoryID = info.Template.CategoryID,
                        SlotID = this.PlayerCharacter.CurrentDressModel,
                        TemplateID = info.TemplateID
                    };
                    this.DressModel.AddDressModel(dress);
                }
            }
        }

        public int[] CreateExps()
        {
            int[] numArray = new int[40];
            int num = 660;
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = num;
                num += 690;
            }
            return numArray;
        }

        public string CreateFightFootballStyle()
        {
            SqlDataProvider.Data.ItemInfo itemAt = this.GetItemAt(eBageType.EquipBag, 0);
            string str = (itemAt == null) ? "" : (itemAt.TemplateID.ToString() + "|" + itemAt.Template.Pic);
            string str2 = str;
            string str3 = str;
            for (int i = 0; i < StyleIndex.Length; i++)
            {
                ItemTemplateInfo info2;
                str2 = str2 + ",";
                str3 = str3 + ",";
                if (StyleIndex[i] == 11)
                {
                    if (this.PlayerCharacter.Sex)
                    {
                        info2 = ItemMgr.FindItemTemplate(0x3505);
                        object obj2 = str2;
                        str2 = string.Concat(new object[] { obj2, info2.TemplateID, "|", info2.Pic });
                        info2 = ItemMgr.FindItemTemplate(0x3504);
                        object obj3 = str3;
                        str3 = string.Concat(new object[] { obj3, info2.TemplateID, "|", info2.Pic });
                    }
                    else
                    {
                        info2 = ItemMgr.FindItemTemplate(0x3507);
                        object obj4 = str2;
                        str2 = string.Concat(new object[] { obj4, info2.TemplateID, "|", info2.Pic });
                        info2 = ItemMgr.FindItemTemplate(0x3506);
                        object obj5 = str3;
                        str3 = string.Concat(new object[] { obj5, info2.TemplateID, "|", info2.Pic });
                    }
                }
                else if (StyleIndex[i] == 6)
                {
                    info2 = ItemMgr.FindItemTemplate(0x112fc);
                    string str4 = info2.TemplateID + "|" + info2.Pic;
                    str2 = str2 + str4;
                    str3 = str3 + str4;
                }
                else
                {
                    itemAt = this.GetItemAt(eBageType.EquipBag, StyleIndex[i]);
                    if (itemAt != null)
                    {
                        string str5 = itemAt.TemplateID + "|" + itemAt.Pic;
                        str2 = str2 + str5;
                        str3 = str3 + str5;
                    }
                }
            }
            return (str2 + ";" + str3);
        }

        public void ChargeToUser()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                int money = 0;
                string title = "Th\x00f4ng b\x00e1o nạp thẻ.";
                if (bussiness.ChargeToUser(this.m_character.UserName, ref money, this.m_character.NickName))
                {
                    bool flag2 = false;
                    if (GameProperties.IsDDTMoneyActive)
                    {
                        this.AddGiftToken(money);
                        string content = string.Format("Bạn vừa chuyển th\x00e0nh c\x00f4ng {0} Xu kh\x00f3a", money);
                        if (money > 0)
                        {
                            flag2 = this.SendMailToUser(bussiness, content, title, eMailType.Default);
                        }
                    }
                    else
                    {
                        this.AddMoney(money);
                    }
                    if (flag2)
                    {
                        this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                }
            }
        }

        public void CheckLevelMagicStone()
        {
            if ((this.MagicStoneBag.GetItems().Count > 0) && (this.PlayerCharacter.Grade < 40))
            {
                this.MagicStoneBag.ClearBag();
                this.SendMessage("Level bạn chưa đạt điều kiện nhận ma thạch. H\x00e3y l\x00ean lv 40 để mở chức năng ma thạch nh\x00e1!");
            }
        }

        public void ChecVipkExpireDay()
        {
            if (this.m_character.IsVIPExpire())
            {
                this.m_character.CanTakeVipReward = false;
            }
            else if (this.m_character.IsLastVIPPackTime())
            {
                this.m_character.CanTakeVipReward = true;
            }
            else
            {
                this.m_character.CanTakeVipReward = false;
            }
        }

        public bool DeletePropItem(int place)
        {
            this.FightBag.RemoveItemAt(place);
            return true;
        }

        public void Disconnect()
        {
            this.m_client.Disconnect();
        }

        public bool EquipItem(SqlDataProvider.Data.ItemInfo item, int place)
        {
            int num2;
            if (!(item.CanEquip() && (item.BagType == this.m_equipBag.BagType)))
            {
                return false;
            }
            int toSlot = this.m_equipBag.FindItemEpuipSlot(item.Template);
            switch (toSlot)
            {
                case 9:
                case 10:
                    num2 = (place == 9) ? 0 : ((place != 10) ? 1 : 0);
                    break;

                default:
                    num2 = 1;
                    break;
            }
            if (num2 == 0)
            {
                toSlot = place;
            }
            else if (((toSlot == 7) || (toSlot == 8)) && ((place == 7) || (place == 8)))
            {
                toSlot = place;
            }
            return this.m_equipBag.MoveItem(item.Place, toSlot, item.Count);
        }

        private void EquipShowImp(int categoryID, int para)
        {
            this.UpdateHide(this.m_character.Hide + ((int) (Math.Pow(10.0, (double) categoryID) * (para - ((this.m_character.Hide / ((int) Math.Pow(10.0, (double) categoryID))) % 10)))));
        }

        public bool FindEmptySlot(eBageType bagType)
        {
            PlayerInventory inventory = this.GetInventory(bagType);
            inventory.FindFirstEmptySlot();
            return (inventory.FindFirstEmptySlot() > 0);
        }

        public void FootballTakeOut(bool isWin)
        {
            if (isWin)
            {
                this.canTakeOut = 2;
                this.takeoutCount = 2;
            }
            else
            {
                this.canTakeOut = 1;
                this.takeoutCount = 1;
            }
        }

        public void FriendsAdd(int playerID, int relation)
        {
            if (!this._friends.ContainsKey(playerID))
            {
                this._friends.Add(playerID, relation);
            }
            else
            {
                this._friends[playerID] = relation;
            }
        }

        public void FriendsRemove(int playerID)
        {
            if (this._friends.ContainsKey(playerID))
            {
                this._friends.Remove(playerID);
            }
        }

        public double GetBaseAgility()
        {
            return (1.0 - (this.m_character.Agility * 0.001));
        }

        public double GetBaseAttack()
        {
            double num = 0.0;
            double defence = 0.0;
            double attack = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            UserRankInfo rank = this.Rank.GetRank(this.PlayerCharacter.Honor);
            if (rank != null)
            {
                num += rank.Damage;
            }
            List<SqlDataProvider.Data.ItemInfo> allEquipItems = this.m_equipBag.GetAllEquipItems();
            foreach (SqlDataProvider.Data.ItemInfo info2 in allEquipItems)
            {
                SubActiveConditionInfo subActiveInfo = SubActiveMgr.GetSubActiveInfo(info2);
                if (subActiveInfo != null)
                {
                    num += subActiveInfo.GetValue("6");
                }
            }
            this.PlayerProp.totalDamage = (int) num;
            for (int i = 0; i < 0x1f; i++)
            {
                SqlDataProvider.Data.ItemInfo item = this.m_BeadBag.GetItemAt(i);
                if (item != null)
                {
                    this.AddRuneProperty(item, ref defence, ref attack);
                }
            }
            this.PlayerProp.UpadateBaseProp(true, "Damage", "Bead", attack);
            this.PlayerProp.UpadateBaseProp(true, "Damage", "Suit", num4);
            List<UsersCardInfo> cards = this.m_cardBag.GetCards(0, 5);
            foreach (UsersCardInfo info5 in cards)
            {
                if (info5.CardID != 0)
                {
                    CardUpdateInfo info6 = CardMgr.FindCardTemplate(info5.TemplateID);
                    if (info6 != null)
                    {
                        num += info6.Damage;
                    }
                    num += info5.Damage;
                }
            }
            List<UserAvatarCollectionInfo> avatarPropertyActived = this.AvatarCollect.GetAvatarPropertyActived();
            if (avatarPropertyActived.Count > 0)
            {
                foreach (UserAvatarCollectionInfo info7 in avatarPropertyActived)
                {
                    ClothPropertyTemplateInfo clothProperty = info7.ClothProperty;
                    if (clothProperty != null)
                    {
                        int num7 = ClothGroupTemplateInfoMgr.CountClothGroupWithID(info7.AvatarID);
                        if ((info7.Items.Count >= (num7 / 2)) && (info7.Items.Count < num7))
                        {
                            num5 += clothProperty.Damage;
                        }
                        else if (info7.Items.Count == num7)
                        {
                            num5 += clothProperty.Damage * 2;
                        }
                    }
                }
            }
            this.PlayerProp.UpadateBaseProp(true, "Damage", "Avatar", num5);
            num += TotemMgr.GetTotemProp(this.m_character.totemId, "dam");
            SqlDataProvider.Data.ItemInfo itemAt = this.m_equipBag.GetItemAt(6);
            if (itemAt != null)
            {
                double num8 = itemAt.Template.Property7;
                int num9 = itemAt.IsGold ? 1 : 0;
                double num10 = (itemAt.StrengthenLevel + num9) + itemAt.LianGrade;
                num += this.getHertAddition(num8, num10) + num8;
            }
            return (((num + attack) + num4) + num5);
        }

        public double GetBaseBlood()
        {
            SqlDataProvider.Data.ItemInfo itemAt = this.EquipBag.GetItemAt(12);
            if (itemAt != null)
            {
                return (((100.0 + itemAt.Template.Property1) + this.PlayerCharacter.necklaceExpAdd) / 100.0);
            }
            return 1.0;
        }

        public double GetBaseDefence()
        {
            double num8;
            double num = 0.0;
            double defence = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double attack = 0.0;
            UserRankInfo rank = this.Rank.GetRank(this.PlayerCharacter.Honor);
            if (rank != null)
            {
                num += rank.Guard;
            }
            List<SqlDataProvider.Data.ItemInfo> allEquipItems = this.m_equipBag.GetAllEquipItems();
            foreach (SqlDataProvider.Data.ItemInfo info2 in allEquipItems)
            {
                SubActiveConditionInfo subActiveInfo = SubActiveMgr.GetSubActiveInfo(info2);
                if (subActiveInfo != null)
                {
                    num += subActiveInfo.GetValue("7");
                }
            }
            this.PlayerProp.totalArmor = (int) num;
            for (int i = 0; i < 0x1f; i++)
            {
                SqlDataProvider.Data.ItemInfo item = this.m_BeadBag.GetItemAt(i);
                if (item != null)
                {
                    this.AddRuneProperty(item, ref defence, ref attack);
                }
            }
            this.PlayerProp.UpadateBaseProp(true, "Armor", "Bead", defence);
            this.PlayerProp.UpadateBaseProp(true, "Armor", "Suit", num3);
            List<UserAvatarCollectionInfo> avatarPropertyActived = this.AvatarCollect.GetAvatarPropertyActived();
            if (avatarPropertyActived.Count > 0)
            {
                foreach (UserAvatarCollectionInfo info5 in avatarPropertyActived)
                {
                    ClothPropertyTemplateInfo clothProperty = info5.ClothProperty;
                    if (clothProperty != null)
                    {
                        int num7 = ClothGroupTemplateInfoMgr.CountClothGroupWithID(info5.AvatarID);
                        if ((info5.Items.Count >= (num7 / 2)) && (info5.Items.Count < num7))
                        {
                            num4 += clothProperty.Guard;
                        }
                        else if (info5.Items.Count == num7)
                        {
                            num4 += clothProperty.Guard * 2;
                        }
                    }
                }
            }
            this.PlayerProp.UpadateBaseProp(true, "Armor", "Avatar", num4);
            List<UsersCardInfo> cards = this.m_cardBag.GetCards(0, 5);
            foreach (UsersCardInfo info7 in cards)
            {
                if (info7.CardID > 0)
                {
                    CardUpdateInfo info8 = CardMgr.FindCardTemplate(info7.TemplateID);
                    if (info8 != null)
                    {
                        num += info8.Guard;
                    }
                    num += info7.Guard;
                }
            }
            num += TotemMgr.GetTotemProp(this.m_character.totemId, "gua");
            SqlDataProvider.Data.ItemInfo itemAt = this.m_equipBag.GetItemAt(0);
            if (itemAt != null)
            {
                num8 = itemAt.Template.Property7;
                int num9 = itemAt.IsGold ? 1 : 0;
                double num10 = (itemAt.StrengthenLevel + num9) + itemAt.LianGrade;
                num += this.getHertAddition(num8, num10) + num8;
            }
            SqlDataProvider.Data.ItemInfo info10 = this.m_equipBag.GetItemAt(4);
            if (info10 != null)
            {
                num8 = info10.Template.Property7;
                int num11 = info10.IsGold ? 1 : 0;
                double num12 = (info10.StrengthenLevel + num11) + info10.LianGrade;
                num += this.getHertAddition(num8, num12) + num8;
            }
            return (((num + defence) + num3) + num4);
        }

        public int GetDrillLevel(int place)
        {
            for (int i = 0; i < this.UserDrills.Count; i++)
            {
                if (this.UserDrills[i].BeadPlace == place)
                {
                    return this.UserDrills[i].HoleLv;
                }
            }
            return 0;
        }

        public string GetFightFootballStyle(int team)
        {
            if (team == 1)
            {
                return this.CreateFightFootballStyle().Split(new char[] { ';' })[0];
            }
            return this.CreateFightFootballStyle().Split(new char[] { ';' })[1];
        }

        public UserGemStone GetGemStone(int place)
        {
            foreach (UserGemStone stone in this.m_GemStone)
            {
                if (place == stone.EquipPlace)
                {
                    return stone;
                }
            }
            return null;
        }

        public double GetGoldBlood()
        {
            GoldEquipTemplateLoadInfo info3;
            SqlDataProvider.Data.ItemInfo itemAt = this.EquipBag.GetItemAt(0);
            SqlDataProvider.Data.ItemInfo info2 = this.EquipBag.GetItemAt(4);
            double num = 0.0;
            if (itemAt != null)
            {
                info3 = GoldEquipMgr.FindGoldEquipCategoryID(itemAt.Template.CategoryID);
                if (itemAt.IsGold)
                {
                    num += info3.Boold;
                }
            }
            if (info2 != null)
            {
                info3 = GoldEquipMgr.FindGoldEquipCategoryID(info2.Template.CategoryID);
                if (info2.IsGold)
                {
                    num += info3.Boold;
                }
            }
            return num;
        }

        public double getHertAddition(double para1, double para2)
        {
            double a = (para1 * Math.Pow(1.1, para2)) - para1;
            return Math.Round(a);
        }

        public PlayerInventory GetInventory(eBageType bageType)
        {
            if (bageType <= eBageType.BeadBag)
            {
                switch (bageType)
                {
                    case eBageType.EquipBag:
                        return this.m_equipBag;

                    case eBageType.PropBag:
                        return this.m_propBag;

                    case eBageType.FightBag:
                        return this.m_fightBag;

                    case eBageType.TempBag:
                        return this.m_tempBag;

                    case eBageType.CaddyBag:
                        return this.m_caddyBag;

                    case eBageType.Consortia:
                        return this.m_ConsortiaBag;

                    case eBageType.Store:
                        return this.m_storeBag;

                    case eBageType.FarmBag:
                        return this.m_farmBag;

                    case eBageType.Vegetable:
                        return this.m_vegetable;

                    case eBageType.BeadBag:
                        return this.m_BeadBag;
                }
            }
            else
            {
                switch (bageType)
                {
                    case eBageType.Food:
                        return this.m_food;

                    case eBageType.PetEgg:
                        return this.m_petEgg;

                    case eBageType.MagicStone:
                        return this.m_magicStoneBag;
                }
            }
            throw new NotSupportedException(string.Format("Did not support this type bag: {0} PlayerID: {1} Nickname: {2}", bageType, this.PlayerCharacter.ID, this.PlayerCharacter.NickName));
        }

        public string GetInventoryName(eBageType bageType)
        {
            switch (bageType)
            {
                case eBageType.EquipBag:
                    return LanguageMgr.GetTranslation("Game.Server.GameObjects.Equip", new object[0]);

                case eBageType.PropBag:
                    return LanguageMgr.GetTranslation("Game.Server.GameObjects.Prop", new object[0]);

                case eBageType.FightBag:
                    return LanguageMgr.GetTranslation("Game.Server.GameObjects.FightBag", new object[0]);

                case eBageType.FarmBag:
                    return LanguageMgr.GetTranslation("Game.Server.GameObjects.FarmBag", new object[0]);

                case eBageType.BeadBag:
                    return LanguageMgr.GetTranslation("Game.Server.GameObjects.BeadBag", new object[0]);
            }
            return bageType.ToString();
        }

        public SqlDataProvider.Data.ItemInfo GetItemAt(eBageType bagType, int place)
        {
            PlayerInventory inventory = this.GetInventory(bagType);
            if (inventory != null)
            {
                return inventory.GetItemAt(place);
            }
            return null;
        }

        public SqlDataProvider.Data.ItemInfo GetItemByItemID(int ItemID)
        {
            SqlDataProvider.Data.ItemInfo itemByItemID = this.GetInventory(eBageType.EquipBag).GetItemByItemID(0x1f, ItemID);
            if (itemByItemID == null)
            {
                itemByItemID = this.GetInventory(eBageType.PropBag).GetItemByItemID(0, ItemID);
            }
            if (itemByItemID == null)
            {
                itemByItemID = this.GetInventory(eBageType.Consortia).GetItemByItemID(0, ItemID);
            }
            return itemByItemID;
        }

        public SqlDataProvider.Data.ItemInfo GetItemByTemplateID(int templateID)
        {
            SqlDataProvider.Data.ItemInfo itemByTemplateID = this.GetInventory(eBageType.EquipBag).GetItemByTemplateID(0x1f, templateID);
            if (itemByTemplateID == null)
            {
                itemByTemplateID = this.GetInventory(eBageType.PropBag).GetItemByTemplateID(0, templateID);
            }
            if (itemByTemplateID == null)
            {
                itemByTemplateID = this.GetInventory(eBageType.Consortia).GetItemByTemplateID(0, templateID);
            }
            return itemByTemplateID;
        }

        public int GetItemCount(int templateId)
        {
            return ((this.m_propBag.GetItemCount(templateId) + this.m_equipBag.GetItemCount(templateId)) + this.m_ConsortiaBag.GetItemCount(templateId));
        }

        public PlayerInventory GetItemInventory(ItemTemplateInfo template)
        {
            return this.GetInventory(template.BagType);
        }

        public double getLianAddition(double para1, double para2)
        {
            return Math.Round((double) ((para1 * Math.Pow(1.1, para2)) - para1));
        }

        public void HideEquip(int categoryID, bool hide)
        {
            if ((categoryID >= 0) && (categoryID < 10))
            {
                this.EquipShowImp(categoryID, hide ? 2 : 1);
            }
        }

        public string InitProcessAward()
        {
            string[] strArray = new string[0x63];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = i.ToString();
            }
            this.ProcessLabyrinthAward = string.Join("-", strArray);
            return this.ProcessLabyrinthAward;
        }

        public char[] InitPvePermission()
        {
            char[] chArray = new char[50];
            for (int i = 0; i < chArray.Length; i++)
            {
                chArray[i] = '1';
            }
            return chArray;
        }

        public bool IsBlackFriend(int playerID)
        {
            return ((this._friends == null) || (this._friends.ContainsKey(playerID) && (this._friends[playerID] == 1)));
        }

        public bool IsConsortia()
        {
            return (ConsortiaMgr.FindConsortiaInfo(this.PlayerCharacter.ConsortiaID) != null);
        }

        public bool isDoubleAward()
        {
            return ((this.m_Labyrinth != null) && this.m_Labyrinth.isDoubleAward);
        }

        public bool IsLimitAuction()
        {
            if (GameProperties.IsLimitAuction)
            {
                if (this.Extra.Info.FreeAddAutionCount >= GameProperties.LimitAuction)
                {
                    this.SendMessage(string.Format("Số lần tham gia đấu gi\x00e1 h\x00f4m nay đ\x00e3 hết. Tối đa l\x00e0 {0}.", GameProperties.LimitAuction));
                    return true;
                }
                UsersExtraInfo info = this.Extra.Info;
                info.FreeAddAutionCount++;
            }
            return false;
        }

        public bool IsLimitCount(int count)
        {
            if (GameProperties.IsLimitCount && (count > GameProperties.LimitCount))
            {
                this.SendMessage(string.Format("Số lượng qu\x00e1 lớn hảy nhập lại. Số lượng tối đa l\x00e0 {0}.", GameProperties.LimitCount));
                return true;
            }
            return false;
        }

        public bool IsLimitMail()
        {
            if (GameProperties.IsLimitMail)
            {
                if (this.Extra.Info.FreeSendMailCount >= GameProperties.LimitMail)
                {
                    this.SendMessage(string.Format("Số lần gửi mail h\x00f4m nay đ\x00e3 hết. Tối đa l\x00e0 {0}.", GameProperties.LimitMail));
                    return true;
                }
                UsersExtraInfo info = this.Extra.Info;
                info.FreeSendMailCount++;
            }
            return false;
        }

        public bool IsLimitMoney(int count)
        {
            if (GameProperties.IsLimitMoney && (count > GameProperties.LimitMoney))
            {
                this.SendMessage(string.Format("Số lượng Xu qu\x00e1 lớn hảy nhập lại. Tối đa l\x00e0 {0}.", GameProperties.LimitMoney));
                return true;
            }
            return false;
        }

        public bool IsPveEpicPermission(int copyId)
        {
            string str = "1-2-3-4-5-6-7-8-9-10-11-12-13";
            if (str.Length > 0)
            {
                foreach (string str2 in str.Split(new char[] { '-' }))
                {
                    if (str2 == copyId.ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsPvePermission(int copyId, eHardLevel hardLevel)
        {
            if ((copyId > this.m_pvepermissions.Length) || (copyId <= 0))
            {
                return true;
            }
            if (hardLevel == eHardLevel.Epic)
            {
                return this.IsPveEpicPermission(copyId);
            }
            return (this.m_pvepermissions[copyId - 1] >= permissionChars[(int) hardLevel]);
        }

        public int LabyrinthTryAgainMoney()
        {
            for (int i = 0; i < this.Labyrinth.myProgress; i += 2)
            {
                if (this.Labyrinth.currentFloor == i)
                {
                    return GameProperties.WarriorFamRaidPriceBig;
                }
            }
            return GameProperties.WarriorFamRaidPriceSmall;
        }

        public void LastVIPPackTime()
        {
            this.m_character.LastVIPPackTime = DateTime.Now;
            this.m_character.CanTakeVipReward = false;
        }

        public void LoadDrills(PlayerBussiness db)
        {
            this.m_userDrills = db.GetPlayerDrillByID(this.m_character.ID);
            if (this.m_userDrills.Count == 0)
            {
                List<int> list = new List<int> { 7, 8, 9 };
                List<int> list2 = new List<int> { 0, 1, 2 };
                for (int i = 0; i < list.Count; i++)
                {
                    UserDrillInfo item = new UserDrillInfo {
                        UserID = this.m_character.ID,
                        BeadPlace = list[i],
                        HoleLv = 0,
                        HoleExp = 0,
                        DrillPlace = list2[i]
                    };
                    db.AddUserUserDrill(item);
                    if (!this.m_userDrills.ContainsKey(item.DrillPlace))
                    {
                        this.m_userDrills.Add(item.DrillPlace, item);
                    }
                }
            }
        }

        public bool LoadFromDatabase()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                PlayerInfo userSingleByUserID = bussiness.GetUserSingleByUserID(this.m_character.ID);
                if (userSingleByUserID == null)
                {
                    this.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid", new object[0]));
                    this.Client.Disconnect();
                    return false;
                }
                this.m_character = userSingleByUserID;
                this.m_character.Texp = bussiness.GetUserTexpInfoSingle(this.m_character.ID);
                if (this.m_character.Texp.IsValidadteTexp())
                {
                    this.m_character.Texp.texpCount = 0;
                }
                if (this.m_character.Grade > 0x13)
                {
                    this.LoadGemStone(bussiness);
                }
                this.LoadDrills(bussiness);
                this.ChargeToUser();
                int[] numArray2 = new int[3];
                numArray2[1] = 1;
                numArray2[2] = 2;
                int[] updatedSlots = numArray2;
                this.Out.SendUpdateInventorySlot(this.FightBag, updatedSlots);
                this.UpdateWeaklessGuildProgress();
                this.UpdateItemForUser(1);
                this.ChecVipkExpireDay();
                this.UpdateLevel();
                this.UpdatePet(this.m_petBag.GetPetIsEquip());
                if (this.m_character.IsValidadteTimeBox())
                {
                    this.m_character.TimeBox = DateTime.Now;
                    this.m_character.receiebox = 0;
                    this.m_character.MaxBuyHonor = 0;
                    this.m_character.GetSoulCount = 30;
                    this.m_farm.ResetFarmProp();
                    this.m_battle.Reset();
                    this.m_actives.ResetChristmas();
                    this.m_actives.Info.activityTanabataNum = 0;
                    this.m_extra.ResetUsersExtra();
                    this.m_dice.Reset();
                    this.m_actives.BoguAdventure.ResetCount = this.m_actives.countBoguReset;
                    this.m_actives.BoguAdventure.Award = "0,0,0";
                    this.m_actives.ResetBoguAdventureInfo();
                    UsersExtraInfo info = this.m_extra.Info;
                    info.MinHotSpring += 60;
                    info.coupleBossHurt = 0;
                    info.coupleBossEnterNum = 0;
                    info.coupleBossBoxNum = 0;
                }
                this.m_pvepermissions = string.IsNullOrEmpty(this.m_character.PvePermission) ? this.InitPvePermission() : this.m_character.PvePermission.ToCharArray();
                this.LoadPvePermission();
                this._friends = new Dictionary<int, int>();
                this._friends = bussiness.GetFriendsIDAll(this.m_character.ID);
                this._viFarms = new List<int>();
                this.m_character.State = 1;
                this.ClearStoreBag();
                this.ClearCaddyBag();
                bussiness.UpdateUserTexpInfo(this.m_character.Texp);
                bussiness.UpdatePlayer(this.m_character);
                this.SavePlayerInfo();
                return true;
            }
        }

        public void LoadGemStone(PlayerBussiness db)
        {
            this.m_GemStone = db.GetSingleGemStones(this.m_character.ID);
            if (this.m_GemStone.Count == 0)
            {
                List<int> list = new List<int> { 11, 5, 2, 3, 13 };
                List<int> list2 = new List<int> { 0x186a2, 0x186a3, 0x186a1, 0x186a4, 0x186a5 };
                for (int i = 0; i < list.Count; i++)
                {
                    UserGemStone item = new UserGemStone {
                        ID = 0,
                        UserID = this.m_character.ID,
                        FigSpiritId = list2[i],
                        FigSpiritIdValue = "0,0,0|0,0,1|0,0,2",
                        EquipPlace = list[i]
                    };
                    this.m_GemStone.Add(item);
                    db.AddUserGemStone(item);
                }
            }
        }

        public UserLabyrinthInfo LoadLabyrinth()
        {
            if (this.m_Labyrinth == null)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    this.m_Labyrinth = bussiness.GetSingleLabyrinth(this.m_character.ID);
                    if (this.m_Labyrinth == null)
                    {
                        this.m_Labyrinth = new UserLabyrinthInfo();
                        this.m_Labyrinth.UserID = this.m_character.ID;
                        this.m_Labyrinth.myProgress = 0;
                        this.m_Labyrinth.myRanking = 0;
                        this.m_Labyrinth.completeChallenge = true;
                        this.m_Labyrinth.isDoubleAward = false;
                        this.m_Labyrinth.currentFloor = 1;
                        this.m_Labyrinth.accumulateExp = 0;
                        this.m_Labyrinth.remainTime = 0;
                        this.m_Labyrinth.currentRemainTime = 0;
                        this.m_Labyrinth.cleanOutAllTime = 0;
                        this.m_Labyrinth.cleanOutGold = 50;
                        this.m_Labyrinth.tryAgainComplete = true;
                        this.m_Labyrinth.isInGame = false;
                        this.m_Labyrinth.isCleanOut = false;
                        this.m_Labyrinth.serverMultiplyingPower = false;
                        this.m_Labyrinth.LastDate = DateTime.Now;
                        this.m_Labyrinth.ProcessAward = this.InitProcessAward();
                        bussiness.AddUserLabyrinth(this.m_Labyrinth);
                    }
                    else
                    {
                        this.ProcessLabyrinthAward = this.m_Labyrinth.ProcessAward;
                    }
                }
            }
            return this.Labyrinth;
        }

        public void LoadMarryMessage()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                MarryApplyInfo[] playerMarryApply = bussiness.GetPlayerMarryApply(this.PlayerCharacter.ID);
                if (playerMarryApply != null)
                {
                    foreach (MarryApplyInfo info in playerMarryApply)
                    {
                        switch (info.ApplyType)
                        {
                            case 1:
                                this.Out.SendPlayerMarryApply(this, info.ApplyUserID, info.ApplyUserName, info.LoveProclamation, info.ID);
                                break;

                            case 2:
                                this.Out.SendMarryApplyReply(this, info.ApplyUserID, info.ApplyUserName, info.ApplyResult, true, info.ID);
                                if (!info.ApplyResult)
                                {
                                    this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
                                }
                                break;

                            case 3:
                                this.Out.SendPlayerDivorceApply(this, true, false);
                                break;
                        }
                    }
                }
            }
        }

        public void LoadMarryProp()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                MarryProp marryProp = bussiness.GetMarryProp(this.PlayerCharacter.ID);
                this.PlayerCharacter.IsMarried = marryProp.IsMarried;
                this.PlayerCharacter.SpouseID = marryProp.SpouseID;
                this.PlayerCharacter.SpouseName = marryProp.SpouseName;
                this.PlayerCharacter.IsCreatedMarryRoom = marryProp.IsCreatedMarryRoom;
                this.PlayerCharacter.SelfMarryRoomID = marryProp.SelfMarryRoomID;
                this.PlayerCharacter.IsGotRing = marryProp.IsGotRing;
                this.Out.SendMarryProp(this, marryProp);
            }
        }

        public void LoadPvePermission()
        {
            foreach (PveInfo info in PveInfoMgr.GetPveInfo())
            {
                if (this.m_character.Grade > info.LevelLimits)
                {
                    bool flag = this.SetPvePermission(info.ID, eHardLevel.Easy);
                    if (flag)
                    {
                        flag = this.SetPvePermission(info.ID, eHardLevel.Normal);
                    }
                    if (flag)
                    {
                        flag = this.SetPvePermission(info.ID, eHardLevel.Hard);
                    }
                }
            }
        }

        public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
        {
        }

        public bool Login()
        {
            if (WorldMgr.AddPlayer(this.m_character.ID, this))
            {
                try
                {
                    if (this.LoadFromDatabase())
                    {
                        this.Out.SendLoginSuccess();
                        this.Out.SendUpdatePublicPlayer(this.PlayerCharacter, this.BattleData.MatchInfo);
                        this.Out.SendWeaklessGuildProgress(this.PlayerCharacter);
                        this.Out.SendUpdateOneKeyFinish(this.PlayerCharacter);
                        this.Out.SendDateTime();
                        this.Out.SendDailyAward(this.PlayerCharacter);
                        this.LoadMarryMessage();
                        if (!this.m_showPP)
                        {
                            this.m_playerProp.ViewCurrent();
                            this.m_showPP = true;
                        }
                        int iD = this.PlayerCharacter.ID;
                        this.Out.SendUserRanks(iD, this.Rank.GetRank());
                        this.Out.SendActivityList(iD);
                        this.Out.SendFindBackIncome(iD);
                        this.Out.SendPlayerDrill(iD, this.UserDrills);
                        this.Out.SendOpenVIP(this.PlayerCharacter);
                        this.Out.SendOpenDDPlay(this.PlayerCharacter);
                        this.SendPkgLimitGrate();
                        if (GameProperties.IsPromotePackageOpen)
                        {
                            this.Out.SendOpenGrowthPackageOpen(iD);
                            this.Out.SendGrowthPackageOpen(iD, this.Actives.Info.AvailTime);
                        }
                        this.Out.SendOpenBoguAdventure();
                        this.Actives.SendEvent();
                        this.EquipBag.UpdatePlayerProperties();
                        this.Out.SendEdictumVersion();
                        this.Out.SendAchievementDataInfo(this.AchievementInventory.GetSuccessAchievement());
                        this.m_playerState = ePlayerState.Online;
                        this.BossBoxStartTime = DateTime.Now;
                        WorldMgr.SendSysNotice("Gà Vườn" + PlayerCharacter.NickName + " Đã Online.");
                        return true;
                    }
                    WorldMgr.RemovePlayer(this.m_character.ID);
                }
                catch (Exception exception)
                {
                    log.Error("Error Login!", exception);
                }
                return false;
            }
            return false;
        }

        public bool MissionEnergyEmpty(int value)
        {
            return (value > this.Extra.Info.MissionEnergy);
        }

        public bool MoneyDirect(int value)
        {
            if (GameProperties.IsDDTMoneyActive)
            {
                return this.MoneyDirect(MoneyType.DDTMoney, value);
            }
            return this.MoneyDirect(MoneyType.Money, value);
        }

        public bool MoneyDirect(MoneyType type, int value)
        {
            if ((value >= 0) && (value <= 0x7fffffff))
            {
                if (type == MoneyType.Money)
                {
                    if (this.PlayerCharacter.Money >= value)
                    {
                        this.RemoveMoney(value);
                        return true;
                    }
                    this.SendInsufficientMoney(0);
                }
                else
                {
                    if (this.PlayerCharacter.GiftToken >= value)
                    {
                        this.RemoveGiftToken(value);
                        return true;
                    }
                    this.SendMessage("Xu kh\x00f3a kh\x00f4ng đủ, thao t\x00e1c thất bại.");
                }
            }
            return false;
        }

        public void OnAchievementFinish(AchievementData info)
        {
            if (this.AchievementFinishEvent != null)
            {
                this.AchievementFinishEvent(info);
            }
        }

        public void OnAdoptPetEvent()
        {
            if (this.AdoptPetEvent != null)
            {
                this.AdoptPetEvent();
            }
        }

        public void OnCropPrimaryEvent()
        {
            if (this.CropPrimaryEvent != null)
            {
                this.CropPrimaryEvent();
            }
        }

        public void OnEnterHotSpring()
        {
            if (this.EnterHotSpringEvent != null)
            {
                this.EnterHotSpringEvent(this);
            }
        }

        public void OnFightAddOffer(int offer)
        {
            if (this.FightAddOfferEvent != null)
            {
                this.FightAddOfferEvent(offer);
            }
        }

        public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
        {
            if (game.RoomType == eRoomType.Match)
            {
                if (isWin)
                {
                    this.m_character.Win++;
                }
                this.m_character.Total++;
            }
            if (this.GameOver != null)
            {
                this.GameOver(game, isWin, gainXp);
            }
            if ((game.RoomType == eRoomType.Dungeon) && (this.CurrentRoom != null))
            {
                this.CurrentRoom.LevelLimits = (int) this.CurrentRoom.GetLevelLimit(this);
                this.CurrentRoom.MapId = 0x2710;
                this.CurrentRoom.HardLevel = eHardLevel.Normal;
                this.CurrentRoom.currentFloor = 0;
                this.CurrentRoom.isOpenBoss = false;
                this.CurrentRoom.SendRoomSetupChange(this.CurrentRoom);
            }
        }

        public void OnGuildChanged()
        {
            if (this.GuildChanged != null)
            {
                this.GuildChanged();
            }
        }

        public void OnHotSpingExpAdd(int minutes, int exp)
        {
            if (this.HotSpingExpAdd != null)
            {
                this.HotSpingExpAdd(minutes, exp);
            }
        }

        public void OnItemCompose(int composeType)
        {
            if (this.ItemCompose != null)
            {
                this.ItemCompose(composeType);
            }
        }

        public void OnItemFusion(int fusionType)
        {
            if (this.ItemFusion != null)
            {
                this.ItemFusion(fusionType);
            }
        }

        public void OnItemInsert()
        {
            if (this.ItemInsert != null)
            {
                this.ItemInsert();
            }
        }

        public void OnItemMelt(int categoryID)
        {
            if (this.ItemMelt != null)
            {
                this.ItemMelt(categoryID);
            }
        }

        public void OnItemStrengthen(int categoryID, int level)
        {
            if (this.ItemStrengthen != null)
            {
                this.ItemStrengthen(categoryID, level);
            }
        }

        public void OnKillingBoss(AbstractGame game, NpcInfo npc, int damage)
        {
            if (this.AfterKillingBoss != null)
            {
                this.AfterKillingBoss(game, npc, damage);
            }
        }

        public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int damage)
        {
            if (this.AfterKillingLiving != null)
            {
                this.AfterKillingLiving(game, type, id, isLiving, damage);
            }
            if (!((this.GameKillDrop == null) || isLiving))
            {
                this.GameKillDrop(game, type, id, isLiving);
            }
        }

        public void OnLevelUp(int grade)
        {
            if (this.LevelUp != null)
            {
                this.LevelUp(this);
            }
        }

        public void OnMissionOver(AbstractGame game, bool isWin, int missionId, int turnNum)
        {
            if (this.MissionOver != null)
            {
                this.MissionOver(game, missionId, isWin);
            }
            if ((this.MissionTurnOver != null) && isWin)
            {
                this.MissionTurnOver(game, missionId, turnNum);
            }
        }

        public void OnNewGearEvent(int CategoryID)
        {
            if (this.NewGearEvent != null)
            {
                this.NewGearEvent(CategoryID);
            }
        }

        public void OnPaid(int money, int gold, int offer, int gifttoken, int medal, string payGoods)
        {
            if (this.Paid != null)
            {
                this.Paid(money, gold, offer, gifttoken, medal, payGoods);
            }
        }

        protected void OnPropertiesChanged()
        {
            if (this.m_changed <= 0)
            {
                if (this.m_changed < 0)
                {
                    log.Error("Player changed count < 0");
                    Thread.VolatileWrite(ref this.m_changed, 0);
                }
                this.UpdateProperties();
            }
        }

        public void OnSeedFoodPetEvent()
        {
            if (this.SeedFoodPetEvent != null)
            {
                this.SeedFoodPetEvent();
            }
        }

        public void OnUnknowQuestConditionEvent()
        {
            if (this.UnknowQuestConditionEvent != null)
            {
                this.UnknowQuestConditionEvent();
            }
        }

        public void OnUpLevelPetEvent()
        {
            if (this.UpLevelPetEvent != null)
            {
                this.UpLevelPetEvent();
            }
        }

        public void OnUseBuffer()
        {
            if (this.UseBuffer != null)
            {
                this.UseBuffer(this);
            }
        }

        public void OnUserToemGemstoneEvent()
        {
            if (this.UserToemGemstonetEvent != null)
            {
                this.UserToemGemstonetEvent();
            }
        }

        public void OnUsingItem(int templateID)
        {
            if (this.AfterUsingItem != null)
            {
                this.AfterUsingItem(templateID);
            }
        }

        public void OpenVIP(int thoigian, DateTime ExpireDayOut)
        {
            int vIPLevel = this.m_character.VIPLevel;
            if ((vIPLevel < 6) && (thoigian == 180))
            {
                this.m_character.typeVIP = 1;
                this.m_character.VIPLevel = 6;
                this.m_character.VIPExp = 0xfa0;
                this.m_character.VIPExpireDay = ExpireDayOut;
                this.m_character.VIPLastDate = DateTime.Now;
                this.m_character.VIPNextLevelDaysNeeded = 0;
                this.m_character.CanTakeVipReward = true;
            }
            else if ((vIPLevel < 4) && (thoigian == 90))
            {
                this.m_character.typeVIP = 1;
                this.m_character.VIPLevel = 4;
                this.m_character.VIPExp = 800;
                this.m_character.VIPExpireDay = ExpireDayOut;
                this.m_character.VIPLastDate = DateTime.Now;
                this.m_character.VIPNextLevelDaysNeeded = 0;
                this.m_character.CanTakeVipReward = true;
            }
            else
            {
                this.m_character.typeVIP = 1;
                this.m_character.VIPLevel = 1;
                this.m_character.VIPExp = 0;
                this.m_character.VIPExpireDay = ExpireDayOut;
                this.m_character.VIPLastDate = DateTime.Now;
                this.m_character.VIPNextLevelDaysNeeded = 0;
                this.m_character.CanTakeVipReward = true;
            }
        }

        public void OutLabyrinth(bool isWin)
        {
            if ((!isWin && (this.m_Labyrinth != null)) && (this.m_Labyrinth.currentFloor > 1))
            {
                this.SendLabyrinthTryAgain();
            }
            this.ResetLabyrinth();
        }

        public virtual bool Quit()
        {
            try
            {
                try
                {
                    if (this.CurrentRoom != null)
                    {
                        this.CurrentRoom.RemovePlayerUnsafe(this);
                        this.CurrentRoom = null;
                    }
                    else
                    {
                        RoomMgr.WaitingRoom.RemovePlayer(this);
                    }
                    if (this.CurrentMarryRoom != null)
                    {
                        this.CurrentMarryRoom.RemovePlayer(this);
                        this.CurrentMarryRoom = null;
                    }
                    if (this.m_currentSevenDoubleRoom != null)
                    {
                        this.CurrentSevenDoubleRoom.RemovePlayer(this);
                        this.CurrentSevenDoubleRoom = null;
                    }
                    RoomMgr.WorldBossRoom.RemovePlayer(this);
                    RoomMgr.ChristmasRoom.SetMonterDie(this.PlayerCharacter.ID);
                    RoomMgr.ChristmasRoom.RemovePlayer(this);
                    RoomMgr.ConsBatRoom.RemovePlayer(this);
                    RoomMgr.CampBattleRoom.RemovePlayer(this);
                    this.Actives.StopChristmasTimer();
                    this.Actives.StopLabyrinthTimer();
                    this.Actives.StopLightriddleTimer();
                }
                catch (Exception exception)
                {
                    log.Error("Player exit Game Error!", exception);
                }
                this.m_character.State = 0;
                this.SaveIntoDatabase();
            }
            catch (Exception exception2)
            {
                log.Error("Player exit Error!!!", exception2);
            }
            finally
            {
                WorldMgr.RemovePlayer(this.m_character.ID);
            }
            return true;
        }

        public int RemoveActiveMoney(int value)
        {
            if ((value > 0) && (value <= this.Actives.Info.ActiveMoney))
            {
                ActiveSystemInfo info = this.Actives.Info;
                info.ActiveMoney -= value;
                this.SendHideMessage(string.Format("Bạn vừa ti\x00eau hao {0} Xu năng động, c\x00f2n lại {1} Xu năng động", value, this.Actives.Info.ActiveMoney));
                return value;
            }
            return 0;
        }

        public bool RemoveAt(eBageType bagType, int place)
        {
            PlayerInventory inventory = this.GetInventory(bagType);
            return ((inventory != null) && inventory.RemoveItemAt(place));
        }

        public int RemoveCardSoul(int value)
        {
            if ((value > 0) && (value <= this.m_character.CardSoul))
            {
                this.m_character.CardSoul -= value;
                return value;
            }
            return 0;
        }

        public bool RemoveCountFromStack(SqlDataProvider.Data.ItemInfo item, int count)
        {
            if (item.BagType == this.m_propBag.BagType)
            {
                return this.m_propBag.RemoveCountFromStack(item, count);
            }
            if (item.BagType == this.m_ConsortiaBag.BagType)
            {
                return this.m_ConsortiaBag.RemoveCountFromStack(item, count);
            }
            if (item.BagType == this.m_BeadBag.BagType)
            {
                return this.m_BeadBag.RemoveCountFromStack(item, count);
            }
            if (item.BagType == this.m_magicStoneBag.BagType)
            {
                return this.m_magicStoneBag.RemoveCountFromStack(item, count);
            }
            return this.m_equipBag.RemoveCountFromStack(item, count);
        }

        public int RemoveDamageScores(int value)
        {
            if ((value > 0) && (value <= this.m_character.damageScores))
            {
                this.m_character.damageScores -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int RemoveDDPlayPoint(int value)
        {
            if ((value > 0) && (value <= this.m_character.DDPlayPoint))
            {
                this.m_character.DDPlayPoint -= value;
                return value;
            }
            return 0;
        }

        public void RemoveFightFootballStyle()
        {
            SqlDataProvider.Data.ItemInfo itemAt = this.GetItemAt(eBageType.EquipBag, 0);
            string str = (itemAt == null) ? "" : (itemAt.TemplateID.ToString() + "|" + itemAt.Template.Pic);
            for (int i = 0; i < StyleIndex.Length; i++)
            {
                str = str + ",";
                itemAt = this.GetItemAt(eBageType.EquipBag, StyleIndex[i]);
                if (itemAt != null)
                {
                    object obj2 = str;
                    str = string.Concat(new object[] { obj2, itemAt.TemplateID, "|", itemAt.Pic });
                }
            }
            if (!string.IsNullOrEmpty(str))
            {
                this.PlayerCharacter.Style = str;
            }
            this.OnPropertiesChanged();
        }

        public void RemoveFistGetPet()
        {
            this.m_character.IsFistGetPet = false;
            this.m_character.LastRefreshPet = DateTime.Now.AddDays(-1.0);
        }

        public int RemoveGold(int value)
        {
            if ((value > 0) && (value <= this.m_character.Gold))
            {
                this.m_character.Gold -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int RemoveGoXu(int value)
        {
            return this.RemoveGoXu(value, true);
        }

        public int RemoveGoXu(int value, bool notice)
        {
            if ((value > 0) && (value <= this.m_character.GoXu))
            {
                this.m_character.GoXu -= value;
                if (notice)
                {
                    this.SendMessage(string.Concat(new object[] { "Bạn bị trừ ", value, " GoXu. GoXu hiện c\x00f2n: ", this.m_character.GoXu }));
                }
                return value;
            }
            return 0;
        }

        public int RemoveGP(int gp)
        {
            if (gp > 0)
            {
                this.m_character.GP -= gp;
                if (this.m_character.GP < 1)
                {
                    this.m_character.GP = 1;
                }
                int level = LevelMgr.GetLevel(this.m_character.GP);
                if (this.Level > level)
                {
                    this.m_character.GP += gp;
                }
                this.UpdateLevel();
                return gp;
            }
            return 0;
        }

        public int RemoveGiftToken(int value)
        {
            if ((value > 0) && (value <= this.m_character.GiftToken))
            {
                this.m_character.GiftToken -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int RemoveHardCurrency(int value)
        {
            if ((value > 0) && (value <= this.m_character.hardCurrency))
            {
                this.m_character.hardCurrency -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public bool RemoveHealstone()
        {
            return false;
        }

        public bool RemoveItem(SqlDataProvider.Data.ItemInfo item)
        {
            if (item.BagType == this.m_farmBag.BagType)
            {
                return this.m_farmBag.RemoveItem(item);
            }
            if (item.BagType == this.m_propBag.BagType)
            {
                return this.m_propBag.RemoveItem(item);
            }
            if (item.BagType == this.m_BeadBag.BagType)
            {
                return this.m_BeadBag.RemoveItem(item);
            }
            if (item.BagType == this.m_magicStoneBag.BagType)
            {
                return this.m_magicStoneBag.RemoveItem(item);
            }
            if (item.BagType == this.m_fightBag.BagType)
            {
                return this.m_fightBag.RemoveItem(item);
            }
            return this.m_equipBag.RemoveItem(item);
        }

        public void RemoveLastRefreshPet()
        {
            this.m_character.LastRefreshPet = DateTime.Now;
        }

        public int RemoveLeagueMoney(int value)
        {
            if ((value > 0) && (value <= this.m_character.LeagueMoney))
            {
                this.m_character.LeagueMoney -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int RemoveMagicStonePoint(int value)
        {
            if ((value > 0) && (value <= this.m_character.MagicStonePoint))
            {
                this.m_character.MagicStonePoint -= value;
                this.Out.SendMagicStonePoint(this.PlayerCharacter);
                return value;
            }
            return 0;
        }

        public int RemoveMedal(int value)
        {
            if ((value > 0) && (value <= this.m_character.medal))
            {
                this.m_character.medal -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public bool RemoveMissionEnergy(int value)
        {
            return ((value > 0) && (value <= this.Extra.Info.MissionEnergy));
        }

        public int RemoveMoney(int value)
        {
            if ((value > 0) && (value <= this.m_character.Money))
            {
                this.m_character.Money -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int RemovemyHonor(int value)
        {
            if ((value > 0) && (value <= this.m_character.myHonor))
            {
                this.m_character.myHonor -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int RemoveOffer(int value)
        {
            if (value > 0)
            {
                if (value >= this.m_character.Offer)
                {
                    value = this.m_character.Offer;
                }
                this.m_character.Offer -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int RemovePetScore(int value)
        {
            if ((value > 0) && (value <= this.m_character.petScore))
            {
                this.m_character.petScore -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public int RemoveScore(int value)
        {
            if ((value > 0) && (value <= this.m_character.Score))
            {
                this.m_character.Score -= value;
                this.OnPropertiesChanged();
                return value;
            }
            return 0;
        }

        public bool RemoveTempate(eBageType bagType, ItemTemplateInfo template, int count)
        {
            PlayerInventory inventory = this.GetInventory(bagType);
            return ((inventory != null) && inventory.RemoveTemplate(template.TemplateID, count));
        }

        public bool RemoveTemplate(ItemTemplateInfo template, int count)
        {
            PlayerInventory itemInventory = this.GetItemInventory(template);
            return ((itemInventory != null) && itemInventory.RemoveTemplate(template.TemplateID, count));
        }

        public bool RemoveTemplate(int templateId, int count)
        {
            int itemCount = this.m_equipBag.GetItemCount(templateId);
            int num2 = this.m_propBag.GetItemCount(templateId);
            int num3 = this.m_ConsortiaBag.GetItemCount(templateId);
            int num4 = (itemCount + num2) + num3;
            ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateId);
            if ((template != null) && (num4 >= count))
            {
                if (((itemCount > 0) && (count > 0)) && this.RemoveTempate(eBageType.EquipBag, template, (itemCount > count) ? count : itemCount))
                {
                    count = (count < itemCount) ? 0 : (count - itemCount);
                }
                if (((num2 > 0) && (count > 0)) && this.RemoveTempate(eBageType.PropBag, template, (num2 > count) ? count : num2))
                {
                    count = (count < num2) ? 0 : (count - num2);
                }
                if (((num3 > 0) && (count > 0)) && this.RemoveTempate(eBageType.Consortia, template, (num3 > count) ? count : num3))
                {
                    count = (count < num3) ? 0 : (count - num3);
                }
                if (count == 0)
                {
                    return true;
                }
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Item Remover Error：PlayerId {0} Remover TemplateId{1} Is Not Zero!", this.m_playerId, templateId));
                }
            }
            return false;
        }

        public void ResetLabyrinth()
        {
            if (this.m_Labyrinth != null)
            {
                this.m_Labyrinth.isInGame = false;
                this.m_Labyrinth.completeChallenge = false;
                this.m_Labyrinth.ProcessAward = this.InitProcessAward();
            }
        }

        public bool SaveIntoDatabase()
        {
            try
            {
                if (this.m_character.IsDirty)
                {
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        bussiness.UpdatePlayer(this.m_character);
                        if (this.m_Labyrinth != null)
                        {
                            bussiness.UpdateLabyrinthInfo(this.m_Labyrinth);
                        }
                        foreach (UserDrillInfo info in this.m_userDrills.Values)
                        {
                            bussiness.UpdateUserDrillInfo(info);
                        }
                        foreach (UserGemStone stone in this.m_GemStone)
                        {
                            bussiness.UpdateGemStoneInfo(stone);
                        }
                    }
                }
                this.EquipBag.SaveToDatabase();
                this.PropBag.SaveToDatabase();
                this.ConsortiaBag.SaveToDatabase();
                this.BeadBag.SaveToDatabase();
                this.MagicStoneBag.SaveToDatabase();
                this.FarmBag.SaveToDatabase();
                this.PetBag.SaveToDatabase(true);
                this.CardBag.SaveToDatabase();
                this.StoreBag.SaveToDatabase();
                this.Farm.SaveToDatabase();
                this.Treasure.SaveToDatabase();
                this.Rank.SaveToDatabase();
                this.DressModel.SaveToDatabase();
                this.AvatarCollect.SaveToDatabase();
                this.QuestInventory.SaveToDatabase();
                this.AchievementInventory.SaveToDatabase();
                this.BufferList.SaveToDatabase();
                this.BattleData.SaveToDatabase();
                this.Actives.SaveToDatabase();
                this.Dice.SaveToDatabase();
                this.Extra.SaveToDatabase();
                return true;
            }
            catch (Exception exception)
            {
                log.Error("Error saving player " + this.m_character.NickName + "!", exception);
                return false;
            }
        }

        public bool SaveNewItems()
        {
            try
            {
                this.EquipBag.SaveToDatabase();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SaveNewsItemIntoDatabase()
        {
            try
            {
                this.EquipBag.SaveNewsItemIntoDatabas();
                this.PropBag.SaveNewsItemIntoDatabas();
                this.BeadBag.SaveNewsItemIntoDatabas();
                this.MagicStoneBag.SaveNewsItemIntoDatabas();
                return true;
            }
            catch (Exception exception)
            {
                log.Error("Error saving Save Bag Into Database " + this.m_character.NickName + "!", exception);
                return false;
            }
        }

        public bool SavePlayerInfo()
        {
            try
            {
                if (this.m_character.IsDirty)
                {
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        bussiness.UpdatePlayer(this.m_character);
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                log.Error("Error saving player info of " + this.m_character.UserName + "!", exception);
                return false;
            }
        }

        public void SendConsortiaBossInfo(ConsortiaInfo info)
        {
            RankingPersonInfo info2 = null;
            List<RankingPersonInfo> list = new List<RankingPersonInfo>();
            foreach (RankingPersonInfo info3 in info.RankList.Values)
            {
                if (info3.Name == this.PlayerCharacter.NickName)
                {
                    info2 = info3;
                }
                else
                {
                    list.Add(info3);
                }
            }
            GSPacketIn pkg = new GSPacketIn(0x81, this.PlayerCharacter.ID);
            pkg.WriteByte(30);
            pkg.WriteByte((byte) info.bossState);
            pkg.WriteBoolean(info2 != null);
            if (info2 != null)
            {
                pkg.WriteInt(info2.ID);
                pkg.WriteInt(info2.TotalDamage);
                pkg.WriteInt(info2.Honor);
                pkg.WriteInt(info2.Damage);
            }
            pkg.WriteByte((byte) list.Count);
            foreach (RankingPersonInfo info4 in list)
            {
                pkg.WriteString(info4.Name);
                pkg.WriteInt(info4.ID);
                pkg.WriteInt(info4.TotalDamage);
                pkg.WriteInt(info4.Honor);
                pkg.WriteInt(info4.Damage);
            }
            pkg.WriteByte((byte) info.extendAvailableNum);
            pkg.WriteDateTime(info.endTime);
            pkg.WriteInt(info.callBossLevel);
            this.SendTCP(pkg);
        }

        public void SendConsortiaBossOpenClose(int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x81, this.PlayerCharacter.ID);
            pkg.WriteByte(0x1f);
            pkg.WriteByte((byte) type);
            this.SendTCP(pkg);
        }

        public void SendConsortiaFight(int consortiaID, int riches, string msg)
        {
            GSPacketIn packet = new GSPacketIn(0x9e);
            packet.WriteInt(consortiaID);
            packet.WriteInt(riches);
            packet.WriteString(msg);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }

        public void SendHideMessage(string msg)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(3);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendInsufficientMoney(int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x58, this.PlayerId);
            pkg.WriteByte((byte) type);
            pkg.WriteBoolean(false);
            this.SendTCP(pkg);
        }

        public void SendItemNotice(SqlDataProvider.Data.ItemInfo info, int typeGet, string Name)
        {
            if (info != null)
            {
                int num;
                switch (typeGet)
                {
                    case 0:
                    case 1:
                        num = 2;
                        break;

                    case 2:
                    case 3:
                    case 4:
                        num = 1;
                        break;

                    default:
                        num = 3;
                        break;
                }
                GSPacketIn packet = new GSPacketIn(14);
                packet.WriteString(this.PlayerCharacter.NickName);
                packet.WriteInt(typeGet);
                packet.WriteInt(info.TemplateID);
                packet.WriteBoolean(info.IsBinds);
                packet.WriteInt(num);
                packet.WriteInt(info.Count);
                if (num == 3)
                {
                    packet.WriteString(Name);
                }
                if ((info.IsTips || (info.Template.Quality >= 5)) || info.IsBead())
                {
                    foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                    {
                        player.Out.SendTCP(packet);
                    }
                }
            }
        }

        public bool SendItemsToMail(List<SqlDataProvider.Data.ItemInfo> items, string content, string title, eMailType type)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
                foreach (SqlDataProvider.Data.ItemInfo info in items)
                {
                    if (info.Template.MaxCount == 1)
                    {
                        for (int i = 0; i < info.Count; i++)
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
                return this.SendItemsToMail(list, content, title, type, bussiness);
            }
        }

        public bool SendItemsToMail(List<SqlDataProvider.Data.ItemInfo> items, string content, string title, eMailType type, PlayerBussiness pb)
        {
            bool flag = true;
            int num = 0;
            while (num < items.Count)
            {
                SqlDataProvider.Data.ItemInfo info2;
                MailInfo mail = new MailInfo {
                    Title = (title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]),
                    Gold = 0,
                    IsExist = true,
                    Money = 0,
                    Receiver = this.PlayerCharacter.NickName,
                    ReceiverID = this.PlayerId,
                    Sender = this.PlayerCharacter.NickName,
                    SenderID = this.PlayerId,
                    Type = (int) type,
                    GiftToken = 0
                };
                List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
                StringBuilder builder = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                builder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark", new object[0]));
                content = (content != null) ? LanguageMgr.GetTranslation(content, new object[0]) : "";
                int num2 = num;
                if (items.Count > num2)
                {
                    info2 = items[num2];
                    if (info2.ItemID == 0)
                    {
                        pb.AddGoods(info2);
                    }
                    else
                    {
                        list.Add(info2);
                    }
                    if (title == null)
                    {
                        mail.Title = info2.Template.Name;
                    }
                    mail.Annex1 = info2.ItemID.ToString();
                    mail.Annex1Name = info2.Template.Name;
                    builder.Append(string.Concat(new object[] { "1、", mail.Annex1Name, "x", info2.Count, ";" }));
                    builder2.Append(string.Concat(new object[] { "1、", mail.Annex1Name, "x", info2.Count, ";" }));
                }
                num2 = num + 1;
                if (items.Count > num2)
                {
                    info2 = items[num2];
                    if (info2.ItemID == 0)
                    {
                        pb.AddGoods(info2);
                    }
                    else
                    {
                        list.Add(info2);
                    }
                    mail.Annex2 = info2.ItemID.ToString();
                    mail.Annex2Name = info2.Template.Name;
                    builder.Append(string.Concat(new object[] { "2、", mail.Annex2Name, "x", info2.Count, ";" }));
                    builder2.Append(string.Concat(new object[] { "2、", mail.Annex2Name, "x", info2.Count, ";" }));
                }
                num2 = num + 2;
                if (items.Count > num2)
                {
                    info2 = items[num2];
                    if (info2.ItemID == 0)
                    {
                        pb.AddGoods(info2);
                    }
                    else
                    {
                        list.Add(info2);
                    }
                    mail.Annex3 = info2.ItemID.ToString();
                    mail.Annex3Name = info2.Template.Name;
                    builder.Append(string.Concat(new object[] { "3、", mail.Annex3Name, "x", info2.Count, ";" }));
                    builder2.Append(string.Concat(new object[] { "3、", mail.Annex3Name, "x", info2.Count, ";" }));
                }
                num2 = num + 3;
                if (items.Count > num2)
                {
                    info2 = items[num2];
                    if (info2.ItemID == 0)
                    {
                        pb.AddGoods(info2);
                    }
                    else
                    {
                        list.Add(info2);
                    }
                    mail.Annex4 = info2.ItemID.ToString();
                    mail.Annex4Name = info2.Template.Name;
                    builder.Append(string.Concat(new object[] { "4、", mail.Annex4Name, "x", info2.Count, ";" }));
                    builder2.Append(string.Concat(new object[] { "4、", mail.Annex4Name, "x", info2.Count, ";" }));
                }
                num2 = num + 4;
                if (items.Count > num2)
                {
                    info2 = items[num2];
                    if (info2.ItemID == 0)
                    {
                        pb.AddGoods(info2);
                    }
                    else
                    {
                        list.Add(info2);
                    }
                    mail.Annex5 = info2.ItemID.ToString();
                    mail.Annex5Name = info2.Template.Name;
                    builder.Append(string.Concat(new object[] { "5、", mail.Annex5Name, "x", info2.Count, ";" }));
                    builder2.Append(string.Concat(new object[] { "5、", mail.Annex5Name, "x", info2.Count, ";" }));
                }
                mail.AnnexRemark = builder.ToString();
                if ((content == null) && (builder2.ToString() == null))
                {
                    mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content", new object[0]);
                }
                else if (content != "")
                {
                    mail.Content = content;
                }
                else
                {
                    mail.Content = builder2.ToString();
                }
                if (pb.SendMail(mail))
                {
                    foreach (SqlDataProvider.Data.ItemInfo info3 in list)
                    {
                        this.TakeOutItem(info3);
                    }
                }
                else
                {
                    goto Label_073F;
                }
            Label_0738:
                num += 5;
                continue;
            Label_073F:
                flag = false;
                goto Label_0738;
            }
            return flag;
        }

        public bool SendItemToMail(int templateID, string content, string title)
        {
            ItemTemplateInfo goods = ItemMgr.FindItemTemplate(templateID);
            if (goods == null)
            {
                return false;
            }
            if (content == "")
            {
                content = goods.Name + "x1";
            }
            SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x68);
            item.IsBinds = true;
            return this.SendItemToMail(item, content, title, eMailType.Active);
        }

        public bool SendItemToMail(SqlDataProvider.Data.ItemInfo item, string content, string title, eMailType type)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                return this.SendItemToMail(item, bussiness, content, title, type);
            }
        }

        public bool SendItemToMail(SqlDataProvider.Data.ItemInfo item, PlayerBussiness pb, string content, string title, eMailType type)
        {
            MailInfo mail = new MailInfo {
                Content = (content != null) ? content : LanguageMgr.GetTranslation("Game.Server.GameUtils.Content", new object[0]),
                Title = (title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]),
                Gold = 0,
                IsExist = true,
                Money = 0,
                GiftToken = 0,
                Receiver = this.PlayerCharacter.NickName,
                ReceiverID = this.PlayerCharacter.ID,
                Sender = this.PlayerCharacter.NickName,
                SenderID = this.PlayerCharacter.ID,
                Type = (int) type
            };
            if (item.ItemID == 0)
            {
                pb.AddGoods(item);
            }
            mail.Annex1 = item.ItemID.ToString();
            mail.Annex1Name = item.Template.Name;
            if (pb.SendMail(mail))
            {
                this.TakeOutItem(item);
                return true;
            }
            return false;
        }

        public void SendLabyrinthTryAgain()
        {
            GSPacketIn pkg = new GSPacketIn(0x83, this.PlayerId);
            pkg.WriteByte(9);
            pkg.WriteInt(this.LabyrinthTryAgainMoney());
            this.SendTCP(pkg);
        }

        public bool SendMailToUser(PlayerBussiness pb, string content, string title, eMailType type)
        {
            MailInfo mail = new MailInfo {
                Content = content,
                Title = title,
                Gold = 0,
                IsExist = true,
                Money = 0,
                GiftToken = 0,
                Receiver = this.PlayerCharacter.NickName,
                ReceiverID = this.PlayerCharacter.ID,
                Sender = this.PlayerCharacter.NickName,
                SenderID = this.PlayerCharacter.ID,
                Type = (int) type
            };
            mail.Annex1 = "";
            mail.Annex1Name = "";
            return pb.SendMail(mail);
        }

        public void SendMessage(string msg)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(0);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public bool SendMoneyMailToUser(PlayerBussiness pb, string content, string title, int money, eMailType type)
        {
            MailInfo mail = new MailInfo {
                Content = content,
                Title = title,
                Gold = 0,
                IsExist = true,
                Money = money,
                GiftToken = 0,
                Receiver = this.PlayerCharacter.NickName,
                ReceiverID = this.PlayerCharacter.ID,
                Sender = this.PlayerCharacter.NickName,
                SenderID = this.PlayerCharacter.ID,
                Type = (int) type
            };
            mail.Annex1 = "";
            mail.Annex1Name = "";
            return pb.SendMail(mail);
        }

        public void SendPkgLimitGrate()
        {
            int iD = this.PlayerCharacter.ID;
            if (this.PlayerCharacter.Grade >= 20)
            {
                if (RoomMgr.WorldBossRoom.WorldbossOpen)
                {
                    this.Out.SendOpenWorldBoss(this.X, this.Y);
                }
                if (ActiveSystemMgr.IsLeagueOpen)
                {
                    this.Out.SendLeagueNotice(iD, this.BattleData.MatchInfo.restCount, this.BattleData.maxCount, 1);
                }
                else
                {
                    this.Out.SendLeagueNotice(iD, this.BattleData.MatchInfo.restCount, this.BattleData.maxCount, 2);
                }
                if (ActiveSystemMgr.IsFightFootballTime)
                {
                    this.Out.SendFightFootballTimeOpenClose(iD, true);
                }
            }
            if (this.PlayerCharacter.Grade >= 30)
            {
                this.Out.SendPlayerFigSpiritinit(iD, this.GemStone);
            }
            if (this.PlayerCharacter.Grade >= 15)
            {
                if (ActiveSystemMgr.IsBattleGoundOpen)
                {
                    this.Out.SendBattleGoundOpen(iD);
                }
                if (this.Actives.IsDragonBoatOpen())
                {
                    this.Out.SendDragonBoat(this.PlayerCharacter);
                }
                if (ActiveSystemMgr.LanternriddlesOpen)
                {
                    this.Out.SendLanternriddlesOpen(iD, true);
                }
            }
            int grade = this.PlayerCharacter.Grade;
            if ((this.PlayerCharacter.Grade >= 13) && this.Actives.IsPyramidOpen())
            {
                this.Out.SendPyramidOpenClose(this.Actives.PyramidConfig);
            }
            if (this.Actives.IsYearMonsterOpen())
            {
                this.Out.SendCatchBeastOpen(iD, true);
            }
        }

        public void SendPrivateChat(int receiverID, string receiver, string sender, string msg, bool isAutoReply)
        {
            GSPacketIn pkg = new GSPacketIn(0x25, this.PlayerCharacter.ID);
            pkg.WriteInt(receiverID);
            pkg.WriteString(receiver);
            pkg.WriteString(sender);
            pkg.WriteString(msg);
            pkg.WriteBoolean(isAutoReply);
            this.SendTCP(pkg);
        }

        public void SendTCP(GSPacketIn pkg)
        {
            if (this.m_client.IsConnected)
            {
                this.m_client.SendTCP(pkg);
            }
        }

        public bool SetPvePermission(int copyId, eHardLevel hardLevel)
        {
            if (hardLevel != eHardLevel.Epic)
            {
                if ((((copyId > this.m_pvepermissions.Length) || (copyId <= 0)) || (hardLevel == eHardLevel.Terror)) || (this.m_pvepermissions[copyId - 1] != permissionChars[(int) hardLevel]))
                {
                    return true;
                }
                this.m_pvepermissions[copyId - 1] = permissionChars[((int) hardLevel) + 1];
                this.m_character.PvePermission = this.ConverterPvePermission(this.m_pvepermissions);
                this.OnPropertiesChanged();
            }
            return true;
        }

        public void ShowAllFootballCard()
        {
            for (int i = 0; i < this.CardsTakeOut.Length; i++)
            {
                if (this.CardsTakeOut[i] == null)
                {
                    this.CardsTakeOut[i] = this.Card[i];
                    if (this.takeoutCount > 0)
                    {
                        this.TakeFootballCard(this.Card[i]);
                    }
                }
            }
        }

        public bool StackItemToAnother(SqlDataProvider.Data.ItemInfo item)
        {
            return this.GetItemInventory(item.Template).StackItemToAnother(item);
        }

        public void TakeFootballCard(CardInfo card)
        {
            List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
            for (int i = 0; i < this.CardsTakeOut.Length; i++)
            {
                if (card.place == i)
                {
                    this.CardsTakeOut[i] = card;
                    this.CardsTakeOut[i].IsTake = true;
                    ItemTemplateInfo goods = ItemMgr.FindItemTemplate(card.templateID);
                    if (goods != null)
                    {
                        infos.Add(SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, card.count, 110));
                    }
                    this.takeoutCount--;
                    break;
                }
            }
            if (infos.Count > 0)
            {
                foreach (SqlDataProvider.Data.ItemInfo info2 in infos)
                {
                    this.AddTemplate(infos);
                }
            }
        }

        public bool TakeOutItem(SqlDataProvider.Data.ItemInfo item)
        {
            if (item.BagType == this.m_propBag.BagType)
            {
                return this.m_propBag.TakeOutItem(item);
            }
            if (item.BagType == this.m_fightBag.BagType)
            {
                return this.m_fightBag.TakeOutItem(item);
            }
            if (item.BagType == this.m_ConsortiaBag.BagType)
            {
                return this.m_ConsortiaBag.TakeOutItem(item);
            }
            if (item.BagType == this.m_BeadBag.BagType)
            {
                return this.m_BeadBag.TakeOutItem(item);
            }
            if (item.BagType == this.m_magicStoneBag.BagType)
            {
                return this.m_magicStoneBag.TakeOutItem(item);
            }
            return this.m_equipBag.TakeOutItem(item);
        }

        public void TestQuest()
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (QuestInfo info in bussiness.GetALlQuest())
                {
                    string str;
                    this.QuestInventory.AddQuest(info, out str);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Id:{0} nickname:{1} room:{2} ", this.PlayerId, this.PlayerCharacter.NickName, this.CurrentRoom);
        }

        public void UpdateAnswerSite(int id)
        {
            if (this.PlayerCharacter.AnswerSite < id)
            {
                this.PlayerCharacter.AnswerSite = id;
            }
            this.UpdateWeaklessGuildProgress();
            this.Out.SendWeaklessGuildProgress(this.PlayerCharacter);
        }

        public void UpdateBadgeId(int Id)
        {
            this.m_character.badgeID = Id;
        }

        public void UpdateBarrier(int barrier, string pic)
        {
            if (this.CurrentRoom != null)
            {
                this.CurrentRoom.Pic = pic;
                this.CurrentRoom.barrierNum = barrier;
                this.CurrentRoom.currentFloor = barrier;
            }
        }

        public void UpdateBaseProperties(int attack, int defence, int agility, int lucky, int magicAttack, int magicDefence, int hp)
        {
            if (((((attack != this.m_character.Attack) || (defence != this.m_character.Defence)) || ((agility != this.m_character.Agility) || (lucky != this.m_character.Luck))) || (magicAttack != this.m_character.MagicAttack)) || (magicDefence != this.m_character.MagicDefence))
            {
                this.m_character.Attack = attack;
                this.m_character.Defence = defence;
                this.m_character.Agility = agility;
                this.m_character.Luck = lucky;
                this.m_character.MagicAttack = magicAttack;
                this.m_character.MagicDefence = magicDefence;
                this.OnPropertiesChanged();
            }
            this.m_character.hp = (int) ((((hp + this.LevelPlusBlood) + (this.m_character.Defence / 10)) + this.GetGoldBlood()) * this.GetBaseBlood());
        }

        public bool UpdateChangedPlaces()
        {
            try
            {
                this.EquipBag.UpdateChangedPlaces();
                this.PropBag.UpdateChangedPlaces();
                this.BeadBag.UpdateChangedPlaces();
                this.MagicStoneBag.UpdateChangedPlaces();
                return true;
            }
            catch (Exception exception)
            {
                log.Error("Error Update Changed Places " + this.m_character.NickName + "!", exception);
                return false;
            }
        }

        public void UpdateDrill(int index, UserDrillInfo drill)
        {
            this.m_userDrills[index] = drill;
        }

        public void UpdateFightBuff(BufferInfo info)
        {
            int type = -1;
            for (int i = 0; i < this.FightBuffs.Count; i++)
            {
                if ((info != null) && (info.Type == this.FightBuffs[i].Type))
                {
                    this.FightBuffs[i] = info;
                    type = info.Type;
                }
            }
            if (type == -1)
            {
                this.FightBuffs.Add(info);
            }
        }

        public void UpdateFightPower()
        {
            int num = 0;
            this.FightPower = 0;
            int hp = this.PlayerCharacter.hp;
            num += this.PlayerCharacter.Attack;
            num += this.PlayerCharacter.Defence;
            num += this.PlayerCharacter.Agility;
            num += this.PlayerCharacter.Luck;
            num += this.PlayerCharacter.MagicAttack;
            num += this.PlayerCharacter.MagicDefence;
            double baseAttack = this.GetBaseAttack();
            double baseDefence = this.GetBaseDefence();
            this.FightPower += (int) ((((num + 0x3e8) * (((baseAttack * baseAttack) * baseAttack) + (((3.5 * baseDefence) * baseDefence) * baseDefence))) / 100000000.0) + (hp * 0.95));
            if (this.m_currentSecondWeapon != null)
            {
                this.FightPower += (int) (this.m_currentSecondWeapon.Template.Property7 * Math.Pow(1.1, (double) this.m_currentSecondWeapon.StrengthenLevel));
            }
            if (this.FightPower < 0)
            {
                this.FightPower = 0x7fffffff;
            }
            this.PlayerCharacter.FightPower = this.FightPower;
        }

        public void UpdateGemStone(int place, UserGemStone gem)
        {
            for (int i = 0; i < this.m_GemStone.Count; i++)
            {
                if (place == this.m_GemStone[i].EquipPlace)
                {
                    this.m_GemStone[i] = gem;
                    break;
                }
            }
        }

        public GSPacketIn UpdateGoodsCount()
        {
            return this.Out.SendUpdateGoodsCount(this.PlayerCharacter, null, null);
        }

        public void UpdateHealstone(SqlDataProvider.Data.ItemInfo item)
        {
            if (item != null)
            {
                this.m_healstone = item;
            }
        }

        public void UpdateHide(int hide)
        {
            if (hide != this.m_character.Hide)
            {
                this.m_character.Hide = hide;
                this.OnPropertiesChanged();
            }
        }

        public void UpdateHonor(string honor)
        {
            this.PlayerCharacter.Honor = honor;
            if (this.Rank.IsRank(honor))
            {
                this.EquipBag.UpdatePlayerProperties();
            }
        }

        public void UpdateItem(SqlDataProvider.Data.ItemInfo item)
        {
            this.m_equipBag.UpdateItem(item);
            this.m_propBag.UpdateItem(item);
        }

        public void UpdateItemForUser(object state)
        {
            this.m_battle.LoadFromDatabase();
            this.m_equipBag.LoadFromDatabase();
            this.m_magicStoneBag.LoadFromDatabase();
            this.m_propBag.LoadFromDatabase();
            this.m_ConsortiaBag.LoadFromDatabase();
            this.m_BeadBag.LoadFromDatabase();
            this.m_farmBag.LoadFromDatabase();
            this.m_petBag.LoadFromDatabase();
            this.m_storeBag.LoadFromDatabase();
            this.m_cardBag.LoadFromDatabase();
            this.m_questInventory.LoadFromDatabase(this.m_character.ID);
            this.m_achievementInventory.LoadFromDatabase(this.m_character.ID);
            this.m_bufferList.LoadFromDatabase(this.m_character.ID);
            this.m_treasure.LoadFromDatabase();
            this.m_rank.LoadFromDatabase();
            this.m_dressmodel.LoadFromDatabase();
            this.m_avatarcollect.LoadFromDatabase();
            this.m_farm.LoadFromDatabase();
            this.m_dice.LoadFromDatabase();
            this.m_actives.LoadFromDatabase();
            this.m_extra.LoadFromDatabase();
        }

        public void UpdateLabyrinth(int floor, int m_missionInfoId, bool bigAward)
        {
            int[] numArray = this.CreateExps();
            int index = ((floor - 1) > numArray.Length) ? (numArray.Length - 1) : (floor - 1);
            index = (index < 0) ? 0 : index;
            int num2 = numArray[index];
            string str = this.labyrinthGolds[index];
            int count = int.Parse(str.Split(new char[] { '|' })[0]);
            int num4 = int.Parse(str.Split(new char[] { '|' })[1]);
            if (this.m_Labyrinth != null)
            {
                floor++;
                this.ProcessLabyrinthAward = this.CompleteGetAward(floor);
                this.m_Labyrinth.ProcessAward = this.ProcessLabyrinthAward;
                if (!((this.PropBag.GetItemByTemplateID(0, 0x2e8c) != null) && this.RemoveTemplate(0x2e8c, 1)))
                {
                    this.m_Labyrinth.isDoubleAward = false;
                }
                if (this.m_Labyrinth.isDoubleAward)
                {
                    int num5 = 2;
                    num2 *= num5;
                    count *= num5;
                    num4 *= num5;
                }
                if (floor > this.m_Labyrinth.myProgress)
                {
                    this.m_Labyrinth.myProgress = floor;
                }
                if (floor > this.m_Labyrinth.currentFloor)
                {
                    this.m_Labyrinth.currentFloor = floor;
                }
                this.m_Labyrinth.accumulateExp += num2;
                string msg = string.Format("Bạn nhận được: {0} exp", num2);
                this.AddGP(num2);
                if (bigAward)
                {
                    List<SqlDataProvider.Data.ItemInfo> list = this.CopyDrop(2, 0x9c42);
                    if (list != null)
                    {
                        foreach (SqlDataProvider.Data.ItemInfo info2 in list)
                        {
                            info2.IsBinds = true;
                            this.AddTemplate(info2, info2.Template.BagType, count, eGameView.dungeonTypeGet);
                            msg = msg + string.Format(", {0} x{1}", info2.Template.Name, count);
                        }
                    }
                    this.AddHardCurrency(num4);
                    msg = msg + ", V\x00e0ng m\x00ea cung x" + num4;
                }
                this.SendHideMessage(msg);
            }
            this.Out.SendLabyrinthUpdataInfo(this.m_Labyrinth.UserID, this.m_Labyrinth);
        }

        public void UpdateLevel()
        {
            this.Level = LevelMgr.GetLevel(this.m_character.GP);
            int maxLevel = LevelMgr.MaxLevel;
            LevelInfo info = LevelMgr.FindLevel(maxLevel);
            if ((this.Level == maxLevel) && (info != null))
            {
                this.m_character.GP = info.GP;
            }
        }

        public void UpdatePet(UsersPetinfo pet)
        {
            this.m_pet = pet;
        }

        public void UpdateProperties()
        {
            this.Out.SendUpdatePrivateInfo(this.m_character);
            GSPacketIn pkg = this.Out.SendUpdatePublicPlayer(this.m_character, this.m_battle.MatchInfo);
            if (this.m_currentRoom != null)
            {
                this.m_currentRoom.SendToAll(pkg, this);
            }
        }

        public void UpdatePveResult(string type, int value, bool isWin)
        {
            int num = 0;
            string translation = "";
            if (type != null)
            {
                int num3;
                if (type != "worldboss")
                {
                    if (type != "consortiaboss")
                    {
                        if (type != "yearmonter")
                        {
                            if (type == "qx")
                            {
                                if (!isWin)
                                {
                                    List<SqlDataProvider.Data.ItemInfo> info = null;
                                    DropInventory.CopyAllDrop(value, ref info);
                                    int num2 = value - 0x11170;
                                    if (value >= 0x11176)
                                    {
                                        num2 -= 2;
                                    }
                                    string title = "Phần thưởng tham gia Đảo hải tặc đợt " + num2;
                                    if (info != null)
                                    {
                                        WorldEventMgr.SendItemsToMail(info, this.PlayerCharacter.ID, this.PlayerCharacter.NickName, title);
                                    }
                                }
                                num = 0;
                            }
                        }
                        else
                        {
                            this.Actives.Info.DamageNum = value;
                            this.Actives.CreateYearMonterBoxState();
                            num = 0;
                        }
                    }
                    else
                    {
                        num3 = value / 800;
                        num = value / 0x4b0;
                        translation = string.Format("Lần tấn c\x00f4ng n\x00e0y nhận {0} cống hiến v\x00e0 {1} điểm vinh dự.", num3, num);
                        this.AddRichesOffer(num3);
                        ConsortiaBossMgr.UpdateBlood(this.PlayerCharacter.ConsortiaID, value);
                        ConsortiaBossMgr.UpdateRank(this.PlayerCharacter.ConsortiaID, value, num3, num, this.PlayerCharacter.NickName, this.PlayerCharacter.ID);
                    }
                }
                else
                {
                    num3 = value / 400;
                    num = value / 0x4b0;
                    translation = LanguageMgr.GetTranslation("GamePlayer.Msg20", new object[] { num3, num });
                    this.AddDamageScores(num3);
                    RoomMgr.WorldBossRoom.UpdateRank(num3, num, this.PlayerCharacter.NickName);
                    RoomMgr.WorldBossRoom.ReduceBlood(value);
                    if (isWin)
                    {
                        RoomMgr.WorldBossRoom.FightOverAll();
                    }
                }
            }
            this.AddHonor(num);
            if (!string.IsNullOrEmpty(translation))
            {
                this.SendMessage(translation);
            }
        }

        public void UpdateReduceDame(SqlDataProvider.Data.ItemInfo item)
        {
            if ((item != null) && (item.Template != null))
            {
                this.PlayerCharacter.ReduceDamePlus = item.Template.Property1;
            }
        }

        public void UpdateRestCount()
        {
            this.BattleData.Update();
        }

        public void UpdateSecondWeapon(SqlDataProvider.Data.ItemInfo item)
        {
            if (item != this.m_currentSecondWeapon)
            {
                this.m_currentSecondWeapon = item;
                this.OnPropertiesChanged();
            }
        }

        public void UpdateStyle(string style, string colors, string skin)
        {
            if (((style != this.m_character.Style) || (colors != this.m_character.Colors)) || (skin != this.m_character.Skin))
            {
                this.m_character.Style = style;
                this.m_character.Colors = colors;
                this.m_character.Skin = skin;
                this.OnPropertiesChanged();
            }
        }

        public void UpdateTimeBox(int receiebox, int receieGrade, int needGetBoxTime)
        {
            this.m_character.receiebox = receiebox;
            this.m_character.receieGrade = receieGrade;
            this.m_character.needGetBoxTime = needGetBoxTime;
        }

        public void UpdateWeaklessGuildProgress()
        {
            if (this.PlayerCharacter.weaklessGuildProgress == null)
            {
                this.PlayerCharacter.weaklessGuildProgress = Base64.decodeToByteArray(this.PlayerCharacter.WeaklessGuildProgressStr);
            }
            this.PlayerCharacter.CheckLevelFunction();
            if (this.PlayerCharacter.Grade == 1)
            {
                this.PlayerCharacter.openFunction(Step.POP_MOVE);
            }
            if (this.PlayerCharacter.IsOldPlayer)
            {
                this.PlayerCharacter.openFunction(Step.OLD_PLAYER);
            }
            this.PlayerCharacter.WeaklessGuildProgressStr = Base64.encodeByteArray(this.PlayerCharacter.weaklessGuildProgress);
        }

        public void UpdateWeapon(SqlDataProvider.Data.ItemInfo item)
        {
            if (item != this.m_MainWeapon)
            {
                this.m_MainWeapon = item;
                this.OnPropertiesChanged();
            }
        }

        public bool UseKingBlessHelpStraw(eRoomType roomType)
        {
            if ((roomType == eRoomType.Lanbyrinth) || (roomType == eRoomType.Dungeon))
            {
                if (this.Extra.UseKingBless(4))
                {
                    return true;
                }
                if (this.BufferList.UserSaveLifeBuff())
                {
                    return true;
                }
            }
            return false;
        }

        public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
        {
            if (bag == 1)
            {
                if (PropItemMgr.PropBag.Contains<int>(templateId) && (place == -1))
                {
                    ItemTemplateInfo info = PropItemMgr.FindFightingProp(templateId);
                    if (isLiving && (info != null))
                    {
                        this.OnUsingItem(info.TemplateID);
                        if ((place == -1) && this.CanUseProp)
                        {
                            return true;
                        }
                        SqlDataProvider.Data.ItemInfo item = this.GetItemAt(eBageType.PropBag, place);
                        if (((item != null) && item.IsValidItem()) && (item.Count >= 0))
                        {
                            item.Count--;
                            this.UpdateItem(item);
                            return true;
                        }
                    }
                }
                return false;
            }
            SqlDataProvider.Data.ItemInfo itemAt = this.GetItemAt(eBageType.FightBag, place);
            if (itemAt.TemplateID == templateId)
            {
                this.OnUsingItem(itemAt.TemplateID);
                return this.RemoveAt(eBageType.FightBag, place);
            }
            return false;
        }

        public void ViFarmsAdd(int playerID)
        {
            if (!this._viFarms.Contains(playerID))
            {
                this._viFarms.Add(playerID);
            }
        }

        public void ViFarmsRemove(int playerID)
        {
            if (this._viFarms.Contains(playerID))
            {
                this._viFarms.Remove(playerID);
            }
        }

        public bool YbxRemoveTempate(eBageType bagType, ItemTemplateInfo template, int count, ref bool IsBind)
        {
            PlayerInventory inventory = this.GetInventory(bagType);
            return ((inventory != null) && inventory.YbxRemoveTemplate(template.TemplateID, count, ref IsBind));
        }

        public bool YbxRemoveTemplate(int templateId, int count, ref bool IsBind)
        {
            int itemCount = this.m_equipBag.GetItemCount(templateId);
            int num2 = this.m_propBag.GetItemCount(templateId);
            int num3 = this.m_ConsortiaBag.GetItemCount(templateId);
            int num4 = (itemCount + num2) + num3;
            ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateId);
            if ((template != null) && (num4 >= count))
            {
                if (((itemCount > 0) && (count > 0)) && this.YbxRemoveTempate(eBageType.EquipBag, template, (itemCount > count) ? count : itemCount, ref IsBind))
                {
                    count = (count < itemCount) ? 0 : (count - itemCount);
                }
                if (((num2 > 0) && (count > 0)) && this.YbxRemoveTempate(eBageType.PropBag, template, (num2 > count) ? count : num2, ref IsBind))
                {
                    count = (count < num2) ? 0 : (count - num2);
                }
                if (((num3 > 0) && (count > 0)) && this.YbxRemoveTempate(eBageType.Consortia, template, (num3 > count) ? count : num3, ref IsBind))
                {
                    count = (count < num3) ? 0 : (count - num3);
                }
                if (count == 0)
                {
                    return true;
                }
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Item Remover Error：PlayerId {0} Remover TemplateId{1} Is Not Zero!", this.m_playerId, templateId));
                }
            }
            return false;
        }

        public string Account
        {
            get
            {
                return this.m_account;
            }
        }

        public PlayerActives Actives
        {
            get
            {
                return this.m_actives;
            }
        }

        public Game.Server.Achievement.AchievementInventory AchievementInventory
        {
            get
            {
                return this.m_achievementInventory;
            }
        }

        public long AllWorldDameBoss { get; set; }

        public PlayerAvatarCollection AvatarCollect
        {
            get
            {
                return this.m_avatarcollect;
            }
        }

        public PlayerBattle BattleData
        {
            get
            {
                return this.m_battle;
            }
        }

        public PlayerBeadInventory BeadBag
        {
            get
            {
                return this.m_BeadBag;
            }
        }

        public bool bool_1 { get; set; }

        public bool Boolean_0
        {
            get
            {
                return this.bool_1;
            }
            set
            {
                this.bool_1 = value;
            }
        }

        public Game.Server.Buffer.BufferList BufferList
        {
            get
            {
                return this.m_bufferList;
            }
        }

        public PlayerInventory CaddyBag
        {
            get
            {
                return this.m_caddyBag;
            }
        }

        public bool CanUseProp { get; set; }

        public bool CanX2Exp { get; set; }

        public bool CanX3Exp { get; set; }

        public CardInventory CardBag
        {
            get
            {
                return this.m_cardBag;
            }
        }

        public GameClient Client
        {
            get
            {
                return this.m_client;
            }
        }

        public PlayerInventory ConsortiaBag
        {
            get
            {
                return this.m_ConsortiaBag;
            }
        }

        public HotSpringRoom CurrentHotSpringRoom
        {
            get
            {
                return this.hotSpringRoom_0;
            }
            set
            {
                this.hotSpringRoom_0 = value;
            }
        }

        public MarryRoom CurrentMarryRoom
        {
            get
            {
                return this.m_currentMarryRoom;
            }
            set
            {
                this.m_currentMarryRoom = value;
            }
        }

        public BaseRoom CurrentRoom
        {
            get
            {
                return this.m_currentRoom;
            }
            set
            {
                BaseRoom room = Interlocked.Exchange<BaseRoom>(ref this.m_currentRoom, value);
                if (room != null)
                {
                    RoomMgr.ExitRoom(room, this);
                }
            }
        }

        public BaseSevenDoubleRoom CurrentSevenDoubleRoom
        {
            get
            {
                return this.m_currentSevenDoubleRoom;
            }
            set
            {
                this.m_currentSevenDoubleRoom = value;
            }
        }

        public PlayerDice Dice
        {
            get
            {
                return this.m_dice;
            }
        }

        public PlayerDressModel DressModel
        {
            get
            {
                return this.m_dressmodel;
            }
        }

        public PlayerEquipInventory EquipBag
        {
            get
            {
                return this.m_equipBag;
            }
        }

        public List<SqlDataProvider.Data.ItemInfo> EquipEffect
        {
            get
            {
                return this.m_equipEffect;
            }
            set
            {
                this.m_equipEffect = value;
            }
        }

        public PlayerExtra Extra
        {
            get
            {
                return this.m_extra;
            }
        }

        public PlayerFarm Farm
        {
            get
            {
                return this.m_farm;
            }
        }

        public PlayerInventory FarmBag
        {
            get
            {
                return this.m_farmBag;
            }
        }

        public PlayerInventory FightBag
        {
            get
            {
                return this.m_fightBag;
            }
        }

        public List<BufferInfo> FightBuffs
        {
            get
            {
                return this.m_fightBuffInfo;
            }
            set
            {
                this.m_fightBuffInfo = value;
            }
        }

        public PlayerInventory Food
        {
            get
            {
                return this.m_food;
            }
        }

        public Dictionary<int, int> Friends
        {
            get
            {
                return this._friends;
            }
        }

        public BaseGame game
        {
            get
            {
                return this.m_game;
            }
            set
            {
                this.m_game = value;
            }
        }

        public int GameId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int GamePlayerId { get; set; }

        public List<UserGemStone> GemStone
        {
            get
            {
                return this.m_GemStone;
            }
            set
            {
                this.m_GemStone = value;
            }
        }

        public SqlDataProvider.Data.ItemInfo Healstone
        {
            get
            {
                if (this.m_healstone == null)
                {
                    return null;
                }
                return this.m_healstone;
            }
        }

        public int Immunity
        {
            get
            {
                return this.m_immunity;
            }
            set
            {
                this.m_immunity = value;
            }
        }

        public bool IsAASInfo
        {
            get
            {
                return this.m_isAASInfo;
            }
            set
            {
                this.m_isAASInfo = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.m_client.IsConnected;
            }
        }

        public bool IsInMarryRoom
        {
            get
            {
                return (this.m_currentMarryRoom != null);
            }
        }

        public bool IsMinor
        {
            get
            {
                return this.m_isMinor;
            }
            set
            {
                this.m_isMinor = value;
            }
        }

        public UserLabyrinthInfo Labyrinth
        {
            get
            {
                return this.m_Labyrinth;
            }
            set
            {
                this.m_Labyrinth = value;
            }
        }

        public int Level
        {
            get
            {
                return this.m_character.Grade;
            }
            set
            {
                if (value != this.m_character.Grade)
                {
                    this.m_character.Grade = value;
                    this.OnLevelUp(value);
                    this.OnPropertiesChanged();
                }
            }
        }

        public int LevelPlusBlood
        {
            get
            {
                return LevelMgr.FindLevel(this.m_character.Grade).Blood;
            }
        }

        public PlayerMagicStoneInventory MagicStoneBag
        {
            get
            {
                return this.m_magicStoneBag;
            }
        }

        public ItemTemplateInfo MainWeapon
        {
            get
            {
                if (this.m_MainWeapon == null)
                {
                    return null;
                }
                if (this.m_MainWeapon.IsValidGoldItem() && (this.m_MainWeapon.GoldEquip != null))
                {
                    return this.m_MainWeapon.GoldEquip;
                }
                return this.m_MainWeapon.Template;
            }
        }

        public UserMatchInfo MatchInfo
        {
            get
            {
                return this.m_battle.MatchInfo;
            }
        }

        public IPacketLib Out
        {
            get
            {
                return this.m_client.Out;
            }
        }

        public UsersPetinfo Pet
        {
            get
            {
                return this.m_pet;
            }
        }

        public PetInventory PetBag
        {
            get
            {
                return this.m_petBag;
            }
        }

        public PlayerInventory PetEgg
        {
            get
            {
                return this.m_petEgg;
            }
        }

        public long PingTime
        {
            get
            {
                return this.m_pingTime;
            }
            set
            {
                this.m_pingTime = value;
                GSPacketIn pkg = this.Out.SendNetWork(this, this.m_pingTime);
                if (this.m_currentRoom != null)
                {
                    this.m_currentRoom.SendToAll(pkg, this);
                }
            }
        }

        public PlayerInfo PlayerCharacter
        {
            get
            {
                return this.m_character;
            }
        }

        public int PlayerId
        {
            get
            {
                return this.m_playerId;
            }
        }

        public PlayerProperty PlayerProp
        {
            get
            {
                return this.m_playerProp;
            }
        }

        public Player Players
        {
            get
            {
                return this.m_players;
            }
        }

        public ePlayerState PlayerState
        {
            get
            {
                return this.m_playerState;
            }
            set
            {
                this.m_playerState = value;
            }
        }

        public string ProcessLabyrinthAward { get; set; }

        public PlayerInventory PropBag
        {
            get
            {
                return this.m_propBag;
            }
        }

        public Game.Server.Quests.QuestInventory QuestInventory
        {
            get
            {
                return this.m_questInventory;
            }
        }

        public PlayerRank Rank
        {
            get
            {
                return this.m_rank;
            }
        }

        public SqlDataProvider.Data.ItemInfo SecondWeapon
        {
            get
            {
                if (this.m_currentSecondWeapon == null)
                {
                    return null;
                }
                return this.m_currentSecondWeapon;
            }
        }

        public int ServerID { get; set; }

        public bool ShowPP
        {
            get
            {
                return this.m_showPP;
            }
            set
            {
                this.m_showPP = value;
            }
        }

        public PlayerInventory StoreBag
        {
            get
            {
                return this.m_storeBag;
            }
        }

        public PlayerInventory TempBag
        {
            get
            {
                return this.m_tempBag;
            }
        }

        public Dictionary<string, object> TempProperties
        {
            get
            {
                return this.m_tempProperties;
            }
        }

        public bool Toemview
        {
            get
            {
                return this.m_toemview;
            }
            set
            {
                this.m_toemview = value;
            }
        }

        public PlayerTreasure Treasure
        {
            get
            {
                return this.m_treasure;
            }
        }

        public Dictionary<int, UserDrillInfo> UserDrills
        {
            get
            {
                return this.m_userDrills;
            }
            set
            {
                this.m_userDrills = value;
            }
        }

        public UserVIPInfo UserVIPInfo_0
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public PlayerInventory Vegetable
        {
            get
            {
                return this.m_vegetable;
            }
        }

        public List<int> ViFarms
        {
            get
            {
                return this._viFarms;
            }
        }

        public long WorldbossBood { get; set; }

        public int ZoneId
        {
            get
            {
                return GameServer.Instance.Configuration.ServerID;
            }
        }

        public string ZoneName
        {
            get
            {
                return GameServer.Instance.Configuration.ServerName;
            }
        }

        public delegate void GameKillDropEventHandel(AbstractGame game, int type, int npcId, bool playResult);

        public delegate void PlayerAchievementFinish(AchievementData info);

        public delegate void PlayerAdoptPetEventHandle();

        public delegate void PlayerCropPrimaryEventHandle();

        public delegate void PlayerEnterHotSpring(GamePlayer player);

        public delegate void PlayerEventHandle(GamePlayer player);

        public delegate void PlayerFightAddOffer(int offer);

        public delegate void PlayerFightOneBloodIsWin(eRoomType roomType);

        public delegate void PlayerGameKillBossEventHandel(AbstractGame game, NpcInfo npc, int damage);

        public delegate void PlayerGameKillEventHandel(AbstractGame game, int type, int id, bool isLiving, int demage);

        public delegate void PlayerGameOverEventHandle(AbstractGame game, bool isWin, int gainXp);

        public delegate void PlayerGoldCollection(int value);

        public delegate void PlayerGiftTokenCollection(int value);

        public delegate void PlayerHotSpingExpAdd(int minutes, int exp);

        public delegate void PlayerItemComposeEventHandle(int composeType);

        public delegate void PlayerItemFusionEventHandle(int fusionType);

        public delegate void PlayerItemInsertEventHandle();

        public delegate void PlayerItemMeltEventHandle(int categoryID);

        public delegate void PlayerItemPropertyEventHandle(int templateID);

        public delegate void PlayerItemStrengthenEventHandle(int categoryID, int level);

        public delegate void PlayerMissionFullOverEventHandle(AbstractGame game, int missionId, bool isWin, int turnNum);

        public delegate void PlayerMissionOverEventHandle(AbstractGame game, int missionId, bool isWin);

        public delegate void PlayerMissionTurnOverEventHandle(AbstractGame game, int missionId, int turnNum);

        public delegate void PlayerNewGearEventHandle(int CategoryID);

        public delegate void PlayerNewGearEventHandle2(SqlDataProvider.Data.ItemInfo item);

        public delegate void PlayerOwnConsortiaEventHandle();

        public delegate void PlayerPropertisChange(PlayerInfo player);

        public delegate void PlayerQuestFinish(QuestDataInfo data, QuestInfo info);

        public delegate void PlayerSeedFoodPetEventHandle();

        public delegate void PlayerShopEventHandle(int money, int gold, int offer, int gifttoken, int medal, string payGoods);

        public delegate void PlayerUnknowQuestConditionEventHandle();

        public delegate void PlayerUpLevelPetEventHandle();

        public delegate void PlayerUseBugle(int value);

        public delegate void PlayerUserToemGemstoneEventHandle();

        public delegate void PlayerVIPUpgrade(int level, int exp);
    }
}


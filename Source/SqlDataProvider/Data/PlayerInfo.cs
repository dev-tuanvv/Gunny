namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class PlayerInfo : DataObject
    {
        private int _agility;
        private int _antiAddiction;
        private DateTime _antiDate;
        private int _attack;
        private int _badLuckNumber;
        private bool _canTakeVipReward;
        private int _cardSoul;
        private string _colors;
        private int _consortiaID;
        private string _consortiaName;
        private bool _consortiaRename;
        private int _CurrentDressModel;
        private string _checkCode;
        private int _checkCount;
        private DateTime _checkDate;
        private int _checkError;
        private int _damageScores;
        private int _dayLoginCount;
        private int _DDPlayPoint;
        private int _defence;
        private int _escape;
        private int _evolutionExp;
        private int _evolutionGrade;
        private DateTime? _expendDate;
        private int _fightPower;
        private int _GetSoulCount;
        private int _gold;
        private int _GoXu;
        private int _gp;
        private int _grade;
        private int _giftGp;
        private int _giftLevel;
        private int _GiftToken;
        private int _hardCurrency;
        private int _hide;
        private PlayerInfoHistory _history;
        private string _honor;
        private int _hp;
        private int _HP;
        private int _id;
        private int _inviter;
        private bool _isConsortia;
        private bool _isCreatedMarryRoom;
        private int _IsFirst;
        private int _isFirstDivorce;
        private bool _isFistGetPet;
        private bool _isGotRing;
        private bool _isLocked = true;
        private bool _isMarried;
        private bool _isOldPlayer;
        private bool _isOldPlayerHasValidEquitAtLogin;
        private bool _isOpenKingBless;
        private bool _isShowConsortia;
        private DateTime _LastAuncherAward;
        private DateTime _LastAward;
        private int _lastLuckNum;
        private DateTime _lastLuckyNumDate;
        private DateTime _LastVIPPackTime;
        private DateTime _LastWeekly;
        private int _LastWeeklyVersion;
        private int _leagueMoney;
        private int _luck;
        private int _luckyNum;
        private int _magicAttack;
        private int _magicdefence;
        private int _MagicStonePoint;
        private int _marryInfoID;
        private int _maxBuyHonor;
        private int _medal;
        private int _money;
        private int _MoneyPlus;
        private int _myHonor;
        private int _necklaceExp;
        private int _necklaceExpAdd;
        private string _nickName;
        private int _nimbus;
        private int _offer;
        private int _optionOnOff;
        private string _password;
        private string _PasswordTwo;
        private int _petScore;
        private byte[] _QuestSite;
        private bool _rename;
        private int _repute;
        private int _richesOffer;
        private int _richesRob;
        private int _score;
        private int _selfMarryRoomID;
        private bool _sex;
        private string _skin;
        private int _spouseID;
        private string _spouseName;
        private int _state;
        private string _style;
        private Dictionary<string, object> _tempInfo = new Dictionary<string, object>();
        private TexpInfo _texp;
        private DateTime _timeBox;
        private int _total;
        private int _totemId;
        private byte _typeVIP;
        private int _uesedFinishTime;
        private string _userName;
        private int _VIPExp;
        private DateTime _VIPExpireDay;
        private int _VIPLevel;
        private int _VIPNextLevelDaysNeeded;
        private int _VIPOfflineDays;
        private int _VIPOnlineDays;
        private byte[] _weaklessGuildProgress;
        private string _weaklessGuildProgressStr;
        private int _win;
        public int AgiAddPlus;
        public int AttackAddPlus;
        public int DameAddPlus;
        public int DefendAddPlus;
        public int GuardAddPlus;
        public int HpAddPlus;
        public int LuckAddPlus;
        private int m_AchievementPoint;
        private int m_AddDayAchievementPoint;
        private DateTime m_AddGPLastDate;
        private int m_AddWeekAchievementPoint;
        private int m_AlreadyGetBox;
        private int m_AnswerSite;
        private int m_BanChat;
        private DateTime m_BanChatEndDate;
        private int m_bossguildid;
        private DateTime m_BoxGetDate;
        private int m_BoxProgression;
        private int m_ChatCount;
        private int m_FailedPasswordAttemptCount;
        private string m_fightlabPermission;
        private int m_gameActiveHide;
        private string m_gameActiveStyle;
        private int m_getBoxLevel;
        private bool m_IsInSpaPubGoldToday;
        private bool m_IsInSpaPubMoneyToday;
        private bool m_IsOpenGift;
        private DateTime m_lastDate;
        private DateTime m_LastGetEgg;
        private DateTime m_LastRefreshPet;
        private DateTime m_LastSpaDate;
        private int m_myScore;
        private int m_needGetBoxTime;
        private int m_OnlineTime;
        private string m_PasswordQuest1;
        private string m_PasswordQuest2;
        private string m_pvePermission;
        private string m_Rank;
        private int m_receiebox;
        private int m_receieGrade;
        private int m_SpaPubGoldRoomLimit;
        private int m_SpaPubMoneyRoomLimit;
        private DateTime m_VIPlastDate;
        public int ReduceDamePlus;
        public int StrengthEnchance;

        public bool bit(int param1)
        {
            param1--;
            int index = param1 / 8;
            int num2 = param1 % 8;
            int num3 = this._weaklessGuildProgress[index] & (((int) 1) << num2);
            return (num3 != 0);
        }

        public void ClearConsortia()
        {
            this.ConsortiaID = 0;
            this.ConsortiaName = "";
            this.RichesOffer = 0;
            this.ConsortiaRepute = 0;
            this.ConsortiaLevel = 0;
            this.StoreLevel = 0;
            this.ShopLevel = 0;
            this.SmithLevel = 0;
            this.ConsortiaHonor = 0;
            this.RichesOffer = 0;
            this.RichesRob = 0;
            this.DutyLevel = 0;
            this.DutyName = "";
            this.Right = 0;
            this.AddDayGP = 0;
            this.AddWeekGP = 0;
            this.AddDayOffer = 0;
            this.AddWeekOffer = 0;
            this.ConsortiaRiches = 0;
        }

        public void CheckLevelFunction()
        {
            int grade = this.Grade;
            if (grade > 1)
            {
                this.openFunction(Step.POP_WELCOME);
                this.openFunction(Step.CHANNEL_OPEN);
            }
            if (grade > 2)
            {
                this.openFunction(Step.SHOP_OPEN);
                this.openFunction(Step.STORE_OPEN);
                this.openFunction(Step.ENERGY);
                this.openFunction(Step.MAIL_OPEN);
                this.openFunction(Step.SIGN_OPEN);
            }
            if (grade > 3)
            {
                this.openFunction(Step.HP_PROP_OPEN);
            }
            if (grade > 4)
            {
                this.openFunction(Step.MOVE);
                this.openFunction(Step.CIVIL_OPEN);
                this.openFunction(Step.IM_OPEN);
                this.openFunction(Step.PLAY_ONE_GLOW);
            }
            if (grade > 5)
            {
                this.openFunction(Step.BEAT_ROBOT);
                this.openFunction(Step.MASTER_ROOM_OPEN);
                this.openFunction(Step.POP_ANGLE);
            }
            if (grade > 6)
            {
                this.openFunction(Step.POP_THREE_FOUR_FIVE);
                this.openFunction(Step.CONSORTIA_OPEN);
                this.openFunction(Step.SPAWN_SMALL_BOGU);
                this.openFunction(Step.PLANE_PROP_OPEN);
            }
            if (grade > 7)
            {
                this.openFunction(Step.CONSORTIA_SHOW);
                this.openFunction(Step.DUNGEON_OPEN);
                this.openFunction(Step.POP_WIN_II);
            }
            if (grade > 8)
            {
                this.openFunction(Step.DUNGEON_SHOW);
                this.openFunction(Step.BEAT_LIVING_LEFT);
            }
            if (grade > 9)
            {
                this.openFunction(Step.CHURCH_OPEN);
            }
            if (grade > 11)
            {
                this.openFunction(Step.CHURCH_SHOW);
                this.openFunction(Step.TOFF_LIST_OPEN);
            }
            if (grade > 12)
            {
                this.openFunction(Step.TOFF_LIST_SHOW);
                this.openFunction(Step.HOT_WELL_OPEN);
            }
            if (grade > 13)
            {
                this.openFunction(Step.HOT_WELL_SHOW);
                this.openFunction(Step.AUCTION_OPEN);
            }
            if (grade > 14)
            {
                this.openFunction(Step.AUCTION_SHOW);
                this.openFunction(Step.CAMPAIGN_LAB_OPEN);
            }
        }

        public bool IsLastVIPPackTime()
        {
            return (this._LastVIPPackTime < DateTime.Now.Date);
        }

        public bool IsValidadteTimeBox()
        {
            return (this._timeBox.Date < DateTime.Now.Date);
        }

        public bool IsVIPExpire()
        {
            return (this._VIPExpireDay.Date < DateTime.Now.Date);
        }

        public bool IsWeakGuildFinish(int id)
        {
            return ((id >= 1) && this.bit(id));
        }

        public void openFunction(Step step)
        {
            int num = ((int) step) - 1;
            int index = num / 8;
            int num3 = num % 8;
            byte[] weaklessGuildProgress = this.weaklessGuildProgress;
            if (weaklessGuildProgress.Length > 0)
            {
                weaklessGuildProgress[index] = (byte) (weaklessGuildProgress[index] | (((int) 1) << num3));
                this.weaklessGuildProgress = weaklessGuildProgress;
            }
        }

        public int _badgeID { get; set; }

        public int AchievementPoint
        {
            get
            {
                return this.m_AchievementPoint;
            }
            set
            {
                this.m_AchievementPoint = value;
            }
        }

        public string AchievementProcess
        {
            get
            {
                return this.string_16;
            }
            set
            {
                this.string_16 = value;
                base._isDirty = true;
            }
        }

        public int AddDayAchievementPoint
        {
            get
            {
                return this.m_AddDayAchievementPoint;
            }
            set
            {
                this.m_AddDayAchievementPoint = value;
            }
        }

        public int AddDayGP { get; set; }

        public int AddDayGiftGp { get; set; }

        public int AddDayOffer { get; set; }

        public DateTime AddGPLastDate
        {
            get
            {
                return this.m_AddGPLastDate;
            }
            set
            {
                this.m_AddGPLastDate = value;
            }
        }

        public int AddWeekAchievementPoint
        {
            get
            {
                return this.m_AddWeekAchievementPoint;
            }
            set
            {
                this.m_AddWeekAchievementPoint = value;
            }
        }

        public int AddWeekGP { get; set; }

        public int AddWeekGiftGp { get; set; }

        public int AddWeekOffer { get; set; }

        public int Agility
        {
            get
            {
                return this._agility;
            }
            set
            {
                this._agility = value;
                base._isDirty = true;
            }
        }

        public int AlreadyGetBox
        {
            get
            {
                return this.m_AlreadyGetBox;
            }
            set
            {
                this.m_AlreadyGetBox = value;
            }
        }

        public int AnswerSite
        {
            get
            {
                return this.m_AnswerSite;
            }
            set
            {
                this.m_AnswerSite = value;
            }
        }

        public int AntiAddiction
        {
            get
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - this._antiDate);
                return (this._antiAddiction + ((int) span.TotalMinutes));
            }
            set
            {
                this._antiAddiction = value;
                this._antiDate = DateTime.Now;
            }
        }

        public DateTime AntiDate
        {
            get
            {
                return this._antiDate;
            }
            set
            {
                this._antiDate = value;
            }
        }

        public int Attack
        {
            get
            {
                return this._attack;
            }
            set
            {
                this._attack = value;
                base._isDirty = true;
            }
        }

        public int badgeID
        {
            get
            {
                return this._badgeID;
            }
            set
            {
                this._badgeID = value;
                base._isDirty = true;
            }
        }

        public int badLuckNumber
        {
            get
            {
                return this._badLuckNumber;
            }
            set
            {
                this._badLuckNumber = value;
            }
        }

        public int BanChat
        {
            get
            {
                return this.m_BanChat;
            }
            set
            {
                this.m_BanChat = value;
            }
        }

        public DateTime BanChatEndDate
        {
            get
            {
                return this.m_BanChatEndDate;
            }
            set
            {
                this.m_BanChatEndDate = value;
            }
        }

        public int BossGuildID
        {
            get
            {
                return this.m_bossguildid;
            }
            set
            {
                this.m_bossguildid = value;
            }
        }

        public DateTime BoxGetDate
        {
            get
            {
                return this.m_BoxGetDate;
            }
            set
            {
                this.m_BoxGetDate = value;
            }
        }

        public int BoxProgression
        {
            get
            {
                return this.m_BoxProgression;
            }
            set
            {
                this.m_BoxProgression = value;
            }
        }

        public bool CanTakeVipReward
        {
            get
            {
                return this._canTakeVipReward;
            }
            set
            {
                this._canTakeVipReward = value;
                base._isDirty = true;
            }
        }

        public int CardSoul
        {
            get
            {
                return this._cardSoul;
            }
            set
            {
                this._cardSoul = value;
            }
        }

        public string Colors
        {
            get
            {
                return this._colors;
            }
            set
            {
                this._colors = value;
                base._isDirty = true;
            }
        }

        public int ConsortiaGiftGp { get; set; }

        public int ConsortiaHonor { get; set; }

        public int ConsortiaID
        {
            get
            {
                return this._consortiaID;
            }
            set
            {
                if ((this._consortiaID == 0) || (value == 0))
                {
                    this._richesRob = 0;
                    this._richesOffer = 0;
                }
                this._consortiaID = value;
            }
        }

        public int ConsortiaLevel { get; set; }

        public string ConsortiaName
        {
            get
            {
                return this._consortiaName;
            }
            set
            {
                this._consortiaName = value;
            }
        }

        public bool ConsortiaRename
        {
            get
            {
                return this._consortiaRename;
            }
            set
            {
                if (this._consortiaRename != value)
                {
                    this._consortiaRename = value;
                    base._isDirty = true;
                }
            }
        }

        public int ConsortiaRepute { get; set; }

        public int ConsortiaRiches { get; set; }

        public int CurrentDressModel
        {
            get
            {
                return this._CurrentDressModel;
            }
            set
            {
                this._CurrentDressModel = value;
                base._isDirty = true;
            }
        }

        public string ChairmanName { get; set; }

        public int ChatCount
        {
            get
            {
                return this.m_ChatCount;
            }
            set
            {
                this.m_ChatCount = value;
            }
        }

        public string CheckCode
        {
            get
            {
                return this._checkCode;
            }
            set
            {
                this._checkDate = DateTime.Now;
                this._checkCode = value;
                string.IsNullOrEmpty(this._checkCode);
            }
        }

        public int CheckCount
        {
            get
            {
                return this._checkCount;
            }
            set
            {
                this._checkCount = value;
                base._isDirty = true;
            }
        }

        public DateTime CheckDate
        {
            get
            {
                return this._checkDate;
            }
        }

        public int CheckError
        {
            get
            {
                return this._checkError;
            }
            set
            {
                this._checkError = value;
            }
        }

        public int damageScores
        {
            get
            {
                return this._damageScores;
            }
            set
            {
                this._damageScores = value;
            }
        }

        public int DayLoginCount
        {
            get
            {
                return this._dayLoginCount;
            }
            set
            {
                this._dayLoginCount = value;
                base._isDirty = true;
            }
        }

        public int DDPlayPoint
        {
            get
            {
                return this._DDPlayPoint;
            }
            set
            {
                this._DDPlayPoint = value;
            }
        }

        public int Defence
        {
            get
            {
                return this._defence;
            }
            set
            {
                this._defence = value;
                base._isDirty = true;
            }
        }

        public int DutyLevel { get; set; }

        public string DutyName { get; set; }

        public int Escape
        {
            get
            {
                return this._escape;
            }
            set
            {
                this._escape = value;
                base._isDirty = true;
            }
        }

        public int evolutionExp
        {
            get
            {
                return this._evolutionExp;
            }
            set
            {
                this._evolutionExp = value;
            }
        }

        public int evolutionGrade
        {
            get
            {
                return this._evolutionGrade;
            }
            set
            {
                this._evolutionGrade = value;
            }
        }

        public DateTime? ExpendDate
        {
            get
            {
                return this._expendDate;
            }
            set
            {
                this._expendDate = value;
                base._isDirty = true;
            }
        }

        public int FailedPasswordAttemptCount
        {
            get
            {
                return this.m_FailedPasswordAttemptCount;
            }
            set
            {
                this.m_FailedPasswordAttemptCount = value;
            }
        }

        public string FightLabPermission
        {
            get
            {
                return this.m_fightlabPermission;
            }
            set
            {
                this.m_fightlabPermission = value;
            }
        }

        public int FightPower
        {
            get
            {
                return this._fightPower;
            }
            set
            {
                if (this._fightPower != value)
                {
                    this._fightPower = value;
                    base._isDirty = true;
                }
            }
        }

        public int GameActiveHide
        {
            get
            {
                return this.m_gameActiveHide;
            }
            set
            {
                this.m_gameActiveHide = value;
            }
        }

        public string GameActiveStyle
        {
            get
            {
                return this.m_gameActiveStyle;
            }
            set
            {
                this.m_gameActiveStyle = value;
            }
        }

        public int GetBoxLevel
        {
            get
            {
                return this.m_getBoxLevel;
            }
            set
            {
                this.m_getBoxLevel = value;
            }
        }

        public int GetSoulCount
        {
            get
            {
                return this._GetSoulCount;
            }
            set
            {
                this._GetSoulCount = value;
            }
        }

        public int Gold
        {
            get
            {
                return this._gold;
            }
            set
            {
                this._gold = value;
                base._isDirty = true;
            }
        }

        public int GoXu
        {
            get
            {
                return this._GoXu;
            }
            set
            {
                this._GoXu = value;
            }
        }

        public int GP
        {
            get
            {
                return this._gp;
            }
            set
            {
                this._gp = value;
                base._isDirty = true;
            }
        }

        public int Grade
        {
            get
            {
                return this._grade;
            }
            set
            {
                this._grade = value;
                base._isDirty = true;
            }
        }

        public int GiftGp
        {
            get
            {
                return this._giftGp;
            }
            set
            {
                this._giftGp = value;
                base._isDirty = true;
            }
        }

        public int GiftLevel
        {
            get
            {
                return this._giftLevel;
            }
            set
            {
                this._giftLevel = value;
                base._isDirty = true;
            }
        }

        public int GiftToken
        {
            get
            {
                return this._GiftToken;
            }
            set
            {
                this._GiftToken = value;
            }
        }

        public int hardCurrency
        {
            get
            {
                return this._hardCurrency;
            }
            set
            {
                this._hardCurrency = value;
            }
        }

        public bool HasBagPassword
        {
            get
            {
                return !string.IsNullOrEmpty(this._PasswordTwo);
            }
        }

        public int Hide
        {
            get
            {
                return this._hide;
            }
            set
            {
                this._hide = value;
                base._isDirty = true;
            }
        }

        public PlayerInfoHistory History
        {
            get
            {
                return this._history;
            }
            set
            {
                this._history = value;
            }
        }

        public string Honor
        {
            get
            {
                return this._honor;
            }
            set
            {
                this._honor = value;
            }
        }

        public int hp
        {
            get
            {
                return this._hp;
            }
            set
            {
                this._hp = value;
                base._isDirty = true;
            }
        }

        public int HP
        {
            get
            {
                return this._HP;
            }
            set
            {
                this._HP = value;
                base._isDirty = true;
            }
        }

        public int ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
                base._isDirty = true;
            }
        }

        public int Inviter
        {
            get
            {
                return this._inviter;
            }
            set
            {
                this._inviter = value;
            }
        }

        public bool IsAutoBot { get; set; }

        public bool IsBanChat { get; set; }

        public bool IsConsortia
        {
            get
            {
                return this._isConsortia;
            }
            set
            {
                this._isConsortia = value;
            }
        }

        public bool IsCreatedMarryRoom
        {
            get
            {
                return this._isCreatedMarryRoom;
            }
            set
            {
                if (this._isCreatedMarryRoom != value)
                {
                    this._isCreatedMarryRoom = value;
                    base._isDirty = true;
                }
            }
        }

        public int IsFirst
        {
            get
            {
                return this._IsFirst;
            }
            set
            {
                this._IsFirst = value;
            }
        }

        public int isFirstDivorce
        {
            get
            {
                return this._isFirstDivorce;
            }
            set
            {
                this._isFirstDivorce = value;
            }
        }

        public bool IsFistGetPet
        {
            get
            {
                return this._isFistGetPet;
            }
            set
            {
                this._isFistGetPet = value;
            }
        }

        public bool IsGotRing
        {
            get
            {
                return this._isGotRing;
            }
            set
            {
                if (this._isGotRing != value)
                {
                    this._isGotRing = value;
                    base._isDirty = true;
                }
            }
        }

        public bool IsInSpaPubGoldToday
        {
            get
            {
                return this.m_IsInSpaPubGoldToday;
            }
            set
            {
                this.m_IsInSpaPubGoldToday = value;
            }
        }

        public bool IsInSpaPubMoneyToday
        {
            get
            {
                return this.m_IsInSpaPubMoneyToday;
            }
            set
            {
                this.m_IsInSpaPubMoneyToday = value;
            }
        }

        public bool IsLocked
        {
            get
            {
                return this._isLocked;
            }
            set
            {
                this._isLocked = value;
            }
        }

        public bool IsMarried
        {
            get
            {
                return this._isMarried;
            }
            set
            {
                this._isMarried = value;
                base._isDirty = true;
            }
        }

        public bool IsOldPlayer
        {
            get
            {
                return this._isOldPlayer;
            }
            set
            {
                this._isOldPlayer = value;
                base._isDirty = true;
            }
        }

        public bool isOldPlayerHasValidEquitAtLogin
        {
            get
            {
                return this._isOldPlayerHasValidEquitAtLogin;
            }
            set
            {
                this._isOldPlayerHasValidEquitAtLogin = value;
            }
        }

        public bool IsOpenGift
        {
            get
            {
                return this.m_IsOpenGift;
            }
            set
            {
                this.m_IsOpenGift = value;
            }
        }

        public bool isOpenKingBless
        {
            get
            {
                return this._isOpenKingBless;
            }
            set
            {
                this._isOpenKingBless = value;
                base._isDirty = true;
            }
        }

        public bool IsShowConsortia
        {
            get
            {
                return this._isShowConsortia;
            }
            set
            {
                this._isShowConsortia = value;
            }
        }

        public DateTime LastAuncherAward
        {
            get
            {
                return this._LastAuncherAward;
            }
            set
            {
                this._LastAuncherAward = value;
            }
        }

        public DateTime LastAward
        {
            get
            {
                return this._LastAward;
            }
            set
            {
                this._LastAward = value;
            }
        }

        public DateTime LastDate
        {
            get
            {
                return this.m_lastDate;
            }
            set
            {
                this.m_lastDate = value;
            }
        }

        public DateTime LastGetEgg
        {
            get
            {
                return this.m_LastGetEgg;
            }
            set
            {
                this.m_LastGetEgg = value;
            }
        }

        public int lastLuckNum
        {
            get
            {
                return this._lastLuckNum;
            }
            set
            {
                this._lastLuckNum = value;
            }
        }

        public DateTime lastLuckyNumDate
        {
            get
            {
                return this._lastLuckyNumDate;
            }
            set
            {
                this._lastLuckyNumDate = value;
            }
        }

        public DateTime LastRefreshPet
        {
            get
            {
                return this.m_LastRefreshPet;
            }
            set
            {
                this.m_LastRefreshPet = value;
            }
        }

        public DateTime LastSpaDate
        {
            get
            {
                return this.m_LastSpaDate;
            }
            set
            {
                this.m_LastSpaDate = value;
            }
        }

        public DateTime LastVIPPackTime
        {
            get
            {
                return this._LastVIPPackTime;
            }
            set
            {
                this._LastVIPPackTime = value;
                base._isDirty = true;
            }
        }

        public DateTime LastWeekly
        {
            get
            {
                return this._LastWeekly;
            }
            set
            {
                this._LastWeekly = value;
            }
        }

        public int LastWeeklyVersion
        {
            get
            {
                return this._LastWeeklyVersion;
            }
            set
            {
                this._LastWeeklyVersion = value;
            }
        }

        public int LeagueMoney
        {
            get
            {
                return this._leagueMoney;
            }
            set
            {
                this._leagueMoney = value;
                base._isDirty = true;
            }
        }

        public int Luck
        {
            get
            {
                return this._luck;
            }
            set
            {
                this._luck = value;
                base._isDirty = true;
            }
        }

        public int luckyNum
        {
            get
            {
                return this._luckyNum;
            }
            set
            {
                this._luckyNum = value;
            }
        }

        public int MagicAttack
        {
            get
            {
                return this._magicAttack;
            }
            set
            {
                this._magicAttack = value;
            }
        }

        public int MagicDefence
        {
            get
            {
                return this._magicdefence;
            }
            set
            {
                this._magicdefence = value;
            }
        }

        public int MagicStonePoint
        {
            get
            {
                return this._MagicStonePoint;
            }
            set
            {
                this._MagicStonePoint = value;
            }
        }

        public int MarryInfoID
        {
            get
            {
                return this._marryInfoID;
            }
            set
            {
                if (this._marryInfoID != value)
                {
                    this._marryInfoID = value;
                    base._isDirty = true;
                }
            }
        }

        public int MaxBuyHonor
        {
            get
            {
                return this._maxBuyHonor;
            }
            set
            {
                this._maxBuyHonor = value;
                base._isDirty = true;
            }
        }

        public int medal
        {
            get
            {
                return this._medal;
            }
            set
            {
                this._medal = value;
                base._isDirty = true;
            }
        }

        public int Money
        {
            get
            {
                return this._money;
            }
            set
            {
                this._money = value;
                base._isDirty = true;
            }
        }

        public int MoneyPlus
        {
            get
            {
                return this._MoneyPlus;
            }
            set
            {
                this._MoneyPlus = value;
            }
        }

        public int myHonor
        {
            get
            {
                return this._myHonor;
            }
            set
            {
                this._myHonor = value;
            }
        }

        public int myScore
        {
            get
            {
                return this.m_myScore;
            }
            set
            {
                this.m_myScore = value;
            }
        }

        public int necklaceExp
        {
            get
            {
                return this._necklaceExp;
            }
            set
            {
                this._necklaceExp = value;
            }
        }

        public int necklaceExpAdd
        {
            get
            {
                return this._necklaceExpAdd;
            }
            set
            {
                this._necklaceExpAdd = value;
            }
        }

        public int needGetBoxTime
        {
            get
            {
                return this.m_needGetBoxTime;
            }
            set
            {
                this.m_needGetBoxTime = value;
            }
        }

        public string NickName
        {
            get
            {
                return this._nickName;
            }
            set
            {
                this._nickName = value;
                base._isDirty = true;
            }
        }

        public int Nimbus
        {
            get
            {
                return this._nimbus;
            }
            set
            {
                if (this._nimbus != value)
                {
                    this._nimbus = value;
                    base._isDirty = true;
                }
            }
        }

        public int Offer
        {
            get
            {
                return this._offer;
            }
            set
            {
                this._offer = value;
                base._isDirty = true;
            }
        }

        public int OnlineTime
        {
            get
            {
                return this.m_OnlineTime;
            }
            set
            {
                this.m_OnlineTime = value;
            }
        }

        public int OptionOnOff
        {
            get
            {
                return this._optionOnOff;
            }
            set
            {
                this._optionOnOff = value;
            }
        }

        public string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
                base._isDirty = true;
            }
        }

        public string PasswordQuest1
        {
            get
            {
                return this.m_PasswordQuest1;
            }
            set
            {
                this.m_PasswordQuest1 = value;
            }
        }

        public string PasswordQuest2
        {
            get
            {
                return this.m_PasswordQuest2;
            }
            set
            {
                this.m_PasswordQuest2 = value;
            }
        }

        public string PasswordTwo
        {
            get
            {
                return this._PasswordTwo;
            }
            set
            {
                this._PasswordTwo = value;
                base._isDirty = true;
            }
        }

        public int petScore
        {
            get
            {
                return this._petScore;
            }
            set
            {
                this._petScore = value;
            }
        }

        public string PvePermission
        {
            get
            {
                return this.m_pvePermission;
            }
            set
            {
                this.m_pvePermission = value;
            }
        }

        public byte[] QuestSite
        {
            get
            {
                return this._QuestSite;
            }
            set
            {
                this._QuestSite = value;
            }
        }

        public string Rank
        {
            get
            {
                return this.m_Rank;
            }
            set
            {
                this.m_Rank = value;
            }
        }

        public int receiebox
        {
            get
            {
                return this.m_receiebox;
            }
            set
            {
                this.m_receiebox = value;
            }
        }

        public int receieGrade
        {
            get
            {
                return this.m_receieGrade;
            }
            set
            {
                this.m_receieGrade = value;
            }
        }

        public bool Rename
        {
            get
            {
                return this._rename;
            }
            set
            {
                if (this._rename != value)
                {
                    this._rename = value;
                    base._isDirty = true;
                }
            }
        }

        public int Repute
        {
            get
            {
                return this._repute;
            }
            set
            {
                this._repute = value;
                base._isDirty = true;
            }
        }

        public int ReputeOffer { get; set; }

        public int Riches
        {
            get
            {
                return (this.RichesRob + this.RichesOffer);
            }
        }

        public int RichesOffer
        {
            get
            {
                return this._richesOffer;
            }
            set
            {
                this._richesOffer = value;
                base._isDirty = true;
            }
        }

        public int RichesRob
        {
            get
            {
                return this._richesRob;
            }
            set
            {
                this._richesRob = value;
                base._isDirty = true;
            }
        }

        public int Right { get; set; }

        public int RoomId { get; set; }

        public int Score
        {
            get
            {
                return this._score;
            }
            set
            {
                this._score = value;
            }
        }

        public int SelfMarryRoomID
        {
            get
            {
                return this._selfMarryRoomID;
            }
            set
            {
                if (this._selfMarryRoomID != value)
                {
                    this._selfMarryRoomID = value;
                    base._isDirty = true;
                }
            }
        }

        public bool Sex
        {
            get
            {
                return this._sex;
            }
            set
            {
                this._sex = value;
                base._isDirty = true;
            }
        }

        public int ShopLevel { get; set; }

        public int SkillLevel { get; set; }

        public string Skin
        {
            get
            {
                return this._skin;
            }
            set
            {
                this._skin = value;
                base._isDirty = true;
            }
        }

        public int SmithLevel { get; set; }

        public int SpaPubGoldRoomLimit
        {
            get
            {
                return this.m_SpaPubGoldRoomLimit;
            }
            set
            {
                this.m_SpaPubGoldRoomLimit = value;
            }
        }

        public int SpaPubMoneyRoomLimit
        {
            get
            {
                return this.m_SpaPubMoneyRoomLimit;
            }
            set
            {
                this.m_SpaPubMoneyRoomLimit = value;
            }
        }

        public int SpouseID
        {
            get
            {
                return this._spouseID;
            }
            set
            {
                if (this._spouseID != value)
                {
                    this._spouseID = value;
                    base._isDirty = true;
                }
            }
        }

        public string SpouseName
        {
            get
            {
                return this._spouseName;
            }
            set
            {
                if (this._spouseName != value)
                {
                    this._spouseName = value;
                    base._isDirty = true;
                }
            }
        }

        public int State
        {
            get
            {
                return this._state;
            }
            set
            {
                this._state = value;
                base._isDirty = true;
            }
        }

        public int StoreLevel { get; set; }

        public string Style
        {
            get
            {
                return this._style;
            }
            set
            {
                this._style = value;
                base._isDirty = true;
            }
        }

        private string string_16 { get; set; }

        public Dictionary<string, object> TempInfo
        {
            get
            {
                return this._tempInfo;
            }
        }

        public TexpInfo Texp
        {
            get
            {
                return this._texp;
            }
            set
            {
                this._texp = value;
                base._isDirty = true;
            }
        }

        public DateTime TimeBox
        {
            get
            {
                return this._timeBox;
            }
            set
            {
                this._timeBox = value;
            }
        }

        public int Total
        {
            get
            {
                return this._total;
            }
            set
            {
                this._total = value;
                base._isDirty = true;
            }
        }

        public int totemId
        {
            get
            {
                return this._totemId;
            }
            set
            {
                this._totemId = value;
            }
        }

        public byte typeVIP
        {
            get
            {
                return this._typeVIP;
            }
            set
            {
                if (this._typeVIP != value)
                {
                    this._typeVIP = value;
                    base._isDirty = true;
                }
            }
        }

        public int uesedFinishTime
        {
            get
            {
                return this._uesedFinishTime;
            }
            set
            {
                this._uesedFinishTime = value;
            }
        }

        public string UserName
        {
            get
            {
                return this._userName;
            }
            set
            {
                this._userName = value;
                base._isDirty = true;
            }
        }

        public int VIPExp
        {
            get
            {
                return this._VIPExp;
            }
            set
            {
                if (this._VIPExp != value)
                {
                    this._VIPExp = value;
                    base._isDirty = true;
                }
            }
        }

        public DateTime VIPExpireDay
        {
            get
            {
                return this._VIPExpireDay;
            }
            set
            {
                this._VIPExpireDay = value;
                base._isDirty = true;
            }
        }

        public DateTime VIPLastDate
        {
            get
            {
                return this.m_VIPlastDate;
            }
            set
            {
                this.m_VIPlastDate = value;
            }
        }

        public int VIPLevel
        {
            get
            {
                return this._VIPLevel;
            }
            set
            {
                if (this._VIPLevel != value)
                {
                    this._VIPLevel = value;
                    base._isDirty = true;
                }
            }
        }

        public int VIPNextLevelDaysNeeded
        {
            get
            {
                return this._VIPNextLevelDaysNeeded;
            }
            set
            {
                this._VIPNextLevelDaysNeeded = value;
                base._isDirty = true;
            }
        }

        public int VIPOfflineDays
        {
            get
            {
                return this._VIPOfflineDays;
            }
            set
            {
                this._VIPOfflineDays = value;
            }
        }

        public int VIPOnlineDays
        {
            get
            {
                return this._VIPOnlineDays;
            }
            set
            {
                this._VIPOnlineDays = value;
            }
        }

        public byte[] weaklessGuildProgress
        {
            get
            {
                return this._weaklessGuildProgress;
            }
            set
            {
                this._weaklessGuildProgress = value;
                base._isDirty = true;
            }
        }

        public string WeaklessGuildProgressStr
        {
            get
            {
                return this._weaklessGuildProgressStr;
            }
            set
            {
                this._weaklessGuildProgressStr = value;
                base._isDirty = true;
            }
        }

        public int Win
        {
            get
            {
                return this._win;
            }
            set
            {
                this._win = value;
                base._isDirty = true;
            }
        }

        public object Username()
        {
            throw new NotImplementedException();
        }
    }
}


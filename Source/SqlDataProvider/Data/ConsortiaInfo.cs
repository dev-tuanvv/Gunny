namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ConsortiaInfo : DataObject
    {
        private int _badgeID;
        private DateTime _buildDate;
        private int _celebCount;
        private int _consortiaID;
        private string _consortiaName;
        private int _count;
        private int _creatorID;
        private string _creatorName;
        private int _chairmanID;
        private string _chairmanName;
        private byte _chairmanTypeVIP;
        private int _chairmanVIPLevel;
        private DateTime _deductDate;
        private string _description;
        private int _extendAvailableNum;
        private int _fightPower;
        private int _honor;
        private string _ip;
        private bool _isExist;
        private int _level;
        private int _maxCount;
        private string _placard;
        private int _port;
        private int _repute;
        private int _riches;
        private Dictionary<string, RankingPersonInfo> m_rankList;
        public long MaxBlood;
        public long TotalAllMemberDame;

        public int AddDayHonor { get; set; }

        public int AddDayRiches { get; set; }

        public int AddWeekHonor { get; set; }

        public int AddWeekRiches { get; set; }

        public string BadgeBuyTime { get; set; }

        public int BadgeID
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

        public string BadgeName { get; set; }

        public int BadgeType { get; set; }

        public int bossState { get; set; }

        public DateTime BuildDate
        {
            get
            {
                return this._buildDate;
            }
            set
            {
                this._buildDate = value;
                base._isDirty = true;
            }
        }

        public int callBossLevel { get; set; }

        public int CelebCount
        {
            get
            {
                return this._celebCount;
            }
            set
            {
                this._celebCount = value;
                base._isDirty = true;
            }
        }

        public int ConsortiaID
        {
            get
            {
                return this._consortiaID;
            }
            set
            {
                this._consortiaID = value;
                base._isDirty = true;
            }
        }

        public string ConsortiaName
        {
            get
            {
                return this._consortiaName;
            }
            set
            {
                this._consortiaName = value;
                base._isDirty = true;
            }
        }

        public int Count
        {
            get
            {
                return this._count;
            }
            set
            {
                this._count = value;
                base._isDirty = true;
            }
        }

        public int CreatorID
        {
            get
            {
                return this._creatorID;
            }
            set
            {
                this._creatorID = value;
                base._isDirty = true;
            }
        }

        public string CreatorName
        {
            get
            {
                return this._creatorName;
            }
            set
            {
                this._creatorName = value;
                base._isDirty = true;
            }
        }

        public int ChairmanID
        {
            get
            {
                return this._chairmanID;
            }
            set
            {
                this._chairmanID = value;
                base._isDirty = true;
            }
        }

        public string ChairmanName
        {
            get
            {
                return this._chairmanName;
            }
            set
            {
                this._chairmanName = value;
                base._isDirty = true;
            }
        }

        public byte ChairmanTypeVIP
        {
            get
            {
                return this._chairmanTypeVIP;
            }
            set
            {
                this._chairmanTypeVIP = value;
                base._isDirty = true;
            }
        }

        public int ChairmanVIPLevel
        {
            get
            {
                return this._chairmanVIPLevel;
            }
            set
            {
                this._chairmanVIPLevel = value;
                base._isDirty = true;
            }
        }

        public DateTime DeductDate
        {
            get
            {
                return this._deductDate;
            }
            set
            {
                this._deductDate = value;
                base._isDirty = true;
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
                base._isDirty = true;
            }
        }

        public DateTime endTime { get; set; }

        public int extendAvailableNum
        {
            get
            {
                return this._extendAvailableNum;
            }
            set
            {
                this._extendAvailableNum = value;
                base._isDirty = true;
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
                this._fightPower = value;
                base._isDirty = true;
            }
        }

        public int Honor
        {
            get
            {
                return this._honor;
            }
            set
            {
                this._honor = value;
                base._isDirty = true;
            }
        }

        public string IP
        {
            get
            {
                return this._ip;
            }
            set
            {
                this._ip = value;
                base._isDirty = true;
            }
        }

        public bool IsBossDie { get; set; }

        public bool IsExist
        {
            get
            {
                return this._isExist;
            }
            set
            {
                this._isExist = value;
                base._isDirty = true;
            }
        }

        public bool IsSendAward { get; set; }

        public bool IsVoting { get; set; }

        public int LastDayRiches { get; set; }

        public DateTime LastOpenBoss { get; set; }

        public int Level
        {
            get
            {
                return this._level;
            }
            set
            {
                this._level = value;
                base._isDirty = true;
            }
        }

        public int MaxCount
        {
            get
            {
                return this._maxCount;
            }
            set
            {
                this._maxCount = value;
                base._isDirty = true;
            }
        }

        public bool OpenApply { get; set; }

        public string Placard
        {
            get
            {
                return this._placard;
            }
            set
            {
                this._placard = value;
                base._isDirty = true;
            }
        }

        public int Port
        {
            get
            {
                return this._port;
            }
            set
            {
                this._port = value;
                base._isDirty = true;
            }
        }

        public Dictionary<string, RankingPersonInfo> RankList
        {
            get
            {
                return this.m_rankList;
            }
            set
            {
                this.m_rankList = value;
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

        public int Riches
        {
            get
            {
                return this._riches;
            }
            set
            {
                this._riches = value;
                base._isDirty = true;
            }
        }

        public bool SendToClient { get; set; }

        public int ShopLevel { get; set; }

        public int SkillLevel { get; set; }

        public int SmithLevel { get; set; }

        public int StoreLevel { get; set; }

        public int ValidDate { get; set; }

        public int VoteRemainDay { get; set; }
    }
}


namespace SqlDataProvider.Data
{
    using System;

    public class ActiveSystemInfo : DataObject
    {
        private int _activeMoney;
        private int _activityTanabataNum;
        private int _AvailTime;
        private string _boxState;
        private int _buyBuffNum;
        private int _canEagleEyeCounts;
        private bool _CanGetGift;
        private int _canOpenCounts;
        private int _challengeNum;
        private int _damageNum;
        private int _dayScore;
        private int _ID;
        private int _isBuy;
        private bool _isShowAll;
        private DateTime _lastEnterYearMonter;
        private DateTime _lastFlushTime;
        private int _luckystarCoins;
        private int _myRank;
        private string _nickName;
        private int _totalScore;
        private int _useableScore;
        private int _userID;

        public int ActiveMoney
        {
            get
            {
                return this._activeMoney;
            }
            set
            {
                this._activeMoney = value;
                base._isDirty = true;
            }
        }

        public int activityTanabataNum
        {
            get
            {
                return this._activityTanabataNum;
            }
            set
            {
                this._activityTanabataNum = value;
                base._isDirty = true;
            }
        }

        public int AvailTime
        {
            get
            {
                return this._AvailTime;
            }
            set
            {
                this._AvailTime = value;
                base._isDirty = true;
            }
        }

        public string BoxState
        {
            get
            {
                return this._boxState;
            }
            set
            {
                this._boxState = value;
                base._isDirty = true;
            }
        }

        public int BuyBuffNum
        {
            get
            {
                return this._buyBuffNum;
            }
            set
            {
                this._buyBuffNum = value;
                base._isDirty = true;
            }
        }

        public int canEagleEyeCounts
        {
            get
            {
                return this._canEagleEyeCounts;
            }
            set
            {
                this._canEagleEyeCounts = value;
                base._isDirty = true;
            }
        }

        public bool CanGetGift
        {
            get
            {
                return this._CanGetGift;
            }
            set
            {
                this._CanGetGift = value;
                base._isDirty = true;
            }
        }

        public int canOpenCounts
        {
            get
            {
                return this._canOpenCounts;
            }
            set
            {
                this._canOpenCounts = value;
                base._isDirty = true;
            }
        }

        public int ChallengeNum
        {
            get
            {
                return this._challengeNum;
            }
            set
            {
                this._challengeNum = value;
                base._isDirty = true;
            }
        }

        public int DamageNum
        {
            get
            {
                return this._damageNum;
            }
            set
            {
                this._damageNum = value;
                base._isDirty = true;
            }
        }

        public int dayScore
        {
            get
            {
                return this._dayScore;
            }
            set
            {
                this._dayScore = value;
                base._isDirty = true;
            }
        }

        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
                base._isDirty = true;
            }
        }

        public int isBuy
        {
            get
            {
                return this._isBuy;
            }
            set
            {
                this._isBuy = value;
                base._isDirty = true;
            }
        }

        public bool isShowAll
        {
            get
            {
                return this._isShowAll;
            }
            set
            {
                this._isShowAll = value;
                base._isDirty = true;
            }
        }

        public DateTime lastEnterYearMonter
        {
            get
            {
                return this._lastEnterYearMonter;
            }
            set
            {
                this._lastEnterYearMonter = value;
                base._isDirty = true;
            }
        }

        public DateTime lastFlushTime
        {
            get
            {
                return this._lastFlushTime;
            }
            set
            {
                this._lastFlushTime = value;
                base._isDirty = true;
            }
        }

        public int LuckystarCoins
        {
            get
            {
                return this._luckystarCoins;
            }
            set
            {
                this._luckystarCoins = value;
                base._isDirty = true;
            }
        }

        public int myRank
        {
            get
            {
                return this._myRank;
            }
            set
            {
                this._myRank = value;
                base._isDirty = true;
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

        public int totalScore
        {
            get
            {
                return this._totalScore;
            }
            set
            {
                this._totalScore = value;
                base._isDirty = true;
            }
        }

        public int useableScore
        {
            get
            {
                return this._useableScore;
            }
            set
            {
                this._useableScore = value;
                base._isDirty = true;
            }
        }

        public int UserID
        {
            get
            {
                return this._userID;
            }
            set
            {
                this._userID = value;
                base._isDirty = true;
            }
        }
    }
}


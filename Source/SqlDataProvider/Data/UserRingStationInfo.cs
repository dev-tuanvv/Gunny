namespace SqlDataProvider.Data
{
    using System;

    public class UserRingStationInfo : DataObject
    {
        private int _BaseDamage;
        private int _BaseEnergy;
        private int _BaseGuard;
        private int _buyCount;
        private int _ChallengeNum;
        private DateTime _ChallengeTime;
        private int _ID;
        private DateTime _LastDate;
        private bool _OnFight;
        private PlayerInfo _playerInfo;
        private int _Rank;
        private string _signMsg;
        private int _Total;
        private int _UserID;
        private int _WeaponID;

        public bool CanChallenge()
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - this._ChallengeTime);
            int num = 0x927c0 - ((int) span.TotalMilliseconds);
            num = ((num / 10) / 60) / 0x3e8;
            return (num <= 0);
        }

        public int BaseDamage
        {
            get
            {
                return this._BaseDamage;
            }
            set
            {
                this._BaseDamage = value;
                base._isDirty = true;
            }
        }

        public int BaseEnergy
        {
            get
            {
                return this._BaseEnergy;
            }
            set
            {
                this._BaseEnergy = value;
                base._isDirty = true;
            }
        }

        public int BaseGuard
        {
            get
            {
                return this._BaseGuard;
            }
            set
            {
                this._BaseGuard = value;
                base._isDirty = true;
            }
        }

        public int buyCount
        {
            get
            {
                return this._buyCount;
            }
            set
            {
                this._buyCount = value;
                base._isDirty = true;
            }
        }

        public int ChallengeNum
        {
            get
            {
                return this._ChallengeNum;
            }
            set
            {
                this._ChallengeNum = value;
                base._isDirty = true;
            }
        }

        public DateTime ChallengeTime
        {
            get
            {
                return this._ChallengeTime;
            }
            set
            {
                this._ChallengeTime = value;
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

        public PlayerInfo Info
        {
            get
            {
                return this._playerInfo;
            }
            set
            {
                this._playerInfo = value;
            }
        }

        public DateTime LastDate
        {
            get
            {
                return this._LastDate;
            }
            set
            {
                this._LastDate = value;
                base._isDirty = true;
            }
        }

        public bool OnFight
        {
            get
            {
                return this._OnFight;
            }
            set
            {
                this._OnFight = value;
            }
        }

        public int Rank
        {
            get
            {
                return this._Rank;
            }
            set
            {
                this._Rank = value;
                base._isDirty = true;
            }
        }

        public string signMsg
        {
            get
            {
                return this._signMsg;
            }
            set
            {
                this._signMsg = value;
                base._isDirty = true;
            }
        }

        public int Total
        {
            get
            {
                return this._Total;
            }
            set
            {
                this._Total = value;
                base._isDirty = true;
            }
        }

        public int UserID
        {
            get
            {
                return this._UserID;
            }
            set
            {
                this._UserID = value;
                base._isDirty = true;
            }
        }

        public int WeaponID
        {
            get
            {
                return this._WeaponID;
            }
            set
            {
                this._WeaponID = value;
                base._isDirty = true;
            }
        }
    }
}


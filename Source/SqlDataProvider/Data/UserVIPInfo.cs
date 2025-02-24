namespace SqlDataProvider.Data
{
    using System;

    public class UserVIPInfo : DataObject
    {
        private bool _CanTakeVipReward;
        private DateTime _LastVIPPackTime;
        private byte _typeVIP;
        private int _UserID;
        private int _VIPExp;
        private DateTime _VIPExpireDay;
        private DateTime _VIPLastdate;
        private int _VIPLevel;
        private int _VIPNextLevelDaysNeeded;
        private int _VIPOfflineDays;
        private int _VIPOnlineDays;

        public UserVIPInfo()
        {
        }

        public UserVIPInfo(int userId)
        {
            this.UserID = userId;
            this.typeVIP = 0;
            this.VIPLevel = 0;
            this.VIPExp = 0;
            this.VIPOnlineDays = 0;
            this.VIPOfflineDays = 0;
            this.VIPExpireDay = DateTime.Now;
            this.LastVIPPackTime = DateTime.Now;
            this.VIPLastdate = DateTime.Now;
            this.VIPNextLevelDaysNeeded = 0;
            this.CanTakeVipReward = false;
        }

        public void ContinousVIP(int days)
        {
            DateTime now = DateTime.Now;
            if (this.VIPExpireDay < DateTime.Now)
            {
                now = DateTime.Now.AddDays((double) days);
            }
            else
            {
                now = this.VIPExpireDay.AddDays((double) days);
            }
            this.VIPExpireDay = now;
            this.typeVIP = this.SetType(days);
        }

        public bool IsLastVIPPackTime()
        {
            return (this.StartOfWeek(this._LastVIPPackTime.Date, DayOfWeek.Monday) < this.StartOfWeek(DateTime.Now.Date, DayOfWeek.Monday));
        }

        public bool IsVIP()
        {
            return (!this.IsVIPExpire() && (this._typeVIP > 0));
        }

        public bool IsVIPExpire()
        {
            return (this._VIPExpireDay.Date < DateTime.Now.Date);
        }

        public void OpenVIP(int days)
        {
            DateTime time = DateTime.Now.AddDays((double) days);
            this.typeVIP = this.SetType(days);
            this.VIPLevel = 1;
            this.VIPExp = 0;
            this.VIPExpireDay = time;
            this.VIPLastdate = DateTime.Now;
            this.VIPNextLevelDaysNeeded = 0;
            this.CanTakeVipReward = true;
        }

        public void SetLastVIPPackTime()
        {
            this.LastVIPPackTime = DateTime.Now;
            this.CanTakeVipReward = false;
        }

        private byte SetType(int days)
        {
            byte num = 1;
            if ((days / 0x1f) >= 3)
            {
                num = 2;
            }
            return num;
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int num = (int) (dt.DayOfWeek - startOfWeek);
            if (num < 0)
            {
                num += 7;
            }
            return dt.AddDays((double) (-1 * num)).Date;
        }

        public bool CanTakeVipReward
        {
            get
            {
                return this._CanTakeVipReward;
            }
            set
            {
                this._CanTakeVipReward = value;
                base._isDirty = true;
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

        public byte typeVIP
        {
            get
            {
                return this._typeVIP;
            }
            set
            {
                this._typeVIP = value;
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

        public int VIPExp
        {
            get
            {
                return this._VIPExp;
            }
            set
            {
                this._VIPExp = value;
                base._isDirty = true;
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

        public DateTime VIPLastdate
        {
            get
            {
                return this._VIPLastdate;
            }
            set
            {
                this._VIPLastdate = value;
                base._isDirty = true;
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
                this._VIPLevel = value;
                base._isDirty = true;
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
                base._isDirty = true;
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
                base._isDirty = true;
            }
        }
    }
}


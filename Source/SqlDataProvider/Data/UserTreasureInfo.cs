namespace SqlDataProvider.Data
{
    using System;

    public class UserTreasureInfo : DataObject
    {
        private int _friendHelpTimes;
        private int _ID;
        private bool _isBeginTreasure;
        private bool _isEndTreasure;
        private DateTime _lastLoginDay;
        private int _logoinDays;
        private string _NickName;
        private int _treasure;
        private int _treasureAdd;
        private int _userID;

        public bool isValidDate()
        {
            return (this._lastLoginDay.Date < DateTime.Now.Date);
        }

        public int friendHelpTimes
        {
            get
            {
                return this._friendHelpTimes;
            }
            set
            {
                this._friendHelpTimes = value;
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

        public bool isBeginTreasure
        {
            get
            {
                return this._isBeginTreasure;
            }
            set
            {
                this._isBeginTreasure = value;
                base._isDirty = true;
            }
        }

        public bool isEndTreasure
        {
            get
            {
                return this._isEndTreasure;
            }
            set
            {
                this._isEndTreasure = value;
                base._isDirty = true;
            }
        }

        public DateTime LastLoginDay
        {
            get
            {
                return this._lastLoginDay;
            }
            set
            {
                this._lastLoginDay = value;
                base._isDirty = true;
            }
        }

        public int logoinDays
        {
            get
            {
                return this._logoinDays;
            }
            set
            {
                this._logoinDays = value;
                base._isDirty = true;
            }
        }

        public string NickName
        {
            get
            {
                return this._NickName;
            }
            set
            {
                this._NickName = value;
                base._isDirty = true;
            }
        }

        public int treasure
        {
            get
            {
                return this._treasure;
            }
            set
            {
                this._treasure = value;
                base._isDirty = true;
            }
        }

        public int treasureAdd
        {
            get
            {
                return this._treasureAdd;
            }
            set
            {
                this._treasureAdd = value;
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


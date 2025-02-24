namespace SqlDataProvider.Data
{
    using System;

    public class UserChristmasInfo : DataObject
    {
        private int _AvailTime;
        private int _awardState;
        private int _count;
        private int _dayPacks;
        private int _exp;
        private DateTime _gameBeginTime;
        private DateTime _gameEndTime;
        private int _ID;
        private bool _isEnter;
        private int _lastPacks;
        private int _packsNumber;
        private int _userID;

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

        public int awardState
        {
            get
            {
                return this._awardState;
            }
            set
            {
                this._awardState = value;
                base._isDirty = true;
            }
        }

        public int count
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

        public int dayPacks
        {
            get
            {
                return this._dayPacks;
            }
            set
            {
                this._dayPacks = value;
                base._isDirty = true;
            }
        }

        public int exp
        {
            get
            {
                return this._exp;
            }
            set
            {
                this._exp = value;
                base._isDirty = true;
            }
        }

        public DateTime gameBeginTime
        {
            get
            {
                return this._gameBeginTime;
            }
            set
            {
                this._gameBeginTime = value;
                base._isDirty = true;
            }
        }

        public DateTime gameEndTime
        {
            get
            {
                return this._gameEndTime;
            }
            set
            {
                this._gameEndTime = value;
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

        public bool isEnter
        {
            get
            {
                return this._isEnter;
            }
            set
            {
                this._isEnter = value;
                base._isDirty = true;
            }
        }

        public int lastPacks
        {
            get
            {
                return this._lastPacks;
            }
            set
            {
                this._lastPacks = value;
                base._isDirty = true;
            }
        }

        public int packsNumber
        {
            get
            {
                return this._packsNumber;
            }
            set
            {
                this._packsNumber = value;
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


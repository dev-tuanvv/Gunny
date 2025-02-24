namespace SqlDataProvider.Data
{
    using System;

    public class DiceDataInfo : DataObject
    {
        private string _awardArray;
        private int _currentPosition;
        private int _freeCount;
        private int _ID;
        private int _level;
        private int _luckIntegral;
        private int _luckIntegralLevel;
        private bool _userFirstCell;
        private int _userID;

        public string AwardArray
        {
            get
            {
                return this._awardArray;
            }
            set
            {
                this._awardArray = value;
                base._isDirty = true;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return this._currentPosition;
            }
            set
            {
                this._currentPosition = value;
                base._isDirty = true;
            }
        }

        public int FreeCount
        {
            get
            {
                return this._freeCount;
            }
            set
            {
                this._freeCount = value;
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

        public int LuckIntegral
        {
            get
            {
                return this._luckIntegral;
            }
            set
            {
                this._luckIntegral = value;
                base._isDirty = true;
            }
        }

        public int LuckIntegralLevel
        {
            get
            {
                return this._luckIntegralLevel;
            }
            set
            {
                this._luckIntegralLevel = value;
                base._isDirty = true;
            }
        }

        public bool UserFirstCell
        {
            get
            {
                return this._userFirstCell;
            }
            set
            {
                this._userFirstCell = value;
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


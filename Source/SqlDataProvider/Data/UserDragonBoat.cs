namespace SqlDataProvider.Data
{
    using System;

    public class UserDragonBoat : DataObject
    {
        private int _exp;
        private int _lessPoint;
        private string _nickname;
        private int _point;
        private int _rankUser;
        private int _totalPoint;
        private int _userID;

        public int Exp
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

        public int LessPoint
        {
            get
            {
                return this._lessPoint;
            }
            set
            {
                this._lessPoint = value;
            }
        }

        public string NickName
        {
            get
            {
                return this._nickname;
            }
            set
            {
                this._nickname = value;
                base._isDirty = true;
            }
        }

        public int Point
        {
            get
            {
                return this._point;
            }
            set
            {
                this._point = value;
                base._isDirty = true;
            }
        }

        public int Rank
        {
            get
            {
                return this._rankUser;
            }
            set
            {
                this._rankUser = value;
            }
        }

        public int TotalPoint
        {
            get
            {
                return this._totalPoint;
            }
            set
            {
                this._totalPoint = value;
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


namespace SqlDataProvider.Data
{
    using System;

    public class UserDrillInfo : DataObject
    {
        private int _beadPlace;
        private int _drillPlace;
        private int _holeExp;
        private int _holeLv;
        private int _userID;

        public int BeadPlace
        {
            get
            {
                return this._beadPlace;
            }
            set
            {
                this._beadPlace = value;
                base._isDirty = true;
            }
        }

        public int DrillPlace
        {
            get
            {
                return this._drillPlace;
            }
            set
            {
                this._drillPlace = value;
                base._isDirty = true;
            }
        }

        public int HoleExp
        {
            get
            {
                return this._holeExp;
            }
            set
            {
                this._holeExp = value;
                base._isDirty = true;
            }
        }

        public int HoleLv
        {
            get
            {
                return this._holeLv;
            }
            set
            {
                this._holeLv = value;
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


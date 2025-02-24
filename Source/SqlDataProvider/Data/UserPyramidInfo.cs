namespace SqlDataProvider.Data
{
    using System;

    public class UserPyramidInfo : DataObject
    {
        private int _currentFreeCount;
        private int _currentLayer;
        private int _currentReviveCount;
        private bool _isDead;
        private bool _isPyramidStart;
        private int _maxLayer;
        private int _pointRatio;
        private int _totalPoint;
        private int _turnPoint;
        private int _UserID;

        public int currentFreeCount
        {
            get
            {
                return this._currentFreeCount;
            }
            set
            {
                this._currentFreeCount = value;
                base._isDirty = true;
            }
        }

        public int currentLayer
        {
            get
            {
                return this._currentLayer;
            }
            set
            {
                this._currentLayer = value;
                base._isDirty = true;
            }
        }

        public int currentReviveCount
        {
            get
            {
                return this._currentReviveCount;
            }
            set
            {
                this._currentReviveCount = value;
                base._isDirty = true;
            }
        }

        public bool isDead
        {
            get
            {
                return this._isDead;
            }
            set
            {
                this._isDead = value;
                base._isDirty = true;
            }
        }

        public bool isPyramidStart
        {
            get
            {
                return this._isPyramidStart;
            }
            set
            {
                this._isPyramidStart = value;
                base._isDirty = true;
            }
        }

        public int maxLayer
        {
            get
            {
                return this._maxLayer;
            }
            set
            {
                this._maxLayer = value;
                base._isDirty = true;
            }
        }

        public int pointRatio
        {
            get
            {
                return this._pointRatio;
            }
            set
            {
                this._pointRatio = value;
                base._isDirty = true;
            }
        }

        public int totalPoint
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

        public int turnPoint
        {
            get
            {
                return this._turnPoint;
            }
            set
            {
                this._turnPoint = value;
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
    }
}


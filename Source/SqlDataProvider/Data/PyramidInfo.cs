namespace SqlDataProvider.Data
{
    using System;

    public class PyramidInfo : DataObject
    {
        private int _currentFreeCount;
        private int _currentLayer;
        private int _currentReviveCount;
        private int _ID;
        private bool _isPyramidStart;
        private string _LayerItems;
        private int _maxLayer;
        private int _pointRatio;
        private int _totalPoint;
        private int _turnPoint;
        private int _userID;

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

        public string LayerItems
        {
            get
            {
                return this._LayerItems;
            }
            set
            {
                this._LayerItems = value;
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


namespace SqlDataProvider.Data
{
    using System;

    public class PyramidConfigInfo
    {
        private DateTime _beginTime;
        private DateTime _endTime;
        private int _freeCount;
        private bool _isOpen;
        private bool _isScoreExchange;
        private int[] _revivePrice;
        private int _turnCardPrice;
        private int _userID;

        public DateTime beginTime
        {
            get
            {
                return this._beginTime;
            }
            set
            {
                this._beginTime = value;
            }
        }

        public DateTime endTime
        {
            get
            {
                return this._endTime;
            }
            set
            {
                this._endTime = value;
            }
        }

        public int freeCount
        {
            get
            {
                return this._freeCount;
            }
            set
            {
                this._freeCount = value;
            }
        }

        public bool isOpen
        {
            get
            {
                return this._isOpen;
            }
            set
            {
                this._isOpen = value;
            }
        }

        public bool isScoreExchange
        {
            get
            {
                return this._isScoreExchange;
            }
            set
            {
                this._isScoreExchange = value;
            }
        }

        public int[] revivePrice
        {
            get
            {
                return this._revivePrice;
            }
            set
            {
                this._revivePrice = value;
            }
        }

        public int turnCardPrice
        {
            get
            {
                return this._turnCardPrice;
            }
            set
            {
                this._turnCardPrice = value;
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
            }
        }
    }
}


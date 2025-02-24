namespace SqlDataProvider.Data
{
    using System;

    public class UserLabyrinthInfo : DataObject
    {
        private int _accumulateExp;
        private int _cleanOutAllTime;
        private int _cleanOutGold;
        private bool _completeChallenge;
        private int _currentFloor;
        private int _currentRemainTime;
        private bool _isCleanOut;
        private bool _isDoubleAward;
        private bool _isInGame;
        private DateTime _lastDate;
        private int _myProgress;
        private int _myRanking;
        private string _processAward;
        private int _remainTime;
        private bool _serverMultiplyingPower;
        private bool _tryAgainComplete;
        private int _userID;

        public bool isValidDate()
        {
            return (this._lastDate.Date < DateTime.Now.Date);
        }

        public int accumulateExp
        {
            get
            {
                return this._accumulateExp;
            }
            set
            {
                this._accumulateExp = value;
                base._isDirty = true;
            }
        }

        public int cleanOutAllTime
        {
            get
            {
                return this._cleanOutAllTime;
            }
            set
            {
                this._cleanOutAllTime = value;
                base._isDirty = true;
            }
        }

        public int cleanOutGold
        {
            get
            {
                return this._cleanOutGold;
            }
            set
            {
                this._cleanOutGold = value;
                base._isDirty = true;
            }
        }

        public bool completeChallenge
        {
            get
            {
                return this._completeChallenge;
            }
            set
            {
                this._completeChallenge = value;
                base._isDirty = true;
            }
        }

        public int currentFloor
        {
            get
            {
                return this._currentFloor;
            }
            set
            {
                this._currentFloor = value;
                base._isDirty = true;
            }
        }

        public int currentRemainTime
        {
            get
            {
                return this._currentRemainTime;
            }
            set
            {
                this._currentRemainTime = value;
                base._isDirty = true;
            }
        }

        public bool isCleanOut
        {
            get
            {
                return this._isCleanOut;
            }
            set
            {
                this._isCleanOut = value;
                base._isDirty = true;
            }
        }

        public bool isDoubleAward
        {
            get
            {
                return this._isDoubleAward;
            }
            set
            {
                this._isDoubleAward = value;
                base._isDirty = true;
            }
        }

        public bool isInGame
        {
            get
            {
                return this._isInGame;
            }
            set
            {
                this._isInGame = value;
                base._isDirty = true;
            }
        }

        public DateTime LastDate
        {
            get
            {
                return this._lastDate;
            }
            set
            {
                this._lastDate = value;
                base._isDirty = true;
            }
        }

        public int myProgress
        {
            get
            {
                return this._myProgress;
            }
            set
            {
                this._myProgress = value;
                base._isDirty = true;
            }
        }

        public int myRanking
        {
            get
            {
                return this._myRanking;
            }
            set
            {
                this._myRanking = value;
                base._isDirty = true;
            }
        }

        public string ProcessAward
        {
            get
            {
                return this._processAward;
            }
            set
            {
                this._processAward = value;
                base._isDirty = true;
            }
        }

        public int remainTime
        {
            get
            {
                return this._remainTime;
            }
            set
            {
                this._remainTime = value;
                base._isDirty = true;
            }
        }

        public bool serverMultiplyingPower
        {
            get
            {
                return this._serverMultiplyingPower;
            }
            set
            {
                this._serverMultiplyingPower = value;
                base._isDirty = true;
            }
        }

        public bool tryAgainComplete
        {
            get
            {
                return this._tryAgainComplete;
            }
            set
            {
                this._tryAgainComplete = value;
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


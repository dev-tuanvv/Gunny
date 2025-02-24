namespace SqlDataProvider.Data
{
    using System;

    public class UserMatchInfo : DataObject
    {
        private int _addDayPrestge;
        private int _dailyGameCount;
        private bool _DailyLeagueFirst;
        private int _DailyLeagueLastScore;
        private int _dailyScore;
        private int _dailyWinCount;
        private int _ID;
        private int _maxCount;
        private int _rank;
        private int _restCount;
        private int _totalPrestige;
        private int _userID;
        private int _weeklyGameCount;
        private int _weeklyRanking;
        private int _weeklyScore;

        public int addDayPrestge
        {
            get
            {
                return this._addDayPrestge;
            }
            set
            {
                this._addDayPrestge = value;
                base._isDirty = true;
            }
        }

        public int dailyGameCount
        {
            get
            {
                return this._dailyGameCount;
            }
            set
            {
                this._dailyGameCount = value;
                base._isDirty = true;
            }
        }

        public bool DailyLeagueFirst
        {
            get
            {
                return this._DailyLeagueFirst;
            }
            set
            {
                this._DailyLeagueFirst = value;
                base._isDirty = true;
            }
        }

        public int DailyLeagueLastScore
        {
            get
            {
                return this._DailyLeagueLastScore;
            }
            set
            {
                this._DailyLeagueLastScore = value;
                base._isDirty = true;
            }
        }

        public int dailyScore
        {
            get
            {
                return this._dailyScore;
            }
            set
            {
                this._dailyScore = value;
                base._isDirty = true;
            }
        }

        public int dailyWinCount
        {
            get
            {
                return this._dailyWinCount;
            }
            set
            {
                this._dailyWinCount = value;
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

        public int maxCount
        {
            get
            {
                return this._maxCount;
            }
            set
            {
                this._maxCount = value;
                base._isDirty = true;
            }
        }

        public int rank
        {
            get
            {
                return this._rank;
            }
            set
            {
                this._rank = value;
                base._isDirty = true;
            }
        }

        public int restCount
        {
            get
            {
                return this._restCount;
            }
            set
            {
                this._restCount = value;
                base._isDirty = true;
            }
        }

        public int totalPrestige
        {
            get
            {
                return this._totalPrestige;
            }
            set
            {
                this._totalPrestige = value;
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

        public int weeklyGameCount
        {
            get
            {
                return this._weeklyGameCount;
            }
            set
            {
                this._weeklyGameCount = value;
                base._isDirty = true;
            }
        }

        public int weeklyRanking
        {
            get
            {
                return this._weeklyRanking;
            }
            set
            {
                this._weeklyRanking = value;
                base._isDirty = true;
            }
        }

        public int weeklyScore
        {
            get
            {
                return this._weeklyScore;
            }
            set
            {
                this._weeklyScore = value;
                base._isDirty = true;
            }
        }
    }
}


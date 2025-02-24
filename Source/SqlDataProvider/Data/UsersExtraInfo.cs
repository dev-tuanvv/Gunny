namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class UsersExtraInfo : DataObject
    {
        private int _buyEnergyCount;
        private int _freeAddAutionCount;
        private int _freeCount;
        private int _freeSendMailCount;
        private int _ID;
        private DateTime _kingBlessEnddate;
        private int _kingBlessIndex;
        private string _kingBlessInfo;
        private int _missionEnergy;
        private int _nowPosition;
        private string _searchGoodItems;
        private int _starlevel;
        private int _userID;
        private DateTime dateTime_0;
        private DateTime dateTime_1;
        private int int_1;

        public bool RenevalDays(int value, int index)
        {
            if (index < this._kingBlessIndex)
            {
                return false;
            }
            if (this._kingBlessEnddate < DateTime.Now)
            {
                this._kingBlessEnddate = DateTime.Now;
            }
            this._kingBlessEnddate = this._kingBlessEnddate.AddDays((double) value);
            this._kingBlessIndex = index;
            return true;
        }

        public int buyEnergyCount
        {
            get
            {
                return this._buyEnergyCount;
            }
            set
            {
                this._buyEnergyCount = value;
                base._isDirty = true;
            }
        }

        public int coupleBossBoxNum
        {
            get
            {
                return this.int_4;
            }
            set
            {
                this.int_4 = value;
            }
        }

        public int coupleBossEnterNum
        {
            get
            {
                return this.int_2;
            }
            set
            {
                this.int_2 = value;
            }
        }

        public int coupleBossHurt
        {
            get
            {
                return this.int_3;
            }
            set
            {
                this.int_3 = value;
            }
        }

        public int FreeAddAutionCount
        {
            get
            {
                return this._freeAddAutionCount;
            }
            set
            {
                this._freeAddAutionCount = value;
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

        public int FreeSendMailCount
        {
            get
            {
                return this._freeSendMailCount;
            }
            set
            {
                this._freeSendMailCount = value;
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

        private int int_2 { get; set; }

        private int int_3 { get; set; }

        private int int_4 { get; set; }

        public string KingBleesInfo
        {
            get
            {
                return this._kingBlessInfo;
            }
            set
            {
                this._kingBlessInfo = value;
                base._isDirty = true;
            }
        }

        public DateTime KingBlessEnddate
        {
            get
            {
                return this._kingBlessEnddate;
            }
            set
            {
                this._kingBlessEnddate = value;
                base._isDirty = true;
            }
        }

        public int KingBlessIndex
        {
            get
            {
                return this._kingBlessIndex;
            }
            set
            {
                this._kingBlessIndex = value;
                base._isDirty = true;
            }
        }

        public DateTime LastFreeTimeHotSpring
        {
            get
            {
                return this.dateTime_1;
            }
            set
            {
                this.dateTime_1 = value;
                base._isDirty = true;
            }
        }

        public DateTime LastTimeHotSpring
        {
            get
            {
                return this.dateTime_0;
            }
            set
            {
                this.dateTime_0 = value;
                base._isDirty = true;
            }
        }

        public int MinHotSpring
        {
            get
            {
                return this.int_1;
            }
            set
            {
                this.int_1 = value;
                base._isDirty = true;
            }
        }

        public int MissionEnergy
        {
            get
            {
                return this._missionEnergy;
            }
            set
            {
                this._missionEnergy = value;
                base._isDirty = true;
            }
        }

        public int nowPosition
        {
            get
            {
                return this._nowPosition;
            }
            set
            {
                this._nowPosition = value;
                base._isDirty = true;
            }
        }

        public string SearchGoodItems
        {
            get
            {
                return this._searchGoodItems;
            }
            set
            {
                this._searchGoodItems = value;
                base._isDirty = true;
            }
        }

        public int starlevel
        {
            get
            {
                return this._starlevel;
            }
            set
            {
                this._starlevel = value;
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


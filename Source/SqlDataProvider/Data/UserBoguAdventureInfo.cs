namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;

    public class UserBoguAdventureInfo : DataObject
    {
        private string _Award;
        private int _CurrentPostion;
        private int _HP;
        private string _Map;
        private List<BoguCeilInfo> _MapData;
        private int _OpenCount;
        private int _ResetCount;
        private int _UserID;

        public string[] GetAward()
        {
            return this._Award.Split(new char[] { ',' });
        }

        public void SetAward(int slot, int value)
        {
            string[] award = this.GetAward();
            award[slot] = value.ToString();
            this._Award = string.Join(",", award);
        }

        public string Award
        {
            get
            {
                return this._Award;
            }
            set
            {
                this._Award = value;
                base._isDirty = true;
            }
        }

        public int CurrentPostion
        {
            get
            {
                return this._CurrentPostion;
            }
            set
            {
                this._CurrentPostion = value;
                base._isDirty = true;
            }
        }

        public int HP
        {
            get
            {
                return this._HP;
            }
            set
            {
                this._HP = value;
            }
        }

        public string Map
        {
            get
            {
                return this._Map;
            }
            set
            {
                this._Map = value;
                base._isDirty = true;
            }
        }

        public List<BoguCeilInfo> MapData
        {
            get
            {
                return this._MapData;
            }
            set
            {
                this._MapData = value;
            }
        }

        public int OpenCount
        {
            get
            {
                return this._OpenCount;
            }
            set
            {
                this._OpenCount = value;
                base._isDirty = true;
            }
        }

        public int ResetCount
        {
            get
            {
                return this._ResetCount;
            }
            set
            {
                this._ResetCount = value;
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


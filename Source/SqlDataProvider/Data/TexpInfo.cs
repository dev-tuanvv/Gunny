namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class TexpInfo : DataObject
    {
        private int _attTexpExp;
        private int _defTexpExp;
        private int _hpTexpExp;
        private int _lukTexpExp;
        private int _spdTexpExp;
        private int _texpCount;
        private int _texpTaskCount;
        private DateTime _texpTaskDate;
        private int _userID;

        public bool IsValidadteTexp()
        {
            return (this._texpTaskDate.Date < DateTime.Now.Date);
        }

        public int attTexpExp
        {
            get
            {
                return this._attTexpExp;
            }
            set
            {
                this._attTexpExp = value;
                base._isDirty = true;
            }
        }

        public int defTexpExp
        {
            get
            {
                return this._defTexpExp;
            }
            set
            {
                this._defTexpExp = value;
                base._isDirty = true;
            }
        }

        public int hpTexpExp
        {
            get
            {
                return this._hpTexpExp;
            }
            set
            {
                this._hpTexpExp = value;
                base._isDirty = true;
            }
        }

        public int ID { get; set; }

        public int lukTexpExp
        {
            get
            {
                return this._lukTexpExp;
            }
            set
            {
                this._lukTexpExp = value;
                base._isDirty = true;
            }
        }

        public int spdTexpExp
        {
            get
            {
                return this._spdTexpExp;
            }
            set
            {
                this._spdTexpExp = value;
                base._isDirty = true;
            }
        }

        public int texpCount
        {
            get
            {
                return this._texpCount;
            }
            set
            {
                this._texpCount = value;
                base._isDirty = true;
            }
        }

        public int texpTaskCount
        {
            get
            {
                return this._texpTaskCount;
            }
            set
            {
                this._texpTaskCount = value;
                base._isDirty = true;
            }
        }

        public DateTime texpTaskDate
        {
            get
            {
                return this._texpTaskDate;
            }
            set
            {
                this._texpTaskDate = value;
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


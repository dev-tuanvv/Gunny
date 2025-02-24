namespace SqlDataProvider.Data
{
    using System;

    public class BufferInfo : DataObject
    {
        private DateTime _beginDate;
        private string _data;
        private bool _isExist;
        private int _templateID;
        private int _type;
        private int _userID;
        private int _validCount;
        private int _validDate;
        private int _value;

        public DateTime GetEndDate()
        {
            return this._beginDate.AddMinutes((double) this._validDate);
        }

        public bool IsValid()
        {
            return (this._beginDate.AddMinutes((double) this._validDate) > DateTime.Now);
        }

        public DateTime BeginDate
        {
            get
            {
                return this._beginDate;
            }
            set
            {
                this._beginDate = value;
                base._isDirty = true;
            }
        }

        public string Data
        {
            get
            {
                return this._data;
            }
            set
            {
                this._data = value;
                base._isDirty = true;
            }
        }

        public bool IsExist
        {
            get
            {
                return this._isExist;
            }
            set
            {
                this._isExist = value;
                base._isDirty = true;
            }
        }

        public int TemplateID
        {
            get
            {
                return this._templateID;
            }
            set
            {
                this._templateID = value;
                base._isDirty = true;
            }
        }

        public int Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
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

        public int ValidCount
        {
            get
            {
                return this._validCount;
            }
            set
            {
                this._validCount = value;
                base._isDirty = true;
            }
        }

        public int ValidDate
        {
            get
            {
                return this._validDate;
            }
            set
            {
                this._validDate = value;
                base._isDirty = true;
            }
        }

        public int Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                base._isDirty = true;
            }
        }
    }
}


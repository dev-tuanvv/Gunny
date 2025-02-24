namespace SqlDataProvider.Data
{
    using System;

    public class UserFieldInfo : DataObject
    {
        private int _accelerateTime;
        private int _autoFertilizerCount;
        private int _autoFertilizerID;
        private DateTime _automaticTime;
        private int _autoSeedID;
        private int _autoSeedIDCount;
        private int _farmID;
        private int _fieldID;
        private int _fieldValidDate;
        private int _gainCount;
        private int _gainFieldId;
        private int _id;
        private bool _isAutomatic;
        private bool _isExist;
        private int _payFieldTime;
        private DateTime _payTime;
        private DateTime _plantTime;
        private int _seedID;

        public bool isDig()
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - this._plantTime);
            int num = this._fieldValidDate - ((int) span.TotalMinutes);
            return (num > 0);
        }

        public bool IsValidField()
        {
            return ((this._payFieldTime == 0) || (DateTime.Compare(this._payTime.AddDays((double) this._payFieldTime), DateTime.Now) > 0));
        }

        public int AccelerateTime
        {
            get
            {
                return this._accelerateTime;
            }
            set
            {
                this._accelerateTime = value;
                base._isDirty = true;
            }
        }

        public int AutoFertilizerCount
        {
            get
            {
                return this._autoFertilizerCount;
            }
            set
            {
                this._autoFertilizerCount = value;
                base._isDirty = true;
            }
        }

        public int AutoFertilizerID
        {
            get
            {
                return this._autoFertilizerID;
            }
            set
            {
                this._autoFertilizerID = value;
                base._isDirty = true;
            }
        }

        public DateTime AutomaticTime
        {
            get
            {
                return this._automaticTime;
            }
            set
            {
                this._automaticTime = value;
                base._isDirty = true;
            }
        }

        public int AutoSeedID
        {
            get
            {
                return this._autoSeedID;
            }
            set
            {
                this._autoSeedID = value;
                base._isDirty = true;
            }
        }

        public int AutoSeedIDCount
        {
            get
            {
                return this._autoSeedIDCount;
            }
            set
            {
                this._autoSeedIDCount = value;
                base._isDirty = true;
            }
        }

        public int FarmID
        {
            get
            {
                return this._farmID;
            }
            set
            {
                this._farmID = value;
                base._isDirty = true;
            }
        }

        public int FieldID
        {
            get
            {
                return this._fieldID;
            }
            set
            {
                this._fieldID = value;
                base._isDirty = true;
            }
        }

        public int FieldValidDate
        {
            get
            {
                return this._fieldValidDate;
            }
            set
            {
                this._fieldValidDate = value;
                base._isDirty = true;
            }
        }

        public int GainCount
        {
            get
            {
                return this._gainCount;
            }
            set
            {
                this._gainCount = value;
                base._isDirty = true;
            }
        }

        public int gainFieldId
        {
            get
            {
                return this._gainFieldId;
            }
            set
            {
                this._gainFieldId = value;
                base._isDirty = true;
            }
        }

        public int ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
                base._isDirty = true;
            }
        }

        public bool isAutomatic
        {
            get
            {
                return this._isAutomatic;
            }
            set
            {
                this._isAutomatic = value;
                base._isDirty = true;
            }
        }

        public bool IsExit
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

        public int payFieldTime
        {
            get
            {
                return this._payFieldTime;
            }
            set
            {
                this._payFieldTime = value;
                base._isDirty = true;
            }
        }

        public DateTime PayTime
        {
            get
            {
                return this._payTime;
            }
            set
            {
                this._payTime = value;
                base._isDirty = true;
            }
        }

        public DateTime PlantTime
        {
            get
            {
                return this._plantTime;
            }
            set
            {
                this._plantTime = value;
                base._isDirty = true;
            }
        }

        public int SeedID
        {
            get
            {
                return this._seedID;
            }
            set
            {
                this._seedID = value;
                base._isDirty = true;
            }
        }
    }
}


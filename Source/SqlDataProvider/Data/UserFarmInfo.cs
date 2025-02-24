namespace SqlDataProvider.Data
{
    using System;

    public class UserFarmInfo : DataObject
    {
        private DateTime _autoPayTime;
        private int _autoValidDate;
        private int _buyExpRemainNum;
        private string _farmerName;
        private int _farmID;
        private UserFieldInfo _field;
        private int _gainFieldId;
        private int _ID;
        private bool _isArrange;
        private int _isAutoId;
        private bool _isFarmHelper;
        private int _killCropId;
        private int _matureId;
        private string _payAutoMoney;
        private string _payFieldMoney;
        private int _vipLimitLevel;

        public DateTime AutoPayTime
        {
            get
            {
                return this._autoPayTime;
            }
            set
            {
                this._autoPayTime = value;
                base._isDirty = true;
            }
        }

        public int AutoValidDate
        {
            get
            {
                return this._autoValidDate;
            }
            set
            {
                this._autoValidDate = value;
                base._isDirty = true;
            }
        }

        public int buyExpRemainNum
        {
            get
            {
                return this._buyExpRemainNum;
            }
            set
            {
                this._buyExpRemainNum = value;
                base._isDirty = true;
            }
        }

        public string FarmerName
        {
            get
            {
                return this._farmerName;
            }
            set
            {
                this._farmerName = value;
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

        public UserFieldInfo Field
        {
            get
            {
                return this._field;
            }
            set
            {
                this._field = value;
                base._isDirty = true;
            }
        }

        public int GainFieldId
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
                return this._ID;
            }
            set
            {
                this._ID = value;
                base._isDirty = true;
            }
        }

        public bool isArrange
        {
            get
            {
                return this._isArrange;
            }
            set
            {
                this._isArrange = value;
                base._isDirty = true;
            }
        }

        public int isAutoId
        {
            get
            {
                return this._isAutoId;
            }
            set
            {
                this._isAutoId = value;
                base._isDirty = true;
            }
        }

        public bool isFarmHelper
        {
            get
            {
                return this._isFarmHelper;
            }
            set
            {
                this._isFarmHelper = value;
                base._isDirty = true;
            }
        }

        public int KillCropId
        {
            get
            {
                return this._killCropId;
            }
            set
            {
                this._killCropId = value;
                base._isDirty = true;
            }
        }

        public int MatureId
        {
            get
            {
                return this._matureId;
            }
            set
            {
                this._matureId = value;
                base._isDirty = true;
            }
        }

        public string PayAutoMoney
        {
            get
            {
                return this._payAutoMoney;
            }
            set
            {
                this._payAutoMoney = value;
                base._isDirty = true;
            }
        }

        public string PayFieldMoney
        {
            get
            {
                return this._payFieldMoney;
            }
            set
            {
                this._payFieldMoney = value;
                base._isDirty = true;
            }
        }

        public int VipLimitLevel
        {
            get
            {
                return this._vipLimitLevel;
            }
            set
            {
                this._vipLimitLevel = value;
                base._isDirty = true;
            }
        }
    }
}


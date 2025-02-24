namespace SqlDataProvider.Data
{
    using System;

    public class TreasureDataInfo : DataObject
    {
        private DateTime _BeginDate;
        private int _Count;
        private int _ID;
        private bool _IsExit;
        private int _pos;
        private int _TemplateID;
        private int _UserID;
        private int _ValidDate;

        public DateTime BeginDate
        {
            get
            {
                return this._BeginDate;
            }
            set
            {
                this._BeginDate = value;
                base._isDirty = true;
            }
        }

        public int Count
        {
            get
            {
                return this._Count;
            }
            set
            {
                this._Count = value;
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

        public bool IsExit
        {
            get
            {
                return this._IsExit;
            }
            set
            {
                this._IsExit = value;
                base._isDirty = true;
            }
        }

        public int pos
        {
            get
            {
                return this._pos;
            }
            set
            {
                this._pos = value;
                base._isDirty = true;
            }
        }

        public int TemplateID
        {
            get
            {
                return this._TemplateID;
            }
            set
            {
                this._TemplateID = value;
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

        public int ValidDate
        {
            get
            {
                return this._ValidDate;
            }
            set
            {
                this._ValidDate = value;
                base._isDirty = true;
            }
        }
    }
}


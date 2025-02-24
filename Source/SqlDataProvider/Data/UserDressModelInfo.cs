namespace SqlDataProvider.Data
{
    using System;

    public class UserDressModelInfo : DataObject
    {
        private int _CategoryID;
        private int _ID;
        private bool _IsDelete;
        private int _ItemID;
        private int _SlotID;
        private int _TemplateID;
        private int _UserID;

        public int CategoryID
        {
            get
            {
                return this._CategoryID;
            }
            set
            {
                this._CategoryID = value;
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

        public bool IsDelete
        {
            get
            {
                return this._IsDelete;
            }
            set
            {
                this._IsDelete = value;
                base._isDirty = true;
            }
        }

        public int ItemID
        {
            get
            {
                return this._ItemID;
            }
            set
            {
                this._ItemID = value;
                base._isDirty = true;
            }
        }

        public int SlotID
        {
            get
            {
                return this._SlotID;
            }
            set
            {
                this._SlotID = value;
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
    }
}


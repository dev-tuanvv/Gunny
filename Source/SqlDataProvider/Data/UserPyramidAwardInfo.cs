namespace SqlDataProvider.Data
{
    using System;

    public class UserPyramidAwardInfo : DataObject
    {
        private int _ID;
        private int _Layer;
        private int _Slot;
        private int _TemplateID;
        private int _UserID;

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

        public int Layer
        {
            get
            {
                return this._Layer;
            }
            set
            {
                this._Layer = value;
                base._isDirty = true;
            }
        }

        public int Slot
        {
            get
            {
                return this._Slot;
            }
            set
            {
                this._Slot = value;
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


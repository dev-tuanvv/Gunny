namespace SqlDataProvider.Data
{
    using System;

    public class PetStarExpInfo
    {
        private int _Exp;
        private int _NewID;
        private int _OldID;

        public int Exp
        {
            get
            {
                return this._Exp;
            }
            set
            {
                this._Exp = value;
            }
        }

        public int NewID
        {
            get
            {
                return this._NewID;
            }
            set
            {
                this._NewID = value;
            }
        }

        public int OldID
        {
            get
            {
                return this._OldID;
            }
            set
            {
                this._OldID = value;
            }
        }
    }
}


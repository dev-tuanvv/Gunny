namespace SqlDataProvider.Data
{
    using System;

    public class SevenItem
    {
        private int _Index;
        private int _PosX;
        private int _Tag;
        private int _Type;

        public SevenItem()
        {
        }

        public SevenItem(int index, int type, int posx, int tag)
        {
            this._Index = index;
            this._Type = type;
            this._PosX = posx;
            this._Tag = tag;
        }

        public int Index
        {
            get
            {
                return this._Index;
            }
            set
            {
                this._Index = value;
            }
        }

        public int PosX
        {
            get
            {
                return this._PosX;
            }
            set
            {
                this._PosX = value;
            }
        }

        public int Tag
        {
            get
            {
                return this._Tag;
            }
            set
            {
                this._Tag = value;
            }
        }

        public int Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }
    }
}


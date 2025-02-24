namespace SqlDataProvider.Data
{
    using System;

    public class FightGroundInfo
    {
        private int _ID;
        private int _Level;
        private string _Name;
        private int _Prestige;
        private int _PrestigeForLose;
        private int _PrestigeForWin;
        private string _Title;

        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        public int Level
        {
            get
            {
                return this._Level;
            }
            set
            {
                this._Level = value;
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        public int Prestige
        {
            get
            {
                return this._Prestige;
            }
            set
            {
                this._Prestige = value;
            }
        }

        public int PrestigeForLose
        {
            get
            {
                return this._PrestigeForLose;
            }
            set
            {
                this._PrestigeForLose = value;
            }
        }

        public int PrestigeForWin
        {
            get
            {
                return this._PrestigeForWin;
            }
            set
            {
                this._PrestigeForWin = value;
            }
        }

        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }
    }
}


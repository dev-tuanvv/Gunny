namespace SqlDataProvider.Data
{
    using System;

    public class MagicStoneTemplateInfo
    {
        private int _Agility;
        private int _attack;
        private int _Defence;
        private int _exp;
        private int _ID;
        private int _level;
        private int _Luck;
        private int _MagicAttack;
        private int _MagicDefence;
        private int _TemplateID;

        public int Agility
        {
            get
            {
                return this._Agility;
            }
            set
            {
                this._Agility = value;
            }
        }

        public int Attack
        {
            get
            {
                return this._attack;
            }
            set
            {
                this._attack = value;
            }
        }

        public int Defence
        {
            get
            {
                return this._Defence;
            }
            set
            {
                this._Defence = value;
            }
        }

        public int Exp
        {
            get
            {
                return this._exp;
            }
            set
            {
                this._exp = value;
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
            }
        }

        public int Level
        {
            get
            {
                return this._level;
            }
            set
            {
                this._level = value;
            }
        }

        public int Luck
        {
            get
            {
                return this._Luck;
            }
            set
            {
                this._Luck = value;
            }
        }

        public int MagicAttack
        {
            get
            {
                return this._MagicAttack;
            }
            set
            {
                this._MagicAttack = value;
            }
        }

        public int MagicDefence
        {
            get
            {
                return this._MagicDefence;
            }
            set
            {
                this._MagicDefence = value;
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
            }
        }
    }
}


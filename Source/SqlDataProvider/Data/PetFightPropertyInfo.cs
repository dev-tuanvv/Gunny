﻿namespace SqlDataProvider.Data
{
    using System;

    public class PetFightPropertyInfo
    {
        private int _Agility;
        private int _Attack;
        private int _Blood;
        private int _Defence;
        private int _Exp;
        private int _ID;
        private int _Lucky;

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
                return this._Attack;
            }
            set
            {
                this._Attack = value;
            }
        }

        public int Blood
        {
            get
            {
                return this._Blood;
            }
            set
            {
                this._Blood = value;
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
                return this._Exp;
            }
            set
            {
                this._Exp = value;
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

        public int Lucky
        {
            get
            {
                return this._Lucky;
            }
            set
            {
                this._Lucky = value;
            }
        }
    }
}


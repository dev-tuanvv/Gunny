namespace SqlDataProvider.Data
{
    using System;

    public class UsersCardInfo : DataObject
    {
        private int _agility;
        private int _attack;
        private int _cardGp;
        private int _cardID;
        private int _count;
        private int _damage;
        private int _defence;
        private int _guard;
        private bool _isFirstGet;
        private int _level;
        private int _luck;
        private int _place;
        private int _templateID;
        private int _userID;

        public int Agility
        {
            get
            {
                return this._agility;
            }
            set
            {
                this._agility = value;
                base._isDirty = true;
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
                base._isDirty = true;
            }
        }

        public int CardGP
        {
            get
            {
                return this._cardGp;
            }
            set
            {
                this._cardGp = value;
                base._isDirty = true;
            }
        }

        public int CardID
        {
            get
            {
                return this._cardID;
            }
            set
            {
                this._cardID = value;
                base._isDirty = true;
            }
        }

        public int Count
        {
            get
            {
                return this._count;
            }
            set
            {
                this._count = value;
                base._isDirty = true;
            }
        }

        public int Damage
        {
            get
            {
                return this._damage;
            }
            set
            {
                this._damage = value;
                base._isDirty = true;
            }
        }

        public int Defence
        {
            get
            {
                return this._defence;
            }
            set
            {
                this._defence = value;
                base._isDirty = true;
            }
        }

        public int Guard
        {
            get
            {
                return this._guard;
            }
            set
            {
                this._guard = value;
                base._isDirty = true;
            }
        }

        public bool isFirstGet
        {
            get
            {
                return this._isFirstGet;
            }
            set
            {
                this._isFirstGet = value;
                base._isDirty = true;
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
                base._isDirty = true;
            }
        }

        public int Luck
        {
            get
            {
                return this._luck;
            }
            set
            {
                this._luck = value;
                base._isDirty = true;
            }
        }

        public int Place
        {
            get
            {
                return this._place;
            }
            set
            {
                this._place = value;
                base._isDirty = true;
            }
        }

        public int TemplateID
        {
            get
            {
                return this._templateID;
            }
            set
            {
                this._templateID = value;
                base._isDirty = true;
            }
        }

        public int UserID
        {
            get
            {
                return this._userID;
            }
            set
            {
                this._userID = value;
                base._isDirty = true;
            }
        }
    }
}


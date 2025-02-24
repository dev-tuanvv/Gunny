namespace SqlDataProvider.Data
{
    using System;

    public class UserWorldBoss
    {
        private int _damage;
        private int _honor;
        private string _nickName;
        private int _userID;

        public int Damage
        {
            get
            {
                return this._damage;
            }
            set
            {
                this._damage = value;
            }
        }

        public int Honor
        {
            get
            {
                return this._honor;
            }
            set
            {
                this._honor = value;
            }
        }

        public string NickName
        {
            get
            {
                return this._nickName;
            }
            set
            {
                this._nickName = value;
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
            }
        }
    }
}


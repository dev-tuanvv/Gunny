namespace SqlDataProvider.Data
{
    using System;

    public class MarryProp
    {
        private bool _isCreatedMarryRoom;
        private bool _isGotRing;
        private bool _isMarried;
        private int _selfMarryRoomID;
        private int _spouseID;
        private string _spouseName;

        public bool IsCreatedMarryRoom
        {
            get
            {
                return this._isCreatedMarryRoom;
            }
            set
            {
                this._isCreatedMarryRoom = value;
            }
        }

        public bool IsGotRing
        {
            get
            {
                return this._isGotRing;
            }
            set
            {
                this._isGotRing = value;
            }
        }

        public bool IsMarried
        {
            get
            {
                return this._isMarried;
            }
            set
            {
                this._isMarried = value;
            }
        }

        public int SelfMarryRoomID
        {
            get
            {
                return this._selfMarryRoomID;
            }
            set
            {
                this._selfMarryRoomID = value;
            }
        }

        public int SpouseID
        {
            get
            {
                return this._spouseID;
            }
            set
            {
                this._spouseID = value;
            }
        }

        public string SpouseName
        {
            get
            {
                return this._spouseName;
            }
            set
            {
                this._spouseName = value;
            }
        }
    }
}


namespace SqlDataProvider.Data
{
    using System;

    public class UserGemStone : DataObject
    {
        private int _equipPlace;
        private int _figSpiritId;
        private string _figSpiritIdValue;
        private int _ID;
        private int _userID;

        public int EquipPlace
        {
            get
            {
                return this._equipPlace;
            }
            set
            {
                this._equipPlace = value;
                base._isDirty = true;
            }
        }

        public int FigSpiritId
        {
            get
            {
                return this._figSpiritId;
            }
            set
            {
                this._figSpiritId = value;
                base._isDirty = true;
            }
        }

        public string FigSpiritIdValue
        {
            get
            {
                return this._figSpiritIdValue;
            }
            set
            {
                this._figSpiritIdValue = value;
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


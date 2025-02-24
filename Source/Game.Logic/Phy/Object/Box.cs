namespace Game.Logic.Phy.Object
{
    using SqlDataProvider.Data;
    using System;
    using System.Drawing;

    public class Box : PhysicalObj
    {
        private int _liveCount;
        private int _userID;
        private SqlDataProvider.Data.ItemInfo m_item;

        public Box(int id, string model, SqlDataProvider.Data.ItemInfo item) : base(id, "", model, "", 1, 1, 0)
        {
            this._userID = 0;
            base.m_rect = new Rectangle(-15, -15, 30, 30);
            this.m_item = item;
        }

        public override void CollidedByObject(Physics phy)
        {
            if (phy is SimpleBomb)
            {
                SimpleBomb bomb = phy as SimpleBomb;
                bomb.Owner.PickBox(this);
            }
        }

        public SqlDataProvider.Data.ItemInfo Item
        {
            get
            {
                return this.m_item;
            }
        }

        public int LiveCount
        {
            get
            {
                return this._liveCount;
            }
            set
            {
                this._liveCount = value;
            }
        }

        public override int Type
        {
            get
            {
                return 1;
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


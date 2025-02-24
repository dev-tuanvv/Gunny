namespace Game.Logic.Phy.Object
{
    using System;
    using System.Drawing;

    public class Ball : PhysicalObj
    {
        private int _liveCount;

        public Ball(int id, string action) : base(id, "", "asset.game.six.ball", action, 1, 1, 0)
        {
            base.m_rect = new Rectangle(-15, -15, 30, 30);
        }

        public Ball(int id, string name, string defaultAction, int scale, int rotation) : base(id, name, "asset.game.six.ball", defaultAction, scale, rotation, 0)
        {
            base.m_rect = new Rectangle(-30, -30, 60, 60);
        }

        public override void CollidedByObject(Physics phy)
        {
            if (phy is SimpleBomb)
            {
                SimpleBomb bomb = phy as SimpleBomb;
                bomb.Owner.PickBall(this);
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
                return 2;
            }
        }
    }
}


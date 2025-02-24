namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Maths;
    using Game.Logic.Phy.Object;
    using System;
    using System.Drawing;

    public class PlayerSetXYAction : BaseAction
    {
        private bool m_isSend;
        private Player m_player;
        private int m_speed;
        private Point m_target;
        private Point m_v;

        public PlayerSetXYAction(Player player, Point target, int delay, int speed) : base(0, delay)
        {
            this.m_player = player;
            this.m_target = target;
            this.m_speed = speed;
            this.m_v = new Point(target.X - this.m_player.X, target.Y - this.m_player.Y);
            this.m_v.Normalize(20);
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            if (!this.m_isSend)
            {
                this.m_isSend = true;
                this.m_player.SpeedMultX(this.m_speed, "speedX");
                game.SendPlayerMove2(this.m_player, 4, this.m_target.X, this.m_target.Y, (this.m_v.X > 0) ? ((byte) 1) : ((byte) 0xff), this.m_player.IsLiving);
            }
            if (this.m_target.Distance(this.m_player.X, this.m_player.Y) > 20.0)
            {
                this.m_player.SetXY(this.m_player.X + this.m_v.X, this.m_player.Y + this.m_v.Y);
            }
            else
            {
                this.m_player.SetXY(this.m_target.X, this.m_target.Y);
                base.Finish(tick);
            }
        }
    }
}


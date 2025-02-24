namespace Game.Server.RingStation.Action
{
    using Game.Server.RingStation;
    using System;

    public class PlayerShotAction : BaseAction
    {
        private int m_angle;
        private int m_force;
        private int m_x;
        private int m_y;

        public PlayerShotAction(int x, int y, int force, int angle, int delay) : base(delay, 0)
        {
            this.m_x = x;
            this.m_y = y;
            this.m_force = force;
            this.m_angle = angle;
        }

        protected override void ExecuteImp(RingStationGamePlayer player, long tick)
        {
            player.SendShootTag(true, 0);
            player.SendGameCMDShoot(this.m_x, this.m_y, this.m_force, this.m_angle);
            base.Finish(tick);
        }
    }
}


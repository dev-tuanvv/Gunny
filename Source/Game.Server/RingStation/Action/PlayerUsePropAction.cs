namespace Game.Server.RingStation.Action
{
    using Game.Server.RingStation;
    using System;

    public class PlayerUsePropAction : BaseAction
    {
        private int m_prop;

        public PlayerUsePropAction(int prop, int delay) : base(delay, 0)
        {
            this.m_prop = prop;
        }

        protected override void ExecuteImp(RingStationGamePlayer player, long tick)
        {
            player.SendUseProp(this.m_prop);
            base.Finish(tick);
        }
    }
}


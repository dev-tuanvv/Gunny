namespace Game.Server.RingStation.Action
{
    using Game.Server.RingStation;
    using System;

    public class PlayerLoadingAction : BaseAction
    {
        private int m_loading;

        public PlayerLoadingAction(int state, int delay) : base(delay, 0)
        {
            this.m_loading = state;
        }

        protected override void ExecuteImp(RingStationGamePlayer player, long tick)
        {
            if (this.m_loading > 100)
            {
                this.m_loading = 100;
            }
            player.SendLoadingComplete(this.m_loading);
            if (this.m_loading < 100)
            {
                Random random = new Random();
                player.AddAction(new PlayerLoadingAction(this.m_loading + random.Next(20, 40), 0x3e8));
            }
            base.Finish(tick);
        }
    }
}


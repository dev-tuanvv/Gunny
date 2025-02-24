namespace Game.Server.RingStation.Action
{
    using Game.Server.RingStation;
    using System;

    public class PlayerBuffStuntAction : BaseAction
    {
        private int m_type;

        public PlayerBuffStuntAction(int type, int delay) : base(delay, 0)
        {
            this.m_type = type;
        }

        protected override void ExecuteImp(RingStationGamePlayer player, long tick)
        {
            player.sendGameCMDStunt(this.m_type);
            base.Finish(tick);
        }
    }
}


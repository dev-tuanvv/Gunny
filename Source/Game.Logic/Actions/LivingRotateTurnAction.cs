namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingRotateTurnAction : BaseAction
    {
        private string m_endPlay;
        private Player m_player;
        private int m_rotation;
        private int m_speed;

        public LivingRotateTurnAction(Player player, int rotation, int speed, string endPlay, int delay) : base(0, delay)
        {
            this.m_player = player;
            this.m_rotation = rotation;
            this.m_speed = speed;
            this.m_endPlay = endPlay;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.SendLivingTurnRotation(this.m_player, this.m_rotation, this.m_speed, this.m_endPlay);
            base.Finish(tick);
        }
    }
}


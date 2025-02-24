namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingDieAction : BaseAction
    {
        private Living m_living;

        public LivingDieAction(Living living, int delay) : base(delay, 0x3e8)
        {
            this.m_living = living;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.m_living.Die();
            base.Finish(tick);
        }
    }
}


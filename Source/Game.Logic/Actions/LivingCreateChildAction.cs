namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingCreateChildAction : BaseAction
    {
        private Living m_living;
        private int m_type;

        public LivingCreateChildAction(Living living, int type, int delay) : base(delay)
        {
            this.m_living = living;
            this.m_type = type;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            base.Finish(tick);
        }
    }
}


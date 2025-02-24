namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingChangeDirectionAction : BaseAction
    {
        private int m_direction;
        private Living m_Living;

        public LivingChangeDirectionAction(Living living, int direction, int delay) : base(delay)
        {
            this.m_Living = living;
            this.m_direction = direction;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.m_Living.Direction = this.m_direction;
            base.Finish(tick);
        }
    }
}


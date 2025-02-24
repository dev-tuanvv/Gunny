namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Effects;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingDelayEffectAction : BaseAction
    {
        private AbstractEffect m_effect;
        private Living m_living;

        public LivingDelayEffectAction(Living living, AbstractEffect effect, int delay) : base(delay)
        {
            this.m_effect = effect;
            this.m_living = living;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.m_effect.Start(this.m_living);
            base.Finish(tick);
        }
    }
}


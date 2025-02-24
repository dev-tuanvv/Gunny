namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingAfterShootedFrozen : BaseAction
    {
        private Living living_0;

        public LivingAfterShootedFrozen(Living living, int delay) : base(delay, 0)
        {
            this.living_0 = living;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.living_0.OnAfterTakedFrozen();
            base.Finish(tick);
        }
    }
}


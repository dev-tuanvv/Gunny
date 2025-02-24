namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingAfterShootedAction : BaseAction
    {
        private Living VrtQboWger;

        public LivingAfterShootedAction(Living living, int delay) : base(delay, 0)
        {
            this.VrtQboWger = living;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.VrtQboWger.OnAfterTakedBomb();
            base.Finish(tick);
        }
    }
}


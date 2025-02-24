namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingBoltMoveAction : BaseAction
    {
        private int int_0;
        private int int_1;
        private Living living_0;

        public LivingBoltMoveAction(Living living, int x, int y, int delay) : base(delay)
        {
            this.living_0 = living;
            this.int_0 = x;
            this.int_1 = y;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.living_0.SetXY(this.int_0, this.int_1);
            game.method_24(this.living_0);
            base.Finish(tick);
        }
    }
}


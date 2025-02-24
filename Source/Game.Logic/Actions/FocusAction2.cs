namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class FocusAction2 : BaseAction
    {
        private int int_0;
        private Physics physics_0;

        public FocusAction2(Physics obj, int type, int delay, int finishTime) : base(delay, finishTime)
        {
            this.physics_0 = obj;
            this.int_0 = type;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.method_11(this.physics_0, this.int_0);
            base.Finish(tick);
        }
    }
}


namespace Game.Logic.Actions
{
    using Game.Logic;
    using System;

    public class FocusFreeAction : BaseAction
    {
        private int int_0;
        private int int_1;
        private int int_2;

        public FocusFreeAction(int x, int y, int type, int delay, int finishTime) : base(delay, finishTime)
        {
            this.int_0 = x;
            this.int_1 = y;
            this.int_2 = type;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.method_10(this.int_0, this.int_1, this.int_2);
            base.Finish(tick);
        }
    }
}


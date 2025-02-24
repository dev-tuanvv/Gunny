namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PhysicalObjDoAction2 : BaseAction
    {
        private PhysicalObj physicalObj_0;
        private string string_0;

        public PhysicalObjDoAction2(PhysicalObj obj, string action, int delay, int movieTime) : base(delay, movieTime)
        {
            this.physicalObj_0 = obj;
            this.string_0 = action;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.physicalObj_0.CurrentAction = this.string_0;
            game.method_12(this.physicalObj_0);
            base.Finish(tick);
        }
    }
}


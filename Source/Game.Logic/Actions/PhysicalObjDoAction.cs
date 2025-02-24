namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PhysicalObjDoAction : BaseAction
    {
        private string m_action;
        private PhysicalObj m_obj;

        public PhysicalObjDoAction(PhysicalObj obj, string action, int delay, int movieTime) : base(delay, movieTime)
        {
            this.m_obj = obj;
            this.m_action = action;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.m_obj.CurrentAction = this.m_action;
            game.SendPhysicalObjPlayAction(this.m_obj);
            base.Finish(tick);
        }
    }
}


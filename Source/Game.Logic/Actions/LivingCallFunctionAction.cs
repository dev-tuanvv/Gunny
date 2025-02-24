namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingCallFunctionAction : BaseAction
    {
        private LivingCallBack m_func;
        private Living m_living;

        public LivingCallFunctionAction(Living living, LivingCallBack func, int delay) : base(delay)
        {
            this.m_living = living;
            this.m_func = func;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            try
            {
                this.m_func();
            }
            finally
            {
                base.Finish(tick);
            }
        }
    }
}


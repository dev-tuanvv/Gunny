namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class CallFunctionAction : BaseAction
    {
        private LivingCallBack m_func;

        public CallFunctionAction(LivingCallBack func, int delay) : base(delay)
        {
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


namespace Game.Logic.Actions
{
    using Game.Logic;
    using System;

    public class BaseAction : IAction
    {
        private long m_finishDelay;
        private long m_finishTick;
        private long m_tick;

        public BaseAction(int delay) : this(delay, 0)
        {
        }

        public BaseAction(int delay, int finishDelay)
        {
            this.m_tick = TickHelper.GetTickCount() + delay;
            this.m_finishDelay = finishDelay;
            this.m_finishTick = 0x7fffffffffffffffL;
        }

        public void Execute(BaseGame game, long tick)
        {
            if ((this.m_tick <= tick) && (this.m_finishTick == 0x7fffffffffffffffL))
            {
                this.ExecuteImp(game, tick);
            }
        }

        protected virtual void ExecuteImp(BaseGame game, long tick)
        {
            this.Finish(tick);
        }

        public void Finish(long tick)
        {
            this.m_finishTick = tick + this.m_finishDelay;
        }

        public bool IsFinished(long tick)
        {
            return (this.m_finishTick <= tick);
        }
    }
}


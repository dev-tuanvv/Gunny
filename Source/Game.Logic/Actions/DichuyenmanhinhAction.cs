namespace Game.Logic.Actions
{
    using Game.Logic;
    using System;

    public class DichuyenmanhinhAction : BaseAction
    {
        private int m_type;
        private int m_x;
        private int m_y;

        public DichuyenmanhinhAction(int x, int y, int type, int delay, int finishTime) : base(delay, finishTime)
        {
            this.m_x = x;
            this.m_y = y;
            this.m_type = type;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.Dichuyenmanhinhmini(this.m_x, this.m_y, this.m_type);
            base.Finish(tick);
        }
    }
}


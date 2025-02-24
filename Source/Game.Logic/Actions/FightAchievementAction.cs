namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class FightAchievementAction : BaseAction
    {
        private int m_delay;
        private Living m_living;
        private int m_num;
        private int m_type;

        public FightAchievementAction(Living living, int type, int num, int delay) : base(delay, 0x5dc)
        {
            this.m_living = living;
            this.m_num = num;
            this.m_type = type;
            this.m_delay = delay;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.SendFightAchievement(this.m_living, this.m_type, this.m_num, this.m_delay);
            base.Finish(tick);
        }
    }
}


namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Effects;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingSealAction : BaseAction
    {
        private Living m_Living;
        private Player m_Target;
        private int m_Type;

        public LivingSealAction(Living Living, Player target, int type, int delay) : base(delay, 0x7d0)
        {
            this.m_Living = Living;
            this.m_Target = target;
            this.m_Type = type;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.m_Target.AddEffect(new SealEffect(2), 0);
            base.Finish(tick);
        }
    }
}


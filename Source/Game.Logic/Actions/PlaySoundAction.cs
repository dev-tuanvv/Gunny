namespace Game.Logic.Actions
{
    using Game.Logic;
    using System;

    public class PlaySoundAction : BaseAction
    {
        private string m_action;

        public PlaySoundAction(string action, int delay) : base(delay, 0x3e8)
        {
            this.m_action = action;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            ((PVEGame) game).SendPlaySound(this.m_action);
            base.Finish(tick);
        }
    }
}


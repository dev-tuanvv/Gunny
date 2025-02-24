namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingPlayeMovieAction : BaseAction
    {
        private string m_action;
        private Living m_living;

        public LivingPlayeMovieAction(Living living, string action, int delay, int movieTime) : base(delay, movieTime)
        {
            this.m_living = living;
            this.m_action = action;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.SendLivingPlayMovie(this.m_living, this.m_action);
            base.Finish(tick);
        }
    }
}


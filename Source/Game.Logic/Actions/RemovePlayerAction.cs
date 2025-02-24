namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class RemovePlayerAction : IAction
    {
        private bool m_isFinished;
        private Player m_player;

        public RemovePlayerAction(Player player)
        {
            this.m_player = player;
            this.m_isFinished = false;
        }

        public void Execute(BaseGame game, long tick)
        {
            this.m_player.DeadLink();
            this.m_isFinished = true;
        }

        public bool IsFinished(long tick)
        {
            return this.m_isFinished;
        }
    }
}


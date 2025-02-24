namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class WaitPlayerLoadingAction : IAction
    {
        private bool m_isFinished;
        private long m_time;

        public WaitPlayerLoadingAction(BaseGame game, int maxTime)
        {
            this.m_time = TickHelper.GetTickCount() + maxTime;
            game.GameStarted += new GameEventHandle(this.game_GameStarted);
        }

        public void Execute(BaseGame game, long tick)
        {
            if ((!this.m_isFinished && (tick > this.m_time)) && (game.GameState == eGameState.Loading))
            {
                if (game.GameState == eGameState.Loading)
                {
                    List<Player> allFightPlayers = game.GetAllFightPlayers();
                    foreach (Player player in allFightPlayers)
                    {
                        if (player.LoadingProcess < 100)
                        {
                            game.SendPlayerRemove(player);
                            game.RemovePlayer(player.PlayerDetail, false);
                        }
                    }
                    game.CheckState(0);
                }
                this.m_isFinished = true;
            }
        }

        private void game_GameStarted(AbstractGame game)
        {
            game.GameStarted -= new GameEventHandle(this.game_GameStarted);
            this.m_isFinished = true;
        }

        public bool IsFinished(long tick)
        {
            return this.m_isFinished;
        }
    }
}


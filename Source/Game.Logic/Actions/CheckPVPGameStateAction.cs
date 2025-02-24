namespace Game.Logic.Actions
{
    using Game.Logic;
    using log4net;
    using System;
    using System.Reflection;

    public class CheckPVPGameStateAction : IAction
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool m_isFinished = false;
        private long m_tick;

        public CheckPVPGameStateAction(int delay)
        {
            this.m_tick += TickHelper.GetTickCount() + delay;
        }

        public void Execute(BaseGame game, long tick)
        {
            if (this.m_tick <= tick)
            {
                PVPGame game2 = game as PVPGame;
                if (game2 != null)
                {
                    switch (game.GameState)
                    {
                        case eGameState.Prepared:
                            game2.StartLoading();
                            break;

                        case eGameState.Loading:
                            if (game2.IsAllComplete())
                            {
                                game2.StartGame();
                            }
                            break;

                        case eGameState.Playing:
                            if ((game2.CurrentPlayer == null) || !game2.CurrentPlayer.IsAttacking)
                            {
                                if (game2.CanGameOver())
                                {
                                    game2.GameOver();
                                }
                                else
                                {
                                    game2.NextTurn();
                                }
                            }
                            break;

                        case eGameState.GameOver:
                            game2.Stop();
                            break;
                    }
                }
                this.m_isFinished = true;
            }
        }

        public bool IsFinished(long tick)
        {
            return this.m_isFinished;
        }
    }
}


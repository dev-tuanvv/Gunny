namespace Game.Logic.Actions
{
    using Game.Logic;
    using log4net;
    using System;
    using System.Reflection;

    public class CheckPVEGameStateAction : IAction
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool m_isFinished;
        private long m_time;

        public CheckPVEGameStateAction(int delay)
        {
            this.m_time = TickHelper.GetTickCount() + delay;
            this.m_isFinished = false;
        }

        public void Execute(BaseGame game, long tick)
        {
            if ((this.m_time <= tick) && (game.GetWaitTimer() < tick))
            {
                PVEGame game2 = game as PVEGame;
                if (game2 != null)
                {
                    switch (game2.GameState)
                    {
                        case eGameState.Inited:
                            game2.Prepare();
                            break;

                        case eGameState.Prepared:
                            game2.PrepareNewSession();
                            break;

                        case eGameState.Loading:
                            if (!game2.IsAllComplete())
                            {
                                game.WaitTime(0x3e8);
                                break;
                            }
                            game2.StartGame();
                            break;

                        case eGameState.GameStartMovie:
                            if (game.CurrentActionCount <= 1)
                            {
                                game2.StartGame();
                                break;
                            }
                            game2.StartGameMovie();
                            break;

                        case eGameState.GameStart:
                            game2.PrepareNewGame();
                            break;

                        case eGameState.Playing:
                            if (((game2.CurrentLiving == null) || !game2.CurrentLiving.IsAttacking) && (game.CurrentActionCount <= 1))
                            {
                                if (!game2.CanGameOver())
                                {
                                    game2.NextTurn();
                                    break;
                                }
                                if (!game2.IsLabyrinth() || !game2.CanEnterGate)
                                {
                                    game2.GameOver();
                                    break;
                                }
                                game2.GameOverMovie();
                            }
                            break;

                        case eGameState.GameOver:
                            if (!game2.HasNextSession())
                            {
                                game2.GameOverAllSession();
                                break;
                            }
                            game2.PrepareNewSession();
                            break;

                        case eGameState.SessionPrepared:
                            if (!game2.CanStartNewSession())
                            {
                                game.WaitTime(0x3e8);
                                break;
                            }
                            game2.StartLoading();
                            break;

                        case eGameState.ALLSessionStopped:
                            if ((game2.PlayerCount != 0) && (game2.WantTryAgain != 0))
                            {
                                if (game2.WantTryAgain == 1)
                                {
                                    game2.ShowDragonLairCard();
                                    game2.PrepareNewSession();
                                }
                                else if (game2.WantTryAgain == 2)
                                {
                                    game2.SessionId--;
                                    game2.PrepareNewSession();
                                }
                                else
                                {
                                    game.WaitTime(0x3e8);
                                }
                                break;
                            }
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


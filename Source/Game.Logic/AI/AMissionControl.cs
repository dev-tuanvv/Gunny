    using Game.Logic;
    using System;

namespace Game.Logic.AI
{
    
    public abstract class AMissionControl
    {
        private PVEGame m_game;

        protected AMissionControl()
        {
        }

        public virtual int CalculateScoreGrade(int score)
        {
            return 0;
        }

        public virtual bool CanGameOver()
        {
            return true;
        }

        public virtual void Dispose()
        {
        }

        public virtual void DoOther()
        {
        }

        public virtual void GameOverAllSession()
        {
        }

        public virtual void OnBeginNewTurn()
        {
        }

        public virtual void OnDied()
        {
            throw new NotImplementedException();
        }

        public virtual void OnGameOver()
        {
        }

        public virtual void OnGameOverMovie()
        {
        }

        public virtual void OnNewTurnStarted()
        {
        }

        public virtual void OnPrepareNewSession()
        {
        }

        public virtual void OnShooted()
        {
        }

        public virtual void OnStartGame()
        {
        }

        public virtual void OnStartMovie()
        {
        }

        public virtual int UpdateUIData()
        {
            return 0;
        }

        public PVEGame Game
        {
            get
            {
                return this.m_game;
            }
            set
            {
                this.m_game = value;
            }
        }
    }
}


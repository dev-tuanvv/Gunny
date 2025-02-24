namespace Game.Logic
{
    using Game.Base.Packets;
    using Game.Logic.Phy.Object;
    using System;
    using System.Threading;

    public class AbstractGame
    {
        private int m_disposed;
        protected eGameType m_gameType;
        private int m_id;
        protected eMapType m_mapType;
        protected eRoomType m_roomType;
        protected int m_timeType;

        public event GameEventHandle GameStarted;

        public event GameEventHandle GameStopped;

        public AbstractGame(int id, eRoomType roomType, eGameType gameType, int timeType)
        {
            this.m_id = id;
            this.m_roomType = roomType;
            this.m_gameType = gameType;
            this.m_timeType = timeType;
            switch (this.m_roomType)
            {
                case eRoomType.Match:
                    this.m_mapType = eMapType.PairUp;
                    break;

                case eRoomType.Freedom:
                    this.m_mapType = eMapType.Normal;
                    break;

                default:
                    this.m_mapType = eMapType.Normal;
                    break;
            }
        }

        public virtual Player AddPlayer(IGamePlayer player)
        {
            return null;
        }

        public virtual bool CanAddPlayer()
        {
            return false;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.m_disposed, 1) == 0)
            {
                this.Dispose(true);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual void MissionStart(IGamePlayer host)
        {
        }

        protected void OnGameStarted()
        {
            if (this.GameStarted != null)
            {
                this.GameStarted(this);
            }
        }

        protected void OnGameStopped()
        {
            if (this.GameStopped != null)
            {
                this.GameStopped(this);
            }
        }

        public virtual void Pause(int time)
        {
        }

        public virtual void ProcessData(GSPacketIn pkg)
        {
        }

        public virtual Player RemovePlayer(IGamePlayer player, bool IsKick)
        {
            return null;
        }

        public virtual void Resume()
        {
        }

        public virtual void Start()
        {
            this.OnGameStarted();
        }

        public virtual void Stop()
        {
            this.OnGameStopped();
        }

        public eGameType GameType
        {
            get
            {
                return this.m_gameType;
            }
        }

        public int Id
        {
            get
            {
                return this.m_id;
            }
        }

        public eRoomType RoomType
        {
            get
            {
                return this.m_roomType;
            }
        }

        public int TimeType
        {
            get
            {
                return this.m_timeType;
            }
        }
    }
}


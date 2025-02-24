namespace Game.Server.GameUtils
{
    using Bussiness;
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PlayerRank
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private UserRankInfo m_currentRank;
        protected object m_lock = new object();
        protected GamePlayer m_player;
        private List<UserRankInfo> m_rank;
        private bool m_saveToDb;

        public PlayerRank(GamePlayer player, bool saveTodb)
        {
            this.m_player = player;
            this.m_saveToDb = saveTodb;
            this.m_rank = new List<UserRankInfo>();
            this.m_currentRank = this.GetRank(this.m_player.PlayerCharacter.Honor);
        }

        public void AddRank(UserRankInfo info)
        {
            lock (this.m_rank)
            {
                this.m_rank.Add(info);
            }
        }

        public void AddRank(string honor)
        {
            UserRankInfo info = new UserRankInfo {
                ID = 0,
                UserID = this.m_player.PlayerCharacter.ID,
                UserRank = honor,
                Attack = 0,
                Defence = 0,
                Luck = 0,
                Agility = 0,
                HP = 0,
                Damage = 0,
                Guard = 0,
                BeginDate = DateTime.Now,
                Validate = 0,
                IsExit = true
            };
            this.AddRank(info);
        }

        public void CreateRank(int UserID)
        {
            new List<UserRankInfo>();
            UserRankInfo info = new UserRankInfo {
                ID = 0,
                UserID = UserID,
                UserRank = "B\x00e9 tập chơi",
                Attack = 0,
                Defence = 0,
                Luck = 0,
                Agility = 0,
                HP = 0,
                Damage = 0,
                Guard = 0,
                BeginDate = DateTime.Now,
                Validate = 0,
                IsExit = true
            };
            this.AddRank(info);
        }

        public List<UserRankInfo> GetRank()
        {
            List<UserRankInfo> list = new List<UserRankInfo>();
            foreach (UserRankInfo info in this.m_rank)
            {
                if (info.IsExit)
                {
                    list.Add(info);
                }
            }
            return list;
        }

        public UserRankInfo GetRank(string honor)
        {
            foreach (UserRankInfo info in this.m_rank)
            {
                if (info.UserRank == honor)
                {
                    return info;
                }
            }
            return null;
        }

        public bool IsRank(string honor)
        {
            foreach (UserRankInfo info in this.m_rank)
            {
                if (info.UserRank == honor)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    List<UserRankInfo> singleUserRank = bussiness.GetSingleUserRank(this.Player.PlayerCharacter.ID);
                    if (singleUserRank.Count == 0)
                    {
                        this.CreateRank(this.Player.PlayerCharacter.ID);
                    }
                    else
                    {
                        foreach (UserRankInfo info in singleUserRank)
                        {
                            if (info.IsValidRank())
                            {
                                this.AddRank(info);
                            }
                            else
                            {
                                this.RemoveRank(info);
                            }
                        }
                    }
                }
            }
        }

        public void RemoveRank(UserRankInfo item)
        {
            item.IsExit = false;
            this.AddRank(item);
        }

        public virtual void SaveToDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    lock (this.m_lock)
                    {
                        for (int i = 0; i < this.m_rank.Count; i++)
                        {
                            UserRankInfo item = this.m_rank[i];
                            if ((item != null) && item.IsDirty)
                            {
                                if (item.ID > 0)
                                {
                                    bussiness.UpdateUserRank(item);
                                }
                                else
                                {
                                    bussiness.AddUserRank(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        public UserRankInfo CurrentRank
        {
            get
            {
                return this.m_currentRank;
            }
            set
            {
                this.m_currentRank = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
        }

        public List<UserRankInfo> Ranks
        {
            get
            {
                return this.m_rank;
            }
            set
            {
                this.m_rank = value;
            }
        }
    }
}


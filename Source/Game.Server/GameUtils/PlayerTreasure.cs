namespace Game.Server.GameUtils
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PlayerTreasure
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected object m_lock = new object();
        protected GamePlayer m_player;
        private bool m_saveToDb;
        private UserTreasureInfo m_Treasure;
        private List<TreasureDataInfo> m_TreasureData;
        private List<TreasureDataInfo> m_TreasureDig;

        public PlayerTreasure(GamePlayer player, bool saveTodb)
        {
            this.m_player = player;
            this.m_saveToDb = saveTodb;
            this.m_TreasureData = new List<TreasureDataInfo>();
            this.m_TreasureDig = new List<TreasureDataInfo>();
            this.m_Treasure = new UserTreasureInfo();
        }

        public void AddfriendHelpTimes()
        {
            lock (this.m_Treasure)
            {
                if (this.m_Treasure.friendHelpTimes < 5)
                {
                    this.m_Treasure.friendHelpTimes++;
                    if ((this.m_Treasure.friendHelpTimes == 5) && (this.m_Treasure.treasureAdd == 0))
                    {
                        this.m_Treasure.treasureAdd++;
                    }
                }
            }
        }

        public void AddTreasureData(List<TreasureDataInfo> datas)
        {
            lock (this.m_TreasureData)
            {
                foreach (TreasureDataInfo info in datas)
                {
                    this.m_TreasureData.Add(info);
                }
            }
        }

        public void AddTreasureDig(TreasureDataInfo info, int index)
        {
            lock (this.m_TreasureDig)
            {
                this.m_TreasureDig.Add(info);
            }
            lock (this.m_TreasureData)
            {
                this.m_TreasureData[index].pos = info.pos;
            }
        }

        public void Clear()
        {
            lock (this.m_TreasureDig)
            {
                this.m_TreasureDig = new List<TreasureDataInfo>();
            }
        }

        public void CreateTreasure()
        {
            if (this.m_Treasure == null)
            {
                this.m_Treasure = new UserTreasureInfo();
            }
            lock (this.m_Treasure)
            {
                this.m_Treasure.ID = 0;
                this.m_Treasure.UserID = this.Player.PlayerCharacter.ID;
                this.m_Treasure.NickName = this.Player.PlayerCharacter.NickName;
                this.m_Treasure.treasure = 1;
                this.m_Treasure.treasureAdd = 0;
                this.m_Treasure.logoinDays = 1;
                this.m_Treasure.friendHelpTimes = 0;
                this.m_Treasure.isBeginTreasure = false;
                this.m_Treasure.isEndTreasure = false;
                this.m_Treasure.LastLoginDay = DateTime.Now;
            }
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    this.m_Treasure = bussiness.GetSingleTreasure(this.Player.PlayerCharacter.ID);
                    List<TreasureDataInfo> singleTreasureData = bussiness.GetSingleTreasureData(this.Player.PlayerCharacter.ID);
                    if (this.m_Treasure == null)
                    {
                        this.CreateTreasure();
                    }
                    foreach (TreasureDataInfo info in singleTreasureData)
                    {
                        this.m_TreasureData.Add(info);
                        if (info.pos > 0)
                        {
                            this.m_TreasureDig.Add(info);
                        }
                    }
                }
            }
        }

        public virtual void SaveToDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    lock (this.m_lock)
                    {
                        if ((this.m_Treasure != null) && this.m_Treasure.IsDirty)
                        {
                            if (this.m_Treasure.ID > 0)
                            {
                                bussiness.UpdateUserTreasureInfo(this.m_Treasure);
                            }
                            else
                            {
                                bussiness.AddUserTreasureInfo(this.m_Treasure);
                            }
                        }
                        for (int i = 0; i < this.m_TreasureData.Count; i++)
                        {
                            TreasureDataInfo item = this.m_TreasureData[i];
                            if ((item != null) && item.IsDirty)
                            {
                                if (item.ID > 0)
                                {
                                    bussiness.UpdateTreasureData(item);
                                }
                                else
                                {
                                    bussiness.AddTreasureData(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UpdateLoginDay()
        {
            List<TreasureDataInfo> list;
            int iD = this.Player.PlayerCharacter.ID;
            if (this.m_TreasureData.Count == 0)
            {
                list = TreasureAwardMgr.CreateTreasureData(iD);
                this.AddTreasureData(list);
            }
            else if (this.m_Treasure.isValidDate())
            {
                list = TreasureAwardMgr.CreateTreasureData(iD);
                this.UpdateTreasureData(list);
                this.Clear();
            }
            lock (this.m_Treasure)
            {
                if (this.m_Treasure.isValidDate())
                {
                    if (((int) DateTime.Now.Subtract(this.m_Treasure.LastLoginDay).TotalDays) > 1)
                    {
                        this.m_Treasure.logoinDays = 0;
                    }
                    this.m_Treasure.logoinDays++;
                    if (this.m_Treasure.logoinDays > 3)
                    {
                        this.m_Treasure.treasure = 3;
                    }
                    else
                    {
                        this.m_Treasure.treasure = this.m_Treasure.logoinDays;
                    }
                    this.m_Treasure.treasureAdd = 0;
                    this.m_Treasure.friendHelpTimes = 0;
                    this.m_Treasure.isBeginTreasure = false;
                    this.m_Treasure.isEndTreasure = false;
                    this.m_Treasure.LastLoginDay = DateTime.Now;
                }
            }
        }

        public void UpdateTreasureData(List<TreasureDataInfo> datas)
        {
            for (int i = 0; i < this.m_TreasureData.Count; i++)
            {
                datas[i].ID = this.m_TreasureData[i].ID;
            }
            lock (this.m_TreasureData)
            {
                this.m_TreasureData = datas;
            }
        }

        public void UpdateUserTreasure(UserTreasureInfo info)
        {
            lock (this.m_Treasure)
            {
                this.m_Treasure = info;
            }
        }

        public UserTreasureInfo CurrentTreasure
        {
            get
            {
                return this.m_Treasure;
            }
            set
            {
                this.m_Treasure = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
        }

        public List<TreasureDataInfo> TreasureData
        {
            get
            {
                return this.m_TreasureData;
            }
            set
            {
                this.m_TreasureData = value;
            }
        }

        public List<TreasureDataInfo> TreasureDig
        {
            get
            {
                return this.m_TreasureDig;
            }
            set
            {
                this.m_TreasureDig = value;
            }
        }
    }
}


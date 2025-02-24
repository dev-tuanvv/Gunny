namespace Game.Server.GameUtils
{
    using Bussiness;
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PlayerDressModel
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private List<UserDressModelInfo> m_dressmodel;
        protected object m_lock = new object();
        protected GamePlayer m_player;
        private bool m_saveToDb;

        public PlayerDressModel(GamePlayer player, bool saveTodb)
        {
            this.m_player = player;
            this.m_saveToDb = saveTodb;
            this.m_dressmodel = new List<UserDressModelInfo>();
        }

        public void AddDressInfo(UserDressModelInfo info)
        {
            lock (this.m_dressmodel)
            {
                this.m_dressmodel.Add(info);
            }
        }

        public virtual void AddDressModel(UserDressModelInfo dress)
        {
            lock (this.m_dressmodel)
            {
                List<UserDressModelInfo> dressModel = this.GetDressModel(dress.SlotID, dress.CategoryID);
                if (dressModel.Count > 0)
                {
                    foreach (UserDressModelInfo info in dressModel)
                    {
                        this.RemoveDressModel(info);
                    }
                }
                dress.IsDelete = false;
                this.m_dressmodel.Add(dress);
            }
        }

        public virtual bool ClearDressInSlot(int slotid)
        {
            lock (this.m_dressmodel)
            {
                foreach (UserDressModelInfo info in this.m_dressmodel)
                {
                    if (info.SlotID == slotid)
                    {
                        info.IsDelete = true;
                    }
                }
                return true;
            }
        }

        public virtual List<UserDressModelInfo> GetDressModel(int slotID, int catId)
        {
            lock (this.m_dressmodel)
            {
                List<UserDressModelInfo> list3 = new List<UserDressModelInfo>();
                if (this.m_dressmodel.Count > 0)
                {
                    foreach (UserDressModelInfo info in this.m_dressmodel)
                    {
                        if (!(((info.SlotID != slotID) || (info.CategoryID != catId)) || info.IsDelete))
                        {
                            list3.Add(info);
                        }
                    }
                }
                return list3;
            }
        }

        public virtual List<UserDressModelInfo> GetDressModelWithSlotID(int slotID)
        {
            lock (this.m_dressmodel)
            {
                List<UserDressModelInfo> list3 = new List<UserDressModelInfo>();
                if (this.m_dressmodel.Count > 0)
                {
                    foreach (UserDressModelInfo info in this.m_dressmodel)
                    {
                        if (!((info.SlotID != slotID) || info.IsDelete))
                        {
                            list3.Add(info);
                        }
                    }
                }
                return list3;
            }
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    List<UserDressModelInfo> allDressModel = bussiness.GetAllDressModel(this.Player.PlayerCharacter.ID);
                    if (allDressModel.Count > 0)
                    {
                        foreach (UserDressModelInfo info in allDressModel)
                        {
                            this.AddDressInfo(info);
                        }
                    }
                }
            }
        }

        public virtual bool RemoveDressModel(UserDressModelInfo dress)
        {
            lock (this.m_dressmodel)
            {
                foreach (UserDressModelInfo info in this.m_dressmodel)
                {
                    if (info == dress)
                    {
                        info.IsDelete = true;
                        return true;
                    }
                }
                return false;
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
                        for (int i = 0; i < this.m_dressmodel.Count; i++)
                        {
                            UserDressModelInfo item = this.m_dressmodel[i];
                            if ((item != null) && item.IsDirty)
                            {
                                if (!((item.ID <= 0) || item.IsDelete))
                                {
                                    bussiness.UpdateUserDressModel(item);
                                }
                                else if (!((item.ID > 0) || item.IsDelete))
                                {
                                    bussiness.AddUserDressModel(item);
                                }
                                else if (item.IsDelete)
                                {
                                    if (item.ID > 0)
                                    {
                                        bussiness.DeleteUserDressModel(item);
                                    }
                                    this.m_dressmodel.Remove(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<UserDressModelInfo> DressModel
        {
            get
            {
                return this.m_dressmodel;
            }
            set
            {
                this.m_dressmodel = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
        }
    }
}


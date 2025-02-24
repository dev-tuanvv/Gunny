namespace Game.Server.GameUtils
{
    using Bussiness;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PlayerAvatarCollection
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private List<UserAvatarCollectionInfo> m_avtcollect;
        protected object m_lock = new object();
        protected GamePlayer m_player;
        private bool m_saveToDb;

        public PlayerAvatarCollection(GamePlayer player, bool saveTodb)
        {
            this.m_player = player;
            this.m_saveToDb = saveTodb;
            this.m_avtcollect = new List<UserAvatarCollectionInfo>();
        }

        public void AddAvatarCollect(UserAvatarCollectionInfo info)
        {
            lock (this.m_avtcollect)
            {
                this.m_avtcollect.Add(info);
            }
        }

        public virtual void AddAvatarCollection(UserAvatarCollectionInfo avt)
        {
            lock (this.m_avtcollect)
            {
                if (this.GetAvatarCollectWithAvatarID(avt.AvatarID) == null)
                {
                    this.m_avtcollect.Add(avt);
                }
            }
        }

        public virtual List<UserAvatarCollectionInfo> GetAvatarCollectActived()
        {
            lock (this.m_avtcollect)
            {
                List<UserAvatarCollectionInfo> list3 = new List<UserAvatarCollectionInfo>();
                if (this.m_avtcollect.Count > 0)
                {
                    foreach (UserAvatarCollectionInfo info in this.m_avtcollect)
                    {
                        if (info.IsActive = info.IsExit)
                        {
                            list3.Add(info);
                        }
                    }
                }
                return list3;
            }
        }

        public virtual UserAvatarCollectionInfo GetAvatarCollectWithAvatarID(int avatarId)
        {
            lock (this.m_avtcollect)
            {
                if (this.m_avtcollect.Count > 0)
                {
                    foreach (UserAvatarCollectionInfo info2 in this.m_avtcollect)
                    {
                        if ((info2.AvatarID == avatarId) && info2.IsExit)
                        {
                            return info2;
                        }
                    }
                }
                return null;
            }
        }

        public virtual UserAvatarCollectionInfo GetAvatarCollectWithAvatarID(int avatarId, int Sex)
        {
            lock (this.m_avtcollect)
            {
                if (this.m_avtcollect.Count > 0)
                {
                    foreach (UserAvatarCollectionInfo info2 in this.m_avtcollect)
                    {
                        if (((info2.AvatarID == avatarId) && (info2.Sex == Sex)) && info2.IsExit)
                        {
                            return info2;
                        }
                    }
                }
                return null;
            }
        }

        public virtual List<UserAvatarCollectionInfo> GetAvatarPropertyActived()
        {
            lock (this.m_avtcollect)
            {
                List<UserAvatarCollectionInfo> list3 = new List<UserAvatarCollectionInfo>();
                List<UserAvatarCollectionInfo> avatarCollectActived = this.m_player.AvatarCollect.GetAvatarCollectActived();
                if (avatarCollectActived.Count > 0)
                {
                    foreach (UserAvatarCollectionInfo info in avatarCollectActived)
                    {
                        ClothPropertyTemplateInfo clothPropertyWithID = ClothPropertyTemplateInfoMgr.GetClothPropertyWithID(info.AvatarID);
                        if (clothPropertyWithID != null)
                        {
                            info.ClothProperty = clothPropertyWithID;
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
                    List<UserAvatarCollectionInfo> singleAvatarCollect = bussiness.GetSingleAvatarCollect(this.Player.PlayerCharacter.ID);
                    if (singleAvatarCollect.Count > 0)
                    {
                        foreach (UserAvatarCollectionInfo info in singleAvatarCollect)
                        {
                            this.AddAvatarCollect(info);
                        }
                    }
                }
            }
        }

        public virtual bool RemoveAvatarCollect(UserAvatarCollectionInfo avt)
        {
            lock (this.m_avtcollect)
            {
                foreach (UserAvatarCollectionInfo info in this.m_avtcollect)
                {
                    if (info == avt)
                    {
                        info.IsExit = false;
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
                        for (int i = 0; i < this.m_avtcollect.Count; i++)
                        {
                            UserAvatarCollectionInfo item = this.m_avtcollect[i];
                            if ((item != null) && item.IsDirty)
                            {
                                if (item.ID > 0)
                                {
                                    bussiness.UpdateUserAvatarCollect(item);
                                }
                                else if (item.ID <= 0)
                                {
                                    bussiness.AddUserAvatarCollect(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void ScanAvatarVaildDate()
        {
            lock (this.m_avtcollect)
            {
                if (this.m_avtcollect.Count > 0)
                {
                    int num = 0;
                    foreach (UserAvatarCollectionInfo info in this.m_avtcollect)
                    {
                        if ((info.IsActive && (info.TimeEnd <= DateTime.Now)) && (info.Items != null))
                        {
                            info.IsActive = false;
                            num++;
                        }
                    }
                    if (num > 0)
                    {
                        this.SaveToDatabase();
                        this.Player.SendMessage("Hiện bạn c\x00f3 " + num + " bộ sưu tập đ\x00e3 hết hạn. H\x00e3y gia hạn ngay nh\x00e9!");
                    }
                }
            }
        }

        public List<UserAvatarCollectionInfo> AvatarCollect
        {
            get
            {
                return this.m_avtcollect;
            }
            set
            {
                this.m_avtcollect = value;
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


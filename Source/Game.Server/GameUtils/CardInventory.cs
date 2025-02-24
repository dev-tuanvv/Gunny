namespace Game.Server.GameUtils
{
    using Bussiness;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    public class CardInventory : CardAbstractInventory
    {
        protected GamePlayer m_player;
        private List<UsersCardInfo> m_removedList;
        private bool m_saveToDb;

        public CardInventory(GamePlayer player, bool saveTodb, int capibility, int beginSlot) : base(capibility, beginSlot)
        {
            this.m_removedList = new List<UsersCardInfo>();
            this.m_player = player;
            this.m_saveToDb = saveTodb;
        }

        public override bool AddCardTo(UsersCardInfo item, int place)
        {
            if (base.AddCardTo(item, place))
            {
                item.UserID = this.m_player.PlayerCharacter.ID;
                return true;
            }
            return false;
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    UsersCardInfo[] userCardSingles = bussiness.GetUserCardSingles(this.m_player.PlayerCharacter.ID);
                    base.BeginChanges();
                    try
                    {
                        foreach (UsersCardInfo info in userCardSingles)
                        {
                            this.AddCardTo(info, info.Place);
                        }
                    }
                    finally
                    {
                        base.CommitChanges();
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
                    lock (base.m_lock)
                    {
                        for (int i = 0; i < base.m_cards.Length; i++)
                        {
                            UsersCardInfo info = base.m_cards[i];
                            if ((info != null) && info.IsDirty)
                            {
                                if (info.CardID > 0)
                                {
                                    bussiness.UpdateCards(info);
                                }
                                else
                                {
                                    bussiness.AddCards(info);
                                }
                            }
                        }
                    }
                    lock (this.m_removedList)
                    {
                        foreach (UsersCardInfo info2 in this.m_removedList)
                        {
                            if (info2.CardID > 0)
                            {
                                bussiness.UpdateCards(info2);
                            }
                        }
                        this.m_removedList.Clear();
                    }
                }
            }
        }

        public override void UpdateChangedPlaces()
        {
            int[] updatedSlots = base.m_changedPlaces.ToArray();
            this.m_player.Out.SendPlayerCardInfo(this, updatedSlots);
            base.UpdateChangedPlaces();
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


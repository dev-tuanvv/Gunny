namespace Game.Server.GameUtils
{
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public abstract class CardAbstractInventory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int m_beginSlot;
        private int m_capalility;
        protected UsersCardInfo[] m_cards;
        private int m_changeCount;
        protected List<int> m_changedPlaces = new List<int>();
        protected object m_lock = new object();
        protected UsersCardInfo temp_card;

        public CardAbstractInventory(int capability, int beginSlot)
        {
            this.m_capalility = capability;
            this.m_beginSlot = beginSlot;
            this.m_cards = new UsersCardInfo[capability];
            this.temp_card = new UsersCardInfo();
        }

        public bool AddCard(UsersCardInfo card)
        {
            return this.AddCard(card, this.m_beginSlot);
        }

        public bool AddCard(UsersCardInfo card, int minSlot)
        {
            if (card == null)
            {
                return false;
            }
            int place = this.FindFirstEmptySlot(minSlot);
            return this.AddCardTo(card, place);
        }

        public virtual bool AddCardTo(UsersCardInfo card, int place)
        {
            if (((card == null) || (place >= this.m_capalility)) || (place < 0))
            {
                return false;
            }
            lock (this.m_lock)
            {
                if (this.m_cards[place] != null)
                {
                    place = -1;
                }
                else
                {
                    this.m_cards[place] = card;
                    card.Place = place;
                }
            }
            if (place != -1)
            {
                this.OnPlaceChanged(place);
            }
            return (place != -1);
        }

        public void BeginChanges()
        {
            Interlocked.Increment(ref this.m_changeCount);
        }

        public virtual void Clear()
        {
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    this.m_cards[i] = null;
                }
            }
        }

        public void ClearBag()
        {
            this.BeginChanges();
            lock (this.m_lock)
            {
                for (int i = 5; i < this.m_capalility; i++)
                {
                    if (this.m_cards[i] != null)
                    {
                        this.RemoveCard(this.m_cards[i]);
                    }
                }
            }
            this.CommitChanges();
        }

        public void CommitChanges()
        {
            int num = Interlocked.Decrement(ref this.m_changeCount);
            if (num < 0)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
                }
                Thread.VolatileWrite(ref this.m_changeCount, 0);
            }
            if ((num <= 0) && (this.m_changedPlaces.Count > 0))
            {
                this.UpdateChangedPlaces();
            }
        }

        protected virtual bool ExchangeCards(int fromSlot, int toSlot)
        {
            UsersCardInfo info = this.m_cards[fromSlot];
            if (info == null)
            {
                return false;
            }
            if (fromSlot == toSlot)
            {
                this.m_cards[toSlot].Count = -1;
                this.m_cards[toSlot].Attack = 0;
                this.m_cards[toSlot].Defence = 0;
                this.m_cards[toSlot].Agility = 0;
                this.m_cards[toSlot].Luck = 0;
                this.m_cards[toSlot].Damage = 0;
                this.m_cards[toSlot].Guard = 0;
            }
            else
            {
                if (this.m_cards[toSlot] == null)
                {
                    this.m_cards[toSlot] = new UsersCardInfo();
                    this.m_cards[toSlot].UserID = info.UserID;
                    this.m_cards[toSlot].Place = toSlot;
                }
                this.m_cards[toSlot].Count = 1;
                this.m_cards[toSlot].TemplateID = info.TemplateID;
                this.m_cards[toSlot].Attack = info.Attack;
                this.m_cards[toSlot].Defence = info.Defence;
                this.m_cards[toSlot].Agility = info.Agility;
                this.m_cards[toSlot].Luck = info.Luck;
                this.m_cards[toSlot].Damage = info.Damage;
                this.m_cards[toSlot].Guard = info.Guard;
                this.m_cards[toSlot].Level = info.Level;
            }
            return true;
        }

        public bool FindEquipCard(int templateId)
        {
            lock (this.m_lock)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (((this.m_cards[i] != null) && (this.m_cards[i].TemplateID == templateId)) && (this.m_cards[i].Count > 0))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public int FindFirstEmptySlot()
        {
            return this.FindFirstEmptySlot(this.m_beginSlot);
        }

        public int FindFirstEmptySlot(int minSlot)
        {
            if (minSlot >= this.m_capalility)
            {
                return -1;
            }
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if (this.m_cards[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public int FindFirstEmptySlot(int minSlot, int maxslot)
        {
            if (minSlot >= this.m_capalility)
            {
                return -1;
            }
            lock (this.m_lock)
            {
                for (int i = minSlot; i <= maxslot; i++)
                {
                    if (this.m_cards[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public int FindLastEmptySlot()
        {
            lock (this.m_lock)
            {
                for (int i = this.m_capalility - 1; i >= 0; i--)
                {
                    if (this.m_cards[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public int FindPlaceByTamplateId(int minSlot, int templateId)
        {
            if (minSlot >= this.m_capalility)
            {
                return -1;
            }
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if ((this.m_cards[i] != null) && (this.m_cards[i].TemplateID == templateId))
                    {
                        return this.m_cards[i].Place;
                    }
                }
                return -1;
            }
        }

        public int FindPlaceByTamplateId(int minSlot, int maxSlot, int templateId)
        {
            if (minSlot >= this.m_capalility)
            {
                return -1;
            }
            lock (this.m_lock)
            {
                for (int i = minSlot; i < maxSlot; i++)
                {
                    if (this.m_cards[i].TemplateID == templateId)
                    {
                        return this.m_cards[i].Place;
                    }
                }
                return -1;
            }
        }

        public virtual List<UsersCardInfo> GetCards()
        {
            return this.GetCards(0, this.m_capalility);
        }

        public virtual List<UsersCardInfo> GetCards(int minSlot, int maxSlot)
        {
            List<UsersCardInfo> list = new List<UsersCardInfo>();
            lock (this.m_lock)
            {
                for (int i = minSlot; i < maxSlot; i++)
                {
                    if (this.m_cards[i] != null)
                    {
                        list.Add(this.m_cards[i]);
                    }
                }
            }
            return list;
        }

        public int GetEmptyCount()
        {
            return this.GetEmptyCount(this.m_beginSlot);
        }

        public virtual int GetEmptyCount(int minSlot)
        {
            if ((minSlot < 0) || (minSlot > (this.m_capalility - 1)))
            {
                return 0;
            }
            int num = 0;
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if (this.m_cards[i] == null)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public UsersCardInfo GetEquipCard(int templateId)
        {
            lock (this.m_lock)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (((this.m_cards[i] != null) && (this.m_cards[i].TemplateID == templateId)) && (this.m_cards[i].Count > 0))
                    {
                        return this.m_cards[i];
                    }
                }
            }
            return null;
        }

        public virtual UsersCardInfo GetItemAt(int slot)
        {
            if ((slot < 0) || (slot >= this.m_capalility))
            {
                return null;
            }
            return this.m_cards[slot];
        }

        public virtual UsersCardInfo GetItemByPlace(int minSlot, int place)
        {
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if ((this.m_cards[i] != null) && (this.m_cards[i].Place == place))
                    {
                        return this.m_cards[i];
                    }
                }
                return null;
            }
        }

        public virtual UsersCardInfo GetItemByTemplateID(int minSlot, int templateId)
        {
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if ((this.m_cards[i] != null) && (this.m_cards[i].TemplateID == templateId))
                    {
                        return this.m_cards[i];
                    }
                }
                return null;
            }
        }

        public UsersCardInfo[] GetRawSpaces()
        {
            lock (this.m_lock)
            {
                return (this.m_cards.Clone() as UsersCardInfo[]);
            }
        }

        public bool IsEmpty(int slot)
        {
            return (((slot < 0) || (slot >= this.m_capalility)) || (this.m_cards[slot] == null));
        }

        public bool IsSolt(int slot)
        {
            return ((slot >= 0) && (slot < this.m_capalility));
        }

        public virtual bool MoveCard(int fromSlot, int toSlot)
        {
            if ((((fromSlot < 0) || (toSlot < 0)) || (fromSlot >= this.m_capalility)) || (toSlot >= this.m_capalility))
            {
                return false;
            }
            bool flag = false;
            lock (this.m_lock)
            {
                flag = this.ExchangeCards(fromSlot, toSlot);
            }
            if (flag)
            {
                this.BeginChanges();
                try
                {
                    this.OnPlaceChanged(toSlot);
                }
                finally
                {
                    this.CommitChanges();
                }
            }
            return flag;
        }

        protected void OnPlaceChanged(int place)
        {
            if (!this.m_changedPlaces.Contains(place))
            {
                this.m_changedPlaces.Add(place);
            }
            if ((this.m_changeCount <= 0) && (this.m_changedPlaces.Count > 0))
            {
                this.UpdateChangedPlaces();
            }
        }

        public virtual bool RemoveCard(UsersCardInfo item)
        {
            if (item == null)
            {
                return false;
            }
            int place = -1;
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if (this.m_cards[i] == item)
                    {
                        place = i;
                        this.m_cards[i] = null;
                        goto Label_006F;
                    }
                }
            }
        Label_006F:
            if (place != -1)
            {
                this.OnPlaceChanged(place);
                item.Place = -1;
            }
            return (place != -1);
        }

        public bool RemoveCardAt(int place)
        {
            return this.RemoveCard(this.GetItemAt(place));
        }

        public virtual bool ReplaceCardTo(UsersCardInfo card, int place)
        {
            if (((card == null) || (place >= this.m_capalility)) || (place < 0))
            {
                return false;
            }
            lock (this.m_lock)
            {
                this.m_cards[place] = card;
                card.Place = place;
                this.OnPlaceChanged(place);
            }
            return true;
        }

        public virtual bool ResetCardSoul()
        {
            lock (this.m_lock)
            {
                for (int i = 0; i < 5; i++)
                {
                    this.m_cards[i].Level = 0;
                    this.m_cards[i].CardGP = 0;
                }
            }
            return true;
        }

        public virtual void UpdateCard()
        {
            int num3;
            int place = this.temp_card.Place;
            int templateID = this.temp_card.TemplateID;
            if (place < 5)
            {
                this.ReplaceCardTo(this.temp_card, place);
                num3 = this.FindPlaceByTamplateId(5, templateID);
                this.MoveCard(place, num3);
            }
            else
            {
                this.ReplaceCardTo(this.temp_card, place);
                num3 = this.FindPlaceByTamplateId(0, 5, templateID);
                if ((this.GetItemAt(num3) != null) && (this.GetItemAt(num3).TemplateID == templateID))
                {
                    this.MoveCard(place, num3);
                }
            }
        }

        public virtual void UpdateChangedPlaces()
        {
            this.m_changedPlaces.Clear();
        }

        public virtual void UpdateTempCard(UsersCardInfo card)
        {
            lock (this.m_lock)
            {
                this.temp_card = card;
            }
        }

        public virtual bool UpGraceSlot(int soulPoint, int lv, int place)
        {
            lock (this.m_lock)
            {
                UsersCardInfo info1 = this.m_cards[place];
                info1.CardGP += soulPoint;
                this.m_cards[place].Level = lv;
            }
            return true;
        }

        public int BeginSlot
        {
            get
            {
                return this.m_beginSlot;
            }
        }

        public int Capalility
        {
            get
            {
                return this.m_capalility;
            }
            set
            {
                this.m_capalility = (value < 0) ? 0 : ((value > this.m_cards.Length) ? this.m_cards.Length : value);
            }
        }
    }
}


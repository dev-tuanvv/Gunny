namespace Game.Server.GameUtils
{
    using Game.Logic;
    using Game.Server.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public abstract class AbstractInventory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool m_autoStack;
        private int m_beginSlot;
        private int m_capalility;
        private int m_changeCount;
        protected List<int> m_changedPlaces = new List<int>();
        protected SqlDataProvider.Data.ItemInfo[] m_items;
        protected object m_lock = new object();
        private int m_type;

        public AbstractInventory(int capability, int type, int beginSlot, bool autoStack)
        {
            this.m_capalility = capability;
            this.m_type = type;
            this.m_beginSlot = beginSlot;
            this.m_autoStack = autoStack;
            this.m_items = new SqlDataProvider.Data.ItemInfo[capability];
        }

        public virtual bool AddCountToStack(SqlDataProvider.Data.ItemInfo item, int count)
        {
            if (item == null)
            {
                return false;
            }
            if ((count <= 0) || (item.BagType != this.m_type))
            {
                return false;
            }
            if ((item.Count + count) > item.Template.MaxCount)
            {
                return false;
            }
            item.Count += count;
            this.OnPlaceChanged(item.Place);
            return true;
        }

        public virtual bool AddItem(SqlDataProvider.Data.ItemInfo item)
        {
            return this.AddItem(item, this.m_beginSlot);
        }

        public virtual bool AddItem(SqlDataProvider.Data.ItemInfo item, int minSlot)
        {
            if (item == null)
            {
                return false;
            }
            int place = this.FindFirstEmptySlot(minSlot);
            return this.AddItemTo(item, place);
        }

        public virtual bool AddItem(SqlDataProvider.Data.ItemInfo item, int minSlot, int maxSlot)
        {
            if (item == null)
            {
                return false;
            }
            int place = this.FindFirstEmptySlot(minSlot, maxSlot);
            return this.AddItemTo(item, place);
        }

        public virtual bool AddItemTo(SqlDataProvider.Data.ItemInfo item, int place)
        {
            if (((item == null) || (place >= this.m_capalility)) || (place < 0))
            {
                return false;
            }
            RuneTemplateInfo info = RuneMgr.FindRuneByTemplateID(item.TemplateID);
            if (info != null)
            {
                int baseLevel = info.BaseLevel;
                int num2 = RuneMgr.FindRuneExp(baseLevel - 1);
                if ((item.IsBead() && (item.Hole1 <= 0)) && (item.Hole2 <= 0))
                {
                    item.Hole1 = baseLevel;
                    item.Hole2 = num2;
                }
            }
            MagicStoneTemplateMgr.SetupMagicStoneWithLevel(item);
            lock (this.m_lock)
            {
                if (this.m_items[place] != null)
                {
                    place = -1;
                }
                else
                {
                    this.m_items[place] = item;
                    item.Place = place;
                    item.BagType = this.m_type;
                }
            }
            if (place != -1)
            {
                this.OnPlaceChanged(place);
            }
            return (place != -1);
        }

        public virtual bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem, int count)
        {
            return this.AddTemplate(cloneItem, count, this.m_beginSlot, this.m_capalility - 1);
        }

        public virtual bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem, int count, int minSlot, int maxSlot)
        {
            bool flag;
            if (cloneItem == null)
            {
                return false;
            }
            ItemTemplateInfo template = cloneItem.Template;
            if (template == null)
            {
                return false;
            }
            if (count <= 0)
            {
                return false;
            }
            if ((minSlot < this.m_beginSlot) || (minSlot > (this.m_capalility - 1)))
            {
                return false;
            }
            if ((maxSlot < this.m_beginSlot) || (maxSlot > (this.m_capalility - 1)))
            {
                return false;
            }
            if (minSlot > maxSlot)
            {
                return false;
            }
            lock (this.m_lock)
            {
                List<int> list = new List<int>();
                int num = count;
                for (int i = minSlot; i <= maxSlot; i++)
                {
                    SqlDataProvider.Data.ItemInfo to = this.m_items[i];
                    if (to == null)
                    {
                        num -= template.MaxCount;
                        list.Add(i);
                    }
                    else if (this.m_autoStack && cloneItem.CanStackedTo(to))
                    {
                        num -= template.MaxCount - to.Count;
                        list.Add(i);
                    }
                    if (num <= 0)
                    {
                        break;
                    }
                }
                if (num <= 0)
                {
                    this.BeginChanges();
                    try
                    {
                        num = count;
                        foreach (int num3 in list)
                        {
                            SqlDataProvider.Data.ItemInfo item = this.m_items[num3];
                            if (item == null)
                            {
                                item = cloneItem.Clone();
                                item.Count = (num < template.MaxCount) ? num : template.MaxCount;
                                num -= item.Count;
                                this.AddItemTo(item, num3);
                            }
                            else if (item.TemplateID == template.TemplateID)
                            {
                                int num4 = ((item.Count + num) < template.MaxCount) ? num : (template.MaxCount - item.Count);
                                item.Count += num4;
                                num -= num4;
                                this.OnPlaceChanged(num3);
                            }
                            else
                            {
                                log.Error("Add template erro: select slot's TemplateId not equest templateId");
                            }
                        }
                        if (num != 0)
                        {
                            log.Error("Add template error: last count not equal Zero.");
                        }
                    }
                    finally
                    {
                        this.CommitChanges();
                    }
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        public virtual bool AddTemplateAt(SqlDataProvider.Data.ItemInfo cloneItem, int count, int place)
        {
            return this.AddTemplate(cloneItem, count, place, this.m_capalility - 1);
        }

        public void BeginChanges()
        {
            Interlocked.Increment(ref this.m_changeCount);
        }

        public virtual void Clear()
        {
            this.BeginChanges();
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    this.m_items[i] = null;
                    this.OnPlaceChanged(i);
                }
            }
            this.CommitChanges();
        }

        public void ClearBag()
        {
            this.BeginChanges();
            lock (this.m_lock)
            {
                for (int i = this.m_beginSlot; i < this.m_capalility; i++)
                {
                    if (this.m_items[i] != null)
                    {
                        this.RemoveItem(this.m_items[i]);
                    }
                }
            }
            this.CommitChanges();
        }

        protected virtual bool CombineItems(int fromSlot, int toSlot)
        {
            return false;
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

        protected virtual bool ExchangeItems(int fromSlot, int toSlot)
        {
            SqlDataProvider.Data.ItemInfo info = this.m_items[toSlot];
            SqlDataProvider.Data.ItemInfo info2 = this.m_items[fromSlot];
            this.m_items[fromSlot] = info;
            this.m_items[toSlot] = info2;
            if (info != null)
            {
                info.Place = fromSlot;
            }
            if (info2 != null)
            {
                info2.Place = toSlot;
            }
            return true;
        }

        public int FindFirstEmptySlot()
        {
            return this.FindFirstEmptySlot(this.m_beginSlot);
        }

        public virtual int FindFirstEmptySlot(int minSlot)
        {
            if (minSlot >= this.m_capalility)
            {
                return -1;
            }
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if (this.m_items[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public int FindFirstEmptySlot(int minSlot, int maxSlot)
        {
            if (minSlot >= maxSlot)
            {
                return -1;
            }
            lock (this.m_lock)
            {
                for (int i = minSlot; i < maxSlot; i++)
                {
                    if (this.m_items[i] == null)
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
                    if (this.m_items[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public int FindLastEmptySlot(int maxSlot)
        {
            lock (this.m_lock)
            {
                for (int i = maxSlot - 1; i >= 0; i--)
                {
                    if (this.m_items[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
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
                    if (this.m_items[i] == null)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public virtual SqlDataProvider.Data.ItemInfo GetItemAt(int slot)
        {
            if ((slot < 0) || (slot >= this.m_capalility))
            {
                return null;
            }
            return this.m_items[slot];
        }

        public virtual SqlDataProvider.Data.ItemInfo GetItemByCategoryID(int minSlot, int categoryID, int property)
        {
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if (((this.m_items[i] != null) && (this.m_items[i].Template.CategoryID == categoryID)) && ((property == -1) || (this.m_items[i].Template.Property1 == property)))
                    {
                        return this.m_items[i];
                    }
                }
                return null;
            }
        }

        public virtual SqlDataProvider.Data.ItemInfo GetItemByItemID(int minSlot, int itemId)
        {
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if ((this.m_items[i] != null) && (this.m_items[i].ItemID == itemId))
                    {
                        return this.m_items[i];
                    }
                }
                return null;
            }
        }

        public virtual SqlDataProvider.Data.ItemInfo GetItemByTemplateID(int minSlot, int templateId)
        {
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if ((this.m_items[i] != null) && (this.m_items[i].TemplateID == templateId))
                    {
                        return this.m_items[i];
                    }
                }
                return null;
            }
        }

        public virtual int GetItemCount(int templateId)
        {
            return this.GetItemCount(this.m_beginSlot, templateId);
        }

        public int GetItemCount(int minSlot, int templateId)
        {
            int num = 0;
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if ((this.m_items[i] != null) && (this.m_items[i].TemplateID == templateId))
                    {
                        num += this.m_items[i].Count;
                    }
                }
            }
            return num;
        }

        public virtual List<SqlDataProvider.Data.ItemInfo> GetItems()
        {
            return this.GetItems(0, this.m_capalility);
        }

        public virtual List<SqlDataProvider.Data.ItemInfo> GetItems(int minSlot, int maxSlot)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            lock (this.m_lock)
            {
                for (int i = minSlot; i < maxSlot; i++)
                {
                    if (this.m_items[i] != null)
                    {
                        list.Add(this.m_items[i]);
                    }
                }
            }
            return list;
        }

        public virtual List<SqlDataProvider.Data.ItemInfo> GetItemsByTemplateID(int minSlot, int templateid)
        {
            lock (this.m_lock)
            {
                List<SqlDataProvider.Data.ItemInfo> list2 = new List<SqlDataProvider.Data.ItemInfo>();
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if ((this.m_items[i] != null) && (this.m_items[i].TemplateID == templateid))
                    {
                        list2.Add(this.m_items[i]);
                    }
                }
                return list2;
            }
        }

        public SqlDataProvider.Data.ItemInfo[] GetRawSpaces()
        {
            lock (this.m_lock)
            {
                return (this.m_items.Clone() as SqlDataProvider.Data.ItemInfo[]);
            }
        }

        public bool IsEmpty(int slot)
        {
            return (((slot < 0) || (slot >= this.m_capalility)) || (this.m_items[slot] == null));
        }

        public bool IsSolt(int slot)
        {
            return ((slot >= 0) && (slot < this.m_capalility));
        }

        public virtual bool MoveItem(int fromSlot, int toSlot, int count)
        {
            if ((((fromSlot < 0) || (toSlot < 0)) || (fromSlot >= this.m_capalility)) || (toSlot >= this.m_capalility))
            {
                return false;
            }
            bool flag = false;
            lock (this.m_lock)
            {
                flag = (this.CombineItems(fromSlot, toSlot) || this.StackItems(fromSlot, toSlot, count)) || this.ExchangeItems(fromSlot, toSlot);
            }
            if (flag)
            {
                this.BeginChanges();
                try
                {
                    this.OnPlaceChanged(fromSlot);
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

        public void RemoveAllItem()
        {
            this.BeginChanges();
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if (this.m_items[i] != null)
                    {
                        this.RemoveItem(this.m_items[i]);
                    }
                }
            }
            this.CommitChanges();
        }

        public void RemoveAllItem(List<int> places)
        {
            this.BeginChanges();
            lock (this.m_lock)
            {
                for (int i = 0; i < places.Count; i++)
                {
                    int index = places[i];
                    if (this.m_items[index] != null)
                    {
                        this.RemoveItem(this.m_items[index]);
                    }
                }
            }
            this.CommitChanges();
        }

        public virtual bool RemoveCountFromStack(SqlDataProvider.Data.ItemInfo item, int count)
        {
            if (item == null)
            {
                return false;
            }
            if ((count <= 0) || (item.BagType != this.m_type))
            {
                return false;
            }
            if (item.Count < count)
            {
                return false;
            }
            if (item.Count == count)
            {
                return this.RemoveItem(item);
            }
            item.Count -= count;
            this.OnPlaceChanged(item.Place);
            return true;
        }

        public virtual bool RemoveCountFromStack(SqlDataProvider.Data.ItemInfo item, int count, eItemRemoveType type)
        {
            if (item == null)
            {
                return false;
            }
            if ((count <= 0) || (item.BagType != this.m_type))
            {
                return false;
            }
            if (item.Count < count)
            {
                return false;
            }
            if (item.Count == count)
            {
                return this.RemoveItem(item);
            }
            item.Count -= count;
            this.OnPlaceChanged(item.Place);
            return true;
        }

        public virtual bool RemoveItem(SqlDataProvider.Data.ItemInfo item)
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
                    if (this.m_items[i] == item)
                    {
                        place = i;
                        this.m_items[i] = null;
                        goto Label_006F;
                    }
                }
            }
        Label_006F:
            if (place != -1)
            {
                this.OnPlaceChanged(place);
                if (item.BagType == this.BagType)
                {
                    item.Place = -1;
                    item.BagType = -1;
                }
            }
            return (place != -1);
        }

        public virtual bool RemoveItem(SqlDataProvider.Data.ItemInfo item, eItemRemoveType type)
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
                    if (this.m_items[i] == item)
                    {
                        place = i;
                        this.m_items[i] = null;
                        goto Label_006F;
                    }
                }
            }
        Label_006F:
            if (place != -1)
            {
                this.OnPlaceChanged(place);
                if ((item.BagType == this.BagType) && (item.Place == place))
                {
                    item.Place = -1;
                    item.BagType = -1;
                }
            }
            return (place != -1);
        }

        public bool RemoveItemAt(int place)
        {
            return this.RemoveItem(this.GetItemAt(place));
        }

        public virtual bool RemoveTemplate(int templateId, int count)
        {
            return this.RemoveTemplate(templateId, count, 0, this.m_capalility - 1);
        }

        public virtual bool RemoveTemplate(int templateId, int count, int minSlot, int maxSlot)
        {
            bool flag;
            if (count <= 0)
            {
                return false;
            }
            if ((minSlot < 0) || (minSlot > (this.m_capalility - 1)))
            {
                return false;
            }
            if ((maxSlot <= 0) || (maxSlot > (this.m_capalility - 1)))
            {
                return false;
            }
            if (minSlot > maxSlot)
            {
                return false;
            }
            lock (this.m_lock)
            {
                List<int> list = new List<int>();
                int num = count;
                for (int i = minSlot; i <= maxSlot; i++)
                {
                    SqlDataProvider.Data.ItemInfo info = this.m_items[i];
                    if ((info != null) && (info.TemplateID == templateId))
                    {
                        list.Add(i);
                        num -= info.Count;
                        if (num <= 0)
                        {
                            break;
                        }
                    }
                }
                if (num <= 0)
                {
                    this.BeginChanges();
                    num = count;
                    try
                    {
                        foreach (int num3 in list)
                        {
                            SqlDataProvider.Data.ItemInfo item = this.m_items[num3];
                            if ((item != null) && (item.TemplateID == templateId))
                            {
                                if (item.Count <= num)
                                {
                                    this.RemoveItem(item);
                                    num -= item.Count;
                                }
                                else
                                {
                                    int num4 = ((item.Count - num) < item.Count) ? num : 0;
                                    item.Count -= num4;
                                    num -= num4;
                                    this.OnPlaceChanged(num3);
                                }
                            }
                        }
                        if (num != 0)
                        {
                            log.Error("Remove templat error:last itemcoutj not equal Zero.");
                        }
                    }
                    finally
                    {
                        this.CommitChanges();
                    }
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        protected virtual bool StackItems(int fromSlot, int toSlot, int itemCount)
        {
            SqlDataProvider.Data.ItemInfo to = this.m_items[fromSlot];
            SqlDataProvider.Data.ItemInfo info2 = this.m_items[toSlot];
            if (itemCount == 0)
            {
                if (to.Count > 0)
                {
                    itemCount = to.Count;
                }
                else
                {
                    itemCount = 1;
                }
            }
            if (((info2 != null) && (info2.TemplateID == to.TemplateID)) && info2.CanStackedTo(to))
            {
                if ((to.Count + info2.Count) > to.Template.MaxCount)
                {
                    to.Count -= info2.Template.MaxCount - info2.Count;
                    info2.Count = info2.Template.MaxCount;
                }
                else
                {
                    info2.Count += itemCount;
                    this.RemoveItem(to);
                }
                return true;
            }
            if ((info2 == null) && (to.Count > itemCount))
            {
                SqlDataProvider.Data.ItemInfo item = to.Clone();
                item.Count = itemCount;
                if (this.AddItemTo(item, toSlot))
                {
                    to.Count -= itemCount;
                    return true;
                }
            }
            return false;
        }

        public bool StackItemToAnother(SqlDataProvider.Data.ItemInfo item)
        {
            lock (this.m_lock)
            {
                for (int i = this.m_capalility - 1; i >= 0; i--)
                {
                    if ((((item != null) && (this.m_items[i] != null)) && ((this.m_items[i] != item) && item.CanStackedTo(this.m_items[i]))) && ((this.m_items[i].Count + item.Count) <= item.Template.MaxCount))
                    {
                        SqlDataProvider.Data.ItemInfo info = this.m_items[i];
                        info.Count += item.Count;
                        item.IsExist = false;
                        item.RemoveType = 0x1a;
                        this.UpdateItem(this.m_items[i]);
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool TakeOutItem(SqlDataProvider.Data.ItemInfo item)
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
                    if (this.m_items[i] == item)
                    {
                        place = i;
                        this.m_items[i] = null;
                        goto Label_006F;
                    }
                }
            }
        Label_006F:
            if (place != -1)
            {
                this.OnPlaceChanged(place);
                if (item.BagType == this.BagType)
                {
                    item.Place = -1;
                    item.BagType = -1;
                }
            }
            return (place != -1);
        }

        public bool TakeOutItemAt(int place)
        {
            return this.TakeOutItem(this.GetItemAt(place));
        }

        public virtual void UpdateChangedPlaces()
        {
            this.m_changedPlaces.Clear();
        }

        public virtual void UpdateItem(SqlDataProvider.Data.ItemInfo item)
        {
            if (item.BagType == this.m_type)
            {
                if (item.Count <= 0)
                {
                    this.RemoveItem(item);
                }
                else
                {
                    this.OnPlaceChanged(item.Place);
                }
            }
        }

        public virtual void UseItem(SqlDataProvider.Data.ItemInfo item)
        {
            bool flag = false;
            if (!(item.IsBinds || ((item.Template.BindType != 2) && (item.Template.BindType != 3))))
            {
                item.IsBinds = true;
                flag = true;
            }
            if (!item.IsUsed)
            {
                item.IsUsed = true;
                item.BeginDate = DateTime.Now;
                flag = true;
            }
            if (flag)
            {
                this.OnPlaceChanged(item.Place);
            }
        }

        public virtual bool YbxRemoveTemplate(int templateId, int count, ref bool IsBind)
        {
            return this.YbxRemoveTemplate(templateId, count, 0, this.m_capalility - 1, ref IsBind);
        }

        public virtual bool YbxRemoveTemplate(int templateId, int count, int minSlot, int maxSlot, ref bool IsBind)
        {
            bool flag;
            if (count <= 0)
            {
                return false;
            }
            if ((minSlot < 0) || (minSlot > (this.m_capalility - 1)))
            {
                return false;
            }
            if ((maxSlot <= 0) || (maxSlot > (this.m_capalility - 1)))
            {
                return false;
            }
            if (minSlot > maxSlot)
            {
                return false;
            }
            lock (this.m_lock)
            {
                List<int> list = new List<int>();
                int num = count;
                for (int i = minSlot; i <= maxSlot; i++)
                {
                    SqlDataProvider.Data.ItemInfo info = this.m_items[i];
                    if ((info != null) && (info.TemplateID == templateId))
                    {
                        list.Add(i);
                        num -= info.Count;
                        if (num <= 0)
                        {
                            break;
                        }
                    }
                }
                if (num <= 0)
                {
                    this.BeginChanges();
                    num = count;
                    try
                    {
                        foreach (int num3 in list)
                        {
                            SqlDataProvider.Data.ItemInfo item = this.m_items[num3];
                            if ((item != null) && (item.TemplateID == templateId))
                            {
                                if (!IsBind && item.IsBinds)
                                {
                                    IsBind = true;
                                }
                                if (item.Count <= num)
                                {
                                    this.RemoveItem(item);
                                    num -= item.Count;
                                }
                                else
                                {
                                    int num4 = ((item.Count - num) < item.Count) ? num : 0;
                                    item.Count -= num4;
                                    num -= num4;
                                    this.OnPlaceChanged(num3);
                                }
                            }
                        }
                        if (num != 0)
                        {
                            log.Error("Remove templat error:last itemcoutj not equal Zero.");
                        }
                    }
                    finally
                    {
                        this.CommitChanges();
                    }
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        public int BagType
        {
            get
            {
                return this.m_type;
            }
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
                this.m_capalility = (value < 0) ? 0 : ((value > this.m_items.Length) ? this.m_items.Length : value);
            }
        }
    }
}


namespace Game.Server.GameUtils
{
    using Bussiness.Managers;
    using Game.Logic;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class PlayerFarmInventory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int m_beginSlot;
        private int m_capalility;
        protected UserFarmInfo m_farm;
        protected int m_farmstatus;
        protected UserFieldInfo[] m_fields;
        protected object m_lock = new object();
        protected UserFarmInfo m_otherFarm;
        protected UserFieldInfo[] m_otherFields;

        public PlayerFarmInventory(int capability, int beginSlot)
        {
            this.m_capalility = capability;
            this.m_beginSlot = beginSlot;
            this.m_fields = new UserFieldInfo[capability];
            this.m_farm = new UserFarmInfo();
            this.m_otherFields = new UserFieldInfo[capability];
            this.m_otherFarm = new UserFarmInfo();
            this.m_farmstatus = 0;
        }

        public virtual bool AccelerateOtherTimeFields()
        {
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if ((this.m_otherFields[i] != null) && (this.m_otherFields[i].SeedID > 0))
                    {
                        DateTime plantTime = this.m_otherFields[i].PlantTime;
                        int fieldValidDate = this.m_otherFields[i].FieldValidDate;
                        this.m_otherFields[i].AccelerateTime = this.AccelerateTimeFields(plantTime, fieldValidDate);
                    }
                }
            }
            return true;
        }

        public virtual bool AccelerateTimeFields()
        {
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if ((this.m_fields[i] != null) && (this.m_fields[i].SeedID > 0))
                    {
                        DateTime plantTime = this.m_fields[i].PlantTime;
                        int fieldValidDate = this.m_fields[i].FieldValidDate;
                        this.m_fields[i].AccelerateTime = this.AccelerateTimeFields(plantTime, fieldValidDate);
                    }
                }
            }
            return true;
        }

        public virtual int AccelerateTimeFields(DateTime PlantTime, int FieldValidDate)
        {
            DateTime now = DateTime.Now;
            int num = now.Hour - PlantTime.Hour;
            int num2 = now.Minute - PlantTime.Minute;
            if (num < 0)
            {
                num = 0x18 + num;
            }
            if (num2 < 0)
            {
                num2 = 60 + num2;
            }
            int num3 = (num * 60) + num2;
            if (num3 > FieldValidDate)
            {
                num3 = FieldValidDate;
            }
            return num3;
        }

        public bool AddField(UserFieldInfo item)
        {
            return this.AddField(item, this.m_beginSlot);
        }

        public bool AddField(UserFieldInfo item, int minSlot)
        {
            if (item == null)
            {
                return false;
            }
            int place = this.FindFirstEmptySlot(minSlot);
            return this.AddFieldTo(item, place);
        }

        public virtual bool AddFieldTo(UserFieldInfo item, int place)
        {
            if (((item == null) || (place >= this.m_capalility)) || (place < 0))
            {
                return false;
            }
            lock (this.m_lock)
            {
                this.m_fields[place] = item;
                if (this.m_fields[place] != null)
                {
                    place = -1;
                }
                else
                {
                    this.m_fields[place] = item;
                    item.FieldID = place;
                }
            }
            return (place != -1);
        }

        public virtual bool AddOtherFieldTo(UserFieldInfo item, int place)
        {
            if (((item == null) || (place >= this.m_capalility)) || (place < 0))
            {
                return false;
            }
            lock (this.m_lock)
            {
                this.m_otherFields[place] = item;
                if (this.m_otherFields[place] != null)
                {
                    place = -1;
                }
                else
                {
                    this.m_otherFields[place] = item;
                    item.FieldID = place;
                }
            }
            return (place != -1);
        }

        public virtual void ClearFarm()
        {
            lock (this.m_lock)
            {
                this.m_farm = null;
            }
        }

        public void ClearFields()
        {
            lock (this.m_lock)
            {
                for (int i = this.m_beginSlot; i < this.m_capalility; i++)
                {
                    if (this.m_fields[i] != null)
                    {
                        this.RemoveField(this.m_fields[i]);
                    }
                }
            }
        }

        public virtual void ClearIsArrange()
        {
            lock (this.m_lock)
            {
                this.m_farm.isArrange = true;
            }
        }

        public void ClearOtherFields()
        {
            lock (this.m_lock)
            {
                for (int i = this.m_beginSlot; i < this.m_capalility; i++)
                {
                    if (this.m_otherFields[i] != null)
                    {
                        this.RemoveOtherField(this.m_otherFields[i]);
                    }
                }
            }
        }

        public virtual void CreateFarm(int ID, string name)
        {
            string str = PetMgr.FindConfig("PayFieldMoney").Value;
            string str2 = PetMgr.FindConfig("PayAutoMoney").Value;
            UserFarmInfo farm = new UserFarmInfo {
                ID = 0,
                FarmID = ID,
                FarmerName = name,
                isFarmHelper = false,
                isAutoId = 0,
                AutoPayTime = DateTime.Now,
                AutoValidDate = 1,
                GainFieldId = 0,
                KillCropId = 0,
                PayAutoMoney = str2,
                PayFieldMoney = str,
                buyExpRemainNum = 20,
                isArrange = false
            };
            this.UpdateFarm(farm);
            this.CreateNewField(ID, 0, 8);
        }

        public virtual UserFarmInfo CreateFarmForNulll(int ID)
        {
            return new UserFarmInfo { FarmID = ID, FarmerName = "Null", isFarmHelper = false, isAutoId = 0, AutoPayTime = DateTime.Now, AutoValidDate = 1, GainFieldId = 0, KillCropId = 0, isArrange = true };
        }

        public virtual bool CreateField(int ID, List<int> fieldIds, int payFieldTime)
        {
            for (int i = 0; i < fieldIds.Count; i++)
            {
                int index = fieldIds[i];
                if (this.m_fields[index] == null)
                {
                    UserFieldInfo item = new UserFieldInfo {
                        FarmID = ID,
                        FieldID = index,
                        SeedID = 0,
                        PayTime = DateTime.Now.AddDays((double) (payFieldTime / 0x18)),
                        payFieldTime = payFieldTime,
                        PlantTime = DateTime.Now,
                        GainCount = 0,
                        FieldValidDate = 1,
                        AccelerateTime = 0,
                        AutomaticTime = DateTime.Now,
                        IsExit = true
                    };
                    this.AddFieldTo(item, index);
                }
                else
                {
                    this.m_fields[index].PayTime = DateTime.Now.AddDays((double) (payFieldTime / 0x18));
                    this.m_fields[index].payFieldTime = payFieldTime;
                }
            }
            return true;
        }

        public virtual UserFieldInfo[] CreateFieldsForNull(int ID)
        {
            List<UserFieldInfo> list = new List<UserFieldInfo>();
            for (int i = 0; i < 8; i++)
            {
                UserFieldInfo item = new UserFieldInfo {
                    FarmID = ID,
                    FieldID = i,
                    SeedID = 0,
                    PayTime = DateTime.Now,
                    payFieldTime = 0x591c8,
                    PlantTime = DateTime.Now,
                    GainCount = 0,
                    FieldValidDate = 1,
                    AccelerateTime = 0,
                    AutomaticTime = DateTime.Now
                };
                list.Add(item);
            }
            return list.ToArray();
        }

        public virtual void CreateNewField(int ID, int minslot, int maxslot)
        {
            for (int i = minslot; i < maxslot; i++)
            {
                UserFieldInfo item = new UserFieldInfo {
                    ID = 0,
                    FarmID = ID,
                    FieldID = i,
                    SeedID = 0,
                    PayTime = DateTime.Now.AddYears(100),
                    payFieldTime = 0xd5de0,
                    PlantTime = DateTime.Now,
                    GainCount = 0,
                    FieldValidDate = 1,
                    AccelerateTime = 0,
                    AutomaticTime = DateTime.Now,
                    IsExit = true
                };
                this.AddFieldTo(item, i);
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
                    if (this.m_fields[i] == null)
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
                    if (this.m_fields[i] == null)
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
                    if (this.m_fields[i] == null)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public virtual UserFieldInfo GetFieldAt(int slot)
        {
            if ((slot < 0) || (slot >= this.m_capalility))
            {
                return null;
            }
            return this.m_fields[slot];
        }

        public virtual UserFieldInfo[] GetFields()
        {
            List<UserFieldInfo> list = new List<UserFieldInfo>();
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if ((this.m_fields[i] != null) && this.m_fields[i].IsValidField())
                    {
                        list.Add(this.m_fields[i]);
                    }
                }
            }
            return list.ToArray();
        }

        public virtual UserFieldInfo GetOtherFieldAt(int slot)
        {
            if ((slot < 0) || (slot >= this.m_capalility))
            {
                return null;
            }
            return this.m_otherFields[slot];
        }

        public virtual UserFieldInfo[] GetOtherFields()
        {
            List<UserFieldInfo> list = new List<UserFieldInfo>();
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if ((this.m_otherFields[i] != null) && this.m_otherFields[i].IsValidField())
                    {
                        list.Add(this.m_otherFields[i]);
                    }
                }
            }
            return list.ToArray();
        }

        public virtual bool GrowField(int fieldId, int templateID)
        {
            ItemTemplateInfo info = ItemMgr.FindItemTemplate(templateID);
            lock (this.m_lock)
            {
                this.m_fields[fieldId].SeedID = info.TemplateID;
                this.m_fields[fieldId].PlantTime = DateTime.Now;
                this.m_fields[fieldId].GainCount = info.Property2;
                this.m_fields[fieldId].FieldValidDate = info.Property3;
            }
            return true;
        }

        public virtual bool HelperSwitchFields(bool isHelper, int seedID, int seedTime, int haveCount, int getCount)
        {
            if (isHelper)
            {
                lock (this.m_lock)
                {
                    for (int i = 0; i < this.m_fields.Length; i++)
                    {
                        if (this.m_fields[i] != null)
                        {
                            this.m_fields[i].SeedID = 0;
                            this.m_fields[i].FieldValidDate = 1;
                            this.m_fields[i].AccelerateTime = 0;
                            this.m_fields[i].GainCount = 0;
                        }
                    }
                }
            }
            lock (this.m_lock)
            {
                this.m_farm.isFarmHelper = isHelper;
                this.m_farm.isAutoId = seedID;
                this.m_farm.AutoPayTime = DateTime.Now;
                this.m_farm.AutoValidDate = seedTime;
                this.m_farm.GainFieldId = getCount / 10;
                this.m_farm.KillCropId = getCount;
            }
            return true;
        }

        public bool IsEmpty(int slot)
        {
            return (((slot < 0) || (slot >= this.m_capalility)) || (this.m_fields[slot] == null));
        }

        public virtual bool killCropField(int fieldId)
        {
            lock (this.m_lock)
            {
                if (this.m_fields[fieldId] != null)
                {
                    this.m_fields[fieldId].SeedID = 0;
                    this.m_fields[fieldId].FieldValidDate = 1;
                    this.m_fields[fieldId].AccelerateTime = 0;
                    this.m_fields[fieldId].GainCount = 0;
                }
            }
            return true;
        }

        public virtual int payFieldMoneyToMonth()
        {
            return int.Parse(this.m_farm.PayFieldMoney.Split(new char[] { '|' })[1].Split(new char[] { ',' })[1]);
        }

        public virtual int payFieldMoneyToWeek()
        {
            return int.Parse(this.m_farm.PayFieldMoney.Split(new char[] { '|' })[0].Split(new char[] { ',' })[1]);
        }

        public virtual int payFieldTimeToMonth()
        {
            return int.Parse(this.m_farm.PayFieldMoney.Split(new char[] { '|' })[1].Split(new char[] { ',' })[0]);
        }

        public virtual int payFieldTimeToWeek()
        {
            return int.Parse(this.m_farm.PayFieldMoney.Split(new char[] { '|' })[0].Split(new char[] { ',' })[0]);
        }

        public virtual bool RemoveField(UserFieldInfo item)
        {
            if (item == null)
            {
                return false;
            }
            int num = -1;
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if (this.m_fields[i] == item)
                    {
                        num = i;
                        this.m_fields[i] = null;
                        break;
                    }
                }
            }
            return (num != -1);
        }

        public bool RemoveFieldAt(int place)
        {
            return this.RemoveField(this.GetFieldAt(place));
        }

        public virtual bool RemoveOtherField(UserFieldInfo item)
        {
            if (item == null)
            {
                return false;
            }
            int num = -1;
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if (this.m_otherFields[i] == item)
                    {
                        num = i;
                        this.m_otherFields[i] = null;
                        break;
                    }
                }
            }
            return (num != -1);
        }

        public virtual void ResetFarmProp()
        {
            lock (this.m_lock)
            {
                if (this.m_farm != null)
                {
                    this.m_farm.isArrange = false;
                    this.m_farm.buyExpRemainNum = 20;
                }
            }
        }

        public virtual void StopHelperSwitchField()
        {
            lock (this.m_lock)
            {
                this.m_farm.isFarmHelper = false;
                this.m_farm.isAutoId = 0;
                this.m_farm.AutoPayTime = DateTime.Now;
                this.m_farm.AutoValidDate = 0;
                this.m_farm.GainFieldId = 0;
                this.m_farm.KillCropId = 0;
            }
        }

        public virtual void UpdateFarm(UserFarmInfo farm)
        {
            lock (this.m_lock)
            {
                this.m_farm = farm;
            }
        }

        public virtual void UpdateGainCount(int fieldId, int count)
        {
            lock (this.m_lock)
            {
                this.m_fields[fieldId].GainCount = count;
            }
        }

        public virtual void UpdateOtherFarm(UserFarmInfo farm)
        {
            lock (this.m_lock)
            {
                this.m_otherFarm = farm;
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
                this.m_capalility = (value < 0) ? 0 : ((value > this.m_fields.Length) ? this.m_fields.Length : value);
            }
        }

        public int Status
        {
            get
            {
                return this.m_farmstatus;
            }
        }
    }
}


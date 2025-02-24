namespace Game.Server.GameUtils
{
    using Game.Logic;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public abstract class PetAbstractInventory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int m_aCapalility;
        protected SqlDataProvider.Data.ItemInfo[] m_adoptItems;
        protected UsersPetinfo[] m_adoptPets;
        private int m_beginSlot;
        private int m_capalility;
        private int m_changeCount;
        protected List<int> m_changedPlaces = new List<int>();
        protected object m_lock = new object();
        protected UsersPetinfo[] m_pets;

        public PetAbstractInventory(int capability, int aCapability, int beginSlot)
        {
            this.m_capalility = capability;
            this.m_aCapalility = aCapability;
            this.m_beginSlot = beginSlot;
            this.m_pets = new UsersPetinfo[capability];
            this.m_adoptPets = new UsersPetinfo[aCapability];
        }

        public virtual bool AddAdoptPetTo(UsersPetinfo pet, int place)
        {
            if (((pet == null) || (place >= this.m_aCapalility)) || (place < 0))
            {
                return false;
            }
            lock (this.m_lock)
            {
                if (this.m_adoptPets[place] != null)
                {
                    place = -1;
                }
                else
                {
                    this.m_adoptPets[place] = pet;
                    pet.Place = place;
                }
            }
            return (place != -1);
        }

        public bool AddPet(UsersPetinfo pet)
        {
            return this.AddPet(pet, this.m_beginSlot);
        }

        public bool AddPet(UsersPetinfo pet, int minSlot)
        {
            if (pet == null)
            {
                return false;
            }
            int place = this.FindFirstEmptySlot(minSlot);
            return this.AddPetTo(pet, place);
        }

        public virtual bool AddPetTo(UsersPetinfo pet, int place)
        {
            if (((pet == null) || (place >= this.m_capalility)) || (place < 0))
            {
                return false;
            }
            lock (this.m_lock)
            {
                if (this.m_pets[place] == null)
                {
                    this.m_pets[place] = pet;
                    pet.Place = place;
                }
                else
                {
                    place = -1;
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
                    this.m_pets[i] = null;
                }
            }
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

        public virtual bool EquipPet(int place, bool isEquip)
        {
            int num = -1;
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_pets.Length; i++)
                {
                    if (this.m_pets[i] != null)
                    {
                        num = this.m_pets[i].Place;
                        if (num == place)
                        {
                            if (this.m_pets[i].Hunger == 0)
                            {
                                return false;
                            }
                            this.m_pets[i].IsEquip = isEquip;
                        }
                        else
                        {
                            this.m_pets[i].IsEquip = false;
                        }
                        this.OnPlaceChanged(num);
                    }
                }
            }
            return (num > -1);
        }

        public virtual bool EquipSkillPet(int place, int killId, int killindex)
        {
            string skill = killId + "," + killindex;
            UsersPetinfo pet = this.m_pets[place];
            lock (this.m_lock)
            {
                if (killId == 0)
                {
                    this.m_pets[place].SkillEquip = this.SetSkillEquip(pet, killindex, skill);
                    this.OnPlaceChanged(place);
                    return true;
                }
                if (this.IsEquipSkill(place, killId.ToString()))
                {
                    this.m_pets[place].SkillEquip = this.SetSkillEquip(pet, killindex, skill);
                    this.OnPlaceChanged(place);
                    return true;
                }
            }
            return false;
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
                    if (this.m_pets[i] == null)
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
                    if (this.m_pets[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public virtual UsersPetinfo[] GetAdoptPet()
        {
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            for (int i = 0; i < this.m_aCapalility; i++)
            {
                if ((this.m_adoptPets[i] != null) && this.m_adoptPets[i].IsExit)
                {
                    list.Add(this.m_adoptPets[i]);
                }
            }
            list.Add(PetMgr.CreateNewPet());
            return list.ToArray();
        }

        public virtual UsersPetinfo GetAdoptPetAt(int slot)
        {
            if ((slot < 0) || (slot >= this.m_aCapalility))
            {
                return null;
            }
            return this.m_adoptPets[slot];
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
                    if (this.m_pets[i] == null)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public virtual UsersPetinfo GetPetAt(int slot)
        {
            if ((slot < 0) || (slot >= this.m_capalility))
            {
                return null;
            }
            return this.m_pets[slot];
        }

        public virtual UsersPetinfo GetPetByTemplateID(int minSlot, int templateId)
        {
            lock (this.m_lock)
            {
                for (int i = minSlot; i < this.m_capalility; i++)
                {
                    if ((this.m_pets[i] != null) && (this.m_pets[i].TemplateID == templateId))
                    {
                        return this.m_pets[i];
                    }
                }
                return null;
            }
        }

        public virtual UsersPetinfo GetPetIsEquip()
        {
            for (int i = 0; i < this.m_capalility; i++)
            {
                if ((this.m_pets[i] != null) && this.m_pets[i].IsEquip)
                {
                    return this.m_pets[i];
                }
            }
            return null;
        }

        public virtual UsersPetinfo[] GetPets()
        {
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            for (int i = 0; i < this.m_capalility; i++)
            {
                if (this.m_pets[i] != null)
                {
                    list.Add(this.m_pets[i]);
                }
            }
            return list.ToArray();
        }

        public bool IsEmpty(int slot)
        {
            return (((slot < 0) || (slot >= this.m_capalility)) || (this.m_pets[slot] == null));
        }

        public bool IsEquipSkill(int slot, string kill)
        {
            List<string> skillEquip = this.m_pets[slot].GetSkillEquip();
            for (int i = 0; i < skillEquip.Count; i++)
            {
                if (skillEquip[i].Split(new char[] { ',' })[0] == kill)
                {
                    return false;
                }
            }
            return true;
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

        public virtual bool RemoveAdoptPet(UsersPetinfo pet)
        {
            if (pet == null)
            {
                return false;
            }
            int num = -1;
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_aCapalility; i++)
                {
                    if (this.m_adoptPets[i] == pet)
                    {
                        num = i;
                        this.m_adoptPets[i] = null;
                        break;
                    }
                }
            }
            return (num != -1);
        }

        public virtual bool RemovePet(UsersPetinfo pet)
        {
            if (pet == null)
            {
                return false;
            }
            int place = -1;
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_capalility; i++)
                {
                    if (this.m_pets[i] == pet)
                    {
                        place = i;
                        this.m_pets[i] = null;
                        goto Label_006F;
                    }
                }
            }
        Label_006F:
            if (place != -1)
            {
                this.OnPlaceChanged(place);
                pet.Place = -1;
            }
            return (place != -1);
        }

        public bool RemovePetAt(int place)
        {
            return this.RemovePet(this.GetPetAt(place));
        }

        public virtual bool RenamePet(int place, string name)
        {
            lock (this.m_lock)
            {
                this.m_pets[place].Name = name;
            }
            this.OnPlaceChanged(place);
            return true;
        }

        public string SetSkillEquip(UsersPetinfo pet, int place, string skill)
        {
            List<string> skillEquip = pet.GetSkillEquip();
            skillEquip[place] = skill;
            string str = skillEquip[0];
            for (int i = 1; i < skillEquip.Count; i++)
            {
                str = str + "|" + skillEquip[i];
            }
            return str;
        }

        public virtual void UpdateChangedPlaces()
        {
            this.m_changedPlaces.Clear();
        }

        public virtual bool UpdatePet(UsersPetinfo pet, int place)
        {
            if (pet == null)
            {
                return false;
            }
            int num = -1;
            lock (this.m_lock)
            {
                for (int i = 0; i < this.m_pets.Length; i++)
                {
                    if (this.m_pets[i] != null)
                    {
                        num = this.m_pets[i].Place;
                        if (num == place)
                        {
                            this.m_pets[i] = pet;
                        }
                        this.OnPlaceChanged(num);
                    }
                }
            }
            return (num > -1);
        }

        public virtual bool UpdateQPet(int petPlace, int eqType, PetEquipDataInfo eq)
        {
            if (petPlace > this.m_capalility)
            {
                return false;
            }
            lock (this.m_lock)
            {
                this.m_pets[petPlace].EquipList[eqType] = eq;
            }
            this.OnPlaceChanged(petPlace);
            return true;
        }

        public virtual bool UpGracePet(UsersPetinfo pet, int place, bool isUpdateProp, int min, int max, int Level, ref string msg)
        {
            UsersPetinfo petinfo = pet;
            if (isUpdateProp)
            {
                int blood = 0;
                int attack = 0;
                int defence = 0;
                int agility = 0;
                int lucky = 0;
                int num6 = PetMgr.UpdateEvolution(petinfo.TemplateID, max);
                if ((num6 != 0) && (num6 != petinfo.TemplateID))
                {
                    petinfo.TemplateID = (num6 == 0) ? petinfo.TemplateID : num6;
                    PetMgr.CreateNewPropPet(ref petinfo);
                }
                else
                {
                    PetMgr.PlusPetProp(petinfo, min, max, ref blood, ref attack, ref defence, ref agility, ref lucky);
                    petinfo.Blood = blood;
                    petinfo.Attack = attack;
                    petinfo.Defence = defence;
                    petinfo.Agility = agility;
                    petinfo.Luck = lucky;
                }
                string skill = petinfo.Skill;
                string str2 = PetMgr.UpdateSkillPet(max, petinfo.TemplateID, Level);
                petinfo.Skill = (str2 == "") ? skill : str2;
                petinfo.SkillEquip = PetMgr.ActiveEquipSkill(max);
                if (max > min)
                {
                    msg = petinfo.Name + " thăng cấp " + max;
                }
            }
            lock (this.m_lock)
            {
                this.m_pets[place] = petinfo;
            }
            this.OnPlaceChanged(place);
            return true;
        }

        public int ACapalility
        {
            get
            {
                return this.m_aCapalility;
            }
            set
            {
                this.m_aCapalility = (value < 0) ? 0 : ((value > this.m_adoptPets.Length) ? this.m_adoptPets.Length : value);
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
                this.m_capalility = (value < 0) ? 0 : ((value > this.m_pets.Length) ? this.m_pets.Length : value);
            }
        }
    }
}


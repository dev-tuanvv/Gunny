namespace Game.Server.GameUtils
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    public class PetInventory : PetAbstractInventory
    {
        protected GamePlayer m_player;
        private List<UsersPetinfo> m_removedList;
        private bool m_saveToDb;

        public PetInventory(GamePlayer player, bool saveTodb, int capibility, int aCapability, int beginSlot) : base(capibility, aCapability, beginSlot)
        {
            this.m_player = player;
            this.m_saveToDb = saveTodb;
            this.m_removedList = new List<UsersPetinfo>();
        }

        public override bool AddAdoptPetTo(UsersPetinfo pet, int place)
        {
            return base.AddAdoptPetTo(pet, place);
        }

        public override bool AddPetTo(UsersPetinfo pet, int place)
        {
            if ((pet.EquipList == null) || (pet.EquipList.Count == 0))
            {
                pet.EquipList = this.EmptyPetEquip(this.m_player.PlayerCharacter.ID);
            }
            if (base.AddPetTo(pet, place))
            {
                pet.UserID = this.m_player.PlayerCharacter.ID;
                return true;
            }
            return false;
        }

        public virtual void ClearAdoptPets()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                lock (base.m_lock)
                {
                    for (int i = 0; i < base.ACapalility; i++)
                    {
                        if ((base.m_adoptPets[i] != null) && (base.m_adoptPets[i].ID > 0))
                        {
                            bussiness.ClearAdoptPet(base.m_adoptPets[i].ID);
                        }
                        base.m_adoptPets[i] = null;
                    }
                }
            }
        }

        private List<PetEquipDataInfo> EmptyPetEquip(int UserID)
        {
            List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
            for (int i = 0; i < 3; i++)
            {
                PetEquipDataInfo item = new PetEquipDataInfo(null) {
                    ID = 0,
                    UserID = UserID,
                    PetID = 0,
                    eqType = i,
                    eqTemplateID = -1,
                    startTime = DateTime.Now,
                    ValidDate = 7,
                    IsExit = true
                };
                list.Add(item);
            }
            return list;
        }

        private List<PetEquipDataInfo> GetPetEquip(int petID, PetEquipDataInfo[] eqs)
        {
            List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
            for (int i = 0; i < eqs.Length; i++)
            {
                PetEquipDataInfo item = eqs[i];
                if (petID == item.PetID)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    int iD = this.m_player.PlayerCharacter.ID;
                    UsersPetinfo[] userPetSingles = bussiness.GetUserPetSingles(iD);
                    UsersPetinfo[] userAdoptPetSingles = bussiness.GetUserAdoptPetSingles(iD);
                    PetEquipDataInfo[] eqPetSingles = bussiness.GetEqPetSingles(iD);
                    base.BeginChanges();
                    try
                    {
                        foreach (UsersPetinfo petinfo in userPetSingles)
                        {
                            petinfo.EquipList = this.GetPetEquip(petinfo.ID, eqPetSingles);
                            this.AddPetTo(petinfo, petinfo.Place);
                        }
                        foreach (UsersPetinfo petinfo2 in userAdoptPetSingles)
                        {
                            this.AddAdoptPetTo(petinfo2, petinfo2.Place);
                        }
                    }
                    finally
                    {
                        base.CommitChanges();
                    }
                }
            }
        }

        public virtual bool MoveEqAllToBag(List<PetEquipDataInfo> eps)
        {
            int num = 0;
            for (int i = 0; i < eps.Count; i++)
            {
                this.MoveEqToBag(eps[i]);
                num++;
            }
            return (num > 0);
        }

        public virtual bool MoveEqFromBag(int place, int eqslot, SqlDataProvider.Data.ItemInfo item)
        {
            UsersPetinfo petAt = this.GetPetAt(place);
            if (petAt == null)
            {
                return false;
            }
            if (item.Template.Property2 > petAt.Level)
            {
                return false;
            }
            PetEquipDataInfo ep = petAt.EquipList[eqslot];
            if (ep == null)
            {
                return false;
            }
            if (ep.eqTemplateID > 0)
            {
                this.MoveEqToBag(ep);
            }
            ep.eqTemplateID = item.TemplateID;
            ep.ValidDate = item.ValidDate;
            ep.startTime = item.BeginDate;
            ep = ep.addTempalte(ItemMgr.FindItemTemplate(item.TemplateID));
            return this.UpdateQPet(place, eqslot, ep);
        }

        public virtual void MoveEqToBag(PetEquipDataInfo ep)
        {
            if (ep.IsValidate())
            {
                ItemTemplateInfo goods = ItemMgr.FindItemTemplate(ep.eqTemplateID);
                if (goods != null)
                {
                    SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x66);
                    item.IsUsed = true;
                    item.IsBinds = true;
                    item.ValidDate = ep.ValidDate;
                    item.BeginDate = ep.startTime;
                    List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo>();
                    if (!this.m_player.EquipBag.AddItem(item))
                    {
                        items.Add(item);
                    }
                    if (items.Count > 0)
                    {
                        this.m_player.SendItemsToMail(items, "Bagfull trả về thư!", "Trả trang bị pet về thư!", eMailType.ItemOverdue);
                        this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                }
            }
        }

        public virtual void ReduceHunger()
        {
            UsersPetinfo petIsEquip = this.GetPetIsEquip();
            if (petIsEquip != null)
            {
                int num = 40;
                int num2 = 100;
                if (petIsEquip.Hunger >= num2)
                {
                    if (petIsEquip.Level >= 60)
                    {
                        petIsEquip.Hunger -= num2;
                    }
                    else
                    {
                        petIsEquip.Hunger -= num;
                    }
                    this.UpdatePet(petIsEquip, petIsEquip.Place);
                }
            }
        }

        public override bool RemoveAdoptPet(UsersPetinfo pet)
        {
            return base.RemoveAdoptPet(pet);
        }

        public List<PetEquipDataInfo> RemoveEq(List<PetEquipDataInfo> Eqs)
        {
            List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
            for (int i = 0; i < Eqs.Count; i++)
            {
                PetEquipDataInfo item = Eqs[i];
                item.eqTemplateID = -1;
                item.ValidDate = 7;
                list.Add(item);
            }
            return list;
        }

        public override bool RemovePet(UsersPetinfo pet)
        {
            List<PetEquipDataInfo> equip = pet.GetEquip();
            if (base.RemovePet(pet))
            {
                this.MoveEqAllToBag(equip);
                lock (this.m_removedList)
                {
                    pet.IsExit = false;
                    this.m_removedList.Add(pet);
                }
                return true;
            }
            return false;
        }

        public virtual void SaveqPet(PlayerBussiness pb, UsersPetinfo p)
        {
            for (int i = 0; i < p.EquipList.Count; i++)
            {
                PetEquipDataInfo info = p.EquipList[i];
                info.PetID = p.ID;
                if ((info != null) && info.IsDirty)
                {
                    if (info.ID > 0)
                    {
                        pb.UpdateqPet(info);
                    }
                    else
                    {
                        pb.AddeqPet(info);
                    }
                }
            }
        }

        public virtual void SaveToDatabase(bool saveAdopt)
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    lock (base.m_lock)
                    {
                        for (int i = 0; i < base.m_pets.Length; i++)
                        {
                            UsersPetinfo info = base.m_pets[i];
                            if ((info != null) && info.IsDirty)
                            {
                                if (info.ID > 0)
                                {
                                    bussiness.UpdateUserPet(info);
                                }
                                else
                                {
                                    bussiness.AddUserPet(info);
                                }
                                this.SaveqPet(bussiness, info);
                            }
                        }
                        if (saveAdopt)
                        {
                            for (int j = 0; j < base.m_adoptPets.Length; j++)
                            {
                                UsersPetinfo petinfo2 = base.m_adoptPets[j];
                                if (((petinfo2 != null) && petinfo2.IsDirty) && (petinfo2.ID == 0))
                                {
                                    bussiness.AddUserAdoptPet(petinfo2, false);
                                }
                            }
                        }
                    }
                    lock (this.m_removedList)
                    {
                        foreach (UsersPetinfo petinfo3 in this.m_removedList)
                        {
                            bussiness.UpdateUserPet(petinfo3);
                        }
                        this.m_removedList.Clear();
                    }
                }
            }
        }

        public override void UpdateChangedPlaces()
        {
            int[] slots = base.m_changedPlaces.ToArray();
            this.m_player.Out.SendUpdateUserPet(this, slots);
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


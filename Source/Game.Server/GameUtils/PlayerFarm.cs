namespace Game.Server.GameUtils
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class PlayerFarm : PlayerFarmInventory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected GamePlayer m_player;
        private List<UserFieldInfo> m_removedList;
        private bool m_saveToDb;

        public PlayerFarm(GamePlayer player, bool saveTodb, int capibility, int beginSlot) : base(capibility, beginSlot)
        {
            this.m_removedList = new List<UserFieldInfo>();
            this.m_player = player;
            this.m_saveToDb = saveTodb;
        }

        public bool AddFieldTo(UserFieldInfo item, int place, int farmId)
        {
            if (base.AddFieldTo(item, place))
            {
                item.FarmID = farmId;
                return true;
            }
            return false;
        }

        public bool AddOtherFieldTo(UserFieldInfo item, int place, int farmId)
        {
            if (base.AddOtherFieldTo(item, place))
            {
                item.FarmID = farmId;
                return true;
            }
            return false;
        }

        public void CropHelperSwitchField(bool isStopFarmHelper)
        {
            if ((base.m_farm != null) && base.m_farm.isFarmHelper)
            {
                int num3;
                SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(ItemMgr.FindItemTemplate(base.m_farm.isAutoId).Property4), 1, 0x66);
                int num = base.m_farm.AutoValidDate * 60;
                TimeSpan span = (TimeSpan) (DateTime.Now - base.m_farm.AutoPayTime);
                int killCropId = base.m_farm.KillCropId;
                if (span.TotalMilliseconds < 0.0)
                {
                    num3 = num;
                }
                else
                {
                    num3 = num - ((int) span.TotalMilliseconds);
                }
                int num4 = ((1 - (num3 / num)) * killCropId) / 0x3e8;
                if (num4 > killCropId)
                {
                    num4 = killCropId;
                    isStopFarmHelper = true;
                }
                if (isStopFarmHelper)
                {
                    item.Count = num4;
                    if (num4 > 0)
                    {
                        string content = string.Concat(new object[] { "Kết th\x00fac trợ thủ, bạn nhận được ", num4, " ", item.Template.Name });
                        string title = "Kết th\x00fac trợ thủ, nhận được thức ăn th\x00fa cưng!";
                        this.m_player.SendItemToMail(item, content, title, eMailType.ItemOverdue);
                        this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                    lock (base.m_lock)
                    {
                        base.m_farm.isFarmHelper = false;
                        base.m_farm.isAutoId = 0;
                        base.m_farm.AutoPayTime = DateTime.Now;
                        base.m_farm.AutoValidDate = 0;
                        base.m_farm.GainFieldId = 0;
                        base.m_farm.KillCropId = 0;
                    }
                    this.m_player.Out.SendHelperSwitchField(this.m_player.PlayerCharacter, base.m_farm);
                }
            }
        }

        public void EnterFarm()
        {
            this.CropHelperSwitchField(false);
            if (base.m_farm == null)
            {
                this.CreateFarm(this.m_player.PlayerCharacter.ID, this.m_player.PlayerCharacter.NickName);
            }
            if (this.AccelerateTimeFields())
            {
                this.m_player.Out.SendEnterFarm(this.m_player.PlayerCharacter, this.CurrentFarm, this.GetFields());
                base.m_farmstatus = 1;
            }
        }

        public void EnterFriendFarm(int userId)
        {
            UserFarmInfo currentFarm;
            UserFieldInfo[] currentFields;
            GamePlayer playerById = WorldMgr.GetPlayerById(userId);
            if (playerById == null)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    currentFarm = bussiness.GetSingleFarm(userId);
                    currentFields = bussiness.GetSingleFields(userId);
                    goto Label_0070;
                }
            }
            currentFarm = playerById.Farm.CurrentFarm;
            currentFields = playerById.Farm.CurrentFields;
            playerById.ViFarmsAdd(this.m_player.PlayerCharacter.ID);
        Label_0070:
            if (currentFarm == null)
            {
                currentFarm = this.CreateFarmForNulll(userId);
                currentFields = this.CreateFieldsForNull(userId);
            }
            base.m_farmstatus = this.m_player.PlayerCharacter.ID;
            this.UpdateOtherFarm(currentFarm);
            base.ClearOtherFields();
            foreach (UserFieldInfo info2 in currentFields)
            {
                if (info2 != null)
                {
                    this.AddOtherFieldTo(info2, info2.FieldID, currentFarm.FarmID);
                }
            }
            if (this.AccelerateOtherTimeFields())
            {
                this.m_player.Out.SendEnterFarm(this.m_player.PlayerCharacter, this.OtherFarm, this.GetOtherFields());
            }
        }

        public void ExitFarm()
        {
            base.m_farmstatus = 0;
        }

        public virtual bool GainField(int fieldId)
        {
            this.AccelerateTimeFields();
            if (!this.GetFieldAt(fieldId).isDig())
            {
                if ((fieldId < 0) || (fieldId > this.GetFields().Count<UserFieldInfo>()))
                {
                    return false;
                }
                ItemTemplateInfo info = ItemMgr.FindItemTemplate(base.m_fields[fieldId].SeedID);
                if (info == null)
                {
                    return false;
                }
                SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info.Property4), 1, 0x66);
                List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo>();
                item.Count = base.m_fields[fieldId].GainCount;
                if (base.killCropField(fieldId))
                {
                    if (!(this.m_player.PropBag.StackItemToAnother(item) || this.m_player.PropBag.AddItem(item)))
                    {
                        items.Add(item);
                    }
                    this.m_player.Out.SendtoGather(this.m_player.PlayerCharacter, base.m_fields[fieldId]);
                    foreach (int num in this.m_player.ViFarms)
                    {
                        GamePlayer playerById = WorldMgr.GetPlayerById(num);
                        if ((playerById != null) && (playerById.Farm.Status == num))
                        {
                            playerById.Out.SendtoGather(this.m_player.PlayerCharacter, base.m_fields[fieldId]);
                        }
                    }
                    this.m_player.OnCropPrimaryEvent();
                    if (items.Count > 0)
                    {
                        this.m_player.SendItemsToMail(items, "Bagfull trả về thư!", "Bagfull trả về thư!", eMailType.ItemOverdue);
                        this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                    return true;
                }
            }
            return false;
        }

        public virtual bool GainFriendFields(int userId, int fieldId)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(userId);
            UserFieldInfo info = base.m_otherFields[fieldId];
            if (info == null)
            {
                return false;
            }
            SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(ItemMgr.FindItemTemplate(info.SeedID).Property4), 1, 0x66);
            List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo>();
            this.AccelerateTimeFields();
            if (this.GetOtherFieldAt(fieldId).isDig())
            {
                return false;
            }
            lock (base.m_lock)
            {
                if (base.m_otherFields[fieldId].GainCount <= 9)
                {
                    return false;
                }
                UserFieldInfo info1 = base.m_otherFields[fieldId];
                info1.GainCount--;
            }
            if (!(this.m_player.PropBag.StackItemToAnother(item) || this.m_player.PropBag.AddItem(item)))
            {
                items.Add(item);
            }
            if (playerById == null)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    for (int i = 0; i < base.m_otherFields.Length; i++)
                    {
                        UserFieldInfo info5 = base.m_otherFields[i];
                        if (info5 != null)
                        {
                            bussiness.UpdateFields(info5);
                        }
                    }
                    goto Label_01C1;
                }
            }
            if (playerById.Farm.Status == 1)
            {
                playerById.Farm.UpdateGainCount(fieldId, base.m_otherFields[fieldId].GainCount);
                playerById.Out.SendtoGather(playerById.PlayerCharacter, base.m_otherFields[fieldId]);
            }
        Label_01C1:
            this.m_player.Out.SendtoGather(this.m_player.PlayerCharacter, base.m_otherFields[fieldId]);
            if (items.Count > 0)
            {
                this.m_player.SendItemsToMail(items, "Bagfull trả về thư!", "Bagfull trả về thư!", eMailType.ItemOverdue);
                this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
            return true;
        }

        public override bool GrowField(int fieldId, int templateID)
        {
            if (base.GrowField(fieldId, templateID))
            {
                foreach (int num in this.m_player.ViFarms)
                {
                    GamePlayer playerById = WorldMgr.GetPlayerById(num);
                    if ((playerById != null) && (playerById.Farm.Status == num))
                    {
                        playerById.Out.SendSeeding(this.m_player.PlayerCharacter, base.m_fields[fieldId]);
                    }
                }
                this.m_player.Out.SendSeeding(this.m_player.PlayerCharacter, base.m_fields[fieldId]);
                return true;
            }
            return false;
        }

        public virtual void HelperSwitchField(bool isHelper, int seedID, int seedTime, int haveCount, int getCount)
        {
            if (base.HelperSwitchFields(isHelper, seedID, seedTime, haveCount, getCount))
            {
                this.m_player.Out.SendHelperSwitchField(this.m_player.PlayerCharacter, base.m_farm);
            }
        }

        public override bool killCropField(int fieldId)
        {
            if (base.killCropField(fieldId))
            {
                this.m_player.Out.SendKillCropField(this.m_player.PlayerCharacter, base.m_fields[fieldId]);
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
                    UserFarmInfo singleFarm = bussiness.GetSingleFarm(this.m_player.PlayerCharacter.ID);
                    UserFieldInfo[] singleFields = bussiness.GetSingleFields(this.m_player.PlayerCharacter.ID);
                    this.UpdateFarm(singleFarm);
                    foreach (UserFieldInfo info2 in singleFields)
                    {
                        this.AddFieldTo(info2, info2.FieldID, singleFarm.FarmID);
                    }
                }
            }
        }

        public virtual void PayField(List<int> fieldIds, int payFieldTime)
        {
            if (base.CreateField(this.m_player.PlayerCharacter.ID, fieldIds, payFieldTime))
            {
                foreach (int num in this.m_player.ViFarms)
                {
                    GamePlayer playerById = WorldMgr.GetPlayerById(num);
                    if ((playerById != null) && (playerById.Farm.Status == num))
                    {
                        playerById.Out.SendPayFields(this.m_player, fieldIds);
                    }
                }
                this.m_player.Out.SendPayFields(this.m_player, fieldIds);
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
                        if ((base.m_farm != null) && base.m_farm.IsDirty)
                        {
                            if (base.m_farm.ID > 0)
                            {
                                bussiness.UpdateFarm(base.m_farm);
                            }
                            else
                            {
                                bussiness.AddFarm(base.m_farm);
                            }
                            for (int i = 0; i < base.m_fields.Length; i++)
                            {
                                UserFieldInfo info = base.m_fields[i];
                                if ((info != null) && info.IsDirty)
                                {
                                    if (info.ID > 0)
                                    {
                                        bussiness.UpdateFields(info);
                                    }
                                    else
                                    {
                                        bussiness.AddFields(info);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public UserFarmInfo CurrentFarm
        {
            get
            {
                return base.m_farm;
            }
        }

        public UserFieldInfo[] CurrentFields
        {
            get
            {
                return base.m_fields;
            }
        }

        public UserFarmInfo OtherFarm
        {
            get
            {
                return base.m_otherFarm;
            }
        }

        public UserFieldInfo[] OtherFields
        {
            get
            {
                return base.m_otherFields;
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


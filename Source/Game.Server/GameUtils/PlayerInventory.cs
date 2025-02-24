namespace Game.Server.GameUtils
{
    using Bussiness;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class PlayerInventory : AbstractInventory
    {
        protected GamePlayer m_player;
        private List<SqlDataProvider.Data.ItemInfo> m_removedList;
        private bool m_saveToDb;

        public PlayerInventory(GamePlayer player, bool saveTodb, int capibility, int type, int beginSlot, bool autoStack) : base(capibility, type, beginSlot, autoStack)
        {
            this.m_removedList = new List<SqlDataProvider.Data.ItemInfo>();
            this.m_player = player;
            this.m_saveToDb = saveTodb;
        }

        public override bool AddItemTo(SqlDataProvider.Data.ItemInfo item, int place)
        {
            if (base.AddItemTo(item, place))
            {
                item.UserID = this.m_player.PlayerCharacter.ID;
                item.IsExist = true;
                return true;
            }
            return false;
        }

        public bool IsEquipSlot(int slot)
        {
            return ((slot >= 0) && (slot < base.BeginSlot));
        }

        public bool IsMagicStoneEquipSlot(int slot)
        {
            switch (slot)
            {
                case 0:
                case 4:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    break;

                case 1:
                case 2:
                case 3:
                case 5:
                    return false;

                default:
                    if (slot != 15)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        public bool IsWrongPlace(SqlDataProvider.Data.ItemInfo item)
        {
            return (((item != null) && (item.Template != null)) && (((((item.Template.CategoryID == 7) && (item.Place != 6)) || ((item.Template.CategoryID == 0x1b) && (item.Place != 6))) || ((item.Template.CategoryID == 0x11) && (item.Place != 15))) || ((item.Template.CategoryID == 0x1f) && (item.Place != 15))));
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    SqlDataProvider.Data.ItemInfo[] userBagByType = bussiness.GetUserBagByType(this.m_player.PlayerCharacter.ID, base.BagType);
                    base.BeginChanges();
                    try
                    {
                        foreach (SqlDataProvider.Data.ItemInfo info in userBagByType)
                        {
                            if ((this.IsWrongPlace(info) && (info.Place < 0x1f)) && (base.BagType == 0))
                            {
                                int toSlot = this.FindFirstEmptySlot(0x1f);
                                if (toSlot != -1)
                                {
                                    this.MoveItem(info.Place, toSlot, info.Count);
                                }
                                else
                                {
                                    this.m_player.AddTemplate(info);
                                }
                            }
                            else
                            {
                                this.AddItemTo(info, info.Place);
                            }
                        }
                    }
                    finally
                    {
                        base.CommitChanges();
                    }
                }
            }
        }

        public override bool RemoveItem(SqlDataProvider.Data.ItemInfo item)
        {
            if (base.RemoveItem(item))
            {
                item.IsExist = false;
                if (this.m_saveToDb)
                {
                    lock (this.m_removedList)
                    {
                        this.m_removedList.Add(item);
                    }
                }
                return true;
            }
            return false;
        }

        public virtual void SaveNewsItemIntoDatabas()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    lock (base.m_lock)
                    {
                        for (int i = 0; i < base.m_items.Length; i++)
                        {
                            SqlDataProvider.Data.ItemInfo item = base.m_items[i];
                            if (((item != null) && item.IsDirty) && (item.ItemID == 0))
                            {
                                bussiness.AddGoods(item);
                            }
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
                    lock (base.m_lock)
                    {
                        for (int i = 0; i < base.m_items.Length; i++)
                        {
                            SqlDataProvider.Data.ItemInfo item = base.m_items[i];
                            if ((item != null) && item.IsDirty)
                            {
                                if (item.ItemID > 0)
                                {
                                    bussiness.UpdateGoods(item);
                                }
                                else
                                {
                                    bussiness.AddGoods(item);
                                }
                            }
                        }
                    }
                    lock (this.m_removedList)
                    {
                        foreach (SqlDataProvider.Data.ItemInfo info2 in this.m_removedList)
                        {
                            if (info2.ItemID > 0)
                            {
                                bussiness.UpdateGoods(info2);
                            }
                        }
                        this.m_removedList.Clear();
                    }
                }
            }
        }

        public bool SendAllItemsToMail(string sender, string title, eMailType type)
        {
            if (this.m_saveToDb)
            {
                base.BeginChanges();
                try
                {
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        lock (base.m_lock)
                        {
                            List<SqlDataProvider.Data.ItemInfo> items = this.GetItems();
                            int count = items.Count;
                            for (int i = 0; i < count; i += 5)
                            {
                                MailInfo mail = new MailInfo {
                                    SenderID = 0,
                                    Sender = sender,
                                    ReceiverID = this.m_player.PlayerCharacter.ID,
                                    Receiver = this.m_player.PlayerCharacter.NickName,
                                    Title = title,
                                    Type = (int) type,
                                    Content = ""
                                };
                                List<SqlDataProvider.Data.ItemInfo> list2 = new List<SqlDataProvider.Data.ItemInfo>();
                                for (int j = 0; j < 5; j++)
                                {
                                    int num4 = (i * 5) + j;
                                    if (num4 < items.Count)
                                    {
                                        list2.Add(items[num4]);
                                    }
                                }
                                if (!this.SendItemsToMail(list2, mail, bussiness))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Send Items Mail Error:" + exception);
                }
                finally
                {
                    this.SaveToDatabase();
                    base.CommitChanges();
                }
                this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
            return true;
        }

        public bool SendItemsToMail(List<SqlDataProvider.Data.ItemInfo> items, MailInfo mail, PlayerBussiness pb)
        {
            if (mail != null)
            {
                if (items.Count > 5)
                {
                    return false;
                }
                if (this.m_saveToDb)
                {
                    List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
                    StringBuilder builder = new StringBuilder();
                    builder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark", new object[0]));
                    if ((items.Count > 0) && this.TakeOutItem(items[0]))
                    {
                        SqlDataProvider.Data.ItemInfo item = items[0];
                        mail.Annex1 = item.ItemID.ToString();
                        mail.Annex1Name = item.Template.Name;
                        builder.Append(string.Concat(new object[] { "1、", mail.Annex1Name, "x", item.Count, ";" }));
                        list.Add(item);
                    }
                    if ((items.Count > 1) && this.TakeOutItem(items[1]))
                    {
                        SqlDataProvider.Data.ItemInfo info2 = items[1];
                        mail.Annex2 = info2.ItemID.ToString();
                        mail.Annex2Name = info2.Template.Name;
                        builder.Append(string.Concat(new object[] { "2、", mail.Annex2Name, "x", info2.Count, ";" }));
                        list.Add(info2);
                    }
                    if ((items.Count > 2) && this.TakeOutItem(items[2]))
                    {
                        SqlDataProvider.Data.ItemInfo info3 = items[2];
                        mail.Annex3 = info3.ItemID.ToString();
                        mail.Annex3Name = info3.Template.Name;
                        builder.Append(string.Concat(new object[] { "3、", mail.Annex3Name, "x", info3.Count, ";" }));
                        list.Add(info3);
                    }
                    if ((items.Count > 3) && this.TakeOutItem(items[3]))
                    {
                        SqlDataProvider.Data.ItemInfo info4 = items[3];
                        mail.Annex4 = info4.ItemID.ToString();
                        mail.Annex4Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "4、", mail.Annex4Name, "x", info4.Count, ";" }));
                        list.Add(info4);
                    }
                    if ((items.Count > 4) && this.TakeOutItem(items[4]))
                    {
                        SqlDataProvider.Data.ItemInfo info5 = items[4];
                        mail.Annex5 = info5.ItemID.ToString();
                        mail.Annex5Name = info5.Template.Name;
                        builder.Append(string.Concat(new object[] { "5、", mail.Annex5Name, "x", info5.Count, ";" }));
                        list.Add(info5);
                    }
                    mail.AnnexRemark = builder.ToString();
                    if (pb.SendMail(mail))
                    {
                        return true;
                    }
                    foreach (SqlDataProvider.Data.ItemInfo info6 in list)
                    {
                        this.AddItem(info6);
                    }
                }
            }
            return false;
        }

        public bool SendItemToMail(SqlDataProvider.Data.ItemInfo item)
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    return this.SendItemToMail(item, bussiness, null);
                }
            }
            return false;
        }

        public bool SendItemToMail(SqlDataProvider.Data.ItemInfo item, PlayerBussiness pb, MailInfo mail)
        {
            if (this.m_saveToDb && (item.BagType == base.BagType))
            {
                if (mail == null)
                {
                    mail = new MailInfo();
                    mail.Annex1 = item.ItemID.ToString();
                    mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]);
                    mail.Gold = 0;
                    mail.IsExist = true;
                    mail.Money = 0;
                    mail.Receiver = this.m_player.PlayerCharacter.NickName;
                    mail.ReceiverID = item.UserID;
                    mail.Sender = this.m_player.PlayerCharacter.NickName;
                    mail.SenderID = item.UserID;
                    mail.Title = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]);
                    mail.Type = 9;
                }
                if (pb.SendMail(mail))
                {
                    this.RemoveItem(item);
                    item.IsExist = true;
                    return true;
                }
            }
            return false;
        }

        public override bool TakeOutItem(SqlDataProvider.Data.ItemInfo item)
        {
            if (base.TakeOutItem(item))
            {
                if (this.m_saveToDb)
                {
                    lock (this.m_removedList)
                    {
                        this.m_removedList.Add(item);
                    }
                }
                return true;
            }
            return false;
        }

        public override void UpdateChangedPlaces()
        {
            int[] updatedSlots = base.m_changedPlaces.ToArray();
            this.m_player.Out.SendUpdateInventorySlot(this, updatedSlots);
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


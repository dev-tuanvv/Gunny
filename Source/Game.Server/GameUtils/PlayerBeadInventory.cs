namespace Game.Server.GameUtils
{
    using Bussiness;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    public class PlayerBeadInventory : PlayerInventory
    {
        private const int BEAD_START = 0x20;
        private Dictionary<int, UserDrillInfo> m_userDrills;

        public PlayerBeadInventory(GamePlayer player) : base(player, true, 0xb3, 0x15, 0x20, false)
        {
            this.m_userDrills = new Dictionary<int, UserDrillInfo>();
        }

        public bool canEquip(int place, int grade, ref int needLv)
        {
            bool flag = true;
            switch (place)
            {
                case 6:
                    needLv = 15;
                    if (grade < needLv)
                    {
                        flag = false;
                    }
                    return flag;

                case 7:
                    needLv = 0x12;
                    if (grade < needLv)
                    {
                        flag = false;
                    }
                    return flag;

                case 8:
                    needLv = 0x15;
                    if (grade < needLv)
                    {
                        flag = false;
                    }
                    return flag;

                case 9:
                    needLv = 0x18;
                    if (grade < needLv)
                    {
                        flag = false;
                    }
                    return flag;

                case 10:
                    needLv = 0x1b;
                    if (grade < needLv)
                    {
                        flag = false;
                    }
                    return flag;

                case 11:
                    needLv = 30;
                    if (grade < needLv)
                    {
                        flag = false;
                    }
                    return flag;

                case 12:
                    needLv = 0x21;
                    if (grade < needLv)
                    {
                        flag = false;
                    }
                    return flag;
            }
            return flag;
        }

        public int GetDrillLevel(int place)
        {
            for (int i = 0; i < this.UserDrills.Count; i++)
            {
                if (this.UserDrills[i].BeadPlace == place)
                {
                    return this.UserDrills[i].HoleLv;
                }
            }
            return 0;
        }

        public bool JudgeLevel(int beadLv, int drillLv)
        {
            switch (drillLv)
            {
                case 1:
                    if ((beadLv < 1) || (beadLv > 4))
                    {
                        break;
                    }
                    return true;

                case 2:
                    if ((beadLv < 1) || (beadLv > 8))
                    {
                        break;
                    }
                    return true;

                case 3:
                    if ((beadLv < 1) || (beadLv > 12))
                    {
                        break;
                    }
                    return true;

                case 4:
                    if ((beadLv < 1) || (beadLv > 0x10))
                    {
                        break;
                    }
                    return true;

                case 5:
                    return true;
            }
            return false;
        }

        public void LoadDrills()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                this.m_userDrills = bussiness.GetPlayerDrillByID(base.m_player.PlayerCharacter.ID);
                if (this.m_userDrills.Count == 0)
                {
                    List<int> list = new List<int> { 13, 14, 15, 0x10, 0x11, 0x12 };
                    List<int> list2 = new List<int> { 0, 1, 2, 3, 4, 5 };
                    for (int i = 0; i < list.Count; i++)
                    {
                        UserDrillInfo item = new UserDrillInfo {
                            UserID = base.m_player.PlayerCharacter.ID,
                            BeadPlace = list[i],
                            HoleLv = 0,
                            HoleExp = 0,
                            DrillPlace = list2[i]
                        };
                        bussiness.AddUserUserDrill(item);
                        if (!this.m_userDrills.ContainsKey(item.DrillPlace))
                        {
                            this.m_userDrills.Add(item.DrillPlace, item);
                        }
                    }
                }
            }
        }

        public override void LoadFromDatabase()
        {
            base.BeginChanges();
            try
            {
                base.LoadFromDatabase();
                this.LoadDrills();
                List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo>();
                for (int i = 1; i < 0x20; i++)
                {
                    SqlDataProvider.Data.ItemInfo item = base.m_items[i];
                    if (((base.m_items[i] != null) && (base.m_items[i].Place >= 13)) && (base.m_items[i].Place <= 0x12))
                    {
                        int drillLevel = this.GetDrillLevel(base.m_items[i].Place);
                        if (!this.JudgeLevel(base.m_items[i].Hole1, drillLevel))
                        {
                            int toSlot = this.FindFirstEmptySlot(0x20);
                            if (toSlot >= 0)
                            {
                                this.MoveItem(item.Place, toSlot, item.Count);
                            }
                            else
                            {
                                items.Add(item);
                            }
                        }
                    }
                }
                if (items.Count > 0)
                {
                    base.m_player.SendItemsToMail(items, "", "Cấp ch\x00e2u b\x00e1u v\x00e0 cấp lỗ kh\x00f4ng khớp", eMailType.ItemOverdue);
                    base.m_player.Out.SendMailResponse(base.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
                }
            }
            finally
            {
                base.CommitChanges();
            }
        }

        public override bool MoveItem(int fromSlot, int toSlot, int count)
        {
            return ((base.m_items[fromSlot] != null) && base.MoveItem(fromSlot, toSlot, count));
        }

        public override void SaveToDatabase()
        {
            base.SaveToDatabase();
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (UserDrillInfo info in this.m_userDrills.Values)
                {
                    bussiness.UpdateUserDrillInfo(info);
                }
            }
        }

        public void SendPlayerDrill()
        {
            base.m_player.Out.SendPlayerDrill(base.m_player.PlayerCharacter.ID, this.m_userDrills);
        }

        public override void UpdateChangedPlaces()
        {
            foreach (int num2 in base.m_changedPlaces.ToArray())
            {
                if (base.IsEquipSlot(num2))
                {
                    SqlDataProvider.Data.ItemInfo itemAt = this.GetItemAt(num2);
                    if (itemAt != null)
                    {
                        itemAt.IsBinds = true;
                    }
                    break;
                }
            }
            base.UpdateChangedPlaces();
        }

        public void UpdateDrill(int index, UserDrillInfo drill)
        {
            this.m_userDrills[index] = drill;
        }

        public Dictionary<int, UserDrillInfo> UserDrills
        {
            get
            {
                return this.m_userDrills;
            }
            set
            {
                this.m_userDrills = value;
            }
        }
    }
}


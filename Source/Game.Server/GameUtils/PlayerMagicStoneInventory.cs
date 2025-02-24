namespace Game.Server.GameUtils
{
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    public class PlayerMagicStoneInventory : PlayerInventory
    {
        private const int MAGIC_END = 0x90;
        private const int MAGIC_START = 0x20;

        public PlayerMagicStoneInventory(GamePlayer player) : base(player, true, 0x90, 0x29, 0x20, false)
        {
        }

        public virtual void ClearAllMagicStone()
        {
            base.RemoveAllItem();
        }

        public override void LoadFromDatabase()
        {
            base.LoadFromDatabase();
        }

        public override void SaveToDatabase()
        {
            base.SaveToDatabase();
        }

        public virtual bool ScanStoneNormalEquip(SqlDataProvider.Data.ItemInfo itemCheck)
        {
            bool flag = false;
            base.BeginChanges();
            try
            {
                List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo>();
                for (int i = 0; i < 0x1f; i++)
                {
                    if ((base.m_items[i] != null) && base.IsMagicStoneEquipSlot(i))
                    {
                        if (MagicStoneTemplateMgr.StoneNormalSame(base.m_items[i], itemCheck))
                        {
                            flag = true;
                        }
                    }
                    else if ((base.m_items[i] != null) && !base.IsMagicStoneEquipSlot(i))
                    {
                        SqlDataProvider.Data.ItemInfo item = base.m_items[i];
                        int toSlot = base.FindFirstEmptySlot();
                        if (toSlot != -1)
                        {
                            this.MoveItem(item.Place, toSlot, item.Count);
                        }
                        else
                        {
                            items.Add(item);
                        }
                    }
                }
                if (items.Count > 0)
                {
                    base.m_player.SendItemsToMail(items, "Ma thạch của bạn đặt sai vị tr\x00ed hoặc vị tr\x00ed kh\x00f4ng tồn tại.", "Ma thạch kh\x00f4ng hợp lệ trả về thư.", eMailType.ItemOverdue);
                    base.m_player.Out.SendMailResponse(base.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
                }
            }
            finally
            {
                base.CommitChanges();
            }
            return flag;
        }
    }
}


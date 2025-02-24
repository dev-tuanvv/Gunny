namespace Game.Server.Packets.Client
{
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using Game.Server.GameUtils;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0xde, "场景用户离开")]
    public class EquipRetrieveHandler : IPacketHandler
    {
        private Random rnd = new Random();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            List<SqlDataProvider.Data.ItemInfo> list;
            int user = 0;
            PlayerInventory inventory = client.Player.GetInventory(eBageType.Store);
            int num2 = 0;
            bool flag = false;
            for (int i = 1; i < 5; i++)
            {
                SqlDataProvider.Data.ItemInfo itemAt = inventory.GetItemAt(i);
                if (itemAt != null)
                {
                    inventory.RemoveItemAt(i);
                }
                if (itemAt.IsBinds)
                {
                    flag = true;
                }
                num2 += itemAt.Template.Quality;
            }
            int num4 = num2;
            if (num4 <= 12)
            {
                if ((num4 != 8) && (num4 != 12))
                {
                    goto Label_00B8;
                }
            }
            else if ((num4 != 15) && (num4 != 20))
            {
                goto Label_00B8;
            }
            user = num2;
        Label_00B8:
            list = null;
            DropInventory.RetrieveDrop(user, ref list);
            int num5 = this.rnd.Next(list.Count);
            SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list[num5].TemplateID), 1, 0x69);
            item.IsBinds = flag;
            item.BeginDate = DateTime.Now;
            if (item.Template.CategoryID != 11)
            {
                item.ValidDate = 30;
                item.IsBinds = true;
            }
            item.IsBinds = true;
            inventory.AddItemTo(item, 0);
            return 1;
        }
    }
}


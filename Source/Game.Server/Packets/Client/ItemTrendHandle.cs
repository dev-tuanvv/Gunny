namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [PacketHandler(120, "物品倾向转移")]
    public class ItemTrendHandle : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            SqlDataProvider.Data.ItemInfo info;
            eBageType bagType = (eBageType) packet.ReadInt();
            int place = packet.ReadInt();
            eBageType type2 = (eBageType) packet.ReadInt();
            List<ShopItemInfo> list = new List<ShopItemInfo>();
            int num2 = packet.ReadInt();
            int operation = packet.ReadInt();
            if (num2 == -1)
            {
                packet.ReadInt();
                packet.ReadInt();
                int num4 = 0;
                int num5 = 0;
                info = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(0x8535), 1, 0x66);
                list = ShopMgr.FindShopbyTemplatID(0x8535);
                for (int i = 0; i < list.Count; i++)
                {
                    if ((list[i].APrice1 == -1) && (list[i].AValue1 != 0))
                    {
                        num5 = list[i].AValue1;
                        info.ValidDate = list[i].AUnit;
                    }
                }
                if (info != null)
                {
                    if ((num4 <= client.Player.PlayerCharacter.Gold) && (num5 <= client.Player.PlayerCharacter.Money))
                    {
                        client.Player.RemoveMoney(num5);
                        client.Player.RemoveGold(num4);
                    }
                    else
                    {
                        info = null;
                    }
                }
            }
            else
            {
                info = client.Player.GetItemAt(type2, num2);
            }
            SqlDataProvider.Data.ItemInfo itemAt = client.Player.GetItemAt(bagType, place);
            StringBuilder builder = new StringBuilder();
            if ((info != null) && (itemAt != null))
            {
                bool result = false;
                ItemTemplateInfo goods = RefineryMgr.RefineryTrend(operation, itemAt, ref result);
                if (result && (goods != null))
                {
                    SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x73);
                    AbstractInventory itemInventory = client.Player.GetItemInventory(goods);
                    if (itemInventory.AddItem(item, itemInventory.BeginSlot))
                    {
                        client.Player.UpdateItem(item);
                        client.Player.RemoveItem(itemAt);
                        info.Count--;
                        client.Player.UpdateItem(info);
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTrendHandle.Success", new object[0]));
                    }
                    else
                    {
                        builder.Append("NoPlace");
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(item.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace", new object[0]));
                    }
                    return 1;
                }
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTrendHandle.Fail", new object[0]));
            }
            return 1;
        }
    }
}


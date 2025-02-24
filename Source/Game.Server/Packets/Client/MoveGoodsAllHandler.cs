namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using log4net;
    using System;
    using System.Collections.Generic;
    using SqlDataProvider.Data;
    using System.Reflection;

    [PacketHandler(124, "物品比较")]
    public class MoveGoodsAllHandler : IPacketHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static MoveGoodsAllHandler()
        {
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool flag = packet.ReadBoolean();
            int num1 = packet.ReadInt();
            int num2 = packet.ReadInt();
            PlayerInventory inventory = client.Player.GetInventory((eBageType)num2);
            List<SqlDataProvider.Data.ItemInfo> items1 = inventory.GetItems(inventory.BeginSlot, inventory.Capalility);
            if (num1 == items1.Count)
            {
                inventory.BeginChanges();
                try
                {
                    SqlDataProvider.Data.ItemInfo[] rawSpaces = inventory.GetRawSpaces();
                    inventory.ClearBag();
                    for (int index1 = 0; index1 < num1; ++index1)
                    {
                        int index2 = packet.ReadInt();
                        int place = packet.ReadInt();
                        SqlDataProvider.Data.ItemInfo itemInfo = rawSpaces[index2];
                        if (!inventory.AddItemTo(itemInfo, place))
                            MoveGoodsAllHandler.log.Warn((object)string.Format("move item error: old place:{0} new place:{1}", (object)index2, (object)place));
                    }
                    if (flag)
                    {
                        List<SqlDataProvider.Data.ItemInfo> items2 = inventory.GetItems(inventory.BeginSlot, inventory.Capalility);
                        List<int> list = new List<int>();
                        for (int index1 = 0; index1 < items2.Count; ++index1)
                        {
                            if (!list.Contains(index1))
                            {
                                for (int index2 = items2.Count - 1; index2 > index1; --index2)
                                {
                                    if (!list.Contains(index2) && items2[index1].TemplateID == items2[index2].TemplateID && items2[index1].CanStackedTo(items2[index2]))
                                    {
                                        inventory.MoveItem(items2[index2].Place, items2[index1].Place, items2[index2].Count);
                                        list.Add(index2);
                                    }
                                }
                            }
                        }
                        List<SqlDataProvider.Data.ItemInfo> items3 = inventory.GetItems(inventory.BeginSlot, inventory.Capalility);
                        if (inventory.FindFirstEmptySlot() != -1)
                        {
                            for (int index = 1; inventory.FindFirstEmptySlot() < items3[items3.Count - index].Place; ++index)
                                inventory.MoveItem(items3[items3.Count - index].Place, inventory.FindFirstEmptySlot(), items3[items3.Count - index].Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MoveGoodsAllHandler.log.ErrorFormat("Arrage bag errror,user id:{0}   msg:{1}", (object)client.Player.PlayerId, (object)ex.Message);
                }
                finally
                {
                    inventory.CommitChanges();
                }
            }
            return 0;
        }
    }
}


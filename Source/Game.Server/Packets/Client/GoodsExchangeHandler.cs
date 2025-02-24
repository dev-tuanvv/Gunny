namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [PacketHandler(0x1f, "场景用户离开")]
    public class GoodsExchangeHandler : IPacketHandler
    {
        private List<ActiveConvertItemInfo> GetActiveConvertItem(ActiveBussiness db, int id, int index, int lengh)
        {
            ActiveConvertItemInfo[] singleActiveConvertItems = db.GetSingleActiveConvertItems(id);
            List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
            foreach (ActiveConvertItemInfo info in singleActiveConvertItems)
            {
                if (info.ItemType == this.GetGoodsAward(index))
                {
                    for (int i = 0; i < lengh; i++)
                    {
                        list.Add(info);
                    }
                }
            }
            return list;
        }

        private int GetGoodsAward(int index)
        {
            switch (index)
            {
                case 1:
                    return 3;

                case 2:
                    return 5;

                case 3:
                    return 7;
            }
            return 1;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            using (ActiveBussiness bussiness = new ActiveBussiness())
            {
                int activeID = packet.ReadInt();
                packet.ReadInt();
                int lengh = packet.ReadInt();
                int num3 = 0;
                int num4 = Convert.ToInt32(bussiness.GetSingleActives(activeID).GoodsExchangeNum);
                SqlDataProvider.Data.ItemInfo item = null;
                PlayerInventory inventory = null;
                while (num3 < lengh)
                {
                    int templateId = packet.ReadInt();
                    packet.ReadInt();
                    int num6 = packet.ReadInt();
                    inventory = client.Player.GetInventory((eBageType) num6);
                    item = inventory.GetItemByTemplateID(0, templateId);
                    int itemCount = inventory.GetItemCount(templateId);
                    if ((itemCount < num4) || (itemCount < 0))
                    {
                        client.Out.SendMessage(eMessageType.Normal, "vật phẩm kh\x00f4ng đủ!");
                        break;
                    }
                    num3++;
                }
                int index = packet.ReadInt();
                StringBuilder builder = new StringBuilder();
                List<ActiveConvertItemInfo> list = this.GetActiveConvertItem(bussiness, activeID, index, lengh);
                foreach (ActiveConvertItemInfo info3 in list)
                {
                    ItemTemplateInfo goods = ItemMgr.FindItemTemplate(info3.TemplateID);
                    if (goods == null)
                    {
                        client.Out.SendMessage(eMessageType.Normal, "Lỗi phần thưởng li\x00ean hệ BQT!");
                        break;
                    }
                    SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x66);
                    cloneItem.IsBinds = info3.IsBind;
                    client.Player.AddTemplate(cloneItem, cloneItem.Template.BagType, info3.ItemCount, eGameView.RouletteTypeGet);
                    builder.Append(goods.Name + " x" + info3.ItemCount.ToString() + ". ");
                }
                inventory.RemoveCountFromStack(item, num4 * lengh);
                if (builder.Length > 0)
                {
                    client.Out.SendMessage(eMessageType.Normal, builder.ToString());
                }
            }
            return 0;
        }
    }
}


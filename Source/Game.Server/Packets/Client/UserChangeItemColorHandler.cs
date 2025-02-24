namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0xb6, "改变物品颜色")]
    public class UserChangeItemColorHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eMessageType normal = eMessageType.Normal;
            string translateId = "UserChangeItemColorHandler.Success";
            packet.ReadInt();
            int slot = packet.ReadInt();
            packet.ReadInt();
            int num2 = packet.ReadInt();
            string str2 = packet.ReadString();
            string str3 = packet.ReadString();
            int templateId = packet.ReadInt();
            SqlDataProvider.Data.ItemInfo itemAt = client.Player.EquipBag.GetItemAt(num2);
            SqlDataProvider.Data.ItemInfo item = client.Player.PropBag.GetItemAt(slot);
            if (itemAt != null)
            {
                client.Player.BeginChanges();
                try
                {
                    bool flag = false;
                    if ((item != null) && item.IsValidItem())
                    {
                        client.Player.PropBag.RemoveItem(item);
                        flag = true;
                    }
                    else
                    {
                        ItemMgr.FindItemTemplate(templateId);
                        List<ShopItemInfo> list = ShopMgr.FindShopbyTemplatID(templateId);
                        int num4 = 0;
                        for (int i = 0; i < list.Count; i++)
                        {
                            if ((list[i].APrice1 == -1) && (list[i].AValue1 != 0))
                            {
                                num4 = list[i].AValue1;
                            }
                        }
                        if (num4 <= client.Player.PlayerCharacter.Money)
                        {
                            client.Player.RemoveMoney(num4);
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        itemAt.Color = (str2 == null) ? "" : str2;
                        itemAt.Skin = (str3 == null) ? "" : str3;
                        client.Player.EquipBag.UpdateItem(itemAt);
                    }
                }
                finally
                {
                    client.Player.CommitChanges();
                }
            }
            client.Out.SendMessage(normal, LanguageMgr.GetTranslation(translateId, new object[0]));
            return 0;
        }
    }
}


﻿namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xe8, "打开物品")]
    public class CaddyClearAllHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            PlayerInventory caddyBag = client.Player.CaddyBag;
            int num = 1;
            int num2 = 0;
            int num3 = 0;
            string translation = "";
            string str2 = "";
            for (int i = 0; i < caddyBag.Capalility; i++)
            {
                SqlDataProvider.Data.ItemInfo itemAt = caddyBag.GetItemAt(i);
                if (itemAt != null)
                {
                    if (itemAt.Template.ReclaimType == 1)
                    {
                        num2 += num * itemAt.Template.ReclaimValue;
                    }
                    if (itemAt.Template.ReclaimType == 2)
                    {
                        num3 += num * itemAt.Template.ReclaimValue;
                    }
                    caddyBag.RemoveItem(itemAt);
                }
            }
            if (num2 > 0)
            {
                translation = LanguageMgr.GetTranslation("ItemReclaimHandler.Success2", new object[] { num2 });
            }
            if (num3 > 0)
            {
                str2 = LanguageMgr.GetTranslation("ItemReclaimHandler.Success1", new object[] { num3 });
            }
            client.Player.BeginChanges();
            client.Player.AddGold(num2);
            client.Player.AddGiftToken(num3);
            client.Player.CommitChanges();
            client.Out.SendMessage(eMessageType.Normal, translation + " " + str2);
            return 1;
        }
    }
}


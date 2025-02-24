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

    [PacketHandler(0x7e, "场景用户离开")]
    public class QuickBuyGoldBoxHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadInt();
            packet.ReadBoolean();
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 1;
            }
            ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(0x1123e5);
            ItemTemplateInfo info2 = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
            int num2 = num * shopItemInfoById.AValue1;
            if (client.Player.MoneyDirect(num2))
            {
                int point = 0;
                int gold = 0;
                int giftToken = 0;
                int medal = 0;
                int exp = 0;
                int honor = 0;
                int hardCurrency = 0;
                int leagueMoney = 0;
                int useableScore = 0;
                int prestge = 0;
                int magicStonePoint = 0;
                List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                ItemBoxMgr.CreateItemBox(info2.TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref magicStonePoint);
                int num14 = num * gold;
                client.Player.AddGold(num14);
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bạn nhận được " + num14 + " v\x00e0ng.", new object[0]));
            }
            return 0;
        }
    }
}


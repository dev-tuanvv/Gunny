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
    using System.Linq;

    [PacketHandler(0x9c, "客户端日记")]
    public class BuyTransnationalGoodsHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int iD = packet.ReadInt();
            PyramidInfo pyramid = client.Player.Actives.Pyramid;
            int gold = 0;
            int money = 0;
            int offer = 0;
            int gifttoken = 0;
            int medal = 0;
            int damageScore = 0;
            int petScore = 0;
            int iTemplateID = 0;
            int iCount = 0;
            int hardCurrency = 0;
            int leagueMoney = 0;
            int useableScore = 0;
            eMessageType normal = eMessageType.Normal;
            string translateId = "UserBuyItemHandler.Success";
            ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
            bool flag = false;
            if (((shopItemInfoById != null) && ShopMgr.IsOnShop(shopItemInfoById.ID)) && (shopItemInfoById.ShopID == 0x62))
            {
                flag = true;
            }
            if (!flag)
            {
                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission", new object[0]));
                return 1;
            }
            Dictionary<int, SqlDataProvider.Data.ItemInfo> dictionary = new Dictionary<int, SqlDataProvider.Data.ItemInfo>();
            SqlDataProvider.Data.ItemInfo info4 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID), 1, 0x66);
            if (shopItemInfoById.BuyType == 0)
            {
                info4.ValidDate = shopItemInfoById.AUnit;
            }
            else
            {
                info4.Count = shopItemInfoById.AUnit;
            }
            info4.IsBinds = true;
            if (!dictionary.Keys.Contains<int>(info4.TemplateID))
            {
                dictionary.Add(info4.TemplateID, info4);
            }
            else
            {
                SqlDataProvider.Data.ItemInfo local1 = dictionary[info4.TemplateID];
                local1.Count += info4.Count;
            }
            ShopMgr.SetItemType(shopItemInfoById, 1, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref leagueMoney, ref useableScore);
            if (dictionary.Values.Count == 0)
            {
                return 1;
            }
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 1;
            }
            if (pyramid.totalPoint < damageScore)
            {
                client.Player.SendMessage("T\x00edch lũy kh\x00f4ng đủ.");
                return 0;
            }
            pyramid.totalPoint -= damageScore;
            if (true)
            {
                string str2 = "";
                foreach (SqlDataProvider.Data.ItemInfo info5 in dictionary.Values)
                {
                    str2 = str2 + ((str2 == "") ? info5.TemplateID.ToString() : ("," + info5.TemplateID.ToString()));
                    if (info4.Template.MaxCount == 1)
                    {
                        for (int i = 0; i < info4.Count; i++)
                        {
                            SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info4.Template, info4);
                            cloneItem.Count = 1;
                            client.Player.AddTemplate(cloneItem);
                        }
                    }
                    else
                    {
                        int num15 = 0;
                        for (int j = 0; j < info4.Count; j++)
                        {
                            if (num15 == info4.Template.MaxCount)
                            {
                                SqlDataProvider.Data.ItemInfo info7 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info4.Template, info4);
                                info7.Count = num15;
                                client.Player.AddTemplate(info7);
                                num15 = 0;
                            }
                            num15++;
                        }
                        if (num15 > 0)
                        {
                            SqlDataProvider.Data.ItemInfo info8 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info4.Template, info4);
                            info8.Count = num15;
                            client.Player.AddTemplate(info8);
                        }
                    }
                }
            }
            else
            {
                normal = eMessageType.ERROR;
                translateId = "UserBuyItemHandler.FailByPermission";
            }
            client.Out.SendMessage(normal, LanguageMgr.GetTranslation(translateId, new object[0]));
            GSPacketIn pkg = new GSPacketIn(0x91, client.Player.PlayerCharacter.ID);
            pkg.WriteByte(2);
            pkg.WriteBoolean(pyramid.isPyramidStart);
            pkg.WriteInt(pyramid.totalPoint);
            pkg.WriteInt(pyramid.turnPoint);
            pkg.WriteInt(pyramid.pointRatio);
            pkg.WriteInt(pyramid.currentLayer);
            client.Player.SendTCP(pkg);
            return 0;
        }
    }
}


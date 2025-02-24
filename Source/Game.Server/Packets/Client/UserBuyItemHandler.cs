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

    [PacketHandler(0x2c, "购买物品")]
    public class UserBuyItemHandler : IPacketHandler
    {
        public static int countConnect;

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num6;
            int num13;
            if (countConnect >= 0xbb8)
            {
                client.Disconnect();
                return 0;
            }
            int gold = 0;
            int money = 0;
            int offer = 0;
            int gifttoken = 0;
            StringBuilder builder = new StringBuilder();
            eMessageType normal = eMessageType.Normal;
            string translateId = "UserBuyItemHandler.Success";
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            List<int> list2 = new List<int>();
            List<bool> list3 = new List<bool>();
            List<int> list4 = new List<int>();
            StringBuilder builder2 = new StringBuilder();
            bool isBinds = false;
            ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
            int num5 = packet.ReadInt();
            for (num6 = 0; num6 < num5; num6++)
            {
                int iD = packet.ReadInt();
                int num8 = packet.ReadInt();
                string str2 = packet.ReadString();
                bool item = packet.ReadBoolean();
                string str3 = packet.ReadString();
                int num9 = packet.ReadInt();
                ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
                if (!ShopMgr.CheckInShopGoodsCanBuy(iD))
                {
                    client.Out.SendMessage(eMessageType.Normal, "Bạn kh\x00f4ng thể mua vật phẩm n\x00e0y.");
                    return 1;
                }
                if (!((shopItemInfoById.ShopID != 2) && ShopMgr.CanBuy(shopItemInfoById.ShopID, (info == null) ? 1 : info.ShopLevel, ref isBinds, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.Riches)))
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission", new object[0]));
                    return 1;
                }
                if (shopItemInfoById != null)
                {
                    SqlDataProvider.Data.ItemInfo info3 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID), 1, 0x66);
                    if (shopItemInfoById.BuyType == 0)
                    {
                        if (1 == num8)
                        {
                            info3.ValidDate = shopItemInfoById.AUnit;
                        }
                        if (2 == num8)
                        {
                            info3.ValidDate = shopItemInfoById.BUnit;
                        }
                        if (3 == num8)
                        {
                            info3.ValidDate = shopItemInfoById.CUnit;
                        }
                    }
                    else
                    {
                        if (1 == num8)
                        {
                            info3.Count = shopItemInfoById.AUnit;
                        }
                        if (2 == num8)
                        {
                            info3.Count = shopItemInfoById.BUnit;
                        }
                        if (3 == num8)
                        {
                            info3.Count = shopItemInfoById.CUnit;
                        }
                    }
                    if ((info3 != null) || (shopItemInfoById != null))
                    {
                        info3.Color = (str2 == null) ? "" : str2;
                        info3.Skin = (str3 == null) ? "" : str3;
                        if (isBinds)
                        {
                            info3.IsBinds = true;
                        }
                        else
                        {
                            info3.IsBinds = Convert.ToBoolean(shopItemInfoById.IsBind);
                        }
                        builder2.Append(num8);
                        builder2.Append(",");
                        list.Add(info3);
                        list3.Add(item);
                        list4.Add(num9);
                        foreach (int num10 in SqlDataProvider.Data.ItemInfo.SetItemType(shopItemInfoById, num8, ref gold, ref money, ref offer, ref gifttoken))
                        {
                            list2.Add(num10);
                        }
                    }
                }
            }
            if (list.Count == 0)
            {
                return 1;
            }
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 1;
            }
            int count = client.Player.EquipBag.GetItems().Count;
            string str4 = client.Player.EquipBag.GetItems().ToString();
            PlayerInventory propBag = client.Player.PropBag;
            int templateID = 0;
            if (propBag.GetItemAt(0x30) != null)
            {
                templateID = propBag.GetItemAt(0x30).TemplateID;
            }
            bool flag4 = true;
            if (templateID == 0x2c90)
            {
                normal = eMessageType.ERROR;
                translateId = "C\x00f3 lỗi, kh\x00f4ng thể mua vật phẩm, vui l\x00f2ng di chuyển huy chương trong t\x00fai qua vị tr\x00ed kh\x00e1c!";
                client.Out.SendMessage(normal, LanguageMgr.GetTranslation(translateId, new object[0]));
                return 1;
            }
            for (num13 = 0; num13 < list2.Count; num13 += 2)
            {
                if (client.Player.GetItemCount(0x2c90) < (list2.Count / 2))
                {
                    flag4 = false;
                }
                if (client.Player.GetItemCount(list2[num13]) < list2[num13 + 1])
                {
                    flag4 = false;
                }
            }
            if (!flag4)
            {
                normal = eMessageType.ERROR;
                translateId = "UserBuyItemHandler.NoBuyItem";
                client.Out.SendMessage(normal, LanguageMgr.GetTranslation(translateId, new object[0]));
                return 1;
            }
            if (((gold <= client.Player.PlayerCharacter.Gold) && (money <= client.Player.PlayerCharacter.Money)) && ((offer <= client.Player.PlayerCharacter.Offer) && (gifttoken <= client.Player.PlayerCharacter.GiftToken)))
            {
                PlayerBussiness bussiness;
                client.Player.RemoveMoney(money);
                client.Player.RemoveGold(gold);
                client.Player.RemoveOffer(offer);
                client.Player.RemoveGiftToken(gifttoken);
                for (num13 = 0; num13 < list2.Count; num13 += 2)
                {
                    client.Player.RemoveTemplate(list2[num13], list2[num13 + 1]);
                    builder.Append(list2[num13].ToString() + ":");
                }
                string str5 = "";
                int num15 = 0;
                MailInfo mail = new MailInfo();
                StringBuilder builder3 = new StringBuilder();
                builder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark", new object[0]));
                for (num6 = 0; num6 < list.Count; num6++)
                {
                    str5 = str5 + ((str5 == "") ? list[num6].TemplateID.ToString() : ("," + list[num6].TemplateID.ToString()));
                    if (client.Player.AddTemplate(list[num6], list[num6].Template.BagType, list[num6].Count, eGameView.OtherTypeGet))
                    {
                        if (list3[num6] && list[num6].CanEquip())
                        {
                            int toSlot = client.Player.EquipBag.FindItemEpuipSlot(list[num6].Template);
                            if (((toSlot == 9) || (toSlot == 10)) && ((list4[num6] == 9) || (list4[num6] == 10)))
                            {
                                toSlot = list4[num6];
                            }
                            else if (((toSlot == 7) || (toSlot == 8)) && ((list4[num6] == 7) || (list4[num6] == 8)))
                            {
                                toSlot = list4[num6];
                            }
                            client.Player.EquipBag.MoveItem(list[num6].Place, toSlot, 0);
                            translateId = "UserBuyItemHandler.Save";
                        }
                    }
                    else
                    {
                        using (bussiness = new PlayerBussiness())
                        {
                            list[num6].UserID = 0;
                            bussiness.AddGoods(list[num6]);
                            num15++;
                            builder3.Append(num15);
                            builder3.Append("、");
                            builder3.Append(list[num6].Template.Name);
                            builder3.Append("x");
                            builder3.Append(list[num6].Count);
                            builder3.Append(";");
                            switch (num15)
                            {
                                case 1:
                                    mail.Annex1 = list[num6].ItemID.ToString();
                                    mail.Annex1Name = list[num6].Template.Name;
                                    break;

                                case 2:
                                    mail.Annex2 = list[num6].ItemID.ToString();
                                    mail.Annex2Name = list[num6].Template.Name;
                                    break;

                                case 3:
                                    mail.Annex3 = list[num6].ItemID.ToString();
                                    mail.Annex3Name = list[num6].Template.Name;
                                    break;

                                case 4:
                                    mail.Annex4 = list[num6].ItemID.ToString();
                                    mail.Annex4Name = list[num6].Template.Name;
                                    break;

                                case 5:
                                    mail.Annex5 = list[num6].ItemID.ToString();
                                    mail.Annex5Name = list[num6].Template.Name;
                                    break;
                            }
                            if (num15 == 5)
                            {
                                num15 = 0;
                                mail.AnnexRemark = builder3.ToString();
                                builder3.Remove(0, builder3.Length);
                                builder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark", new object[0]));
                                mail.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title", new object[0]) + mail.Annex1Name + "]";
                                mail.Gold = 0;
                                mail.Money = 0;
                                mail.Receiver = client.Player.PlayerCharacter.NickName;
                                mail.ReceiverID = client.Player.PlayerCharacter.ID;
                                mail.Sender = mail.Receiver;
                                mail.SenderID = mail.ReceiverID;
                                mail.Title = mail.Content;
                                mail.Type = 8;
                                bussiness.SendMail(mail);
                                normal = eMessageType.ERROR;
                                translateId = "UserBuyItemHandler.Mail";
                                mail.Revert();
                            }
                        }
                    }
                }
                if (num15 > 0)
                {
                    using (bussiness = new PlayerBussiness())
                    {
                        mail.AnnexRemark = builder3.ToString();
                        mail.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title", new object[0]) + mail.Annex1Name + "]";
                        mail.Gold = 0;
                        mail.Money = 0;
                        mail.Receiver = client.Player.PlayerCharacter.NickName;
                        mail.ReceiverID = client.Player.PlayerCharacter.ID;
                        mail.Sender = mail.Receiver;
                        mail.SenderID = mail.ReceiverID;
                        mail.Title = mail.Content;
                        mail.Type = 8;
                        bussiness.SendMail(mail);
                        normal = eMessageType.ERROR;
                        translateId = "UserBuyItemHandler.Mail";
                    }
                }
                if (normal == eMessageType.ERROR)
                {
                    client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                }
                client.Player.OnPaid(money, gold, offer, gifttoken, 0, builder.ToString());
            }
            else
            {
                if (gold > client.Player.PlayerCharacter.Gold)
                {
                    translateId = "UserBuyItemHandler.NoGold";
                }
                if (offer > client.Player.PlayerCharacter.Offer)
                {
                    translateId = "UserBuyItemHandler.NoOffer";
                }
                if (gifttoken > client.Player.PlayerCharacter.GiftToken)
                {
                    translateId = "UserBuyItemHandler.GiftToken";
                }
                normal = eMessageType.ERROR;
            }
            client.Out.SendMessage(normal, LanguageMgr.GetTranslation(translateId, new object[0]));
            return 0;
        }
    }
}


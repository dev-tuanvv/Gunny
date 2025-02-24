namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [PacketHandler(0xd8, "防沉迷系统开关")]
    internal class CardDataHander : IPacketHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static ThreadSafeRandom random = new ThreadSafeRandom();

        private string cardQuality(int Type)
        {
            if (Type == 2)
            {
                return "Bạc";
            }
            if (Type == 1)
            {
                return "V\x00e0ng";
            }
            if (Type != 4)
            {
                return "Đồng";
            }
            return "Bạch kim";
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num6;
            UsersCardInfo info6;
            SqlDataProvider.Data.ItemInfo info9;
            int num = packet.ReadInt();
            PlayerInfo playerCharacter = client.Player.PlayerCharacter;
            CardInventory cardBag = client.Player.CardBag;
            List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
            switch (num)
            {
                case 0:
                {
                    int slot = packet.ReadInt();
                    int toSlot = packet.ReadInt();
                    UsersCardInfo itemAt = cardBag.GetItemAt(slot);
                    if (toSlot <= 4)
                    {
                        if (itemAt == null)
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Kh\x00f4ng t\x00ecm thấy thẻ n\x00e0y, thử lại sau.");
                            return 0;
                        }
                        ItemTemplateInfo info3 = ItemMgr.FindItemTemplate(itemAt.TemplateID);
                        if ((info3.Property8 == 0) && (toSlot == 0))
                        {
                            return 0;
                        }
                        if (((info3.Property8 == 1) && (toSlot > 0)) && (toSlot <= 4))
                        {
                            return 0;
                        }
                        if (slot == toSlot)
                        {
                            cardBag.MoveCard(slot, toSlot);
                            client.Player.CardBag.UpdateChangedPlaces();
                            client.Player.EquipBag.UpdatePlayerProperties();
                        }
                        else
                        {
                            string str;
                            if (cardBag.FindEquipCard(itemAt.TemplateID))
                            {
                                str = "Thẻ n\x00e0y đ\x00e3 trang bị!";
                            }
                            else
                            {
                                cardBag.MoveCard(slot, toSlot);
                                client.Player.CardBag.UpdateChangedPlaces();
                                client.Player.EquipBag.UpdatePlayerProperties();
                                str = "Trang bị th\x00e0nh c\x00f4ng!";
                            }
                            client.Out.SendMessage(eMessageType.Normal, str);
                        }
                        goto Label_0871;
                    }
                    return 0;
                }
                case 1:
                    num6 = packet.ReadInt();
                    info6 = new UsersCardInfo {
                        Count = -1,
                        UserID = client.Player.PlayerCharacter.ID,
                        Place = num6,
                        TemplateID = 0x4caf5,
                        isFirstGet = true,
                        Damage = 0,
                        Guard = 0,
                        Attack = 0,
                        Defence = 0,
                        Luck = 0,
                        Agility = 0
                    };
                    client.Player.CardBag.AddCardTo(info6, num6);
                    goto Label_0871;

                case 2:
                {
                    int num23 = packet.ReadInt();
                    int num24 = packet.ReadInt();
                    List<UsersCardInfo> cards = cardBag.GetCards(5, cardBag.Capalility);
                    info9 = client.Player.EquipBag.GetItemAt(num23);
                    if (info9.Count <= 1)
                    {
                        client.Player.EquipBag.RemoveItem(info9);
                        break;
                    }
                    info9.Count--;
                    client.Player.EquipBag.UpdateItem(info9);
                    break;
                }
                case 3:
                {
                    int num4 = packet.ReadInt();
                    UsersCardInfo info4 = cardBag.GetItemAt(num4);
                    if (info4 != null)
                    {
                        if (info4.Count < 3)
                        {
                            client.Out.SendMessage(eMessageType.ALERT, "Số thẻ b\x00e0i cần lớn hơn 3");
                        }
                        else
                        {
                            info4.Count -= 3;
                            int num5 = random.Next(2, 5);
                            info4.CardGP += num5;
                            info4.Level = CardMgr.FindLv(info4.CardGP);
                            UsersCardInfo equipCard = cardBag.GetEquipCard(info4.TemplateID);
                            if (equipCard == null)
                            {
                                int[] updatedSlots = new int[] { num4 };
                                client.Out.SendPlayerCardInfo(cardBag, updatedSlots);
                            }
                            else
                            {
                                equipCard.Level = info4.Level;
                                client.Player.EquipBag.UpdatePlayerProperties();
                                int[] numArray = new int[] { num4, equipCard.Place };
                                client.Out.SendPlayerCardInfo(cardBag, numArray);
                            }
                        }
                    }
                    goto Label_0871;
                }
                case 4:
                {
                    num6 = packet.ReadInt();
                    int count = packet.ReadInt();
                    SqlDataProvider.Data.ItemInfo info7 = client.Player.EquipBag.GetItemAt(num6);
                    if (num == 4)
                    {
                        info7 = client.Player.PropBag.GetItemAt(num6);
                    }
                    if (info7 == null)
                    {
                        return 0;
                    }
                    if ((info7.Count < count) || (count < 1))
                    {
                        return 0;
                    }
                    if (!info7.IsCard())
                    {
                        client.Out.SendMessage(eMessageType.Normal, "Hộp thẻ b\x00e0i lỗi!");
                        return 0;
                    }
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
                    bool flag = false;
                    int num19 = 0;
                    int num20 = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (ItemBoxMgr.CreateItemBox(info7.TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref magicStonePoint))
                        {
                            int num22 = random.Next(itemInfos.Count);
                            SqlDataProvider.Data.ItemInfo info8 = itemInfos[num22];
                            flag = this.TakeCard(client, info8.Template.Property5);
                            if (flag)
                            {
                                num19++;
                            }
                        }
                        if (!flag)
                        {
                            if (info7.TemplateID == 0x4eb6)
                            {
                                num20 += random.Next(50, 500);
                            }
                            else
                            {
                                num20 += random.Next(5, 0x19);
                            }
                        }
                    }
                    client.Player.RemoveTemplate(info7.TemplateID, count);
                    client.Player.AddCardSoul(num20);
                    client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, true, num20);
                    string message = string.Format("Bạn nhận được {0} điểm thẻ hồn,{1} Thẻ b\x00e0i.", num20, num19);
                    client.Out.SendMessage(eMessageType.Normal, message);
                    goto Label_0871;
                }
                case 5:
                {
                    int num27 = 50;
                    if (client.Player.MoneyDirect(num27))
                    {
                        if (client.Player.PlayerCharacter.GetSoulCount <= 0)
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Số lần sử dụng thẻ hồn h\x00f4m nay đ\x00e3 hết.");
                        }
                        else
                        {
                            int num28 = 50;
                            if (random.Next(0x3e8) < 100)
                            {
                                num28 = 120;
                            }
                            client.Player.AddCardSoul(num28);
                            PlayerInfo info1 = client.Player.PlayerCharacter;
                            info1.GetSoulCount--;
                            client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, true, num28);
                            client.Out.SendMessage(eMessageType.Normal, string.Format("Bạn nhận được {0} điểm thẻ hồn.", num28));
                        }
                    }
                    goto Label_0871;
                }
                default:
                    goto Label_0871;
            }
            cardBag.BeginChanges();
            try
            {
                CardUpdateInfo singleCardByTemplae = CardMgr.GetSingleCardByTemplae(info9.Template.Property5);
                UsersCardInfo itemByTemplateID = cardBag.GetItemByTemplateID(5, info9.Template.Property5);
                int place = client.Player.CardBag.FindFirstEmptySlot(5);
                int num26 = random.Next(1, 3);
                if (itemByTemplateID == null)
                {
                    info6 = new UsersCardInfo {
                        Count = num26,
                        UserID = client.Player.PlayerCharacter.ID,
                        Place = place,
                        TemplateID = singleCardByTemplae.Id,
                        isFirstGet = true,
                        Damage = 0,
                        Guard = 0,
                        Attack = 0,
                        Defence = 0,
                        Luck = 0,
                        Agility = 0
                    };
                    client.Player.CardBag.AddCardTo(info6, place);
                }
                else
                {
                    info6 = itemByTemplateID;
                    info6.Count += num26;
                }
                client.Out.SendGetCard(client.Player.PlayerCharacter, info6);
                return 0;
            }
            catch (Exception exception)
            {
                log.ErrorFormat("Arrage bag errror,user id:{0}   msg:{1}", client.Player.PlayerId, exception.Message);
            }
            cardBag.CommitChanges();
        Label_0871:
            return 0;
        }

        private bool TakeCard(GameClient client, int templateId)
        {
            int num = client.Player.CardBag.FindFirstEmptySlot(5);
            CardUpdateInfo card = CardMgr.GetCard(templateId);
            if (card != null)
            {
                UsersCardInfo itemAt;
                int slot = client.Player.CardBag.FindPlaceByTamplateId(5, templateId);
                if (slot == -1)
                {
                    itemAt = new UsersCardInfo {
                        Count = 1,
                        UserID = client.Player.PlayerCharacter.ID,
                        Place = num,
                        TemplateID = card.Id,
                        isFirstGet = true,
                        Attack = 0,
                        Agility = 0,
                        Defence = 0,
                        Luck = 0,
                        Damage = 0,
                        Guard = 0
                    };
                }
                else
                {
                    itemAt = client.Player.CardBag.GetItemAt(slot);
                }
                if (itemAt != null)
                {
                    client.Out.SendGetCard(client.Player.PlayerCharacter, itemAt);
                }
            }
            return false;
        }
    }
}


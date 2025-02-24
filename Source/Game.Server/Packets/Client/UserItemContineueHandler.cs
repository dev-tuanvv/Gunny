namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Text;

    [PacketHandler(0x3e, "续费")]
    public class UserItemContineueHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            new StringBuilder();
            int num = packet.ReadInt();
            string translateId = "UserItemContineueHandler.Success";
            for (int i = 0; i < num; i++)
            {
                eBageType bagType = (eBageType) packet.ReadByte();
                int place = packet.ReadInt();
                int iD = packet.ReadInt();
                int type = packet.ReadByte();
                bool flag = packet.ReadBoolean();
                packet.ReadInt();
                ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
                SqlDataProvider.Data.ItemInfo itemAt = client.Player.GetItemAt(bagType, place);
                if ((((bagType == eBageType.EquipBag) && (itemAt != null)) && (shopItemInfoById != null)) && (shopItemInfoById.TemplateID == itemAt.TemplateID))
                {
                    if (itemAt.ValidDate != 0)
                    {
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
                        int validDate = itemAt.ValidDate;
                        DateTime beginDate = itemAt.BeginDate;
                        int count = itemAt.Count;
                        bool flag2 = itemAt.IsValidItem();
                        if (!ShopMgr.SetItemType(shopItemInfoById, type, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref leagueMoney, ref medal))
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Vật phẩm kh\x00f4ng thể tiếp ph\x00ed.", new object[0]));
                            return 0;
                        }
                        if ((((gold <= client.Player.PlayerCharacter.Gold) && (money <= client.Player.PlayerCharacter.Money)) && (offer <= client.Player.PlayerCharacter.Offer)) && (gifttoken <= client.Player.PlayerCharacter.GiftToken))
                        {
                            if (!flag2)
                            {
                                if (1 == type)
                                {
                                    itemAt.ValidDate = shopItemInfoById.AUnit;
                                }
                                if (2 == type)
                                {
                                    itemAt.ValidDate = shopItemInfoById.BUnit;
                                }
                                if (3 == type)
                                {
                                    itemAt.ValidDate = shopItemInfoById.CUnit;
                                }
                                itemAt.BeginDate = DateTime.Now;
                                itemAt.IsUsed = true;
                                client.Player.RemoveMoney(money);
                                client.Player.RemoveGold(gold);
                                client.Player.RemoveOffer(offer);
                                client.Player.RemoveGiftToken(gifttoken);
                            }
                            else
                            {
                                translateId = "Vật phẩm chưa hết hạn!";
                            }
                        }
                        else
                        {
                            itemAt.ValidDate = validDate;
                            itemAt.Count = count;
                            translateId = "UserItemContineueHandler.NoMoney";
                        }
                    }
                    if (flag)
                    {
                        int num19 = client.Player.EquipBag.FindItemEpuipSlot(itemAt.Template);
                        if ((client.Player.GetItemAt(bagType, num19) == null) && (place > client.Player.EquipBag.BeginSlot))
                        {
                            client.Player.EquipBag.MoveItem(place, num19, 1);
                        }
                    }
                    else
                    {
                        client.Player.EquipBag.UpdateItem(itemAt);
                    }
                }
            }
            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
            return 0;
        }
    }
}


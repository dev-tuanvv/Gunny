namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using System;
    using System.Collections.Generic;
    using SqlDataProvider.Data;

    [PacketHandler(170, "场景用户离开")]
    public class CardSlotHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadInt();
            string message = "";
            List<UsersCardInfo> cards = client.Player.CardBag.GetCards(0, 5);
            switch (num)
            {
                case 0:
                {
                    int place = packet.ReadInt();
                    int soulPoint = packet.ReadInt();
                    if ((soulPoint > 0) && (soulPoint <= client.Player.PlayerCharacter.CardSoul))
                    {
                        int count = cards[place].Count;
                        int gP = cards[place].CardGP + soulPoint;
                        int level = CardMgr.GetLevel(gP, count);
                        int num7 = CardMgr.GetGP(level, count) - cards[place].CardGP;
                        if (level == CardMgr.MaxLv(count))
                        {
                            soulPoint = num7;
                        }
                        client.Player.CardBag.UpGraceSlot(soulPoint, level, place);
                        client.Player.RemoveCardSoul(soulPoint);
                        client.Player.Out.SendPlayerCardSlot(client.Player.PlayerCharacter, cards[place]);
                        client.Player.EquipBag.UpdatePlayerProperties();
                    }
                    break;
                }
                case 1:
                {
                    packet.ReadBoolean();
                    if (!client.Player.MoneyDirect(300))
                    {
                        message = "Xu kh\x00f4ng đủ!";
                        break;
                    }
                    int num8 = 0;
                    for (int i = 0; i < cards.Count; i++)
                    {
                        num8 += cards[i].CardGP;
                    }
                    client.Player.CardBag.ResetCardSoul();
                    client.Player.AddCardSoul(num8);
                    message = LanguageMgr.GetTranslation("UpdateSLOT.ResetComplete", new object[] { num8 });
                    client.Player.Out.SendPlayerCardSlot(client.Player.PlayerCharacter, cards);
                    client.Player.EquipBag.UpdatePlayerProperties();
                    break;
                }
            }
            if (message != "")
            {
                client.Out.SendMessage(eMessageType.Normal, message);
            }
            client.Player.CardBag.SaveToDatabase();
            return 0;
        }
    }
}


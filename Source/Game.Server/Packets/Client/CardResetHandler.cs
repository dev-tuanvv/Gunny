namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0xc4, "场景用户离开")]
    public class CardResetHandler : IPacketHandler
    {
        public static ThreadSafeRandom random = new ThreadSafeRandom();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num5;
            int num = packet.ReadInt();
            int place = packet.ReadInt();
            UsersCardInfo itemByPlace = client.Player.CardBag.GetItemByPlace(0, place);
            if (itemByPlace == null)
            {
                client.Out.SendMessage(eMessageType.Normal, "Xảy ra lổi, chuyển k\x00eanh v\x00e0 thử lại.");
                return 0;
            }
            List<int> points = new List<int>();
            string message = "Tẩy điểm th\x00e0nh c\x00f4ng!";
            int minValue = 1;
            int maxValue = 10;
            switch (num)
            {
                case 0:
                    num5 = 0;
                    goto Label_00E4;

                case 1:
                {
                    int[] numArray;
                    message = "Cập nhật thay đổi th\x00e0nh c\x00f4ng!";
                    UsersCardInfo equipCard = client.Player.CardBag.GetEquipCard(itemByPlace.TemplateID);
                    if (equipCard == null)
                    {
                        numArray = new int[] { itemByPlace.Place };
                        client.Out.SendPlayerCardInfo(client.Player.CardBag, numArray);
                    }
                    else
                    {
                        equipCard.Attack = itemByPlace.Attack;
                        equipCard.Defence = itemByPlace.Defence;
                        equipCard.Agility = itemByPlace.Agility;
                        equipCard.Luck = itemByPlace.Luck;
                        client.Player.EquipBag.UpdatePlayerProperties();
                        numArray = new int[] { itemByPlace.Place, equipCard.Place };
                        client.Out.SendPlayerCardInfo(client.Player.CardBag, numArray);
                        client.Player.EquipBag.UpdatePlayerProperties();
                    }
                    client.Player.CardBag.SaveToDatabase();
                    goto Label_0258;
                }
                default:
                    goto Label_0258;
            }
        Label_00DD:
            num5++;
        Label_00E4:
            if (num5 < 4)
            {
                int item = random.Next(minValue, maxValue);
                points.Add(item);
                switch (num5)
                {
                    case 0:
                        itemByPlace.Attack = item;
                        break;

                    case 1:
                        itemByPlace.Defence = item;
                        break;

                    case 2:
                        itemByPlace.Agility = item;
                        break;

                    case 3:
                        itemByPlace.Luck = item;
                        break;
                }
                goto Label_00DD;
            }
            itemByPlace.Count -= 3;
            client.Player.CardBag.UpdateTempCard(itemByPlace);
            client.Player.Out.SendGetCard(client.Player.PlayerCharacter, itemByPlace);
            client.Player.Out.SendPlayerCardReset(client.Player.PlayerCharacter, points);
        Label_0258:
            client.Out.SendMessage(eMessageType.Normal, message);
            return 0;
        }
    }
}


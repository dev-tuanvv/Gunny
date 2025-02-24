namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0x12, "场景用户离开")]
    public class CardInfoHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            PlayerInfo playerCharacter;
            List<UsersCardInfo> cards;
            int playerId = packet.ReadInt();
            GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
            if (playerById != null)
            {
                playerCharacter = playerById.PlayerCharacter;
                cards = playerById.CardBag.GetCards(0, 5);
            }
            else
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    playerCharacter = bussiness.GetUserSingleByUserID(playerId);
                    cards = bussiness.GetUserCardEuqip(playerId);
                }
            }
            if ((cards != null) && (playerCharacter != null))
            {
                client.Player.Out.SendPlayerCardSlot(playerCharacter, cards);
                client.Player.Out.SendPlayerCardEquip(playerCharacter, cards);
            }
            return 0;
        }
    }
}


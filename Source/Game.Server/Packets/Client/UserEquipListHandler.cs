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

    [PacketHandler(0x4a, "获取用户装备")]
    public class UserEquipListHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GamePlayer playerById;
            PlayerInfo playerCharacter;
            List<SqlDataProvider.Data.ItemInfo> items;
            List<SqlDataProvider.Data.ItemInfo> userBeadEuqip;
            List<SqlDataProvider.Data.ItemInfo> userMagicStoneEuqip;
            List<UserGemStone> gemStone;
            bool flag = packet.ReadBoolean();
            int playerId = 0;
            if (flag)
            {
                playerId = packet.ReadInt();
                playerById = WorldMgr.GetPlayerById(playerId);
            }
            else
            {
                playerById = WorldMgr.GetClientByPlayerNickName(packet.ReadString());
            }
            if (playerById != null)
            {
                playerCharacter = playerById.PlayerCharacter;
                items = playerById.EquipBag.GetItems(0, 0x1f);
                userBeadEuqip = playerById.BeadBag.GetItems(0, 0x1f);
                userMagicStoneEuqip = playerById.MagicStoneBag.GetItems(0, 30);
                gemStone = playerById.GemStone;
            }
            else
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    playerCharacter = bussiness.GetUserSingleByUserID(playerId);
                    if (playerCharacter == null)
                    {
                        return 0;
                    }
                    items = bussiness.GetUserEuqip(playerId);
                    userBeadEuqip = bussiness.GetUserBeadEuqip(playerId);
                    userMagicStoneEuqip = bussiness.GetUserMagicStoneEuqip(playerId);
                    playerCharacter.Texp = bussiness.GetUserTexpInfoSingle(playerId);
                    gemStone = bussiness.GetSingleGemStones(playerId);
                }
            }
            if ((((playerCharacter != null) && (items != null)) && ((userBeadEuqip != null) && (gemStone != null))) && (userMagicStoneEuqip != null))
            {
                client.Player.Out.SendUserEquip(playerCharacter, items, gemStone, userBeadEuqip, userMagicStoneEuqip);
                if (playerCharacter.ID != client.Player.PlayerCharacter.ID)
                {
                    client.Player.PlayerProp.ViewOther(playerCharacter);
                }
            }
            return 0;
        }
    }
}


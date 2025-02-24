namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x92, "场景用户离开")]
    public class CampBattleHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int iD = client.Player.PlayerCharacter.ID;
            BaseCampBattleRoom campBattleRoom = RoomMgr.CampBattleRoom;
            switch (num)
            {
                case 2:
                {
                    int posX = packet.ReadInt();
                    int posY = packet.ReadInt();
                    int zoneId = packet.ReadInt();
                    int playerId = packet.ReadInt();
                    client.Player.X = posX;
                    client.Player.Y = posY;
                    campBattleRoom.PlayerMove(client.Player, posX, posY, zoneId, playerId);
                    return 0;
                }
                case 3:
                    campBattleRoom.RemovePlayer(client.Player);
                    return 0;

                case 5:
                    packet.ReadInt();
                    if (client.Player.MainWeapon == null)
                    {
                        client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
                        return 0;
                    }
                    campBattleRoom.SendCampFightMonster(client.Player);
                    return 0;

                case 6:
                    campBattleRoom.SendCampInitSecen(client.Player);
                    return 0;

                case 0x15:
                    campBattleRoom.SendPerScoreRank(client.Player);
                    return 0;
            }
            Console.WriteLine("CampBattleHandler." + ((CampPackageType) num));
            return 0;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using Game.Server.Rooms;
    using System;
    using System.Collections.Generic;
    using Game.Server.GameObjects;

    [PacketHandler(0x56, "任务完成")]
    public class QuestOneKeyFinishHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            BaseRoom currentRoom = client.Player.CurrentRoom;
            List<GamePlayer> players = client.Player.CurrentRoom.GetPlayers();
            if ((currentRoom != null) && (currentRoom.Host == client.Player))
            {
                if (client.Player.MainWeapon == null)
                {
                    client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
                    return 0;
                }
                if (!((currentRoom.RoomType != eRoomType.Dungeon) || client.Player.IsPvePermission(currentRoom.MapId, currentRoom.HardLevel)))
                {
                    client.Player.SendMessage("Kh\x00f4ng thể tham gia ph\x00f3 bản n\x00e0y!");
                    return 0;
                }
                RoomMgr.StartGame(client.Player.CurrentRoom);
            }
            return 0;
        }
    }
}


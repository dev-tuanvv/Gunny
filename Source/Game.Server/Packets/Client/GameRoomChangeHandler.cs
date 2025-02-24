namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x6b, "游戏创建")]
    public class GameRoomChangeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (!(((client.Player.CurrentRoom == null) || (client.Player != client.Player.CurrentRoom.Host)) || client.Player.CurrentRoom.IsPlaying))
            {
                int mapId = packet.ReadInt();
                eRoomType roomType = (eRoomType) packet.ReadByte();
                string roomname = packet.ReadString();
                string password = packet.ReadString();
                byte timeMode = packet.ReadByte();
                byte num3 = packet.ReadByte();
                int levelLimits = packet.ReadInt();
                bool isCrosszone = packet.ReadBoolean();
                RoomMgr.UpdateRoomGameType(client.Player.CurrentRoom, roomType, timeMode, (eHardLevel) num3, levelLimits, mapId, password, roomname, isCrosszone);
            }
            else if (!client.Player.CurrentRoom.IsPlaying)
            {
                client.Player.SendMessage("Lỗi kh\x00f4ng t\x00ecm thấy ph\x00f2ng chơi");
            }
            return 0;
        }
    }
}


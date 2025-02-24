namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x5e, "游戏创建")]
    public class GameRoomHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            byte timeType = packet.ReadByte();
            string name = packet.ReadString();
            string password = packet.ReadString();
            RoomMgr.CreateRoom(client.Player, name, password, (eRoomType) num, timeType);
            return 0;
        }
    }
}


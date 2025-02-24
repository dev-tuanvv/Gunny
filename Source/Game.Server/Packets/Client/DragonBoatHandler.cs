namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(100, "客户端日记")]
    public class DragonBoatHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if ((client.Player.CurrentRoom != null) && (client.Player == client.Player.CurrentRoom.Host))
            {
                byte pos = packet.ReadByte();
                int place = packet.ReadInt();
                bool isOpened = packet.ReadBoolean();
                int placeView = packet.ReadInt();
                RoomMgr.UpdateRoomPos(client.Player.CurrentRoom, pos, isOpened, place, placeView);
            }
            return 0;
        }
    }
}


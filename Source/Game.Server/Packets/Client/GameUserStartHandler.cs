namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x52, "游戏开始")]
    public class GameUserStartHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool flag = packet.ReadBoolean();
            BaseRoom currentRoom = client.Player.CurrentRoom;
            if (flag && (currentRoom != null))
            {
                RoomMgr.StartGameMission(currentRoom);
            }
            return 0;
        }
    }
}


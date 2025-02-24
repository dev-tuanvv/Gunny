namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x53, "游戏创建")]
    public class GameRoomOut : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom != null)
            {
                client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
            }
            return 0;
        }
    }
}


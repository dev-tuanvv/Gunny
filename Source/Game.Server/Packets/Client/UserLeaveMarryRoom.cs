namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xf4, "玩家退出礼堂")]
    public class UserLeaveMarryRoom : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.IsInMarryRoom)
            {
                client.Player.CurrentMarryRoom.RemovePlayer(client.Player);
            }
            return 0;
        }
    }
}


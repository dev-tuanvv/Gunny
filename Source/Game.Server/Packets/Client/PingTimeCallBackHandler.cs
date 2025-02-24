namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(4, "测试网络")]
    public class PingTimeCallBackHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            client.Player.PingTime = DateTime.Now.Ticks - client.Player.PingStart;
            return 0;
        }
    }
}


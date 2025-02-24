namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x1c, "打开物品")]
    public class LotteryFinishBoxHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            packet.ClearContext();
            client.SendTCP(pkg);
            client.Player.ClearCaddyBag();
            client.Lottery = -1;
            return 1;
        }
    }
}


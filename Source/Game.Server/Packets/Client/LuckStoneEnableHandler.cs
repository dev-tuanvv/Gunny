namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xa5, "场景用户离开")]
    public class LuckStoneEnableHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            client.Player.UpdateProperties();
            return 0;
        }
    }
}


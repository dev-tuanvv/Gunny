namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xa8, "场景用户离开")]
    public class UpdateGoodsCountHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            client.Player.UpdateGoodsCount();
            return 0;
        }
    }
}


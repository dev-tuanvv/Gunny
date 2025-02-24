namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xf9, "礼堂数据")]
    public class MarryDataHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentMarryRoom != null)
            {
                client.Player.CurrentMarryRoom.ProcessData(client.Player, packet);
            }
            return 0;
        }
    }
}


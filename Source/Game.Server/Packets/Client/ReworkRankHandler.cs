namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xbd, "场景用户离开")]
    public class ReworkRankHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string str = packet.ReadString();
            if (!string.IsNullOrEmpty(str))
            {
                client.Player.UpdateHonor(str);
            }
            return 0;
        }
    }
}


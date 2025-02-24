namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xcf, "场景用户离开")]
    public class RequestPayHander : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int iD = client.Player.PlayerCharacter.ID;
            return 0;
        }
    }
}


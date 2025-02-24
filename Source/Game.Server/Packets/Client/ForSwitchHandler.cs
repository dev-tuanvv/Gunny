namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xe1, "场景用户离开")]
    public class ForSwitchHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(0xe1, client.Player.PlayerCharacter.ID);
            client.SendTCP(pkg);
            return 0;
        }
    }
}


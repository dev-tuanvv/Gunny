namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(34, "场景用户离开")]
    public class ChangeDesignationHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool flag = packet.ReadBoolean();
            client.Player.PlayerCharacter.IsShowConsortia = flag;
            return 0;
        }
    }
}


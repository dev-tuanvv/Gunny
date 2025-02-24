namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using System;

    [PacketHandler(0x24, "用户同步动作")]
    public class UserSynchActionHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(packet.ClientID);
            if (playerById != null)
            {
                packet.Code = 0x23;
                packet.ClientID = client.Player.PlayerCharacter.ID;
                playerById.Out.SendTCP(packet);
            }
            return 1;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xdb, "场景用户离开")]
    public class UserWeeklyClickHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int iD = client.Player.PlayerCharacter.ID;
            GSPacketIn pkg = new GSPacketIn(0xdb, iD);
            if (DateTime.Now.Date != client.Player.PlayerCharacter.LastGetEgg.Date)
            {
                pkg.WriteBoolean(true);
            }
            else
            {
                pkg.WriteBoolean(false);
            }
            client.SendTCP(pkg);
            return 0;
        }
    }
}


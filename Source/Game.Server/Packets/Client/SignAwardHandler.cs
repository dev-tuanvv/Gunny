namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using System;

    [PacketHandler(90, "场景用户离开")]
    public class SignAwardHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int dailyLog = packet.ReadInt();
            string message = "Nhận thưởng t\x00edch lũy hằng ng\x00e0y th\x00e0nh c\x00f4ng!";
            if (AwardMgr.AddSignAwards(client.Player, dailyLog))
            {
                client.Out.SendMessage(eMessageType.Normal, message);
            }
            return 0;
        }
    }
}


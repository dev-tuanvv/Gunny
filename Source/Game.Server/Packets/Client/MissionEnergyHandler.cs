namespace Game.Server.Packets.Client
{
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x69, "场景用户离开")]
    public class MissionEnergyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ReadBoolean();
            PlayerExtra extra = client.Player.Extra;
            if (extra.Info.buyEnergyCount == 0)
            {
                extra.Info.buyEnergyCount = 1;
            }
            if (extra.Info.MissionEnergy <= 250)
            {
                MissionEnergyInfo missionEnergyInfo = MissionEnergyMgr.GetMissionEnergyInfo(extra.Info.buyEnergyCount);
                if (missionEnergyInfo != null)
                {
                    client.Player.AddMissionEnergy(missionEnergyInfo.Energy);
                    UsersExtraInfo info = extra.Info;
                    info.buyEnergyCount++;
                    client.Player.SendMessage("Mua điểm hoạt b\x00e1t th\x00e0nh c\x00f4ng. Tăng th\x00eam " + missionEnergyInfo.Energy + " điểm hoạt b\x00e1t!");
                }
                else
                {
                    client.Player.SendMessage("Số lần mua trong ng\x00e0y đ\x00e3 đạt tối đa!");
                }
            }
            else
            {
                client.Player.SendMessage("Kh\x00f4ng cần mua th\x00eam điểm hoạt b\x00e1t.");
            }
            client.Player.Out.SendMissionEnergy(extra.Info);
            return 0;
        }
    }
}


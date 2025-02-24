namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x9c, "银行升级")]
    public class ConsortiaStoreUpGradeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                return 0;
            }
            bool val = false;
            string msg = "ConsortiaStoreUpGradeHandler.Failed";
            ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
            if (info == null)
            {
                msg = "ConsortiaStoreUpGradeHandler.NoConsortia";
                return 0;
            }
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (bussiness.UpGradeStoreConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
                {
                    info.StoreLevel++;
                    GameServer.Instance.LoginServer.SendConsortiaStoreUpGrade(info);
                    msg = "ConsortiaStoreUpGradeHandler.Success";
                    val = true;
                }
            }
            packet.WriteBoolean(val);
            client.Out.SendTCP(packet);
            return 1;
        }
    }
}


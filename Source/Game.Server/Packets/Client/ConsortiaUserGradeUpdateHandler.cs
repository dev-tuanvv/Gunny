namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x99, "用户等级更新")]
    public class ConsortiaUserGradeUpdateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int id = packet.ReadInt();
                bool upGrade = packet.ReadBoolean();
                bool val = false;
                string msg = "ConsortiaUserGradeUpdateHandler.Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    string tempUserName = "";
                    ConsortiaDutyInfo info = new ConsortiaDutyInfo();
                    if (bussiness.UpdateConsortiaUserGrade(id, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, upGrade, ref msg, ref info, ref tempUserName))
                    {
                        msg = "ConsortiaUserGradeUpdateHandler.Success";
                        val = true;
                        GameServer.Instance.LoginServer.SendConsortiaDuty(info, upGrade ? 6 : 7, client.Player.PlayerCharacter.ConsortiaID, id, tempUserName, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName);
                    }
                }
                packet.WriteBoolean(val);
                packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
                client.Out.SendTCP(packet);
            }
            return 0;
        }
    }
}


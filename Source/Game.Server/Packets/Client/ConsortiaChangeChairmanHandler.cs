namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x9a, "转让会长")]
    public class ConsortiaChangeChairmanHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                string str = packet.ReadString();
                bool val = false;
                string msg = "ConsortiaChangeChairmanHandler.Failed";
                if (string.IsNullOrEmpty(str))
                {
                    msg = "ConsortiaChangeChairmanHandler.NoName";
                }
                else if (str == client.Player.PlayerCharacter.NickName)
                {
                    msg = "ConsortiaChangeChairmanHandler.Self";
                }
                else
                {
                    using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                    {
                        string tempUserName = "";
                        int tempUserID = 0;
                        ConsortiaDutyInfo info = new ConsortiaDutyInfo();
                        if (bussiness.UpdateConsortiaChairman(str, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg, ref info, ref tempUserID, ref tempUserName))
                        {
                            ConsortiaDutyInfo info2 = new ConsortiaDutyInfo {
                                Level = client.Player.PlayerCharacter.DutyLevel,
                                DutyName = client.Player.PlayerCharacter.DutyName,
                                Right = client.Player.PlayerCharacter.Right
                            };
                            msg = "ConsortiaChangeChairmanHandler.Success1";
                            val = true;
                            GameServer.Instance.LoginServer.SendConsortiaDuty(info2, 9, client.Player.PlayerCharacter.ConsortiaID, tempUserID, tempUserName, 0, "");
                            GameServer.Instance.LoginServer.SendConsortiaDuty(info, 8, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, 0, "");
                        }
                    }
                }
                string translation = LanguageMgr.GetTranslation(msg, new object[0]);
                if (msg == "ConsortiaChangeChairmanHandler.Success1")
                {
                    translation = translation + str + LanguageMgr.GetTranslation("ConsortiaChangeChairmanHandler.Success2", new object[0]);
                }
                packet.WriteBoolean(val);
                packet.WriteString(translation);
                client.Out.SendTCP(packet);
            }
            return 0;
        }
    }
}


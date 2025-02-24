namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x85, "申请进入通过")]
    public class ConsortiaApplyLoginPassHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int applyID = packet.ReadInt();
                bool val = false;
                string msg = "ConsortiaApplyLoginPassHandler.Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    int consortiaRepute = 0;
                    ConsortiaUserInfo info = new ConsortiaUserInfo();
                    if (bussiness.PassConsortiaApplyUsers(applyID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ConsortiaID, ref msg, info, ref consortiaRepute))
                    {
                        msg = "ConsortiaApplyLoginPassHandler.Success";
                        val = true;
                        if (info.UserID != 0)
                        {
                            info.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
                            info.ConsortiaName = client.Player.PlayerCharacter.ConsortiaName;
                            GameServer.Instance.LoginServer.SendConsortiaUserPass(client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, info, false, consortiaRepute, info.LoginName, client.Player.PlayerCharacter.FightPower, 0);
                        }
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


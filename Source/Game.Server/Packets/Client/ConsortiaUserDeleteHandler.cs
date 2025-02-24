namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x84, "删除公会成员")]
    public class ConsortiaUserDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int kickUserID = packet.ReadInt();
                bool val = false;
                string nickName = "";
                string msg = (kickUserID == client.Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitFailed" : "ConsortiaUserDeleteHandler.KickFailed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    if (bussiness.DeleteConsortiaUser(client.Player.PlayerCharacter.ID, kickUserID, client.Player.PlayerCharacter.ConsortiaID, ref msg, ref nickName))
                    {
                        msg = (kickUserID == client.Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitSuccess" : "ConsortiaUserDeleteHandler.KickSuccess";
                        int consortiaID = client.Player.PlayerCharacter.ConsortiaID;
                        if (kickUserID == client.Player.PlayerCharacter.ID)
                        {
                            client.Player.ClearConsortia();
                            client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                        }
                        GameServer.Instance.LoginServer.SendConsortiaUserDelete(kickUserID, consortiaID, kickUserID != client.Player.PlayerCharacter.ID, nickName, client.Player.PlayerCharacter.NickName);
                        val = true;
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


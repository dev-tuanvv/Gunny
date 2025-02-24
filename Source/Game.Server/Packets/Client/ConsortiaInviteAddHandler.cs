namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(140, "公会邀请")]
    public class ConsortiaInviteAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                string str = packet.ReadString();
                bool val = false;
                string msg = "ConsortiaInviteAddHandler.Failed";
                if (string.IsNullOrEmpty(str))
                {
                    return 0;
                }
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaInviteUserInfo info = new ConsortiaInviteUserInfo {
                        ConsortiaID = client.Player.PlayerCharacter.ConsortiaID,
                        ConsortiaName = client.Player.PlayerCharacter.ConsortiaName,
                        InviteDate = DateTime.Now,
                        InviteID = client.Player.PlayerCharacter.ID,
                        InviteName = client.Player.PlayerCharacter.NickName,
                        IsExist = true,
                        Remark = "",
                        UserID = 0,
                        UserName = str
                    };
                    if (bussiness.AddConsortiaInviteUsers(info, ref msg))
                    {
                        msg = "ConsortiaInviteAddHandler.Success";
                        val = true;
                        GameServer.Instance.LoginServer.SendConsortiaInvite(info.ID, info.UserID, info.UserName, info.InviteID, info.InviteName, info.ConsortiaName, info.ConsortiaID);
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


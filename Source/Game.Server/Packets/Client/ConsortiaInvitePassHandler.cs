namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x8e, "通过邀请")]
    public class ConsortiaInvitePassHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                int inviteID = packet.ReadInt();
                bool val = false;
                int consortiaID = 0;
                string consortiaName = "";
                string msg = "ConsortiaInvitePassHandler.Failed";
                int tempID = 0;
                string tempName = "";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    int consortiaRepute = 0;
                    ConsortiaUserInfo info = new ConsortiaUserInfo();
                    if (bussiness.PassConsortiaInviteUsers(inviteID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, ref consortiaID, ref consortiaName, ref msg, info, ref tempID, ref tempName, ref consortiaRepute))
                    {
                        client.Player.PlayerCharacter.ConsortiaID = consortiaID;
                        client.Player.PlayerCharacter.ConsortiaName = consortiaName;
                        client.Player.PlayerCharacter.DutyLevel = info.Level;
                        client.Player.PlayerCharacter.DutyName = info.DutyName;
                        client.Player.PlayerCharacter.Right = info.Right;
                        ConsortiaInfo info2 = ConsortiaMgr.FindConsortiaInfo(consortiaID);
                        if (info2 != null)
                        {
                            client.Player.PlayerCharacter.ConsortiaLevel = info2.Level;
                        }
                        msg = "ConsortiaInvitePassHandler.Success";
                        val = true;
                        info.UserID = client.Player.PlayerCharacter.ID;
                        info.UserName = client.Player.PlayerCharacter.NickName;
                        info.Grade = client.Player.PlayerCharacter.Grade;
                        info.Offer = client.Player.PlayerCharacter.Offer;
                        info.RichesOffer = client.Player.PlayerCharacter.RichesOffer;
                        info.RichesRob = client.Player.PlayerCharacter.RichesRob;
                        info.Win = client.Player.PlayerCharacter.Win;
                        info.Total = client.Player.PlayerCharacter.Total;
                        info.Escape = client.Player.PlayerCharacter.Escape;
                        info.ConsortiaID = consortiaID;
                        info.ConsortiaName = consortiaName;
                        GameServer.Instance.LoginServer.SendConsortiaUserPass(tempID, tempName, info, true, consortiaRepute, client.Player.PlayerCharacter.UserName, client.Player.PlayerCharacter.FightPower, 0);
                    }
                }
                packet.WriteBoolean(val);
                packet.WriteInt(consortiaID);
                packet.WriteString(consortiaName);
                packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
                client.Out.SendTCP(packet);
            }
            return 0;
        }
    }
}


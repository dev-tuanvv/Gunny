namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x97, "禁言")]
    public class ConsortiaIsBanChatHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int banUserID = packet.ReadInt();
                bool isBanChat = packet.ReadBoolean();
                int tempID = 0;
                string tempName = "";
                bool val = false;
                string msg = "ConsortiaIsBanChatHandler.Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    if (bussiness.UpdateConsortiaIsBanChat(banUserID, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, isBanChat, ref tempID, ref tempName, ref msg))
                    {
                        msg = "ConsortiaIsBanChatHandler.Success";
                        val = true;
                        GameServer.Instance.LoginServer.SendConsortiaBanChat(tempID, tempName, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, isBanChat);
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


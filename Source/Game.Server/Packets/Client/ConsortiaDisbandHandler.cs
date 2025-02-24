namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x83, "公会解散")]
    public class ConsortiaDisbandHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int consortiaID = client.Player.PlayerCharacter.ConsortiaID;
                string consortiaName = client.Player.PlayerCharacter.ConsortiaName;
                bool val = false;
                string msg = "ConsortiaDisbandHandler.Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    if (bussiness.DeleteConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
                    {
                        val = true;
                        msg = "ConsortiaDisbandHandler.Success1";
                        client.Player.ClearConsortia();
                        GameServer.Instance.LoginServer.SendConsortiaDelete(consortiaID);
                    }
                }
                string translation = LanguageMgr.GetTranslation(msg, new object[0]);
                if (msg == "ConsortiaDisbandHandler.Success1")
                {
                    translation = translation + consortiaName + LanguageMgr.GetTranslation("ConsortiaDisbandHandler.Success2", new object[0]);
                }
                packet.WriteBoolean(val);
                packet.WriteInt(client.Player.PlayerCharacter.ID);
                packet.WriteString(translation);
                client.Out.SendTCP(packet);
            }
            return 0;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x91, "申请通过")]
    public class ConsortiaApplyAllyPassHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int applyID = packet.ReadInt();
            bool val = false;
            int tempID = 0;
            int state = 0;
            string msg = "ConsortiaApplyAllyPassHandler.Failed";
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (bussiness.PassConsortiaApplyAlly(applyID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref tempID, ref state, ref msg))
                {
                    msg = "ConsortiaApplyAllyPassHandler.Success";
                    val = true;
                    GameServer.Instance.LoginServer.SendConsortiaAlly(client.Player.PlayerCharacter.ConsortiaID, tempID, state);
                }
            }
            packet.WriteBoolean(val);
            packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
            client.Out.SendTCP(packet);
            return 0;
        }
    }
}


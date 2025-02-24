namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x88, "公会申请状态")]
    public class ConsotiaApplyStateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                return 1;
            }
            bool state = packet.ReadBoolean();
            bool val = false;
            string msg = "CONSORTIA_APPLY_STATE.Failed";
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (bussiness.UpdateConsotiaApplyState(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, state, ref msg))
                {
                    msg = "CONSORTIA_APPLY_STATE.Success";
                    val = true;
                }
            }
            packet.WriteBoolean(val);
            packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
            client.Out.SendTCP(packet);
            return 0;
        }
    }
}


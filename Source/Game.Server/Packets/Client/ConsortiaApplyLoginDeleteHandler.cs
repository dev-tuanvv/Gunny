namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x86, "删除进入申请")]
    public class ConsortiaApplyLoginDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int applyID = packet.ReadInt();
            bool val = false;
            string msg = "ConsortiaApplyAllyDeleteHandler.Failed";
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (bussiness.DeleteConsortiaApplyUsers(applyID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref msg))
                {
                    msg = (client.Player.PlayerCharacter.ID == 0) ? "ConsortiaApplyAllyDeleteHandler.Success" : "ConsortiaApplyAllyDeleteHandler.Success2";
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


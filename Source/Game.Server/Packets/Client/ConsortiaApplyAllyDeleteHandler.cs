namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x92, "删除申请")]
    public class ConsortiaApplyAllyDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int applyID = packet.ReadInt();
            bool val = false;
            string msg = "ConsortiaApplyAllyDeleteHandler.Failed";
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (bussiness.DeleteConsortiaApplyAlly(applyID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref msg))
                {
                    msg = "ConsortiaApplyAllyDeleteHandler.Success";
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


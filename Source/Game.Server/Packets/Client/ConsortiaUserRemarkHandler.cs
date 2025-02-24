namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;
    using System.Text;

    [PacketHandler(0x98, "修改成员备注")]
    public class ConsortiaUserRemarkHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int id = packet.ReadInt();
                string str = packet.ReadString();
                if (string.IsNullOrEmpty(str) || (Encoding.Default.GetByteCount(str) > 100))
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaUserRemarkHandler.Long", new object[0]));
                    return 1;
                }
                bool val = false;
                string msg = "ConsortiaUserRemarkHandler.Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    if (bussiness.UpdateConsortiaUserRemark(id, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, str, ref msg))
                    {
                        msg = "ConsortiaUserRemarkHandler.Success";
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


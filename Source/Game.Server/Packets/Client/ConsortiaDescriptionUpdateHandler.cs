namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;
    using System.Text;

    [PacketHandler(0x95, "更新介绍")]
    public class ConsortiaDescriptionUpdateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string s = packet.ReadString();
            if (Encoding.Default.GetByteCount(s) > 300)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDescriptionUpdateHandler.Long", new object[0]));
                return 1;
            }
            bool val = false;
            string msg = "ConsortiaDescriptionUpdateHandler.Failed";
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (bussiness.UpdateConsortiaDescription(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, s, ref msg))
                {
                    msg = "ConsortiaDescriptionUpdateHandler.Success";
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


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;
    using System.Text;

    [PacketHandler(150, "更新公告")]
    public class ConsortiaPlacardUpdateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string s = packet.ReadString();
            if (Encoding.Default.GetByteCount(s) > 300)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaPlacardUpdateHandler.Long", new object[0]));
                return 1;
            }
            bool val = false;
            string msg = "ConsortiaPlacardUpdateHandler.Failed";
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (bussiness.UpdateConsortiaPlacard(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, s, ref msg))
                {
                    msg = "ConsortiaPlacardUpdateHandler.Success";
                    val = true;
                }
            }
            packet.WriteBoolean(val);
            client.Out.SendTCP(packet);
            client.Player.SendMessage(LanguageMgr.GetTranslation(msg, new object[0]));
            return 0;
        }
    }
}


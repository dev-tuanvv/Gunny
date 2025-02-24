namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x76, "取消付款邮件")]
    public class MailPaymentCancelHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            new GSPacketIn(0x76, client.Player.PlayerCharacter.ID);
            int mailID = packet.ReadInt();
            int senderID = 0;
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                if (bussiness.CancelPaymentMail(client.Player.PlayerCharacter.ID, mailID, ref senderID))
                {
                    client.Out.SendMailResponse(senderID, eMailRespose.Receiver);
                    packet.WriteBoolean(true);
                }
                else
                {
                    packet.WriteBoolean(false);
                }
            }
            client.Out.SendTCP(packet);
            return 1;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x70, "删除邮件")]
    public class UserDeleteMailHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn @in = new GSPacketIn(0x70, client.Player.PlayerCharacter.ID);
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            int val = packet.ReadInt();
            @in.WriteInt(val);
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                int num2;
                if (bussiness.DeleteMail(client.Player.PlayerCharacter.ID, val, out num2))
                {
                    client.Out.SendMailResponse(num2, eMailRespose.Receiver);
                    @in.WriteBoolean(true);
                }
                else
                {
                    @in.WriteBoolean(false);
                }
            }
            client.Out.SendTCP(@in);
            return 0;
        }
    }
}


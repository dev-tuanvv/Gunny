namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x8f, "删除公会邀请")]
    internal class ConsortiaInviteDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int intiveID = packet.ReadInt();
            bool val = false;
            string translateId = "ConsortiaInviteDeleteHandler.Failed";
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (bussiness.DeleteConsortiaInviteUsers(intiveID, client.Player.PlayerCharacter.ID))
                {
                    translateId = "ConsortiaInviteDeleteHandler.Success";
                    val = true;
                }
            }
            packet.WriteBoolean(val);
            packet.WriteString(LanguageMgr.GetTranslation(translateId, new object[0]));
            client.Out.SendTCP(packet);
            return 0;
        }
    }
}


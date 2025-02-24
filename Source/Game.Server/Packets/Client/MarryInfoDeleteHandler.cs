namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0xea, "撤消征婚信息")]
    public class MarryInfoDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int iD = packet.ReadInt();
            string translation = LanguageMgr.GetTranslation("MarryInfoDeleteHandler.Fail", new object[0]);
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                if (bussiness.DeleteMarryInfo(iD, client.Player.PlayerCharacter.ID, ref translation))
                {
                    client.Out.SendAuctionRefresh(null, iD, false, null);
                }
                client.Out.SendMessage(eMessageType.Normal, translation);
            }
            return 0;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xc2, "撤消拍卖")]
    public class AuctionDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int auctionID = packet.ReadInt();
            string translation = LanguageMgr.GetTranslation("AuctionDeleteHandler.Fail", new object[0]);
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                if (bussiness.DeleteAuction(auctionID, client.Player.PlayerCharacter.ID, ref translation))
                {
                    client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                    client.Out.SendAuctionRefresh(null, auctionID, false, null);
                }
                else
                {
                    AuctionInfo auctionSingle = bussiness.GetAuctionSingle(auctionID);
                    client.Out.SendAuctionRefresh(auctionSingle, auctionID, auctionSingle != null, null);
                }
                client.Out.SendMessage(eMessageType.Normal, translation);
            }
            return 0;
        }
    }
}


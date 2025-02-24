namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xc1, "更新拍卖")]
    public class AuctionUpdateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int auctionID = packet.ReadInt();
            int num2 = packet.ReadInt();
            bool val = false;
            int num3 = GameProperties.LimitLevel(0);
            if (client.Player.PlayerCharacter.Grade < num3)
            {
                client.Out.SendMessage(eMessageType.Normal, string.Format("Cấp {0} trở l\x00ean mới c\x00f3 thể đấu gi\x00e1 vật phẩm.", num3));
                return 0;
            }
            GSPacketIn @in = new GSPacketIn(0xc1, client.Player.PlayerCharacter.ID);
            string translateId = "AuctionUpdateHandler.Fail";
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                AuctionInfo auctionSingle = bussiness.GetAuctionSingle(auctionID);
                if (auctionSingle == null)
                {
                    translateId = "AuctionUpdateHandler.Msg1";
                }
                else if ((auctionSingle.PayType == 0) && (num2 > client.Player.PlayerCharacter.Gold))
                {
                    translateId = "AuctionUpdateHandler.Msg2";
                }
                else if (!((auctionSingle.PayType != 1) || client.Player.ActiveMoneyEnable(num2)))
                {
                    translateId = "";
                }
                else if ((auctionSingle.BuyerID == 0) && (auctionSingle.Price > num2))
                {
                    translateId = "AuctionUpdateHandler.Msg4";
                }
                else if (((auctionSingle.BuyerID != 0) && ((auctionSingle.Price + auctionSingle.Rise) > num2)) && ((auctionSingle.Mouthful == 0) || (auctionSingle.Mouthful > num2)))
                {
                    translateId = "AuctionUpdateHandler.Msg5";
                }
                else
                {
                    int buyerID = auctionSingle.BuyerID;
                    auctionSingle.BuyerID = client.Player.PlayerCharacter.ID;
                    auctionSingle.BuyerName = client.Player.PlayerCharacter.NickName;
                    auctionSingle.Price = num2;
                    if ((auctionSingle.Mouthful != 0) && (num2 >= auctionSingle.Mouthful))
                    {
                        auctionSingle.Price = auctionSingle.Mouthful;
                        auctionSingle.IsExist = false;
                    }
                    if (bussiness.UpdateAuction(auctionSingle))
                    {
                        if (auctionSingle.PayType == 0)
                        {
                            client.Player.RemoveGold(auctionSingle.Price);
                        }
                        if (auctionSingle.IsExist)
                        {
                            translateId = "AuctionUpdateHandler.Msg6";
                        }
                        else
                        {
                            translateId = "AuctionUpdateHandler.Msg7";
                            client.Out.SendMailResponse(auctionSingle.AuctioneerID, eMailRespose.Receiver);
                            client.Out.SendMailResponse(auctionSingle.BuyerID, eMailRespose.Receiver);
                        }
                        if (buyerID != 0)
                        {
                            client.Out.SendMailResponse(buyerID, eMailRespose.Receiver);
                        }
                        val = true;
                    }
                }
                client.Out.SendAuctionRefresh(auctionSingle, auctionID, (auctionSingle != null) && auctionSingle.IsExist, null);
                if (translateId != "")
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
                }
            }
            @in.WriteBoolean(val);
            @in.WriteInt(auctionID);
            client.Out.SendTCP(@in);
            return 0;
        }
    }
}


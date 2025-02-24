namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using Game.Server.Statics;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x87, "捐献公会财富")]
    public class ConsortiaRichesOfferHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int num = packet.ReadInt();
                if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                    return 1;
                }
                if ((num < 1) || (client.Player.PlayerCharacter.Money < num))
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaRichesOfferHandler.NoMoney", new object[0]));
                    return 1;
                }
                bool val = false;
                string translateId = "ConsortiaRichesOfferHandler.Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    if (client.Player.PlayerCharacter.GP < 0)
                    {
                        val = false;
                        translateId = "Cấp độ nhỏ hơn 16 kh\x00f4ng thể cống hiến Guild";
                    }
                    else
                    {
                        int riches = num / 2;
                        if (bussiness.ConsortiaRichAdd(client.Player.PlayerCharacter.ConsortiaID, ref riches, 5, client.Player.PlayerCharacter.NickName))
                        {
                            val = true;
                            PlayerInfo playerCharacter = client.Player.PlayerCharacter;
                            playerCharacter.RichesOffer += riches;
                            client.Player.RemoveMoney(num);
                            LogMgr.LogMoneyAdd(LogMoneyType.Consortia, LogMoneyType.Consortia_Rich, client.Player.PlayerCharacter.ID, num, client.Player.PlayerCharacter.Money, 0, 0, 5, "", "", "");
                            translateId = "ConsortiaRichesOfferHandler.Successed";
                            GameServer.Instance.LoginServer.SendConsortiaRichesOffer(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, riches);
                        }
                    }
                }
                packet.WriteBoolean(val);
                packet.WriteString(LanguageMgr.GetTranslation(translateId, new object[0]));
                client.Out.SendTCP(packet);
            }
            return 0;
        }
    }
}


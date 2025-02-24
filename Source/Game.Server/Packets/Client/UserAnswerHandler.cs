namespace Game.Server.Packets.Client
{
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(15, "New User Answer Question")]
    public class UserAnswerHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int answerId = packet.ReadInt();
            bool flag = false;
            if (num == 1)
            {
                flag = packet.ReadBoolean();
            }
            if (num == 1)
            {
                List<SqlDataProvider.Data.ItemInfo> list = null;
                if (DropInventory.AnswerDrop(answerId, ref list))
                {
                    int gold = 0;
                    int money = 0;
                    int giftToken = 0;
                    int medal = 0;
                    int honor = 0;
                    int hardCurrency = 0;
                    int token = 0;
                    int dragonToken = 0;
                    int magicStonePoint = 0;
                    foreach (SqlDataProvider.Data.ItemInfo info in list)
                    {
                        ShopMgr.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken, ref magicStonePoint);
                        if (info != null)
                        {
                            client.Player.AddTemplate(info, info.Template.BagType, info.Count, eGameView.CaddyTypeGet);
                        }
                        client.Player.AddGold(gold);
                        client.Player.AddMoney(money);
                        client.Player.AddGiftToken(giftToken);
                    }
                }
                if (flag)
                {
                    client.Player.PlayerCharacter.openFunction((Step) answerId);
                }
            }
            if (num == 2)
            {
                client.Player.PlayerCharacter.openFunction((Step) answerId);
            }
            client.Player.UpdateAnswerSite(answerId);
            return 1;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0x71, "获取邮件到背包")]
    public class MailGetAttachHandler : IPacketHandler
    {
        public bool GetAnnex(string value, GamePlayer player, ref string msg, ref bool result, ref eMessageType eMsg)
        {
            int itemID = int.Parse(value);
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                SqlDataProvider.Data.ItemInfo userItemSingle = bussiness.GetUserItemSingle(itemID);
                if (userItemSingle != null)
                {
                    if (userItemSingle.TemplateID == -400)
                    {
                        player.AddGoXu(userItemSingle.Count);
                    }
                    else if (player.AddTemplate(userItemSingle))
                    {
                        eMsg = eMessageType.Normal;
                        return true;
                    }
                }
            }
            return false;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int mailID = packet.ReadInt();
            byte num2 = packet.ReadByte();
            List<int> list = new List<int>();
            List<string> list2 = new List<string>();
            int num3 = 0;
            int gold = 0;
            int giftToken = 0;
            string str = "";
            eMessageType normal = eMessageType.Normal;
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            GSPacketIn @in = new GSPacketIn(0x71, client.Player.PlayerCharacter.ID);
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                client.Player.LastAttachMail = DateTime.Now;
                MailInfo mailSingle = bussiness.GetMailSingle(client.Player.PlayerCharacter.ID, mailID);
                if (mailSingle != null)
                {
                    bool result = true;
                    int money = mailSingle.Money;
                    if (!((mailSingle.Type <= 100) || client.Player.ActiveMoneyEnable(money)))
                    {
                        return 0;
                    }
                    GamePlayer playerById = WorldMgr.GetPlayerById(mailSingle.ReceiverID);
                    if (!mailSingle.IsRead)
                    {
                        mailSingle.IsRead = true;
                        mailSingle.ValidDate = 0x48;
                        mailSingle.SendTime = DateTime.Now;
                    }
                    if (!((!result || ((num2 != 0) && (num2 != 1))) || string.IsNullOrEmpty(mailSingle.Annex1)))
                    {
                        list.Add(1);
                        list2.Add(mailSingle.Annex1);
                        mailSingle.Annex1 = null;
                    }
                    if (!((!result || ((num2 != 0) && (num2 != 2))) || string.IsNullOrEmpty(mailSingle.Annex2)))
                    {
                        list.Add(2);
                        list2.Add(mailSingle.Annex2);
                        mailSingle.Annex2 = null;
                    }
                    if (!((!result || ((num2 != 0) && (num2 != 3))) || string.IsNullOrEmpty(mailSingle.Annex3)))
                    {
                        list.Add(3);
                        list2.Add(mailSingle.Annex3);
                        mailSingle.Annex3 = null;
                    }
                    if (!((!result || ((num2 != 0) && (num2 != 4))) || string.IsNullOrEmpty(mailSingle.Annex4)))
                    {
                        list.Add(4);
                        list2.Add(mailSingle.Annex4);
                        mailSingle.Annex4 = null;
                    }
                    if (!((!result || ((num2 != 0) && (num2 != 5))) || string.IsNullOrEmpty(mailSingle.Annex5)))
                    {
                        list.Add(5);
                        list2.Add(mailSingle.Annex5);
                        mailSingle.Annex5 = null;
                    }
                    if (((num2 == 0) || (num2 == 6)) && (mailSingle.Gold > 0))
                    {
                        list.Add(6);
                        gold = mailSingle.Gold;
                        mailSingle.Gold = 0;
                    }
                    if ((((num2 == 0) || (num2 == 7)) && (mailSingle.Type < 100)) && (mailSingle.Money > 0))
                    {
                        list.Add(7);
                        num3 = mailSingle.Money;
                        mailSingle.Money = 0;
                    }
                    if ((mailSingle.Type > 100) && (mailSingle.GiftToken > 0))
                    {
                        list.Add(8);
                        giftToken = mailSingle.GiftToken;
                        mailSingle.GiftToken = 0;
                    }
                    if ((mailSingle.Type > 100) && (mailSingle.Money > 0))
                    {
                        mailSingle.Money = 0;
                        str = LanguageMgr.GetTranslation("MailGetAttachHandler.Deduct", new object[0]) + (string.IsNullOrEmpty(str) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success", new object[0]) : str);
                    }
                    if (bussiness.UpdateMail(mailSingle, money))
                    {
                        if ((mailSingle.Type > 100) && (money > 0))
                        {
                            client.Out.SendMailResponse(mailSingle.SenderID, eMailRespose.Receiver);
                            client.Out.SendMailResponse(mailSingle.ReceiverID, eMailRespose.Send);
                        }
                        playerById.AddMoney(num3);
                        playerById.AddGold(gold);
                        playerById.AddGiftToken(giftToken);
                        foreach (string str2 in list2)
                        {
                            this.GetAnnex(str2, client.Player, ref str, ref result, ref normal);
                        }
                    }
                    @in.WriteInt(mailID);
                    @in.WriteInt(list.Count);
                    foreach (int num7 in list)
                    {
                        @in.WriteInt(num7);
                    }
                    client.Out.SendTCP(@in);
                    client.Out.SendMessage(normal, string.IsNullOrEmpty(str) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success", new object[0]) : str);
                }
                else
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MailGetAttachHandler.Falied", new object[0]));
                }
            }
            return 0;
        }
    }
}


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
    using System.Text;

    [PacketHandler(116, "发送邮件")]
    public class UserSendMailHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.Gold < 100) // check 100 gold send email
            {
                return 1;
            }
            if (client.Player.IsLimitMail())
            {
                return 0;
            }
            string translateId = "UserSendMailHandler.Success";
            eMessageType type = eMessageType.Normal;
            GSPacketIn gSPacketIn = new GSPacketIn(116, client.Player.PlayerCharacter.ID);
            ItemInfo itemInfo = null;
            string text = packet.ReadString();
            string title = packet.ReadString();
            string content = packet.ReadString();
            bool flag = packet.ReadBoolean();
            int num = packet.ReadInt();
            int num2 = packet.ReadInt();
            eBageType bagType = (eBageType)packet.ReadByte();
            int num3 = packet.ReadInt();
            int num4 = packet.ReadInt();
            if (client.Player.IsLimitMoney(num2))
            {
                return 0;
            }
            //int num5 = GameProperties.LimitLevel(0); //level mind send mail
            int levellimit = 27;
            if (num3 != -1 && client.Player.PlayerCharacter.Grade < levellimit)
            {
                client.Out.SendMessage(eMessageType.Normal, string.Format("Cấp {0} trở lên mới có thể gởi file đính kèm.", levellimit));
                gSPacketIn.WriteBoolean(false);
                client.Out.SendTCP(gSPacketIn);
                return 0;
            }
            if ((num2 != 0 || num3 != -1) && client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                gSPacketIn.WriteBoolean(false);
                client.Out.SendTCP(gSPacketIn);
                return 1;
            }
            ItemInfo itemInfo2 = null;
            using (PlayerBussiness playerBussiness = new PlayerBussiness())
            {
                GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
                PlayerInfo playerInfo;
                if (clientByPlayerNickName == null)
                {
                    playerInfo = playerBussiness.GetUserSingleByNickName(text);
                }
                else
                {
                    playerInfo = clientByPlayerNickName.PlayerCharacter;
                }
                if (playerInfo != null && !string.IsNullOrEmpty(text))
                {
                    if (playerInfo.Grade < levellimit)
                    {
                        client.Out.SendMessage(eMessageType.Normal, "Không thể gửi cho nhân vật nhỏ hơn cấp " + levellimit);
                        gSPacketIn.WriteBoolean(false);
                        client.Out.SendTCP(gSPacketIn);
                        int result = 0;
                        return result;
                    }
                    if (playerInfo.NickName != client.Player.PlayerCharacter.NickName)
                    {
                        MailInfo mailInfo = new MailInfo();
                        mailInfo.SenderID = client.Player.PlayerCharacter.ID;
                        mailInfo.Sender = client.Player.PlayerCharacter.NickName;
                        mailInfo.ReceiverID = playerInfo.ID;
                        mailInfo.Receiver = playerInfo.NickName;
                        mailInfo.IsExist = true;
                        mailInfo.Gold = 0;
                        mailInfo.Money = 0;
                        mailInfo.Title = title;
                        mailInfo.Content = content;
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(LanguageMgr.GetTranslation("UserSendMailHandler.AnnexRemark", new object[0]));
                        int num6 = 0;
                        if (num3 != -1)
                        {
                            itemInfo = client.Player.GetItemAt(bagType, num3);
                            if (itemInfo == null)
                            {
                                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Đặt item cần gửi vào!", new object[0]));
                                int result = 0;
                                return result;
                            }
                            if (itemInfo != null && !itemInfo.IsBinds)
                            {
                               // if (itemInfo.Count < num4 || num4 < 0)
                               // {
                               //     client.Out.SendMessage(type, LanguageMgr.GetTranslation("Số lượng không có thực, thao tác thất bại!", new object[0]));
                               //     int result = 0;
                               //     return result;
                               // }
                               // if (client.Player.IsLimitCount(num4))
                               // {
                               //     int result = 0;
                               //     return result;
                               // }
                                itemInfo2 = ItemInfo.CloneFromTemplate(itemInfo.Template, itemInfo);
                                ItemInfo itemInfo3 = ItemInfo.CloneFromTemplate(itemInfo.Template, itemInfo);
                                //itemInfo3.Count = num4; //??? clgt
                                if (itemInfo3.ItemID == 0)
                                {
                                    using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
                                    {
                                        playerBussiness2.AddGoods(itemInfo3);
                                    }
                                }
                                mailInfo.Annex1Name = itemInfo3.Template.Name;
                                mailInfo.Annex1 = itemInfo3.ItemID.ToString();
                                num6++;
                                stringBuilder.Append(num6);
                                stringBuilder.Append("、");
                                stringBuilder.Append(mailInfo.Annex1Name);
                                stringBuilder.Append("x");
                                stringBuilder.Append(itemInfo3.Count);
                                stringBuilder.Append(";");
                            }
                        }
                        if (flag)
                        {
                            if (num2 <= 0 || (string.IsNullOrEmpty(mailInfo.Annex1) && string.IsNullOrEmpty(mailInfo.Annex2) && string.IsNullOrEmpty(mailInfo.Annex3) && string.IsNullOrEmpty(mailInfo.Annex4)))
                            {
                                int result = 1;
                                return result;
                            }
                            mailInfo.ValidDate = 72;// ((num == 1) ? 1 : 6);
                            mailInfo.Type = 101;
                            if (num2 > 0)
                            {
                                mailInfo.Money = num2;
                                num6++;
                                stringBuilder.Append(num6);
                                stringBuilder.Append("、");
                                stringBuilder.Append(LanguageMgr.GetTranslation("UserSendMailHandler.PayMoney", new object[0]));
                                stringBuilder.Append(num2);
                                stringBuilder.Append(";");
                            }
                        }
                        else
                        {
                            mailInfo.Type = 1;
                            if (client.Player.PlayerCharacter.Money >= num2 && num2 > 0)
                            {
                                mailInfo.Money = num2;
                                client.Player.RemoveMoney(num2);
                                num6++;
                                stringBuilder.Append(num6);
                                stringBuilder.Append("、");
                                stringBuilder.Append(LanguageMgr.GetTranslation("UserSendMailHandler.Money", new object[0]));
                                stringBuilder.Append(num2);
                                stringBuilder.Append(";");
                            }
                        }
                        if (stringBuilder.Length > 1)
                        {
                            mailInfo.AnnexRemark = stringBuilder.ToString();
                        }
                        if (playerBussiness.SendMail(mailInfo))
                        {
                            client.Player.RemoveGold(100);
                            if (itemInfo != null)
                            {
                                int num7 = itemInfo.Count - num4;
                                client.Player.RemoveItem(itemInfo);
                                if (num7 > 0)
                                {
                                    itemInfo2.Count = num7;
                                    client.Player.AddTemplate(itemInfo2, bagType, num7, eGameView.RouletteTypeGet);
                                }
                            }
                        }
                        gSPacketIn.WriteBoolean(true);
                        if (clientByPlayerNickName != null)
                        {
                            client.Player.Out.SendMailResponse(playerInfo.ID, eMailRespose.Receiver);
                        }
                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
                    }
                    else
                    {
                        translateId = "UserSendMailHandler.Failed1";
                        gSPacketIn.WriteBoolean(false);
                    }
                }
                else
                {
                    type = eMessageType.ERROR;
                    translateId = "UserSendMailHandler.Failed2";
                    gSPacketIn.WriteBoolean(false);
                }
            }
            client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId, new object[0]));
            client.Out.SendTCP(gSPacketIn);
            return 0;
        }
    }
}


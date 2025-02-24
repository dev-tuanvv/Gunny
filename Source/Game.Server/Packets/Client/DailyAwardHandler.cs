namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [PacketHandler(13, "场景用户离开")]
    public class DailyAwardHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int type = packet.ReadInt();
            int point = 0;
            int gold = 0;
            int giftToken = 0;
            int medal = 0;
            int exp = 0;
            int honor = 0;
            int hardCurrency = 0;
            int leagueMoney = 0;
            int useableScore = 0;
            int prestge = 0;
            int magicStonePoint = 0;
            StringBuilder builder = new StringBuilder();
            List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
            string translation = "";
            switch (type)
            {
                case 0:
                    if (AwardMgr.AddDailyAward(client.Player))
                    {
                        using (PlayerBussiness bussiness = new PlayerBussiness())
                        {
                            if (bussiness.UpdatePlayerLastAward(client.Player.PlayerCharacter.ID, type))
                            {
                                builder.Append(LanguageMgr.GetTranslation("Nhận được Thẻ x2 kinh nghiệm 60 ph\x00fat", new object[0]));
                            }
                            else
                            {
                                builder.Append(LanguageMgr.GetTranslation("GameUserDailyAward.Fail", new object[0]));
                            }
                            goto Label_03AB;
                        }
                    }
                    builder.Append(LanguageMgr.GetTranslation("GameUserDailyAward.Fail1", new object[0]));
                    goto Label_03AB;

                case 2:
                    if (DateTime.Now.Date == client.Player.PlayerCharacter.LastGetEgg.Date)
                    {
                        builder.Append("Bạn đ\x00e3 nhận 1 lần h\x00f4m nay!");
                        goto Label_03AB;
                    }
                    using (PlayerBussiness bussiness2 = new PlayerBussiness())
                    {
                        bussiness2.UpdatePlayerLastAward(client.Player.PlayerCharacter.ID, type);
                    }
                    if (ItemBoxMgr.CreateItemBox(ItemMgr.FindItemTemplate(0x1b5bb).TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref magicStonePoint))
                    {
                        goto Label_03AB;
                    }
                    client.Player.SendMessage("Xảy ra lỗi h\x00e3y thử lại sau.");
                    return 0;

                case 3:
                    if (client.Player.PlayerCharacter.CanTakeVipReward)
                    {
                        int vIPLevel = client.Player.PlayerCharacter.VIPLevel;
                        client.Player.LastVIPPackTime();
                        if (!ItemBoxMgr.CreateItemBox(ItemMgr.FindItemTemplate(ItemMgr.FindItemBoxTypeAndLv(2, vIPLevel).TemplateID).TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref magicStonePoint))
                        {
                            client.Player.SendMessage("Xảy ra lỗi h\x00e3y thử lại sau.");
                            return 0;
                        }
                        using (PlayerBussiness bussiness3 = new PlayerBussiness())
                        {
                            bussiness3.UpdateLastVIPPackTime(client.Player.PlayerCharacter.ID);
                            goto Label_03AB;
                        }
                    }
                    builder.Append("Bạn đ\x00e3 nhận 1 lần h\x00f4m nay!");
                    goto Label_03AB;

                case 5:
                    break;

                default:
                    goto Label_03AB;
            }
            using (ProduceBussiness bussiness4 = new ProduceBussiness())
            {
                DailyLogListInfo dailyLogListSingle = bussiness4.GetDailyLogListSingle(client.Player.PlayerCharacter.ID);
                string dayLog = dailyLogListSingle.DayLog;
                dayLog.Split(new char[] { ',' });
                if (string.IsNullOrEmpty(dayLog) || string.IsNullOrEmpty(dayLog.Split(new char[] { ',' })[0]))
                {
                    dayLog = "True";
                    dailyLogListSingle.UserAwardLog = 0;
                }
                else
                {
                    dayLog = dayLog + ",True";
                }
                dailyLogListSingle.DayLog = dayLog;
                dailyLogListSingle.UserAwardLog++;
                bussiness4.UpdateDailyLogList(dailyLogListSingle);
            }
            builder.Append("Điểm danh th\x00e0nh c\x00f4ng!");
        Label_03AB:
            if (point != 0)
            {
                builder.Append(point + LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]));
                client.Player.AddMoney(point);
            }
            if (gold != 0)
            {
                builder.Append(gold + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]));
                client.Player.AddGold(gold);
            }
            if (giftToken != 0)
            {
                builder.Append(giftToken + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]));
                client.Player.AddGiftToken(giftToken);
            }
            if (medal != 0)
            {
                builder.Append(medal + LanguageMgr.GetTranslation("OpenUpArkHandler.Medal", new object[0]));
                client.Player.AddMedal(medal);
            }
            StringBuilder builder2 = new StringBuilder();
            foreach (SqlDataProvider.Data.ItemInfo info3 in itemInfos)
            {
                builder2.Append(info3.Template.Name + "x" + info3.Count.ToString() + ",");
                if (!client.Player.AddTemplate(info3, info3.Template.BagType, info3.Count, eGameView.RouletteTypeGet))
                {
                    using (PlayerBussiness bussiness5 = new PlayerBussiness())
                    {
                        MailInfo info4;
                        info3.UserID = 0;
                        bussiness5.AddGoods(info3);
                        info4 = new MailInfo
                        {
                            Annex1 = info3.ItemID.ToString(),
                            Content = LanguageMgr.GetTranslation("OpenUpArkHandler.Content1", new object[0]) + info3.Template.Name + LanguageMgr.GetTranslation("OpenUpArkHandler.Content2", new object[0]),
                            Gold = 0,
                            Money = 0,
                            Receiver = client.Player.PlayerCharacter.NickName,
                            ReceiverID = client.Player.PlayerCharacter.ID,
                            //Sender = info4.Receiver,
                            //SenderID = info4.ReceiverID,
                            Title = LanguageMgr.GetTranslation("OpenUpArkHandler.Title", new object[0]) + info3.Template.Name + "]",
                            Type = 12
                        };
                        bussiness5.SendMail(info4);
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.Mail", new object[0]);
                    }
                }
            }
            if (builder2.Length > 0)
            {
                builder2.Remove(builder2.Length - 1, 1);
                string[] strArray = builder2.ToString().Split(new char[] { ',' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    string[] strArray2;
                    IntPtr ptr;
                    int num15 = 1;
                    for (int j = i + 1; j < strArray.Length; j++)
                    {
                        if (strArray[i].Contains(strArray[j]) && (strArray[j].Length == strArray[i].Length))
                        {
                            num15++;
                            strArray[j] = j.ToString();
                        }
                    }
                    if (num15 > 1)
                    {
                        strArray[i] = strArray[i].Remove(strArray[i].Length - 1, 1);
                        (strArray2 = strArray)[(int)(ptr = (IntPtr)i)] = strArray2[(int)ptr] + num15.ToString();
                    }
                    if (strArray[i] != i.ToString())
                    {
                        (strArray2 = strArray)[(int)(ptr = (IntPtr)i)] = strArray2[(int)ptr] + ",";
                        builder.Append(strArray[i]);
                    }
                }
            }
            if ((builder.Length - 1) > 0)
            {
                builder.Remove(builder.Length - 1, 1);
                builder.Append(".");
            }
            client.Out.SendMessage(eMessageType.Normal, translation + builder.ToString());
            if (!string.IsNullOrEmpty(translation))
            {
                client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
            return 2;
        }
    }
}


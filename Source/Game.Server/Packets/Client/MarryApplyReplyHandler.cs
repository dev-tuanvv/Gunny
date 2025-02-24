namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(250, "求婚答复")]
    internal class MarryApplyReplyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool result = packet.ReadBoolean();
            int userID = packet.ReadInt();
            int answerId = packet.ReadInt();
            if (result && client.Player.PlayerCharacter.IsMarried)
            {
                client.Player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg2", new object[0]));
            }
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                PlayerInfo userSingleByUserID = bussiness.GetUserSingleByUserID(userID);
                if (!result)
                {
                    this.SendGoodManCard(userSingleByUserID.NickName, userSingleByUserID.ID, client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ID, bussiness);
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
                }
                if ((userSingleByUserID == null) || (userSingleByUserID.Sex == client.Player.PlayerCharacter.Sex))
                {
                    return 1;
                }
                if (userSingleByUserID.IsMarried)
                {
                    client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg3", new object[0]));
                }
                MarryApplyInfo info = new MarryApplyInfo {
                    UserID = userID,
                    ApplyUserID = client.Player.PlayerCharacter.ID,
                    ApplyUserName = client.Player.PlayerCharacter.NickName,
                    ApplyType = 2,
                    LoveProclamation = "",
                    ApplyResult = result
                };
                int id = 0;
                if (bussiness.SavePlayerMarryNotice(info, answerId, ref id))
                {
                    if (result)
                    {
                        client.Player.Out.SendMarryApplyReply(client.Player, userSingleByUserID.ID, userSingleByUserID.NickName, result, false, id);
                        client.Player.LoadMarryProp();
                    }
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
                    return 0;
                }
            }
            return 1;
        }

        public void SendGoodManCard(string receiverName, int receiverID, string senderName, int senderID, PlayerBussiness db)
        {
            SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(0x2b61), 1, 0x70);
            item.IsBinds = true;
            item.UserID = 0;
            db.AddGoods(item);
            MailInfo mail = new MailInfo {
                Annex1 = item.ItemID.ToString(),
                Content = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Content", new object[0]),
                Gold = 0,
                IsExist = true,
                Money = 0,
                Receiver = receiverName,
                ReceiverID = receiverID,
                Sender = senderName,
                SenderID = senderID,
                Title = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Title", new object[0]),
                Type = 14
            };
            db.SendMail(mail);
        }

        public void SendSYSMessages(GamePlayer player, PlayerInfo spouse)
        {
            string str = player.PlayerCharacter.Sex ? player.PlayerCharacter.NickName : spouse.NickName;
            string str2 = player.PlayerCharacter.Sex ? spouse.NickName : player.PlayerCharacter.NickName;
            string translation = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg1", new object[] { str, str2 });
            GSPacketIn packet = new GSPacketIn(10);
            packet.WriteInt(2);
            packet.WriteString(translation);
            GameServer.Instance.LoginServer.SendPacket(packet);
            foreach (GamePlayer player2 in WorldMgr.GetAllPlayers())
            {
                player2.Out.SendTCP(packet);
            }
        }
    }
}


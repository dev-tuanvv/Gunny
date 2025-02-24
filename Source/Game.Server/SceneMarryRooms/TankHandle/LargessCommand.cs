namespace Game.Server.SceneMarryRooms.TankHandle
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using Game.Server.SceneMarryRooms;
    using SqlDataProvider.Data;
    using System;

    [MarryCommandAttbute(5)]
    public class LargessCommand : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom == null)
            {
                return false;
            }
            int num = packet.ReadInt();
            int num2 = GameProperties.LimitLevel(3);
            if (player.PlayerCharacter.Grade < num2)
            {
                player.Out.SendMessage(eMessageType.Normal, string.Format("Cấp {0} trở l\x00ean mới c\x00f3 thể tặng xu.", num2));
                return false;
            }
            if (num <= 0)
            {
                return false;
            }
            if (!player.ActiveMoneyEnable(num))
            {
                return false;
            }
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                string translation = LanguageMgr.GetTranslation("LargessCommand.Content", new object[] { player.PlayerCharacter.NickName, num / 2 });
                string str2 = LanguageMgr.GetTranslation("LargessCommand.Title", new object[] { player.PlayerCharacter.NickName });
                MailInfo mail = new MailInfo {
                    Annex1 = "",
                    Content = translation,
                    Gold = 0,
                    IsExist = true,
                    Money = num / 2,
                    Receiver = player.CurrentMarryRoom.Info.BrideName,
                    ReceiverID = player.CurrentMarryRoom.Info.BrideID,
                    Sender = LanguageMgr.GetTranslation("LargessCommand.Sender", new object[0]),
                    SenderID = 0,
                    Title = str2,
                    Type = 14
                };
                bussiness.SendMail(mail);
                player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
                MailInfo info2 = new MailInfo {
                    Annex1 = "",
                    Content = translation,
                    Gold = 0,
                    IsExist = true,
                    Money = num / 2,
                    Receiver = player.CurrentMarryRoom.Info.GroomName,
                    ReceiverID = player.CurrentMarryRoom.Info.GroomID,
                    Sender = LanguageMgr.GetTranslation("LargessCommand.Sender", new object[0]),
                    SenderID = 0,
                    Title = str2,
                    Type = 14
                };
                bussiness.SendMail(info2);
                player.Out.SendMailResponse(info2.ReceiverID, eMailRespose.Receiver);
            }
            player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LargessCommand.Succeed", new object[0]));
            GSPacketIn @in = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("LargessCommand.Notice", new object[] { player.PlayerCharacter.NickName, num }));
            player.CurrentMarryRoom.SendToPlayerExceptSelf(@in, player);
            return true;
        }
    }
}


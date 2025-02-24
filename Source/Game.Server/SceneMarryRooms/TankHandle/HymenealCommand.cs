namespace Game.Server.SceneMarryRooms.TankHandle
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using Game.Server.SceneMarryRooms;
    using SqlDataProvider.Data;
    using System;

    [MarryCommandAttbute(2)]
    public class HymenealCommand : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if ((player.CurrentMarryRoom == null) || (player.CurrentMarryRoom.RoomState != eRoomState.FREE))
            {
                return false;
            }
            if ((player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID) && (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID))
            {
                return false;
            }
            int num = GameProperties.PRICE_PROPOSE;
            if (player.CurrentMarryRoom.Info.IsHymeneal && (player.PlayerCharacter.Money < num))
            {
                player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
                return false;
            }
            GamePlayer playerByUserID = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.GroomID);
            if (playerByUserID == null)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoGroom", new object[0]));
                return false;
            }
            GamePlayer player3 = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.BrideID);
            if (player3 == null)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoBride", new object[0]));
                return false;
            }
            bool val = false;
            bool flag2 = false;
            GSPacketIn @in = packet.Clone();
            int num2 = packet.ReadInt();
            if (1 == num2)
            {
                player.CurrentMarryRoom.RoomState = eRoomState.FREE;
                goto Label_059B;
            }
            player.CurrentMarryRoom.RoomState = eRoomState.Hymeneal;
            player.CurrentMarryRoom.BeginTimerForHymeneal(0x29810);
            if (!player.PlayerCharacter.IsGotRing)
            {
                flag2 = true;
                ItemTemplateInfo goods = ItemMgr.FindItemTemplate(0x233e);
                SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x70);
                item.IsBinds = true;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    item.UserID = 0;
                    bussiness.AddGoods(item);
                    string translation = LanguageMgr.GetTranslation("HymenealCommand.Content", new object[] { player3.PlayerCharacter.NickName });
                    MailInfo mail = new MailInfo {
                        Annex1 = item.ItemID.ToString(),
                        Content = translation,
                        Gold = 0,
                        IsExist = true,
                        Money = 0,
                        Receiver = playerByUserID.PlayerCharacter.NickName,
                        ReceiverID = playerByUserID.PlayerCharacter.ID,
                        Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender", new object[0]),
                        SenderID = 0,
                        Title = LanguageMgr.GetTranslation("HymenealCommand.Title", new object[0]),
                        Type = 14
                    };
                    if (bussiness.SendMail(mail))
                    {
                        val = true;
                    }
                    player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
                }
                SqlDataProvider.Data.ItemInfo info4 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x70);
                info4.IsBinds = true;
                using (PlayerBussiness bussiness2 = new PlayerBussiness())
                {
                    info4.UserID = 0;
                    bussiness2.AddGoods(info4);
                    string str2 = LanguageMgr.GetTranslation("HymenealCommand.Content", new object[] { playerByUserID.PlayerCharacter.NickName });
                    MailInfo info5 = new MailInfo {
                        Annex1 = info4.ItemID.ToString(),
                        Content = str2,
                        Gold = 0,
                        IsExist = true,
                        Money = 0,
                        Receiver = player3.PlayerCharacter.NickName,
                        ReceiverID = player3.PlayerCharacter.ID,
                        Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender", new object[0]),
                        SenderID = 0,
                        Title = LanguageMgr.GetTranslation("HymenealCommand.Title", new object[0]),
                        Type = 14
                    };
                    if (bussiness2.SendMail(info5))
                    {
                        val = true;
                    }
                    player.Out.SendMailResponse(info5.ReceiverID, eMailRespose.Receiver);
                }
                player.CurrentMarryRoom.Info.IsHymeneal = true;
                using (PlayerBussiness bussiness3 = new PlayerBussiness())
                {
                    bussiness3.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);
                    bussiness3.UpdatePlayerGotRingProp(playerByUserID.PlayerCharacter.ID, player3.PlayerCharacter.ID);
                    playerByUserID.LoadMarryProp();
                    player3.LoadMarryProp();
                    goto Label_04E4;
                }
            }
            flag2 = false;
            val = true;
        Label_04E4:
            if (!flag2)
            {
                player.RemoveMoney(num);
                CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, num, 0, 0, 1);
            }
            @in.WriteInt(player.CurrentMarryRoom.Info.ID);
            @in.WriteBoolean(val);
            player.CurrentMarryRoom.SendToAll(@in);
            if (val)
            {
                string message = LanguageMgr.GetTranslation("HymenealCommand.Succeed", new object[] { playerByUserID.PlayerCharacter.NickName, player3.PlayerCharacter.NickName });
                GSPacketIn in2 = player.Out.SendMessage(eMessageType.ChatNormal, message);
                player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(in2, player);
            }
        Label_059B:
            return true;
        }
    }
}


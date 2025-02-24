namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xf8, "离婚")]
    internal class DivorceApplyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool flag = packet.ReadBoolean();
            if (!client.Player.PlayerCharacter.IsMarried)
            {
                return 1;
            }
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
            {
                client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg2", new object[0]));
                return 1;
            }
            int num = GameProperties.PRICE_DIVORCED;
            if (flag)
            {
                num = GameProperties.PRICE_DIVORCED_DISCOUNT;
            }
            if (!client.Player.MoneyDirect(num))
            {
                return 1;
            }
            CountBussiness.InsertSystemPayCount(client.Player.PlayerCharacter.ID, num, 0, 0, 3);
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                PlayerInfo userSingleByUserID = bussiness.GetUserSingleByUserID(client.Player.PlayerCharacter.SpouseID);
                if ((userSingleByUserID == null) || (userSingleByUserID.Sex == client.Player.PlayerCharacter.Sex))
                {
                    return 1;
                }
                MarryApplyInfo info = new MarryApplyInfo {
                    UserID = client.Player.PlayerCharacter.SpouseID,
                    ApplyUserID = client.Player.PlayerCharacter.ID,
                    ApplyUserName = client.Player.PlayerCharacter.NickName,
                    ApplyType = 3,
                    LoveProclamation = "",
                    ApplyResult = false
                };
                int id = 0;
                if (bussiness.SavePlayerMarryNotice(info, 0, ref id))
                {
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
                    client.Player.LoadMarryProp();
                }
            }
            client.Player.QuestInventory.ClearMarryQuest();
            client.Player.Out.SendPlayerDivorceApply(client.Player, true, true);
            client.Player.SendMessage(LanguageMgr.GetTranslation("DivorceApplyHandler.Msg3", new object[0]));
            return 0;
        }
    }
}


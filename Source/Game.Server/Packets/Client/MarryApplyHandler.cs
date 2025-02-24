namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Linq;

    [PacketHandler(0xf7, "求婚")]
    internal class MarryApplyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.IsMarried)
            {
                return 1;
            }
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 1;
            }
            int userID = packet.ReadInt();
            string loveProclamation = packet.ReadString();
            packet.ReadBoolean();
            bool flag = true;
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                PlayerInfo userSingleByUserID = bussiness.GetUserSingleByUserID(userID);
                if ((userSingleByUserID == null) || (userSingleByUserID.Sex == client.Player.PlayerCharacter.Sex))
                {
                    return 1;
                }
                if (userSingleByUserID.IsMarried)
                {
                    client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg2", new object[0]));
                    return 1;
                }
                SqlDataProvider.Data.ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, 0x2b5f);
                if (itemByTemplateID == null)
                {
                    ShopItemInfo info3 = ShopMgr.FindShopbyTemplatID(0x2b5f).FirstOrDefault<ShopItemInfo>();
                    if (info3 == null)
                    {
                        client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg6", new object[0]));
                        return 1;
                    }
                    if (!client.Player.MoneyDirect(info3.AValue1))
                    {
                        return 1;
                    }
                    flag = false;
                }
                MarryApplyInfo info = new MarryApplyInfo {
                    UserID = userID,
                    ApplyUserID = client.Player.PlayerCharacter.ID,
                    ApplyUserName = client.Player.PlayerCharacter.NickName,
                    ApplyType = 1,
                    LoveProclamation = loveProclamation,
                    ApplyResult = false
                };
                int id = 0;
                if (bussiness.SavePlayerMarryNotice(info, 0, ref id))
                {
                    if (flag)
                    {
                        client.Player.RemoveItem(itemByTemplateID);
                    }
                    else
                    {
                        ShopMgr.FindShopbyTemplatID(0x2b5f).FirstOrDefault<ShopItemInfo>();
                    }
                    client.Player.Out.SendPlayerMarryApply(client.Player, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, loveProclamation, id);
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userID);
                    string nickName = userSingleByUserID.NickName;
                    client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg3", new object[0]));
                }
            }
            return 0;
        }
    }
}


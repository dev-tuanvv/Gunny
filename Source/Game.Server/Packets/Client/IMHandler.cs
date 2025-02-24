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

    [PacketHandler(160, "添加好友")]
    public class IMHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num5;
            switch (packet.ReadByte())
            {
                case 160:
                {
                    string nickName = packet.ReadString();
                    int relation = packet.ReadInt();
                    if ((relation >= 0) && (relation <= 1))
                    {
                        using (PlayerBussiness bussiness = new PlayerBussiness())
                        {
                            PlayerInfo playerCharacter;
                            GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(nickName);
                            if (clientByPlayerNickName != null)
                            {
                                playerCharacter = clientByPlayerNickName.PlayerCharacter;
                            }
                            else
                            {
                                playerCharacter = bussiness.GetUserSingleByNickName(nickName);
                            }
                            if (playerCharacter != null)
                            {
                                if (!client.Player.Friends.ContainsKey(playerCharacter.ID) || (client.Player.Friends[playerCharacter.ID] != relation))
                                {
                                    FriendInfo info = new FriendInfo {
                                        FriendID = playerCharacter.ID,
                                        IsExist = true,
                                        Remark = "",
                                        UserID = client.Player.PlayerCharacter.ID,
                                        Relation = relation
                                    };
                                    if (bussiness.AddFriends(info))
                                    {
                                        client.Player.FriendsAdd(playerCharacter.ID, relation);
                                        if ((relation != 1) && (playerCharacter.State != 0))
                                        {
                                            GSPacketIn pkg = new GSPacketIn(160, client.Player.PlayerCharacter.ID);
                                            pkg.WriteByte(0xa6);
                                            pkg.WriteInt(playerCharacter.ID);
                                            pkg.WriteString(client.Player.PlayerCharacter.NickName);
                                            pkg.WriteBoolean(false);
                                            if (clientByPlayerNickName != null)
                                            {
                                                clientByPlayerNickName.SendTCP(pkg);
                                            }
                                            else
                                            {
                                                GameServer.Instance.LoginServer.SendPacket(pkg);
                                            }
                                        }
                                        client.Out.SendAddFriend(playerCharacter, relation, true);
                                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Success2", new object[0]));
                                    }
                                }
                                else
                                {
                                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Falied", new object[0]));
                                }
                            }
                            else
                            {
                                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("FriendAddHandler.Success", new object[0]) + nickName);
                            }
                            return 1;
                        }
                    }
                    return 1;
                }
                case 0xa1:
                    break;

                case 0xa2:
                case 0xa3:
                case 0xa4:
                    return 1;

                case 0xa5:
                    goto Label_0334;

                case 0x33:
                {
                    int playerId = packet.ReadInt();
                    string msg = packet.ReadString();
                    packet.ReadBoolean();
                    GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
                    if (playerById != null)
                    {
                        client.Player.Out.sendOneOnOneTalk(playerId, false, client.Player.PlayerCharacter.NickName, msg, client.Player.PlayerCharacter.ID);
                        playerById.Out.sendOneOnOneTalk(client.Player.PlayerCharacter.ID, false, client.Player.PlayerCharacter.NickName, msg, playerId);
                    }
                    else
                    {
                        client.Player.Out.SendMessage(eMessageType.Normal, "Người chơi kh\x00f4ng online!");
                    }
                    goto Label_049B;
                }
                case 0x2d:
                    goto Label_049B;

                default:
                    return 1;
            }
            int friendID = packet.ReadInt();
            using (PlayerBussiness bussiness2 = new PlayerBussiness())
            {
                if (bussiness2.DeleteFriends(client.Player.PlayerCharacter.ID, friendID))
                {
                    client.Player.FriendsRemove(friendID);
                    client.Out.SendFriendRemove(friendID);
                }
                return 1;
            }
        Label_0334:
            num5 = packet.ReadInt();
            GSPacketIn in2 = new GSPacketIn(160, client.Player.PlayerCharacter.ID);
            in2.WriteByte(0xa5);
            in2.WriteInt(num5);
            in2.WriteInt(client.Player.PlayerCharacter.typeVIP);
            in2.WriteInt(client.Player.PlayerCharacter.VIPLevel);
            in2.WriteBoolean(false);
            GameServer.Instance.LoginServer.SendPacket(in2);
            WorldMgr.ChangePlayerState(client.Player.PlayerCharacter.ID, num5, client.Player.PlayerCharacter.ConsortiaID);
        Label_049B:
            return 1;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x13, "用户场景聊天")]
    public class SceneChatHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ClientID = client.Player.PlayerCharacter.ID;
            byte val = packet.ReadByte();
            bool flag = packet.ReadBoolean();
            packet.ReadString();
            string str = packet.ReadString();
            GSPacketIn @in = new GSPacketIn(0x13, client.Player.PlayerCharacter.ID);
            @in.WriteInt(client.Player.ZoneId);
            @in.WriteByte(val);
            @in.WriteBoolean(flag);
            @in.WriteString(client.Player.PlayerCharacter.NickName);
            @in.WriteString(str);
            if (((client.Player.CurrentRoom != null) && (client.Player.CurrentRoom.RoomType == eRoomType.Match)) && (client.Player.CurrentRoom.Game != null))
            {
                client.Player.CurrentRoom.BattleServer.Server.SendChatMessage(str, client.Player, flag);
                return 1;
            }
            switch (val)
            {
                case 3:
                    if (client.Player.PlayerCharacter.ConsortiaID == 0)
                    {
                        return 0;
                    }
                    if (client.Player.PlayerCharacter.IsBanChat)
                    {
                        client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat", new object[0]));
                        return 1;
                    }
                    @in.WriteInt(client.Player.PlayerCharacter.ConsortiaID);
                    foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                    {
                        if (!((player.PlayerCharacter.ConsortiaID != client.Player.PlayerCharacter.ConsortiaID) || player.IsBlackFriend(client.Player.PlayerCharacter.ID)))
                        {
                            player.Out.SendTCP(@in);
                        }
                    }
                    break;

                case 9:
                    if (client.Player.CurrentMarryRoom == null)
                    {
                        return 1;
                    }
                    client.Player.CurrentMarryRoom.SendToAllForScene(@in, client.Player.MarryMap);
                    break;

                default:
                    if (client.Player.CurrentRoom != null)
                    {
                        if (flag)
                        {
                            client.Player.CurrentRoom.SendToTeam(@in, client.Player.CurrentRoomTeam, client.Player);
                        }
                        else
                        {
                            client.Player.CurrentRoom.SendToAll(@in);
                        }
                    }
                    else
                    {
                        if ((DateTime.Compare(client.Player.LastChatTime.AddSeconds(1.0), DateTime.Now) > 0) && (val == 5))
                        {
                            return 1;
                        }
                        if (flag)
                        {
                            return 1;
                        }
                        if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(30.0), DateTime.Now) > 0)
                        {
                            client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SceneChatHandler.Fast", new object[0]));
                            return 1;
                        }
                        client.Player.LastChatTime = DateTime.Now;
                        foreach (GamePlayer player2 in WorldMgr.GetAllPlayers())
                        {
                            if (!(((player2.CurrentRoom != null) || (player2.CurrentMarryRoom != null)) || player2.IsBlackFriend(client.Player.PlayerCharacter.ID)))
                            {
                                player2.Out.SendTCP(@in);
                            }
                            if (str.Equals("-goxu"))
                            {
                                int goXu = client.Player.PlayerCharacter.GoXu;
                                client.Out.SendMessage(eMessageType.Normal, string.Format("[Hệ Thống] Hiện tại bạn đang c\x00f3 {0} Goxu", goXu));
                                return 1;
                            }
                            if (str.Equals("-win"))
                            {
                                int win = client.Player.PlayerCharacter.Win;
                                client.Out.SendMessage(eMessageType.Normal, string.Format("[Hệ thống] Hiện tại bạn đang c\x00f3 {0} trận thắng", win));
                                return 1;
                            }
                            if (str.Equals("-online"))
                            {
                                int clientCount = client.Server.ClientCount;
                                GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
                                client.Out.SendMessage(eMessageType.Normal, string.Format("[Hệ thống] Hiện c\x00f3 {0} người online", clientCount));
                                return 1;
                            }
                            if (str.Equals("-lc"))
                            {
                                int fightPower = client.Player.PlayerCharacter.FightPower;
                                client.Out.SendMessage(eMessageType.Normal, string.Format("[Hệ Thống] Hiện tại lực chiến thực của bạn l\x00e0 {0}", fightPower));
                                return 1;
                            }
                            if (str.Equals("-admmk2"))
                            {
                                string passwordTwo = client.Player.PlayerCharacter.PasswordTwo;
                                client.Out.SendMessage(eMessageType.Normal, string.Format("[Hệ Thống] Passwords Rương của bạn l\x00e0 {0}", passwordTwo));
                                return 1;
                            }
                        }
                    }
                    break;
            }
            return 1;
        }
    }
}


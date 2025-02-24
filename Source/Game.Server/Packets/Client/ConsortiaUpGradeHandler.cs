namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x9f, "公会升级")]
    public class ConsortiaUpGradeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string str;
            ConsortiaInfo consortiaSingle;
            ConsortiaBussiness bussiness2;
            string translation;
            GSPacketIn @in;
            bool flag;
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                return 0;
            }
            byte num = packet.ReadByte();
            int num2 = packet.ReadInt();
            switch (num)
            {
                case 1:
                    str = "ConsortiaUpGradeHandler.Failed";
                    using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                    {
                        consortiaSingle = bussiness.GetConsortiaSingle(client.Player.PlayerCharacter.ConsortiaID);
                        if (consortiaSingle == null)
                        {
                            str = "ConsortiaUpGradeHandler.NoConsortia";
                        }
                        else
                        {
                            ConsortiaLevelInfo info2 = ConsortiaLevelMgr.FindConsortiaLevelInfo(consortiaSingle.Level + 1);
                            if (info2 == null)
                            {
                                str = "ConsortiaUpGradeHandler.NoUpGrade";
                            }
                            else if (info2.NeedGold > client.Player.PlayerCharacter.Gold)
                            {
                                str = "ConsortiaUpGradeHandler.NoGold";
                            }
                            else
                            {
                                using (bussiness2 = new ConsortiaBussiness())
                                {
                                    if (bussiness2.UpGradeConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref str))
                                    {
                                        consortiaSingle.Level++;
                                        client.Player.RemoveGold(info2.NeedGold);
                                        GameServer.Instance.LoginServer.SendConsortiaUpGrade(consortiaSingle);
                                        str = "ConsortiaUpGradeHandler.Success";
                                    }
                                }
                            }
                        }
                        if (consortiaSingle.Level >= 5)
                        {
                            translation = LanguageMgr.GetTranslation("ConsortiaUpGradeHandler.Notice", new object[] { consortiaSingle.ConsortiaName, consortiaSingle.Level });
                            @in = new GSPacketIn(10);
                            @in.WriteInt(2);
                            GameServer.Instance.LoginServer.SendPacket(@in);
                            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                            {
                                if ((player != client.Player) && (player.PlayerCharacter.ConsortiaID != client.Player.PlayerCharacter.ConsortiaID))
                                {
                                    player.Out.SendTCP(@in);
                                }
                            }
                        }
                    }
                    client.Player.SendMessage("Ch\x00fac mừng bạn n\x00e2ng cấp Guild th\x00e0nh c\x00f4ng!");
                    break;

                case 2:
                    if (client.Player.PlayerCharacter.ConsortiaID != 0)
                    {
                        flag = false;
                        str = "ConsortiaStoreUpGradeHandler.Failed";
                        consortiaSingle = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                        if (consortiaSingle == null)
                        {
                            str = "ConsortiaStoreUpGradeHandler.NoConsortia";
                            return 0;
                        }
                        using (bussiness2 = new ConsortiaBussiness())
                        {
                            if (bussiness2.UpGradeStoreConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref str))
                            {
                                consortiaSingle.StoreLevel++;
                                GameServer.Instance.LoginServer.SendConsortiaStoreUpGrade(consortiaSingle);
                                str = "ConsortiaStoreUpGradeHandler.Success";
                                flag = true;
                            }
                        }
                        client.Player.SendMessage("Ch\x00fac mừng bạn n\x00e2ng cấp K\x00e9t Guild th\x00e0nh c\x00f4ng!");
                        break;
                    }
                    return 0;

                case 3:
                    if (client.Player.PlayerCharacter.ConsortiaID != 0)
                    {
                        str = "ConsortiaShopUpGradeHandler.Failed";
                        consortiaSingle = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                        if (consortiaSingle == null)
                        {
                            str = "ConsortiaShopUpGradeHandler.NoConsortia";
                        }
                        else
                        {
                            using (bussiness2 = new ConsortiaBussiness())
                            {
                                if (bussiness2.UpGradeShopConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref str))
                                {
                                    consortiaSingle.ShopLevel++;
                                    GameServer.Instance.LoginServer.SendConsortiaShopUpGrade(consortiaSingle);
                                    str = "ConsortiaShopUpGradeHandler.Success";
                                }
                            }
                        }
                        if (consortiaSingle.ShopLevel >= 2)
                        {
                            translation = LanguageMgr.GetTranslation("ConsortiaShopUpGradeHandler.Notice", new object[] { client.Player.PlayerCharacter.ConsortiaName, consortiaSingle.ShopLevel });
                            @in = new GSPacketIn(10);
                            @in.WriteInt(2);
                            GameServer.Instance.LoginServer.SendPacket(@in);
                            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                            {
                                if (player != client.Player)
                                {
                                    player.Out.SendTCP(@in);
                                }
                            }
                        }
                        client.Player.SendMessage("Ch\x00fac mừng bạn n\x00e2ng cấp ShopGuild th\x00e0nh c\x00f4ng!");
                        break;
                    }
                    return 0;

                case 4:
                    if (client.Player.PlayerCharacter.ConsortiaID != 0)
                    {
                        flag = false;
                        str = "ConsortiaSmithUpGradeHandler.Failed";
                        consortiaSingle = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                        if (consortiaSingle == null)
                        {
                            str = "ConsortiaSmithUpGradeHandler.NoConsortia";
                        }
                        else
                        {
                            using (bussiness2 = new ConsortiaBussiness())
                            {
                                if (bussiness2.UpGradeSmithConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref str))
                                {
                                    consortiaSingle.SmithLevel++;
                                    GameServer.Instance.LoginServer.SendConsortiaSmithUpGrade(consortiaSingle);
                                    str = "ConsortiaSmithUpGradeHandler.Success";
                                    flag = true;
                                }
                            }
                        }
                        if (consortiaSingle.SmithLevel >= 3)
                        {
                            translation = LanguageMgr.GetTranslation("ConsortiaSmithUpGradeHandler.Notice", new object[] { client.Player.PlayerCharacter.ConsortiaName, consortiaSingle.SmithLevel });
                            @in = new GSPacketIn(10);
                            @in.WriteInt(2);
                            GameServer.Instance.LoginServer.SendPacket(@in);
                            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                            {
                                if (player != client.Player)
                                {
                                    player.Out.SendTCP(@in);
                                }
                            }
                        }
                        client.Player.SendMessage("Ch\x00fac mừng bạn n\x00e2ng cấp R\x00e8n th\x00e0nh c\x00f4ng!");
                        break;
                    }
                    return 0;
            }
            return 1;
        }
    }
}


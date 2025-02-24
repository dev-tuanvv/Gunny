namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Interface;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;
    using System.Text;

    [PacketHandler(1, "User Login handler")]
    public class UserLoginHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            try
            {
                if (client.Player == null)
                {
                    int num = packet.ReadInt();
                    packet.ReadInt();
                    byte[] data = new byte[8];
                    byte[] rgb = packet.ReadBytes();
                    try
                    {
                        rgb = WorldMgr.RsaCryptor.Decrypt(rgb, false);
                    }
                    catch (ExecutionEngineException exception)
                    {
                        client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError", new object[0]));
                        client.Disconnect();
                        GameServer.log.Error("ExecutionEngineException", exception);
                        return 0;
                    }
                    catch (Exception exception2)
                    {
                        client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError", new object[0]));
                        client.Disconnect();
                        GameServer.log.Error("RsaCryptor", exception2);
                        return 0;
                    }
                    string edition = GameServer.Edition;
                    for (int i = 0; i < 8; i++)
                    {
                        data[i] = rgb[i + 7];
                    }
                    client.setKey(data);
                    string[] strArray = Encoding.UTF8.GetString(rgb, 15, rgb.Length - 15).Split(new char[] { ',' });
                    if (strArray.Length == 2)
                    {
                        string account = strArray[0];
                        string pass = strArray[1];
                        if (!LoginMgr.ContainsUser(account))
                        {
                            bool isFirst = false;
                            PlayerInfo info = BaseInterface.CreateInterface().LoginGame(account, pass, ref isFirst);
                            if ((info != null) && (info.ID != 0))
                            {
                                if (info.ID == -2)
                                {
                                    client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid", new object[0]));
                                    client.Disconnect();
                                    Console.WriteLine("{0} Login Forbid....", account);
                                    return 0;
                                }
                                if (!isFirst)
                                {
                                    client.Player = new GamePlayer(info.ID, account, client, info);
                                    LoginMgr.Add(info.ID, client);
                                    client.Server.LoginServer.SendAllowUserLogin(info.ID);
                                    client.Version = num;
                                    Console.WriteLine("Tai Khoan {0} login Game ....", account);
                                }
                                else
                                {
                                    client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Register", new object[0]));
                                    client.Disconnect();
                                }
                            }
                            else
                            {
                                Console.WriteLine("{0} Login with {1} OverTime....", account, pass);
                                client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.OverTime", new object[0]));
                                client.Disconnect();
                            }
                        }
                        else
                        {
                            client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LoginError", new object[0]));
                            client.Disconnect();
                        }
                    }
                    else
                    {
                        client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LengthError", new object[0]));
                        client.Disconnect();
                    }
                }
            }
            catch (Exception exception3)
            {
                client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.ServerError", new object[0]));
                client.Disconnect();
                GameServer.log.Error(LanguageMgr.GetTranslation("UserLoginHandler.ServerError", new object[0]), exception3);
            }
            return 1;
        }
    }
}


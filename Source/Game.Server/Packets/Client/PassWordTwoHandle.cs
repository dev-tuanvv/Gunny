namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x19, "二级密码")]
    public class PassWordTwoHandle : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string translateId = "二级密码";
            bool val = false;
            int num = 0;
            bool flag2 = false;
            int count = 0;
            string passwordTwo = packet.ReadString();
            string str3 = packet.ReadString();
            int num3 = packet.ReadInt();
            string str4 = packet.ReadString();
            string str5 = packet.ReadString();
            string str6 = packet.ReadString();
            string str7 = packet.ReadString();
            switch (num3)
            {
                case 1:
                    num = 1;
                    if (string.IsNullOrEmpty(client.Player.PlayerCharacter.PasswordTwo))
                    {
                        using (PlayerBussiness bussiness = new PlayerBussiness())
                        {
                            if ((passwordTwo != "") && bussiness.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, passwordTwo))
                            {
                                client.Player.PlayerCharacter.PasswordTwo = passwordTwo;
                                client.Player.PlayerCharacter.IsLocked = false;
                                translateId = "SetPassword.success";
                            }
                            if ((((str4 != "") && (str5 != "")) && (str6 != "")) && (str7 != ""))
                            {
                                if (bussiness.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, str4, str5, str6, str7, 5))
                                {
                                    client.Player.PlayerCharacter.PasswordQuest1 = str4;
                                    client.Player.PlayerCharacter.PasswordQuest2 = str5;
                                    client.Player.PlayerCharacter.FailedPasswordAttemptCount = 5;
                                    val = true;
                                    flag2 = false;
                                    translateId = "UpdatePasswordInfo.Success";
                                }
                                else
                                {
                                    val = false;
                                }
                            }
                            else
                            {
                                val = true;
                                flag2 = true;
                            }
                            break;
                        }
                    }
                    translateId = "SetPassword.Fail";
                    val = false;
                    flag2 = false;
                    break;

                case 2:
                    num = 2;
                    if (!(passwordTwo == client.Player.PlayerCharacter.PasswordTwo))
                    {
                        translateId = "PasswordTwo.error";
                        val = false;
                        flag2 = false;
                        break;
                    }
                    client.Player.PlayerCharacter.IsLocked = false;
                    translateId = "BagUnlock.success";
                    val = true;
                    break;

                case 3:
                {
                    num = 3;
                    using (PlayerBussiness bussiness2 = new PlayerBussiness())
                    {
                        bussiness2.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref str4, ref str5, ref str6, ref str7, ref count);
                        count--;
                        bussiness2.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, str4, str5, str6, str7, count);
                        if (passwordTwo == client.Player.PlayerCharacter.PasswordTwo)
                        {
                            if (bussiness2.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, str3))
                            {
                                client.Player.PlayerCharacter.IsLocked = false;
                                client.Player.PlayerCharacter.PasswordTwo = str3;
                                translateId = "UpdatePasswordTwo.Success";
                                val = true;
                                flag2 = false;
                            }
                            else
                            {
                                translateId = "UpdatePasswordTwo.Fail";
                                val = false;
                                flag2 = false;
                            }
                        }
                        else
                        {
                            translateId = "PasswordTwo.error";
                            val = false;
                            flag2 = false;
                        }
                        break;
                    }
                }
                case 4:
                {
                    num = 4;
                    string str8 = "";
                    string str9 = "";
                    string str10 = "";
                    using (PlayerBussiness bussiness3 = new PlayerBussiness())
                    {
                        bussiness3.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref str4, ref str8, ref str6, ref str10, ref count);
                        count--;
                        bussiness3.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, str4, str5, str6, str7, count);
                        if ((((str8 == str5) && (str10 == str7)) && (str8 != "")) && (str10 != ""))
                        {
                            if (bussiness3.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, str9))
                            {
                                client.Player.PlayerCharacter.PasswordTwo = str9;
                                client.Player.PlayerCharacter.IsLocked = false;
                                translateId = "DeletePassword.success";
                                val = true;
                                flag2 = false;
                            }
                            else
                            {
                                translateId = "DeletePassword.Fail";
                                val = false;
                            }
                        }
                        else if (passwordTwo == client.Player.PlayerCharacter.PasswordTwo)
                        {
                            if (bussiness3.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, str9))
                            {
                                client.Player.PlayerCharacter.PasswordTwo = str9;
                                client.Player.PlayerCharacter.IsLocked = false;
                                translateId = "DeletePassword.success";
                                val = true;
                                flag2 = false;
                            }
                        }
                        else
                        {
                            translateId = "DeletePassword.Fail";
                            val = false;
                        }
                        break;
                    }
                }
                case 5:
                    num = 5;
                    if ((((client.Player.PlayerCharacter.PasswordTwo != null) && (str4 != "")) && ((str5 != "") && (str6 != ""))) && (str7 != ""))
                    {
                        using (PlayerBussiness bussiness4 = new PlayerBussiness())
                        {
                            if (bussiness4.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, str4, str5, str6, str7, 5))
                            {
                                val = true;
                                flag2 = false;
                                translateId = "UpdatePasswordInfo.Success";
                            }
                            else
                            {
                                val = false;
                            }
                        }
                    }
                    break;
            }
            GSPacketIn @in = new GSPacketIn(0x19, client.Player.PlayerCharacter.ID);
            @in.WriteInt(client.Player.PlayerCharacter.ID);
            @in.WriteInt(num);
            @in.WriteBoolean(val);
            @in.WriteBoolean(flag2);
            @in.WriteString(LanguageMgr.GetTranslation(translateId, new object[0]));
            @in.WriteInt(count);
            @in.WriteString(str4);
            @in.WriteString(str6);
            client.Out.SendTCP(@in);
            return 0;
        }
    }
}


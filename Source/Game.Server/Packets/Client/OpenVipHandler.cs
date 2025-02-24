namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x5c, "VIP")]
    public class OpenVipHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string nickName = packet.ReadString();
            int renewalDays = packet.ReadInt();
            int num2 = 0x239;
            int num3 = 0x239;
            int num4 = 0x6ab;
            int num5 = 0xbb8;
            string message = "K\x00edch hoạt VIP th\x00e0nh c\x00f4ng!";
            int thoigian = renewalDays;
            switch (thoigian)
            {
                case 30:
                    num2 = num3;
                    break;

                case 90:
                    num2 = num4;
                    break;

                case 180:
                    num2 = num5;
                    break;
            }
            GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(nickName);
            if (client.Player.MoneyDirect(num2))
            {
                DateTime now = DateTime.Now;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    bussiness.VIPRenewal(nickName, renewalDays, ref now);
                    if (clientByPlayerNickName == null)
                    {
                        message = "Tiếp ph\x00ed VIP cho " + nickName + " th\x00e0ng c\x00f4ng!";
                    }
                    else if (client.Player.PlayerCharacter.NickName == nickName)
                    {
                        if (client.Player.PlayerCharacter.typeVIP == 0)
                        {
                            client.Player.OpenVIP(thoigian, now);
                        }
                        else
                        {
                            client.Player.ContinousVIP(thoigian, now);
                            message = "Gia hạn VIP th\x00e0nh c\x00f4ng!";
                        }
                        client.Out.SendOpenVIP(client.Player.PlayerCharacter);
                    }
                    else
                    {
                        string str3;
                        if (clientByPlayerNickName.PlayerCharacter.typeVIP == 0)
                        {
                            clientByPlayerNickName.OpenVIP(thoigian, now);
                            message = "K\x00edch hoạt VIP cho " + nickName + " th\x00e0ng c\x00f4ng!";
                            str3 = client.Player.PlayerCharacter.NickName + ", tiếp ph\x00ed VIP cho bạn th\x00e0nh c\x00f4ng!";
                        }
                        else
                        {
                            clientByPlayerNickName.ContinousVIP(thoigian, now);
                            message = "Gia hạn VIP cho " + nickName + " th\x00e0ng c\x00f4ng!";
                            str3 = client.Player.PlayerCharacter.NickName + ", gia hạn VIP cho bạn th\x00e0nh c\x00f4ng!";
                        }
                        clientByPlayerNickName.Out.SendOpenVIP(clientByPlayerNickName.PlayerCharacter);
                        clientByPlayerNickName.Out.SendMessage(eMessageType.Normal, str3);
                    }
                    client.Player.AddExpVip(num2);
                    client.Out.SendMessage(eMessageType.Normal, message);
                }
            }
            return 0;
        }
    }
}


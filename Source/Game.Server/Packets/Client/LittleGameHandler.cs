namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0xa6, "场景用户离开")]
    public class LittleGameHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int iD = client.Player.PlayerCharacter.ID;
            GSPacketIn pkg = new GSPacketIn(0xa6, iD);
            switch (num)
            {
                case 2:
                    pkg.WriteByte(2);
                    pkg.WriteInt(1);
                    pkg.WriteInt(1);
                    pkg.WriteString("bogu1,bogu2,bogu3,bogu4,bogu5,bogu6,bogu7,bogu8");
                    pkg.WriteString("2001");
                    client.Player.SendTCP(pkg);
                    break;

                case 3:
                {
                    int val = 1;
                    pkg.WriteByte(3);
                    pkg.WriteInt(val);
                    for (int i = 0; i < val; i++)
                    {
                        pkg.WriteInt(1);
                        pkg.WriteInt(0x7f7);
                        pkg.WriteInt(0x7b);
                        pkg.WriteInt(1);
                        pkg.WriteInt(client.Player.PlayerCharacter.ID);
                        pkg.WriteInt(client.Player.PlayerCharacter.Grade);
                        pkg.WriteInt(client.Player.PlayerCharacter.Repute);
                        pkg.WriteString(client.Player.PlayerCharacter.NickName);
                        pkg.WriteByte(client.Player.PlayerCharacter.typeVIP);
                        pkg.WriteInt(client.Player.PlayerCharacter.VIPLevel);
                        pkg.WriteBoolean(client.Player.PlayerCharacter.Sex);
                        pkg.WriteString(client.Player.PlayerCharacter.Style);
                        pkg.WriteString(client.Player.PlayerCharacter.Colors);
                        pkg.WriteString(client.Player.PlayerCharacter.Skin);
                        pkg.WriteInt(client.Player.PlayerCharacter.Hide);
                        pkg.WriteInt(client.Player.PlayerCharacter.FightPower);
                        pkg.WriteInt(client.Player.PlayerCharacter.Win);
                        pkg.WriteInt(client.Player.PlayerCharacter.Total);
                        pkg.WriteBoolean(false);
                        int num5 = 1;
                        int num6 = 0;
                        pkg.WriteInt(num5);
                        while (num6 < num5)
                        {
                            pkg.WriteString("livingInhale");
                            pkg.WriteInt(1);
                            pkg.WriteString("stand");
                            pkg.WriteString("1");
                            pkg.WriteInt(1);
                            pkg.WriteInt(0x7f7);
                            pkg.WriteInt(0x7b);
                            num6++;
                        }
                    }
                    client.Player.SendTCP(pkg);
                    break;
                }
            }
            return 0;
        }
    }
}


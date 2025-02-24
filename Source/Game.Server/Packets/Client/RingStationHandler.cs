namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x194, "场景用户离开")]
    public class RingStationHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int iD = client.Player.PlayerCharacter.ID;
            GSPacketIn pkg = new GSPacketIn(0x194, iD);
            byte num3 = num;
            if (num3 == 1)
            {
                pkg.WriteByte(1);
                pkg.WriteInt(1);
                pkg.WriteInt(10);
                pkg.WriteInt(10);
                pkg.WriteInt(0x13880);
                pkg.WriteDateTime(DateTime.Now);
                pkg.WriteInt(0x186a0);
                pkg.WriteInt(0);
                pkg.WriteString("E h\x00e8m Sắp ra Mắt L\x00f4i Đ\x00e0i");
                pkg.WriteInt(300);
                pkg.WriteDateTime(DateTime.Now.AddDays(7.0));
                pkg.WriteString("E h\x00e8m");
                pkg.WriteInt(4);
                for (int i = 0; i < 4; i++)
                {
                    pkg.WriteInt(iD);
                    pkg.WriteString(client.Player.PlayerCharacter.UserName);
                    pkg.WriteString(client.Player.PlayerCharacter.NickName);
                    pkg.WriteByte(client.Player.PlayerCharacter.typeVIP);
                    pkg.WriteInt(client.Player.PlayerCharacter.VIPLevel);
                    pkg.WriteInt(client.Player.PlayerCharacter.Grade);
                    pkg.WriteBoolean(client.Player.PlayerCharacter.Sex);
                    pkg.WriteString(client.Player.PlayerCharacter.Style);
                    pkg.WriteString(client.Player.PlayerCharacter.Colors);
                    pkg.WriteString(client.Player.PlayerCharacter.Skin);
                    pkg.WriteString(client.Player.PlayerCharacter.ConsortiaName);
                    pkg.WriteInt(client.Player.PlayerCharacter.Hide);
                    pkg.WriteInt(client.Player.PlayerCharacter.Offer);
                    pkg.WriteInt(client.Player.PlayerCharacter.Win);
                    pkg.WriteInt(client.Player.PlayerCharacter.Total);
                    pkg.WriteInt(client.Player.PlayerCharacter.Escape);
                    pkg.WriteInt(client.Player.PlayerCharacter.Repute);
                    pkg.WriteInt(client.Player.PlayerCharacter.Nimbus);
                    pkg.WriteInt(client.Player.PlayerCharacter.GP);
                    pkg.WriteInt(client.Player.PlayerCharacter.FightPower);
                    pkg.WriteInt(client.Player.PlayerCharacter.AchievementPoint);
                    pkg.WriteInt(2 + i);
                    if (client.Player.MainWeapon == null)
                    {
                        pkg.WriteInt(0x1b60);
                    }
                    else
                    {
                        pkg.WriteInt(client.Player.MainWeapon.TemplateID);
                    }
                    pkg.WriteString("Nh\x00f3c Qu\x00fd Tử");
                }
                client.Player.SendTCP(pkg);
            }
            else if (num3 == 6)
            {
                pkg.WriteByte(6);
                pkg.WriteInt(0);
                pkg.WriteDateTime(DateTime.Now);
                client.Player.SendTCP(pkg);
            }
            else
            {
                Console.WriteLine("RingStationPackageType." + ((RingStationPackageType) num));
            }
            return 0;
        }
    }
}


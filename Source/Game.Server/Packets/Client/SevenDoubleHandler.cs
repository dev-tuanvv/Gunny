namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x94, "物品强化")]
    public class SevenDoubleHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            GSPacketIn @in = new GSPacketIn(0x94);
            switch (num)
            {
                case 3:
                {
                    int val = packet.ReadInt();
                    packet.ReadBoolean();
                    @in.WriteByte(3);
                    @in.WriteInt(val);
                    client.Out.SendTCP(@in);
                    return 0;
                }
                case 6:
                    @in.WriteByte(6);
                    client.Out.SendTCP(@in);
                    return 0;

                case 7:
                    return 0;

                case 9:
                    @in.WriteByte(0x10);
                    @in.WriteDateTime(DateTime.Now.AddMinutes(20.0));
                    client.Out.SendTCP(@in);
                    return 0;

                case 0x23:
                    @in.WriteByte(0x23);
                    @in.WriteBoolean(true);
                    client.Out.SendTCP(@in);
                    return 0;
            }
            Console.WriteLine("SevenDoubleHandler." + ((SevenDoublePackageType) num));
            return 0;
        }

        public void SendSevenDoubleStartGame(GamePlayer player)
        {
            GSPacketIn pkg = new GSPacketIn(0x94);
            pkg.WriteByte(8);
            pkg.WriteInt(1);
            pkg.WriteInt(4);
            pkg.WriteInt(player.PlayerCharacter.ID);
            pkg.WriteInt(2);
            pkg.WriteString(player.PlayerCharacter.NickName);
            player.SendTCP(pkg);
        }
    }
}


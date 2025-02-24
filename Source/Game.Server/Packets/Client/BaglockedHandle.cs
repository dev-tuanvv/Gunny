namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x193, "二级密码")]
    public class BaglockedHandle : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadByte();
            GSPacketIn @in = new GSPacketIn(0x193, client.Player.PlayerCharacter.ID);
            if (num == 5)
            {
                @in.WriteByte(5);
                @in.WriteBoolean(true);
                client.Out.SendTCP(@in);
            }
            else
            {
                Console.WriteLine("BaglockedPackageType." + ((BaglockedPackageType) num));
            }
            return 0;
        }
    }
}


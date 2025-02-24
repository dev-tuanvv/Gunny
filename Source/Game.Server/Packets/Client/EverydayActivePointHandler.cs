namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x9b, "场景用户离开")]
    public class EverydayActivePointHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int iD = client.Player.PlayerCharacter.ID;
            if (num == 8)
            {
                client.Player.Out.SendExpBlessedData(iD);
            }
            else
            {
                Console.WriteLine("ActivityPackageType." + ((ActivityPackageType) num));
            }
            return 0;
        }
    }
}


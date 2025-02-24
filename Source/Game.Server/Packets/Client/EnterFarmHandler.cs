namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x51, "游戏创建")]
    public class EnterFarmHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ReadBoolean();
            int type = packet.ReadInt();
            int num2 = packet.ReadInt();
            int roomId = -1;
            string pwd = null;
            if (num2 == -1)
            {
                roomId = packet.ReadInt();
                pwd = packet.ReadString();
            }
            switch (type)
            {
                case 1:
                    type = 0;
                    break;

                case 2:
                    type = 4;
                    break;
            }
            RoomMgr.EnterRoom(client.Player, roomId, pwd, type);
            return 0;
        }
    }
}


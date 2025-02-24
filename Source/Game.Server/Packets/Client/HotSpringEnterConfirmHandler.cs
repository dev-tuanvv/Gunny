namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using System;

    [PacketHandler(0xd4, "礼堂数据")]
    public class HotSpringEnterConfirmHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            if (client.Player.CurrentHotSpringRoom == null)
            {
                if (HotSpringMgr.GetHotSpringRoombyID(id) != null)
                {
                    GSPacketIn @in = new GSPacketIn(0xd4);
                    @in.WriteInt(id);
                    client.Out.SendTCP(@in);
                }
                else
                {
                    client.Player.SendMessage("Ph\x00f2ng đ\x00e3 đầy");
                }
            }
            return 0;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Rooms;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x62, "客户端日记")]
    public class SearchGoodsHandler : IPacketHandler
    {
        private readonly int[] mapID = new int[] { 1, 2, 3 };
        private static ThreadSafeRandom rand = new ThreadSafeRandom();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if ((client.Player.CurrentRoom != null) && (client.Player == client.Player.CurrentRoom.Host))
            {
                RoomMgr.KickPlayer(client.Player.CurrentRoom, packet.ReadByte());
            }
            return 0;
        }

        private GSPacketIn PlayerEnter(PlayerExtra Extra)
        {
            GSPacketIn @in = new GSPacketIn(0x62);
            @in.WriteByte(0x10);
            @in.WriteInt(Extra.MapId);
            @in.WriteInt(Extra.Info.starlevel);
            @in.WriteInt(Extra.Info.nowPosition);
            @in.WriteInt(Extra.Info.FreeCount);
            @in.WriteInt(Extra.SearchGoodItems.Count);
            foreach (EventAwardInfo info in Extra.SearchGoodItems)
            {
                @in.WriteInt(info.Position);
                @in.WriteInt(info.TemplateID);
            }
            return @in;
        }
    }
}


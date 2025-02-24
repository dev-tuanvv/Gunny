namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(210, "物品炼化")]
    public class ItemRefineryHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if ((client.Player.CurrentRoom != null) && (client.Player.CurrentRoom.BattleServer != null))
            {
                client.Player.CurrentRoom.BattleServer.RemoveRoom(client.Player.CurrentRoom);
                client.Player.CurrentRoom.SendCancelPickUp();
                RoomMgr.UpdatePlayerState(client.Player, 2);
            }
            return 0;
        }
    }
}


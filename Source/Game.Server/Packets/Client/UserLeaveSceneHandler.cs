namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x15, "场景用户离开")]
    public class UserLeaveSceneHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            client.Player.PlayerState = ePlayerState.Online;
            RoomMgr.ExitWaitingRoom(client.Player);
            return 0;
        }
    }
}


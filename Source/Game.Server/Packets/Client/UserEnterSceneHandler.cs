namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x10, "Player enter scene.")]
    public class UserEnterSceneHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            switch (packet.ReadInt())
            {
                case 1:
                    client.Player.PlayerState = ePlayerState.Online;
                    break;

                case 2:
                    client.Player.PlayerState = ePlayerState.Away;
                    break;

                default:
                    client.Player.PlayerState = ePlayerState.Online;
                    break;
            }
            RoomMgr.WaitingRoom.SendSceneUpdate(client.Player);
            return 1;
        }
    }
}


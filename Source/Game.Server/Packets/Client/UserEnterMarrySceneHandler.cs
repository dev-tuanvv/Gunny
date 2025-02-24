namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.SceneMarryRooms;
    using System;

    [PacketHandler(240, "Player enter marry scene.")]
    public class UserEnterMarrySceneHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn @in = new GSPacketIn(240, client.Player.PlayerCharacter.ID);
            if (WorldMgr.MarryScene.AddPlayer(client.Player))
            {
                @in.WriteBoolean(true);
            }
            else
            {
                @in.WriteBoolean(false);
            }
            client.Out.SendTCP(@in);
            if (client.Player.CurrentMarryRoom == null)
            {
                foreach (MarryRoom room in MarryRoomMgr.GetAllMarryRoom())
                {
                    client.Player.Out.SendMarryRoomInfo(client.Player, room);
                }
            }
            return 0;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.HotSpringRooms;
    using Game.Server.Managers;
    using System;

    [PacketHandler(0xbb, "礼堂数据")]
    public class HotSpringEnterHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (WorldMgr.HotSpringScene.GetClientFromID(client.Player.PlayerCharacter.ID) == null)
            {
                WorldMgr.HotSpringScene.AddPlayer(client.Player);
            }
            HotSpringRoom[] allHotSpringRoom = HotSpringMgr.GetAllHotSpringRoom();
            HotSpringMgr.SendUpdateAllRoom(client.Player, allHotSpringRoom);
            return 0;
        }
    }
}


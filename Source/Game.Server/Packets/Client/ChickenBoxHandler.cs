namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x57, "客户端日记")]
    public class ChickenBoxHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.MainWeapon == null)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
                return 0;
            }
            if (client.Player.CurrentRoom != null)
            {
                RoomMgr.UpdatePlayerState(client.Player, packet.ReadByte());
            }
            return 0;
        }
    }
}


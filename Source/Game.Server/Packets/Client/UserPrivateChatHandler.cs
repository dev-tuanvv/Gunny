namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x25, "用户与用户之间的聊天")]
    public class UserPrivateChatHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GameServer.Instance.LoginServer.SendPacket(packet);
            return 1;
        }
    }
}


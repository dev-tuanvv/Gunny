namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    public interface IPacketHandler
    {
        int HandlePacket(GameClient client, GSPacketIn packet);
    }
}


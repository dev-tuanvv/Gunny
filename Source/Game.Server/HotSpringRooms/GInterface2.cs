namespace Game.Server.HotSpringRooms
{
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using System;

    public interface GInterface2
    {
        void OnGameData(HotSpringRoom game, GamePlayer player, GSPacketIn packet);
        void OnTick(HotSpringRoom room);
    }
}


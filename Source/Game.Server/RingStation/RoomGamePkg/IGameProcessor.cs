namespace Game.Server.RingStation.RoomGamePkg
{
    using Game.Base.Packets;
    using Game.Server.RingStation;
    using System;

    public interface IGameProcessor
    {
        void OnGameData(RoomGame game, RingStationGamePlayer player, GSPacketIn packet);
        void OnTick(RoomGame room);
    }
}


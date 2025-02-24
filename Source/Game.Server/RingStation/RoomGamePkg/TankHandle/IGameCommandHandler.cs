namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
    using Game.Base.Packets;
    using Game.Server.RingStation;
    using Game.Server.RingStation.RoomGamePkg;
    using System;

    public interface IGameCommandHandler
    {
        bool HandleCommand(TankGameLogicProcessor process, RingStationGamePlayer player, GSPacketIn packet);
    }
}


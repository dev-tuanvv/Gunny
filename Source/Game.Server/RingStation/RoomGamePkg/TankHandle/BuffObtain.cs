namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
    using Game.Base.Packets;
    using Game.Server.RingStation;
    using Game.Server.RingStation.RoomGamePkg;
    using System;

    [GameCommandAttbute(0xba)]
    public class BuffObtain : IGameCommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, RingStationGamePlayer player, GSPacketIn packet)
        {
            return true;
        }
    }
}


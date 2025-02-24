namespace Game.Server.RingStation.RoomGamePkg
{
    using Game.Base.Packets;
    using Game.Server.RingStation;
    using System;

    public abstract class AbstractGameProcessor : IGameProcessor
    {
        protected AbstractGameProcessor()
        {
        }

        public virtual void OnGameData(RoomGame game, RingStationGamePlayer player, GSPacketIn packet)
        {
        }

        public virtual void OnTick(RoomGame room)
        {
        }
    }
}


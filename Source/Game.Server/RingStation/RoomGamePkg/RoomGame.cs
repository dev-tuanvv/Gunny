namespace Game.Server.RingStation.RoomGamePkg
{
    using Game.Base.Packets;
    using Game.Server.RingStation;
    using System;

    public class RoomGame
    {
        private IGameProcessor _processor = new TankGameLogicProcessor();
        private static object _syncStop = new object();

        protected void OnTick(object obj)
        {
            this._processor.OnTick(this);
        }

        public void ProcessData(RingStationGamePlayer player, GSPacketIn data)
        {
            lock (_syncStop)
            {
                this._processor.OnGameData(this, player, data);
            }
        }
    }
}


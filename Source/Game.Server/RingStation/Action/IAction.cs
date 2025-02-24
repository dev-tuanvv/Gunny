namespace Game.Server.RingStation.Action
{
    using Game.Server.RingStation;
    using System;

    public interface IAction
    {
        void Execute(RingStationGamePlayer player, long tick);
        bool IsFinished(long tick);
    }
}


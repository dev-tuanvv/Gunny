namespace Game.Server.Packets
{
    using System;

    public enum BattleGoundPackageType
    {
        OPEN = 1,
        OVER = 2,
        UPDATE_PLAYER_DATA = 5,
        UPDATE_VALUE = 7,
        UPDATE_VALUE_REP = 4,
        UPDATE_VALUE_REQ = 3
    }
}


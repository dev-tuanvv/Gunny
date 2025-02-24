namespace Game.Server.Packets
{
    using System;

    public enum DragonBoatPackageType
    {
        BUILD_DECORATE = 2,
        EXCHANGE = 4,
        REFRESH_BOAT_STATUS = 3,
        REFRESH_RANK = 0x10,
        REFRESH_RANK_OTHER = 0x11,
        START_OR_CLOSE = 1
    }
}


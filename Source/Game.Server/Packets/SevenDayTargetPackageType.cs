namespace Game.Server.Packets
{
    using System;

    public enum SevenDayTargetPackageType
    {
        NEWPLAYERREWARD_ENTER = 0x61,
        NEWPLAYERREWARD_GET_REWARD = 0x62,
        NEWPLAYERREWARD_OPEN_CLOSE = 0x60,
        SEVENDAYTARGET_ENTER = 0x51,
        SEVENDAYTARGET_GET_REWARD = 0x52,
        SEVENDAYTARGET_OPEN_CLOSE = 80
    }
}


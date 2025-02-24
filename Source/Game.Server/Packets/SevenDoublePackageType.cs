namespace Game.Server.Packets
{
    using System;

    public enum SevenDoublePackageType
    {
        ALL_READY = 0x10,
        ARRIVE = 0x17,
        CALL = 3,
        CANCEL_GAME = 7,
        DESTROY = 0x16,
        ENTER_GAME = 8,
        ENTER_OR_LEAVE_SCENE = 0x26,
        IS_CAN_ENTER = 0x23,
        MOVE = 0x11,
        RANK_LIST = 0x18,
        RE_ENTER_ALL_INFO = 0x22,
        READY = 9,
        REFRESH_BUFF = 20,
        REFRESH_ENTER_COUNT = 0x27,
        REFRESH_FIGHT_STATE = 0x24,
        REFRESH_ITEM = 0x13,
        START_GAME = 6,
        START_OR_END = 1,
        USE_SKILL = 0x15
    }
}


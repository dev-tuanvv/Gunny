namespace Game.Server.Packets
{
    using System;

    public enum LittleGamePackageIn
    {
        CANCEL_CLICK = 0x42,
        CLICK = 0x41,
        ENTER_WORLD = 2,
        LEAVE_WORLD = 4,
        LOAD_COMPLETED = 3,
        LOAD_WORLD_LIST = 1,
        MOVE = 0x20,
        PING = 6,
        POS_SYNC = 0x21,
        REPORT_SCORE = 0x40
    }
}


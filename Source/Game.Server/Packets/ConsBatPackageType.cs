namespace Game.Server.Packets
{
    using System;

    public enum ConsBatPackageType
    {
        ADD_PLAYER = 3,
        BROADCAST = 0x13,
        CONFIRM_ENTER = 0x15,
        CONSUME = 0x11,
        CHALLENGE = 6,
        DELETE_PLAYER = 5,
        ENTER_SELF_INFO = 2,
        PLAYER_MOVE = 4,
        PLAYER_STATUS = 7,
        SPLIT_MERGE = 8,
        START_OR_CLOSE = 1,
        UPDATE_SCENE_INFO = 9,
        UPDATE_SCORE = 0x10
    }
}


namespace Game.Server.Packets
{
    using System;

    public enum LabyrinthPackageType
    {
        CLEAN_OUT = 3,
        CLEAN_OUT_COMPLETE = 8,
        DOUBLE_REWARD = 1,
        PUSH_CLEAN_OUT_INFO = 7,
        REQUEST_UPDATE = 2,
        RESET_LABYRINTH = 6,
        SPEEDED_UP_CLEAN_OUT = 4,
        STOP_CLEAN_OUT = 5,
        TRY_AGAIN = 9
    }
}


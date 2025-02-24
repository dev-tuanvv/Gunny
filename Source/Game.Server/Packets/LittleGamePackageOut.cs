namespace Game.Server.Packets
{
    using System;

    public enum LittleGamePackageOut
    {
        ADD_OBJECT = 0x40,
        ADD_SPRITE = 0x10,
        DoAction = 0x60,
        DoMovie = 0x51,
        GAME_START = 3,
        GETSCORE = 0x31,
        INVOKE_OBJECT = 0x42,
        KICK_PLAYE = 0x12,
        MOVE = 0x20,
        NET_DELAY = 7,
        PONG = 6,
        REMOVE_OBJECT = 0x41,
        REMOVE_SPRITE = 0x11,
        SETCLOCK = 5,
        START_LOAD = 2,
        UPDATE_POS = 0x21,
        UPDATELIVINGSPROPERTY = 80,
        WORLD_LIST = 1
    }
}


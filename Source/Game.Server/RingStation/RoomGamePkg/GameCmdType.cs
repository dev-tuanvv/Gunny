namespace Game.Server.RingStation.RoomGamePkg
{
    using System;

    public enum GameCmdType
    {
        BUFF_OBTAIN = 0xba,
        DISCONNECT = 0x53,
        GAME_CMD = 0x5b,
        GAME_MISSION_START = 0x52,
        GAME_ROOM = 0x5e,
        GAME_ROOM_REMOVEPLAYER = 5,
        SYS_MESSAGE = 3
    }
}


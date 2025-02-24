namespace Game.Server.Packets
{
    using System;

    public enum GameRoomPackageType
    {
        FAST_INVITE_CALL = 0x13,
        GAME_ENERGY_NOT_ENOUGH = 20,
        GAME_PICKUP_CANCEL = 11,
        GAME_PICKUP_STYLE = 12,
        GAME_PICKUP_WAIT = 13,
        GAME_PLAYER_STATE_CHANGE = 15,
        GAME_ROOM_ADDPLAYER = 4,
        GAME_ROOM_CREATE = 0,
        GAME_ROOM_FULL = 0x11,
        GAME_ROOM_KICK = 3,
        GAME_ROOM_LOGIN = 1,
        GAME_ROOM_REMOVEPLAYER = 5,
        GAME_ROOM_SETUP_CHANGE = 2,
        GAME_ROOM_UPDATE_PLACE = 10,
        GAME_START = 7,
        GAME_TEAM = 6,
        LAST_MISSION_FOR_WARRIORSARENA = 0x21,
        No_WARRIORSARENA_TICKET = 0x23,
        PASSED_WARRIORSARENA_10 = 0x22,
        ROOM_PASS = 14,
        ROOMLIST_UPDATE = 9,
        SINGLE_ROOM_BEGIN = 0x12
    }
}


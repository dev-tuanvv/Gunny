namespace Game.Server.Packets
{
    using System;

    public enum ChristmasPackageType
    {
        ADDPLAYER = 0x12,
        CHRISTMAS_BUY_TIMER = 0x1d,
        CHRISTMAS_EXIT = 0x13,
        CHRISTMAS_MAKING_SNOWMAN_ENTER = 0x18,
        CHRISTMAS_MONSTER = 0x16,
        CHRISTMAS_OPENORCLOSE = 0x10,
        CHRISTMAS_PACKS = 0x1a,
        CHRISTMAS_PLAYERING_SNOWMAN_ENTER = 0x11,
        CHRISTMAS_ROOM_SPEAK = 0x1c,
        CHRISTMAS_SCORECONVERT = 5,
        FIGHT_MONSTER = 0x16,
        FIGHT_SPIRIT_LEVELUP = 0x19,
        GET_PAKCS_TO_PLAYER = 0x1b,
        MOVE = 0x15,
        PLAYER_STATUE = 20,
        UPDATE_TIMES_ROOM = 30
    }
}


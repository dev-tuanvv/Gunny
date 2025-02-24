namespace Game.Server.Packets
{
    using System;

    public enum CampPackageType
    {
        ACTION_ISOPEN = 10,
        ADD_MONSTER_LIST = 4,
        ADD_ROLE_LIST = 1,
        CAMP_SOCER_RANK = 20,
        CAPTURE_MAP = 0x11,
        DOOR_STATUS = 0x17,
        ENTER_MONSTER_FIGHT = 5,
        INIT_SECEN = 6,
        MAP_CHANGE = 9,
        MONSTER_STATE_CHANGE = 7,
        OUT_CAMPBATTLE = 0x19,
        PER_SCORE_RANK = 0x15,
        PLAYER_STATE_CHANGE = 8,
        PVP_TO_FIGHT = 0x10,
        REMOVE_ROLE = 3,
        RESURRECT = 0x12,
        ROLE_MOVE = 2,
        UPDATE_SCORE = 0x16,
        WIN_COUNT_PTP = 0x13
    }
}


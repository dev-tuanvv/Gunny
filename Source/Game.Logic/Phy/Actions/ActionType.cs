namespace Game.Logic.Phy.Actions
{
    using System;

    public enum ActionType
    {
        AUP = 0x13,
        BOMB = 2,
        BOMBED = 0x11,
        CURE = 10,
        CHANGE_SPEED = 8,
        CHANGE_STATE = 12,
        DANDER = 9,
        DO_ACTION = 13,
        FLY_OUT = 4,
        FORZEN = 7,
        GEM_DEFENSE_CHANGED = 11,
        KILL_PLAYER = 5,
        Laser = 15,
        NULLSHOOT = -1,
        PET = 20,
        PICK = 1,
        PLAYBUFFER = 14,
        PUP = 0x12,
        START_MOVE = 3,
        TRANSLATE = 6,
        UNANGLE = 13,
        UNFORZEN = 9
    }
}


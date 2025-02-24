namespace Game.Server.Packets
{
    using System;

    public enum ePetType
    {
        ADD_PET = 2,
        ADD_PET_EQUIP = 20,
        ADOPT_PET = 6,
        DEL_PET_EQUIP = 0x15,
        EQUIP_PET_SKILL = 7,
        FEED_PET = 4,
        FIGHT_PET = 0x11,
        MOVE_PETBAG = 3,
        PAY_SKILL = 0x10,
        PET_EVOLUTION = 0x17,
        PET_RISINGSTAR = 0x16,
        REFRESH_PET = 5,
        RELEASE_PET = 8,
        RENAME_PET = 9,
        REVER_PET = 0x12,
        UPDATE_PET = 1
    }
}


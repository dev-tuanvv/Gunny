namespace Game.Server.Packets
{
    using System;

    public enum MagicStonePackageType
    {
        CONVERT_SCORE = 5,
        EXPLORE_MAGIC_STONE = 1,
        LEVEL_UP = 2,
        LEVEL_UP_COMPLETE = 8,
        LOCK_MAGIC_STONE = 6,
        MAGIC_STONE_SCORE = 4,
        MOVE_PLACE = 3,
        SORT_BAG = 9,
        UPDATE_BAG = 7,
        UPDATE_REMAIN_COUNT = 0x10
    }
}


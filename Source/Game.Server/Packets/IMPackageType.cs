namespace Game.Server.Packets
{
    using System;

    public enum IMPackageType
    {
        ADD_CUSTOM_FRIENDS = 0xd0,
        FRIEND_ADD = 160,
        FRIEND_REMOVE = 0xa1,
        FRIEND_RESPONSE = 0xa6,
        FRIEND_STATE = 0xa5,
        FRIEND_UPDATE = 0xa2,
        ONE_ON_ONE_TALK = 0x33,
        ONS_EQUIP = 0x2d,
        SAME_CITY_FRIEND = 0xa4
    }
}


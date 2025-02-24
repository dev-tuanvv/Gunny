namespace Game.Server.Packets
{
    using System;

    public enum ActivityPackageType
    {
        ADD_ACTIVITY_DATA_CHANGE = 7,
        EVERYDAYACTIVEPOINT_CHANGE = 2,
        EVERYDAYACTIVEPOINT_DATA = 1,
        GET_EXPBLESSED_DATA = 8,
        GETACTIVEPOINT_REWARD = 4,
        REGETACTIVEPOINT_REWARD = 6,
        REUSEMONEYPOINT_COMPLETE = 5,
        USEMONEYPOINT_COMPLETE = 3
    }
}


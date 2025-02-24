namespace Game.Server.Packets
{
    using System;

    public enum DicePackageType
    {
        DICE_ACTIVE_CLOSE = 2,
        DICE_ACTIVE_OPEN = 1,
        DICE_RECEIVE_DATA = 3,
        DICE_RECEIVE_RESULT = 4
    }
}


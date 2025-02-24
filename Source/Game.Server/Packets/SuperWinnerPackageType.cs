namespace Game.Server.Packets
{
    using System;

    public enum SuperWinnerPackageType
    {
        END_GAME = 0x35,
        ENTER_ROOM = 0x31,
        JOIN_ROOM = 0x39,
        OUT_ROOM = 0x33,
        RETURN_DICES = 0x34,
        ROLLS_DICES = 50,
        START_ROLL_DICES = 0x37,
        SUPER_WINNER_OPEN = 0x30,
        TIMES_UP = 0x36
    }
}


namespace Game.Server.Packets
{
    using System;

    public enum WorldBossGamePackageType
    {
        ADDPLAYERS = 0x22,
        BUFF_BUY = 0x26,
        ENTER_WORLDBOSSROOM = 0x20,
        LEAVE_ROOM = 0x21,
        MOVE = 0x23,
        REQUEST_REVIVE = 0x25,
        STAUTS = 0x24
    }
}


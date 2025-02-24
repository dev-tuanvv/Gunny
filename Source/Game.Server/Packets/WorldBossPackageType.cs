namespace Game.Server.Packets
{
    using System;

    public enum WorldBossPackageType
    {
        CANENTER = 2,
        ENTER = 3,
        MOVE = 6,
        OPEN = 0,
        OVER = 1,
        WORLDBOSS_BLOOD_UPDATE = 5,
        WORLDBOSS_BUYBUFF = 12,
        WORLDBOSS_EXIT = 4,
        WORLDBOSS_FIGHTOVER = 8,
        WORLDBOSS_PLAYER_REVIVE = 11,
        WORLDBOSS_PLAYERSTAUTSUPDATE = 7,
        WORLDBOSS_PRIVATE_INFO = 0x16,
        WORLDBOSS_RANKING = 10,
        WORLDBOSS_ROOM_CLOSE = 9
    }
}


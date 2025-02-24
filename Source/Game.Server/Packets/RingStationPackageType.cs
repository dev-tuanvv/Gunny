namespace Game.Server.Packets
{
    using System;

    public enum RingStationPackageType
    {
        LANDERSAWARD_GET = 0x31,
        LANDERSAWARD_RECEIVE = 0x30,
        RINGSTATION_ARMORY = 3,
        RINGSTATION_BUYCOUNTORTIME = 2,
        RINGSTATION_CHALLENGE = 5,
        RINGSTATION_FIGHTFLAG = 6,
        RINGSTATION_NEWBATTLEFIELD = 4,
        RINGSTATION_SENDSIGNMSG = 7,
        RINGSTATION_VIEWINFO = 1
    }
}


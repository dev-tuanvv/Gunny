namespace Game.Server.Packets
{
    using System;

    public enum CatchBeastPackageType
    {
        CATCHBEAST_BEGIN = 0x20,
        CATCHBEAST_BUYBUFF = 0x23,
        CATCHBEAST_CHALLENGE = 0x22,
        CATCHBEAST_GETAWARD = 0x24,
        CATCHBEAST_VIEWINFO = 0x21
    }
}


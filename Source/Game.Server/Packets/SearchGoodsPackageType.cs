namespace Game.Server.Packets
{
    using System;

    public enum SearchGoodsPackageType
    {
        BackStep = 20,
        BackToStart = 0x16,
        BeforeStep = 0x13,
        FlopCard = 0x18,
        GetGoods = 0x17,
        OneStep = 0x22,
        PlayerEnter = 0x10,
        PlayerRollDice = 0x11,
        PlayerUpgradeStartLevel = 0x12,
        PlayNowPosition = 0x19,
        QuitTakeCard = 5,
        ReachTheEnd = 0x15,
        Refresh = 4,
        RemoveEvent = 0x21,
        RollDice = 1,
        TakeCard = 3,
        TakeCardResponse = 0x20,
        TRYENTER = 0,
        UpgradeStartLevel = 2
    }
}


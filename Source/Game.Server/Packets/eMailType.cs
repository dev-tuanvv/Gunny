namespace Game.Server.Packets
{
    using System;

    public enum eMailType
    {
        Active = 0x34,
        AdvertMail = 0x3a,
        AuctionFail = 3,
        AuctionSuccess = 2,
        BidFail = 5,
        BidSuccess = 4,
        BuyItem = 8,
        Common = 1,
        ConsortionEmail = 0x3b,
        ConsortionSkillMail = 0x3e,
        DailyAward = 15,
        Default = 0,
        FriendBrithday = 60,
        GiftGuide = 0x37,
        ItemOverdue = 9,
        Manage = 0x33,
        Marry = 14,
        MyseftBrithday = 0x3d,
        OpenUpArk = 12,
        Payment = 0x65,
        PaymentCancel = 7,
        PaymentFinish = 11,
        PresentItem = 10,
        ReturnPayment = 6,
        StoreCanel = 13
    }
}


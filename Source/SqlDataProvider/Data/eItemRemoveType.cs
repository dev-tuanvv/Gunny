namespace SqlDataProvider.Data
{
    using System;

    public enum eItemRemoveType
    {
        Auction = 8,
        Compose = 6,
        Delete = 1,
        FastError = 0x18,
        FireError = 0x15,
        Fold = 10,
        Fusion = 7,
        ItemInFight = 0x1d,
        ItemInLottery = 0x23,
        ItemInTemp = 0x1c,
        ItemTransfer = 0x17,
        Mail = 3,
        MailDelete = 11,
        MoveError = 0x16,
        OpenHoles = 0x24,
        Other = 9,
        Reclaim = 0x21,
        Refinery = 0x19,
        Sell = 30,
        Shopping = 0x1b,
        ShoppingForContinue = 0x1f,
        Spa = 0x22,
        Stack = 0x1a,
        Strengthen = 5,
        StrengthFailed = 4,
        Task = 0x20,
        Use = 2,
        Wedding = 12
    }
}


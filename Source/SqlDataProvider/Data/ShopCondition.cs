namespace SqlDataProvider.Data
{
    using System;

    public class ShopCondition
    {
        public static bool isDDTMoney(int type)
        {
            return (type == 2);
        }

        public static bool isLabyrinth(int type)
        {
            return (type == 0x5e);
        }

        public static bool isLeague(int type)
        {
            return (type == 0x5d);
        }

        public static bool isMoney(int type)
        {
            return (type == 1);
        }

        public static bool isOffer(int type)
        {
            switch (type)
            {
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    return true;
            }
            return false;
        }

        public static bool isPetScrore(int type)
        {
            return (type == 0x5c);
        }

        public static bool isSearchGoods(int type)
        {
            return (type == 0x63);
        }

        public static bool isWorldBoss(int type)
        {
            return (type == 0x5b);
        }
    }
}


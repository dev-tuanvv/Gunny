﻿namespace Bussiness.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public static class ShopMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock = new ReaderWriterLock();
        private static Dictionary<int, ShopItemInfo> m_shop = new Dictionary<int, ShopItemInfo>();
        private static Dictionary<int, ShopGoodsShowListInfo> m_ShopGoodsCanBuy = new Dictionary<int, ShopGoodsShowListInfo>();
        private static Dictionary<int, ShopGoodsShowListInfo> m_shopGoodsShowLists = new Dictionary<int, ShopGoodsShowListInfo>();
        private static Dictionary<int, ShopGoXuInfo> m_shopGXLists = new Dictionary<int, ShopGoXuInfo>();

        public static bool CanBuy(int shopID, int consortiaShopLevel, ref bool isBinds, int cousortiaID, int playerRiches)
        {
            bool flag = false;
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                ConsortiaEquipControlInfo info;
                switch (shopID)
                {
                    case 1:
                        flag = true;
                        isBinds = false;
                        return flag;

                    case 2:
                    case 3:
                    case 4:
                        flag = true;
                        isBinds = true;
                        return flag;

                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        return flag;

                    case 11:
                        info = bussiness.GetConsortiaEuqipRiches(cousortiaID, 1, 1);
                        if ((consortiaShopLevel >= info.Level) && (playerRiches >= info.Riches))
                        {
                            flag = true;
                            isBinds = true;
                        }
                        return flag;

                    case 12:
                        info = bussiness.GetConsortiaEuqipRiches(cousortiaID, 2, 1);
                        if ((consortiaShopLevel >= info.Level) && (playerRiches >= info.Riches))
                        {
                            flag = true;
                            isBinds = true;
                        }
                        return flag;

                    case 13:
                        info = bussiness.GetConsortiaEuqipRiches(cousortiaID, 3, 1);
                        if ((consortiaShopLevel >= info.Level) && (playerRiches >= info.Riches))
                        {
                            flag = true;
                            isBinds = true;
                        }
                        return flag;

                    case 14:
                        info = bussiness.GetConsortiaEuqipRiches(cousortiaID, 4, 1);
                        if ((consortiaShopLevel >= info.Level) && (playerRiches >= info.Riches))
                        {
                            flag = true;
                            isBinds = true;
                        }
                        return flag;

                    case 15:
                        info = bussiness.GetConsortiaEuqipRiches(cousortiaID, 5, 1);
                        if ((consortiaShopLevel >= info.Level) && (playerRiches >= info.Riches))
                        {
                            flag = true;
                            isBinds = true;
                        }
                        return flag;

                    case 0x5b:
                    case 0x5c:
                    case 0x5d:
                    case 0x5e:
                    case 0x5f:
                    case 0x62:
                    case 0x63:
                    case 100:
                    case 0x48:
                        break;

                    case 0x60:
                    case 0x61:
                        return flag;

                    default:
                        return flag;
                }
                flag = true;
                isBinds = true;
            }
            return flag;
        }

        public static bool CheckInShopGoodsCanBuy(int iTemplateID)
        {
            return m_ShopGoodsCanBuy.ContainsKey(iTemplateID);
        }

        public static int FindItemTemplateID(int id)
        {
            if (m_shop.ContainsKey(id))
            {
                return m_shop[id].TemplateID;
            }
            return 0;
        }

        //public static ShopItemInfo FindShopbyTemplateID(int TemplatID)
        //{
        //    foreach (ShopItemInfo info in m_shop.Values)
        //    {
        //        if (info.TemplateID == TemplatID)
        //        {
        //            return info;
        //        }
        //    }
        //   return null;
        //}

        public static List<ShopItemInfo> FindShopByTemplatID(int TemplatID)
        {
            List<ShopItemInfo> shopItem = new List<ShopItemInfo>();
            foreach (ShopItemInfo shop in ShopMgr.m_shop.Values)
            {
                if (shop.TemplateID == TemplatID)
                {
                    shopItem.Add(shop);
                }
            }
            return shopItem;
        }

        public static List<ShopItemInfo> FindShopbyTemplatID(int TemplatID)
        {
            List<ShopItemInfo> list = new List<ShopItemInfo>();
            foreach (ShopItemInfo info in m_shop.Values)
            {
                if (info.TemplateID == TemplatID)
                {
                    list.Add(info);
                }
            }
            return list;
        }

        public static void FindSpecialItemInfo(SqlDataProvider.Data.ItemInfo info, ref int gold, ref int money, ref int giftToken, ref int medal, ref int honor, ref int hardCurrency, ref int token, ref int dragonToken, ref int magicStonePoint)
        {
            int templateID = info.TemplateID;
            if (templateID <= -1000)
            {
                if (templateID <= -1200)
                {
                    if (templateID == -1400)
                    {
                        magicStonePoint += info.Count;
                        info = null;
                    }
                    else if (templateID == -1200)
                    {
                        dragonToken += info.Count;
                        info = null;
                    }
                }
                else if (templateID == -1100)
                {
                    giftToken += info.Count;
                    info = null;
                }
                else if (templateID == -1000)
                {
                    token += info.Count;
                    info = null;
                }
            }
            else
            {
                switch (templateID)
                {
                    case -900:
                        hardCurrency += info.Count;
                        info = null;
                        break;

                    case -800:
                        honor += info.Count;
                        info = null;
                        break;

                    case -300:
                        medal += info.Count;
                        info = null;
                        break;

                    case -200:
                        money += info.Count;
                        info = null;
                        break;

                    case -100:
                        gold += info.Count;
                        info = null;
                        break;
                }
            }
        }

        public static void GetItemPrice(int Prices, int Values, decimal beat, ref int damageScore, ref int petScore, ref int iTemplateID, ref int iCount, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int medal, ref int hardCurrency, ref int LeagueMoney, ref int useableScore)
        {
            if (Prices <= -900)
            {
                if (Prices == -1200)
                {
                    useableScore += (int) (Values * beat);
                    return;
                }
                if (Prices == -1000)
                {
                    LeagueMoney += (int) (Values * beat);
                    return;
                }
                if (Prices == -900)
                {
                    hardCurrency += (int) (Values * beat);
                    return;
                }
            }
            else
            {
                switch (Prices)
                {
                    case -8:
                        petScore += (int) (Values * beat);
                        return;

                    case -7:
                        damageScore += (int) (Values * beat);
                        return;

                    case -6:
                        medal += (int) (Values * beat);
                        return;

                    case -4:
                        offer += (int) (Values * beat);
                        return;

                    case -3:
                        gold += (int) (Values * beat);
                        return;

                    case -2:
                        gifttoken += (int) (Values * beat);
                        return;

                    case -1:
                        money += (int) (Values * beat);
                        return;

                    case -300:
                        medal += (int) (Values * beat);
                        return;

                    case -800:
                        medal += (int) (Values * beat);
                        return;
                }
            }
            if (Prices > 0)
            {
                iTemplateID = Prices;
                iCount = Values;
            }
        }

        public static ShopGoXuInfo GetShopGoXu(int TemplateID)
        {
            if (m_shopGXLists == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (m_shopGXLists.Keys.Contains<int>(TemplateID))
                {
                    return m_shopGXLists[TemplateID];
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static ShopItemInfo GetShopItemInfoById(int ID)
        {
            if (m_shop.ContainsKey(ID))
            {
                return m_shop[ID];
            }
            return null;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool IsOnShop(int Id)
        {
            if (m_shopGoodsShowLists == null)
            {
                Init();
            }
            if (IsSpecialItem(Id))
            {
                return true;
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (m_shopGoodsShowLists.Keys.Contains<int>(Id))
                {
                    return true;
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return false;
        }

        public static bool IsSpecialItem(int Id)
        {
            if (Id <= 0x10cc01)
            {
                if ((Id != 0x10ca71) && (Id != 0x10cc01))
                {
                    return false;
                }
            }
            else if ((Id != 0x10cd91) && (Id != 0x10cf21))
            {
                return false;
            }
            return true;
        }

        private static Dictionary<int, ShopItemInfo> LoadFromDatabase()
        {
            Dictionary<int, ShopItemInfo> dictionary = new Dictionary<int, ShopItemInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (ShopItemInfo info in bussiness.GetALllShop())
                {
                    if (!dictionary.ContainsKey(info.ID))
                    {
                        dictionary.Add(info.ID, info);
                    }
                }
            }
            return dictionary;
        }

        private static Dictionary<int, ShopGoodsShowListInfo> LoadShopGoodsCanBuyFromDatabase()
        {
            Dictionary<int, ShopGoodsShowListInfo> dictionary = new Dictionary<int, ShopGoodsShowListInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                ShopGoodsShowListInfo[] allShopGoodsShowList = bussiness.GetAllShopGoodsShowList();
                foreach (ShopGoodsShowListInfo info in allShopGoodsShowList)
                {
                    if (!dictionary.ContainsKey(info.ShopId))
                    {
                        dictionary.Add(info.ShopId, info);
                    }
                }
            }
            return dictionary;
        }

        private static Dictionary<int, ShopGoXuInfo> LoadShopGoXuFromDatabase()
        {
            Dictionary<int, ShopGoXuInfo> dictionary = new Dictionary<int, ShopGoXuInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (ShopGoXuInfo info in bussiness.GetAllShopGoXu())
                {
                    if (!dictionary.ContainsKey(info.TemplateID))
                    {
                        dictionary.Add(info.TemplateID, info);
                    }
                }
            }
            return dictionary;
        }

        private static Dictionary<int, ShopGoodsShowListInfo> LoadShowListFromDatabase()
        {
            Dictionary<int, ShopGoodsShowListInfo> dictionary = new Dictionary<int, ShopGoodsShowListInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (ShopGoodsShowListInfo info in bussiness.GetAllShopGoodsShowList())
                {
                    if (!dictionary.ContainsKey(info.ShopId))
                    {
                        dictionary.Add(info.ShopId, info);
                    }
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            Exception exception;
            bool flag = false;
            try
            {
                Dictionary<int, ShopItemInfo> dictionary = LoadFromDatabase();
                Dictionary<int, ShopGoodsShowListInfo> dictionary2 = LoadShowListFromDatabase();
                Dictionary<int, ShopGoXuInfo> dictionary3 = LoadShopGoXuFromDatabase();
                if (dictionary.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, ShopItemInfo>>(ref m_shop, dictionary);
                }
                if (dictionary2.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, ShopGoodsShowListInfo>>(ref m_shopGoodsShowLists, dictionary2);
                }
                if (dictionary3.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, ShopGoXuInfo>>(ref m_shopGXLists, dictionary3);
                }
                try
                {
                    Dictionary<int, ShopGoodsShowListInfo> dictionary4 = LoadShopGoodsCanBuyFromDatabase();
                    if (dictionary4.Count > 0)
                    {
                        Interlocked.Exchange<Dictionary<int, ShopGoodsShowListInfo>>(ref m_ShopGoodsCanBuy, dictionary4);
                    }
                    flag = true;
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    log.Error("ShopInfoMgr", exception);
                }
                return true;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                log.Error("ShopInfoMgr", exception);
            }
            return false;
        }

        public static bool SetItemType(ShopItemInfo shop, int type, ref int damageScore, ref int petScore, ref int iTemplateID, ref int iCount, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int medal, ref int hardCurrency, ref int LeagueMoney, ref int useableScore)
        {
            if (type == 1)
            {
                GetItemPrice(shop.APrice1, shop.AValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
                GetItemPrice(shop.APrice2, shop.AValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
                GetItemPrice(shop.APrice3, shop.AValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
            }
            if (type == 2)
            {
                GetItemPrice(shop.BPrice1, shop.BValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
                GetItemPrice(shop.BPrice2, shop.BValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
                GetItemPrice(shop.BPrice3, shop.BValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
            }
            if (type == 3)
            {
                GetItemPrice(shop.CPrice1, shop.CValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
                GetItemPrice(shop.CPrice2, shop.CValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
                GetItemPrice(shop.CPrice3, shop.CValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
            }
            return true;
        }
        public static ShopItemInfo FindShopbyTemplateID(int TemplatID)
        {
            foreach (ShopItemInfo shop in ShopMgr.m_shop.Values)
            {
                if (shop.TemplateID == TemplatID)
                {
                    return shop;
                }
            }
            return null;
        }

    }
}


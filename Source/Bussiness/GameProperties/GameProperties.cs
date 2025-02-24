namespace Bussiness
{
    using Game.Base.Config;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class GameProperties
    {
        [ConfigProperty("AssState", "\x00b7\x00c0\x00b3\x00c1\x00c3\x00d4\x00cf\x00b5\x00cd\x00b3\x00b5\x00c4\x00bf\x00aa\x00b9\x00d8,True\x00b4\x00f2\x00bf\x00aa,False\x00b9\x00d8\x00b1\x00d5", false)]
        public static bool ASS_STATE;
        [ConfigProperty("BagMailEnable", "BagMailEnable", true)]
        public static readonly bool BagMailEnable;
        [ConfigProperty("BeginAuction", "\x00c5\x00c4\x00c2\x00f2\x00ca\x00b1\x00c6\x00f0\x00ca\x00bc\x00cb\x00e6\x00bb\x00fa\x00ca\x00b1\x00bc\x00e4", 20)]
        public static int BeginAuction;
        [ConfigProperty("BigDicePrice", "BigDicePrice", 0xc350)]
        public static readonly int BigDicePrice;
        [ConfigProperty("BigExp", "\x00b5\x00b1\x00c7\x00b0\x00d3\x00ce\x00cf\x00b7\x00b0\x00e6\x00b1\x00be", "11906|99")]
        public static readonly string BigExp;
        [ConfigProperty("BoguAdventurePrice", "BoguAdventurePrice", "200, 500, 100")]
        public static readonly string BoguAdventurePrice;
        [ConfigProperty("BoxAppearCondition", "\x00cf\x00e4\x00d7\x00d3\x00ce\x00ef\x00c6\x00b7\x00cc\x00e1\x00ca\x00be\x00b5\x00c4\x00b5\x00c8\x00bc\x00b6", 4)]
        public static readonly int BOX_APPEAR_CONDITION;
        [ConfigProperty("Cess", "\x00bd\x00bb\x00d2\x00d7\x00bf\x00db\x00cb\x00b0", 0.1)]
        public static readonly double Cess;
        [ConfigProperty("CommonDicePrice", "CommonDicePrice", 0x7530)]
        public static readonly int CommonDicePrice;
        [ConfigProperty("ConsortiaStrengthenEx", "Kinh nghiệm", "1|2")]
        public static readonly string ConsortiaStrengthenEx;
        [ConfigProperty("CustomLimit", "sendattackmail|addaution|PresentGoods|PresentMoney|unknow", "20|20|20|20|20")]
        public static readonly string CustomLimit;
        [ConfigProperty("CheckCount", "\x00d7\x00ee\x00b4\x00f3\x00d1\x00e9\x00d6\x00a4\x00c2\x00eb\x00ca\x00a7\x00b0\x00dc\x00b4\x00ce\x00ca\x00fd", 2)]
        public static readonly int CHECK_MAX_FAILED_COUNT;
        [ConfigProperty("CheckRewardItem", "\x00d1\x00e9\x00d6\x00a4\x00c2\x00eb\x00bd\x00b1\x00c0\x00f8\x00ce\x00ef\x00c6\x00b7", 0x2af9)]
        public static readonly int CHECK_REWARD_ITEM;
        [ConfigProperty("ChristmasBeginDate", "ChristmasBeginDate", "2013/12/17 0:00:00")]
        public static readonly string ChristmasBeginDate;
        [ConfigProperty("ChristmasBuildSnowmanDoubleMoney", "ChristmasBuildSnowmanDoubleMoney", 10)]
        public static readonly int ChristmasBuildSnowmanDoubleMoney;
        [ConfigProperty("ChristmasBuyMinute", "ChristmasBuyMinute", 10)]
        public static readonly int ChristmasBuyMinute;
        [ConfigProperty("ChristmasBuyTimeMoney", "ChristmasBuyTimeMoney", 150)]
        public static readonly int ChristmasBuyTimeMoney;
        [ConfigProperty("ChristmasEndDate", "ChristmasEndDate", "2013/12/25 0:00:00")]
        public static readonly string ChristmasEndDate;
        [ConfigProperty("ChristmasGifts", "ChristmasGifts", "201148,10|201149,35|201150,70|201151,120|201152,220|201153,370|201154,650|201155,1000|201156,100")]
        public static readonly string ChristmasGifts;
        [ConfigProperty("ChristmasGiftsMaxNum", "ChristmasGiftsMaxNum", 0x3e8)]
        public static readonly int ChristmasGiftsMaxNum;
        [ConfigProperty("ChristmasMinute", "ChristmasMinute", 60)]
        public static readonly int ChristmasMinute;
        [ConfigProperty("DailyAwardState", "\x00c3\x00bf\x00c8\x00d5\x00bd\x00b1\x00c0\x00f8\x00bf\x00aa\x00b9\x00d8,True\x00b4\x00f2\x00bf\x00aa,False\x00b9\x00d8\x00b1\x00d5", true)]
        public static bool DAILY_AWARD_STATE;
        [ConfigProperty("DDPlayActivityBeginDate", "DDPlayActivityBeginDate", "2014/07/01 0:00:00")]
        public static readonly string DDPlayActivityBeginDate;
        [ConfigProperty("DDPlayActivityEndDate", "DDPlayActivityEndDate", "2015/07/01 0:00:00")]
        public static readonly string DDPlayActivityEndDate;
        [ConfigProperty("DDPlayActivityMoney", "DDPlayActivityMoney", 100)]
        public static readonly int DDPlayActivityMoney;
        [ConfigProperty("DiceBeginTime", "DiceBeginTime", "2013/12/17 0:00:00")]
        public static readonly string DiceBeginTime;
        [ConfigProperty("DiceEndTime", "DiceEndTime", "2013/12/25 0:00:00")]
        public static readonly string DiceEndTime;
        [ConfigProperty("DiceGameAwardAndCount", "DiceGameAwardAndCount", "32|16|8|4|2|1")]
        public static readonly string DiceGameAwardAndCount;
        [ConfigProperty("DiceRefreshPrice", "DiceRefreshPrice", 0x9c40)]
        public static readonly int DiceRefreshPrice;
        [ConfigProperty("DisableCommands", "\x00bd\x00fb\x00d6\x00b9\x00ca\x00b9\x00d3\x00c3\x00b5\x00c4\x00c3\x00fc\x00c1\x00ee", "")]
        public static readonly string DISABLED_COMMANDS;
        [ConfigProperty("DoubleDicePrice", "DoubleDicePrice", 0x9c40)]
        public static readonly int DoubleDicePrice;
        [ConfigProperty("DragonBoatAreaMinScore", "DragonBoatAreaMinScore", 0x4e20)]
        public static readonly int DragonBoatAreaMinScore;
        [ConfigProperty("DragonBoatBeginDate", "DragonBoatBeginDate", "2013/12/19 0:00:00")]
        public static readonly string DragonBoatBeginDate;
        [ConfigProperty("DragonBoatByMoney", "DragonBoatByMoney", "100:10,10")]
        public static readonly string DragonBoatByMoney;
        [ConfigProperty("DragonBoatByProps", "DragonBoatByProps", "1:10,10")]
        public static readonly string DragonBoatByProps;
        [ConfigProperty("DragonBoatConvertHours", "DragonBoatConvertHours", 0x48)]
        public static readonly int DragonBoatConvertHours;
        [ConfigProperty("DragonBoatEndDate", "DragonBoatEndDate", "2013/12/26 0:00:00")]
        public static readonly string DragonBoatEndDate;
        [ConfigProperty("DragonBoatMaxScore", "DragonBoatMaxScore", 0x7530)]
        public static readonly int DragonBoatMaxScore;
        [ConfigProperty("DragonBoatMinScore", "DragonBoatMinScore", 0x32c8)]
        public static readonly int DragonBoatMinScore;
        [ConfigProperty("DragonBoatProp", "DragonBoatProp", 0x2daa)]
        public static readonly int DragonBoatProp;
        [ConfigProperty("Edition", "\x00b5\x00b1\x00c7\x00b0\x00d3\x00ce\x00cf\x00b7\x00b0\x00e6\x00b1\x00be", "2612558")]
        public static readonly string EDITION;
        [ConfigProperty("EndAuction", "\x00c5\x00c4\x00c2\x00f2\x00ca\x00b1\x00bd\x00e1\x00ca\x00f8\x00cb\x00e6\x00bb\x00fa\x00ca\x00b1\x00bc\x00e4", 40)]
        public static int EndAuction;
        [ConfigProperty("EquipMaxRefineryLevel", "\x00c5\x00c4\x00c2\x00f2\x00ca\x00b1\x00bd\x00e1\x00ca\x00f8\x00cb\x00e6\x00bb\x00fa\x00ca\x00b1\x00bc\x00e4", 5)]
        public static int EquipMaxRefineryLevel;
        [ConfigProperty("EquipRefineryExp", "\x00c5\x00c4\x00c2\x00f2\x00ca\x00b1\x00bd\x00e1\x00ca\x00f8\x00cb\x00e6\x00bb\x00fa\x00ca\x00b1\x00bc\x00e4", "500|2000|7250|28250|112250")]
        public static string EquipRefineryExp;
        [ConfigProperty("FightFootballTime", "FightFootballTime", "19|60")]
        public static readonly string FightFootballTime;
        [ConfigProperty("FreeExp", "\x00b5\x00b1\x00c7\x00b0\x00d3\x00ce\x00cf\x00b7\x00b0\x00e6\x00b1\x00be", "11901|1")]
        public static readonly string FreeExp;
        [ConfigProperty("FreeMoney", "\x00b5\x00b1\x00c7\x00b0\x00d3\x00ce\x00cf\x00b7\x00b0\x00e6\x00b1\x00be", 0x986f70)]
        public static readonly int FreeMoney;
        [ConfigProperty("HoleLevelUpExpList", "HoleLevelUpExpList", "1|2")]
        public static readonly string HoleLevelUpExpList;
        [ConfigProperty("HotSpringExp", "Kinh nghiệm Spa", "1|2")]
        public static readonly string HotSpringExp;
        [ConfigProperty("IsActiveMoney", "IsActiveMoney", true)]
        public static readonly bool IsActiveMoney;
        [ConfigProperty("IsDDTMoneyActive", "IsDDTMoneyActive", false)]
        public static readonly bool IsDDTMoneyActive;
        [ConfigProperty("IsLimitAuction", "IsLimitAuction", false)]
        public static readonly bool IsLimitAuction;
        [ConfigProperty("IsLimitCount", "IsLimitCount", false)]
        public static readonly bool IsLimitCount;
        [ConfigProperty("IsLimitMail", "IsLimitMail", false)]
        public static readonly bool IsLimitMail;
        [ConfigProperty("IsLimitMoney", "IsLimitMoney", false)]
        public static readonly bool IsLimitMoney;
        [ConfigProperty("IsPromotePackageOpen", "IsPromotePackageOpen", false)]
        public static readonly bool IsPromotePackageOpen;
        [ConfigProperty("IsWishBeadLimit", "IsWishBeadLimit", false)]
        public static readonly bool IsWishBeadLimit;
        [ConfigProperty("KingBuffPrice", "KingBuffPrice", "475,1425,2500")]
        public static readonly string KingBuffPrice;
        [ConfigProperty("LightRiddleAnswerScore", "LightRiddleAnswerScore", "29|9")]
        public static readonly string LightRiddleAnswerScore;
        [ConfigProperty("LightRiddleAnswerTime", "LightRiddleAnswerTime", 15)]
        public static readonly int LightRiddleAnswerTime;
        [ConfigProperty("LightRiddleBeginDate", "LightRiddleBeginDate", "2014/2/13 0:00:00")]
        public static readonly string LightRiddleBeginDate;
        [ConfigProperty("LightRiddleBeginTime", "LightRiddleBeginTime", "2014/2/13 12:30:00")]
        public static readonly string LightRiddleBeginTime;
        [ConfigProperty("LightRiddleComboMoney", "LightRiddleComboMoney", 30)]
        public static readonly int LightRiddleComboMoney;
        [ConfigProperty("LightRiddleEndDate", "LightRiddleEndDate", "2014/2/28 0:00:00")]
        public static readonly string LightRiddleEndDate;
        [ConfigProperty("LightRiddleEndTime", "LightRiddleEndTime", "2014/2/13 13:00:00")]
        public static readonly string LightRiddleEndTime;
        [ConfigProperty("LightRiddleFreeComboNum", "LightRiddleFreeComboNum", 2)]
        public static readonly int LightRiddleFreeComboNum;
        [ConfigProperty("LightRiddleFreeHitNum", "LightRiddleFreeHitNum", 2)]
        public static readonly int LightRiddleFreeHitNum;
        [ConfigProperty("LightRiddleHitMoney", "LightRiddleHitMoney", 30)]
        public static readonly int LightRiddleHitMoney;
        [ConfigProperty("LightRiddleOpenLevel", "LightRiddleOpenLevel", 15)]
        public static readonly int LightRiddleOpenLevel;
        [ConfigProperty("LimitAuction", "LimitAuction", 3)]
        public static readonly int LimitAuction;
        [ConfigProperty("LimitCount", "LimitCount", 10)]
        public static readonly int LimitCount;
        [ConfigProperty("LimitMail", "LimitMail", 3)]
        public static readonly int LimitMail;
        [ConfigProperty("LimitMoney", "LimitMoney", 0xf3e58)]
        public static readonly int LimitMoney;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        [ConfigProperty("LuckStarActivityBeginDate", "LuckStarActivityBeginDate", "2013/12/1 0:00:00")]
        public static readonly string LuckStarActivityBeginDate;
        [ConfigProperty("LuckStarActivityEndDate", "LuckStarActivityEndDate", "2014/12/24 0:00:00")]
        public static readonly string LuckStarActivityEndDate;
        [ConfigProperty("MagicPackageID", "MagicPackageID", "0|112378|112379|112380")]
        public static readonly string MagicPackageID;
        [ConfigProperty("MagicStoneOpenPoint", "MagicStoneOpenPoint", "0|10|25|50")]
        public static readonly string MagicStoneOpenPoint;
        [ConfigProperty("MaxMissionEnergy", "MaxMissionEnergy", 300)]
        public static readonly int MaxMissionEnergy;
        [ConfigProperty("MinUseNum", "MinUseNum", 0x3e8)]
        public static readonly int MinUseNum;
        [ConfigProperty("NewChickenBeginTime", "NewChickenBeginTime", "2013/12/17 0:00:00")]
        public static readonly string NewChickenBeginTime;
        [ConfigProperty("NewChickenEagleEyePrice", "NewChickenEagleEyePrice", "3000, 2000, 1000")]
        public static readonly string NewChickenEagleEyePrice;
        [ConfigProperty("NewChickenEndTime", "NewChickenEndTime", "2013/12/25 0:00:00")]
        public static readonly string NewChickenEndTime;
        [ConfigProperty("NewChickenFlushPrice", "NewChickenFlushPrice", 0x2710)]
        public static readonly int NewChickenFlushPrice;
        [ConfigProperty("NewChickenOpenCardPrice", "NewChickenOpenCardPrice", "2500, 2000, 1500, 1000, 500")]
        public static readonly string NewChickenOpenCardPrice;
        [ConfigProperty("OpenMagicStonePackageMoney", "OpenMagicStonePackageMoney", "0|150|350|650")]
        public static readonly string OpenMagicStonePackageMoney;
        [ConfigProperty("OpenRunePackageMoney", "OpenRunePackageMoney", "10|20|50|100")]
        public static readonly string OpenRunePackageMoney;
        [ConfigProperty("OpenRunePackageRange", "OpenRunePackageRange", "1,6|1,5|1,4")]
        public static readonly string OpenRunePackageRange;
        [ConfigProperty("PetExp", "\x00b5\x00b1\x00c7\x00b0\x00d3\x00ce\x00cf\x00b7\x00b0\x00e6\x00b1\x00be", "334103|999")]
        public static readonly string PetExp;
        [ConfigProperty("MustComposeGold", "\x00ba\x00cf\x00b3\x00c9\x00cf\x00fb\x00ba\x00c4\x00bd\x00f0\x00b1\x00d2\x00bc\x00db\x00b8\x00f1", 0x3e8)]
        public static readonly int PRICE_COMPOSE_GOLD;
        [ConfigProperty("DivorcedMoney", "\x00c0\x00eb\x00bb\x00e9\x00b5\x00c4\x00bc\x00db\x00b8\x00f1", 0x5db)]
        public static readonly int PRICE_DIVORCED;
        [ConfigProperty("DivorcedDiscountMoney", "\x00c0\x00eb\x00bb\x00e9\x00b5\x00c4\x00bc\x00db\x00b8\x00f1", 0x3e7)]
        public static readonly int PRICE_DIVORCED_DISCOUNT;
        [ConfigProperty("MustFusionGold", "\x00c8\x00db\x00c1\x00b6\x00cf\x00fb\x00ba\x00c4\x00bd\x00f0\x00b1\x00d2\x00bc\x00db\x00b8\x00f1", 400)]
        public static readonly int PRICE_FUSION_GOLD;
        [ConfigProperty("MarryRoomCreateMoney", "\x00bd\x00e1\x00bb\x00e9\x00b7\x00bf\x00bc\x00e4\x00b5\x00c4\x00bc\x00db\x00b8\x00f1,2\x00d0\x00a1\x00ca\x00b1\x00a1\x00a23\x00d0\x00a1\x00ca\x00b1\x00a1\x00a24\x00d0\x00a1\x00ca\x00b1\x00d3\x00c3\x00b6\x00ba\x00ba\x00c5\x00b7\x00d6\x00b8\x00f4", "2000,2700,3400")]
        public static readonly string PRICE_MARRY_ROOM;
        [ConfigProperty("HymenealMoney", "\x00c7\x00f3\x00bb\x00e9\x00b5\x00c4\x00bc\x00db\x00b8\x00f1", 300)]
        public static readonly int PRICE_PROPOSE;
        [ConfigProperty("MustStrengthenGold", "\x00c7\x00bf\x00bb\x00af\x00bd\x00f0\x00b1\x00d2\x00cf\x00fb\x00ba\x00c4\x00bc\x00db\x00b8\x00f1", 0x3e8)]
        public static readonly int PRICE_STRENGHTN_GOLD;
        [ConfigProperty("PromotePackagePrice", "PromotePackagePrice", 0xe10)]
        public static readonly int PromotePackagePrice;
        [ConfigProperty("PyramidBeginTime", "PyramidBeginTime", "2013/12/17 0:00:00")]
        public static readonly string PyramidBeginTime;
        [ConfigProperty("PyramidEndTime", "NewChickenEndTime", "2013/12/25 0:00:00")]
        public static readonly string PyramidEndTime;
        [ConfigProperty("PyramidRevivePrice", "PyramidRevivePrice", "10000, 30000, 50000")]
        public static readonly string PyramidRevivePrice;
        [ConfigProperty("PyramydTurnCardPrice", "PyramydTurnCardPrice", 0x1388)]
        public static readonly int PyramydTurnCardPrice;
        [ConfigProperty("RingLevel", "\x00c5\x00c4\x00c2\x00f2\x00ca\x00b1\x00bd\x00e1\x00ca\x00f8\x00cb\x00e6\x00bb\x00fa\x00ca\x00b1\x00bc\x00e4", "1000|4000|9000|16000|25000|37000|52000|70000|91000")]
        public static string RingLevel;
        [ConfigProperty("RingMaxRefineryLevel", "\x00c5\x00c4\x00c2\x00f2\x00ca\x00b1\x00bd\x00e1\x00ca\x00f8\x00cb\x00e6\x00bb\x00fa\x00ca\x00b1\x00bc\x00e4", 9)]
        public static int RingMaxRefineryLevel;
        [ConfigProperty("RuneLevelUpExp", "Kinh nghiệm ch\x00e2u b\x00e1u", "1|2")]
        public static readonly string RuneLevelUpExp;
        [ConfigProperty("RunePackageID", "RunePackageID", "311100|311200|311300|311400")]
        public static readonly string RunePackageID;
        [ConfigProperty("SearchGoodsFreeCount", "SearchGoodsFreeCount", 3)]
        public static readonly int SearchGoodsFreeCount;
        [ConfigProperty("SearchGoodsFreeLimit", "SearchGoodsFreeLimit", 0)]
        public static readonly int SearchGoodsFreeLimit;
        [ConfigProperty("SearchGoodsPayMoney", "SearchGoodsPayMoney", 20)]
        public static readonly int SearchGoodsPayMoney;
        [ConfigProperty("SearchGoodsTakeCardMoney", "SearchGoodsTakeCardMoney", "0|50|120")]
        public static readonly string SearchGoodsTakeCardMoney;
        [ConfigProperty("SmallDicePrice", "SmallDicePrice", 0xea60)]
        public static readonly int SmallDicePrice;
        public static int SpaAddictionMoneyNeeded = 0x2710;
        [ConfigProperty("SpaPriRoomContinueTime", "\x00c5\x00c4\x00c2\x00f2\x00ca\x00b1\x00bd\x00e1\x00ca\x00f8\x00cb\x00e6\x00bb\x00fa\x00ca\x00b1\x00bc\x00e4", 60)]
        public static int SpaPriRoomContinueTime;
        [ConfigProperty("SpaPubRoomLoginPay", "\x00c5\x00c4\x00c2\x00f2\x00ca\x00b1\x00bd\x00e1\x00ca\x00f8\x00cb\x00e6\x00bb\x00fa\x00ca\x00b1\x00bc\x00e4", "10000,200")]
        public static string SpaPubRoomLoginPay;
        [ConfigProperty("TestActive", "TestActive", false)]
        public static readonly bool TestActive;
        [ConfigProperty("VIPExpForEachLv", "VIPExpForEachLv", "1|2")]
        public static readonly string VIPExpForEachLv;
        [ConfigProperty("VIPStrengthenEx", "VIPStrengthenEx", "1|2")]
        public static readonly string VIPStrengthenEx;
        [ConfigProperty("VirtualName", "VirtualName", "Doreamon,Nobita,Xuneo,Xuka")]
        public static readonly string VirtualName;
        [ConfigProperty("WarriorFamRaidDDTPrice", "WarriorFamRaidDDTPrice", 0x1388)]
        public static readonly int WarriorFamRaidDDTPrice;
        [ConfigProperty("WarriorFamRaidPriceBig", "WarriorFamRaidPriceBig ", 0x9c40)]
        public static readonly int WarriorFamRaidPriceBig;
        [ConfigProperty("WarriorFamRaidPricePerMin", "WarriorFamRaidPricePerMin", 10)]
        public static readonly int WarriorFamRaidPricePerMin;
        [ConfigProperty("WarriorFamRaidPriceSmall", "WarriorFamRaidPriceSmall", 0x7530)]
        public static readonly int WarriorFamRaidPriceSmall;
        [ConfigProperty("WarriorFamRaidTimeRemain", "WarriorFamRaidTimeRemain", 120)]
        public static readonly int WarriorFamRaidTimeRemain;
        [ConfigProperty("WishBeadLimitLv", "WishBeadLimitLv", 12)]
        public static readonly int WishBeadLimitLv;
        [ConfigProperty("YearMonsterBeginDate", "YearMonsterBeginDate", "2014/1/17 0:00:00")]
        public static readonly string YearMonsterBeginDate;
        [ConfigProperty("YearMonsterBoxInfo", "YearMonsterBoxInfo", "112370,5|112371,30|112372,90|112373,150|112374,300")]
        public static readonly string YearMonsterBoxInfo;
        [ConfigProperty("YearMonsterBuffMoney", "YearMonsterBuffMoney", 300)]
        public static readonly int YearMonsterBuffMoney;
        [ConfigProperty("YearMonsterEndDate", "YearMonsterEndDate", "2014/2/25 0:00:00")]
        public static readonly string YearMonsterEndDate;
        [ConfigProperty("YearMonsterFightNum", "YearMonsterFightNum", 1)]
        public static readonly int YearMonsterFightNum;
        [ConfigProperty("YearMonsterHP", "YearMonsterHP", 0x2dc6c0)]
        public static readonly int YearMonsterHP;
        [ConfigProperty("YearMonsterOpenLevel", "YearMonsterOpenLevel", 15)]
        public static readonly int YearMonsterOpenLevel;

        protected GameProperties()
        {
        }

        public static int ConsortiaStrengExp(int Lv)
        {
            return getProp(ConsortiaStrengthenEx)[Lv];
        }

        public static int[] ConvertStringArrayToIntArray(string str)
        {
            List<int> list = new List<int>();
            string[] strArray = new string[] { "99999", "999999", "9999999" };
            if (str != null)
            {
                if (str != "NewChickenEagleEyePrice")
                {
                    if (str != "NewChickenOpenCardPrice")
                    {
                        if (str != "PyramidRevivePrice")
                        {
                            if (str == "BoguAdventurePrice")
                            {
                                strArray = BoguAdventurePrice.Split(new char[] { ',' });
                            }
                        }
                        else
                        {
                            strArray = PyramidRevivePrice.Split(new char[] { ',' });
                        }
                    }
                    else
                    {
                        strArray = NewChickenOpenCardPrice.Split(new char[] { ',' });
                    }
                }
                else
                {
                    strArray = NewChickenEagleEyePrice.Split(new char[] { ',' });
                }
            }
            foreach (string str2 in strArray)
            {
                list.Add(Convert.ToInt32(str2));
            }
            return list.ToArray();
        }

        public static List<int> getProp(string prop)
        {
            List<int> list = new List<int>();
            foreach (string str in prop.Split(new char[] { '|' }))
            {
                list.Add(Convert.ToInt32(str));
            }
            return list;
        }

        public static int HoleLevelUpExp(int lv)
        {
            return getProp(HoleLevelUpExpList)[lv];
        }

        public static int LimitLevel(int index)
        {
            return Convert.ToInt32(CustomLimit.Split(new char[] { '|' })[index]);
        }

        private static void Load(Type type)
        {
            using (ServiceBussiness bussiness = new ServiceBussiness())
            {
                foreach (FieldInfo info in type.GetFields())
                {
                    if (info.IsStatic)
                    {
                        object[] customAttributes = info.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
                        if (customAttributes.Length != 0)
                        {
                            ConfigPropertyAttribute attrib = (ConfigPropertyAttribute) customAttributes[0];
                            info.SetValue(null, LoadProperty(attrib, bussiness));
                        }
                    }
                }
            }
        }

        private static object LoadProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb)
        {
            string key = attrib.Key;
            ServerProperty serverPropertyByKey = sb.GetServerPropertyByKey(key);
            if (serverPropertyByKey == null)
            {
                serverPropertyByKey = new ServerProperty {
                    Key = key,
                    Value = attrib.DefaultValue.ToString()
                };
                log.Error("Cannot find server property " + key + ",keep it default value!");
            }
            try
            {
                return Convert.ChangeType(serverPropertyByKey.Value, attrib.DefaultValue.GetType());
            }
            catch (Exception exception)
            {
                log.Error("Exception in GameProperties Load: ", exception);
                return null;
            }
        }

        public static void Refresh()
        {
            log.Info("Refreshing game properties!");
            Load(typeof(GameProperties));
        }

        public static List<int> RuneExp()
        {
            return getProp(RuneLevelUpExp);
        }

        public static void Save()
        {
            log.Info("Saving game properties into db!");
            Save(typeof(GameProperties));
        }

        private static void Save(Type type)
        {
            using (ServiceBussiness bussiness = new ServiceBussiness())
            {
                foreach (FieldInfo info in type.GetFields())
                {
                    if (info.IsStatic)
                    {
                        object[] customAttributes = info.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
                        if (customAttributes.Length != 0)
                        {
                            ConfigPropertyAttribute attrib = (ConfigPropertyAttribute) customAttributes[0];
                            SaveProperty(attrib, bussiness, info.GetValue(null));
                        }
                    }
                }
            }
        }

        private static void SaveProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb, object value)
        {
            try
            {
                sb.UpdateServerPropertyByKey(attrib.Key, value.ToString());
            }
            catch (Exception exception)
            {
                log.Error("Exception in GameProperties Save: ", exception);
            }
        }

        public static List<int> VIPExp()
        {
            return getProp(VIPExpForEachLv);
        }

        public static int VIPStrengthenExp(int vipLv)
        {
            return getProp(VIPStrengthenEx)[vipLv];
        }
    }
}


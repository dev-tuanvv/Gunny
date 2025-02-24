namespace Game.Server.Achievement
{
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;

    public class BaseCondition
    {
        private BaseAchievement baseAchievement_0;
        private static readonly ILog ilog_0;
        private int int_0;
        protected AchievementCondictionInfo m_info;

        static BaseCondition()
        {
            ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public BaseCondition(BaseAchievement ach, AchievementCondictionInfo info, int value)
        {
            this.baseAchievement_0 = ach;
            this.m_info = info;
            this.int_0 = value;
        }

        public virtual void AddTrigger(GamePlayer player)
        {
        }

        public static BaseCondition CreateCondition(BaseAchievement ach, AchievementCondictionInfo info, int value)
        {
            switch (info.CondictionType)
            {
                case 1:
                    return new PropertisCharacterCondition(ach, info, value, "attack");

                case 2:
                    return new PropertisCharacterCondition(ach, info, value, "defence");

                case 3:
                    return new PropertisCharacterCondition(ach, info, value, "agility");

                case 4:
                    return new PropertisCharacterCondition(ach, info, value, "luck");

                case 5:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0xfa9, 0x100d, 0x1071, 0x10d5 });

                case 6:
                    return new GameOverPassCondition(ach, info, 6, value);

                case 7:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0xcf5 });

                case 8:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0x10d5 });

                case 9:
                    return new PropertisCharacterCondition(ach, info, value, "fightpower");

                case 10:
                    return new LevelUpgradeCondition(ach, info, value);

                case 11:
                    return new FightCompleteCondition(ach, info, value);

                case 13:
                    return new OnlineTimeCondition(ach, info, value);

                case 14:
                    return new FightMatchWinCondition(ach, info, value);

                case 15:
                    return new GuildFightWinCondition(ach, info, value);

                case 0x13:
                    return new FightKillPlayerCondition(ach, info, value);

                case 0x15:
                    return new QuestGreenFinishCondition(ach, info, value);

                case 0x16:
                    return new QuestDailyFinishCondition(ach, info, value);

                case 0x18:
                    return new DefaultCondition(ach, info, value);

                case 0x19:
                    return new DefaultCondition(ach, info, value);

                case 0x1a:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0x7d3, 0x837 });

                case 0x1b:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0x3ee, 0x452, 0x4b6, 0x51a });

                case 0x1c:
                    return new GameOverPassCondition(ach, info, 5, value);

                case 0x1d:
                    return new GameOverPassCondition(ach, info, 4, value);

                case 30:
                    return new GameOverPassCondition(ach, info, 3, value);

                case 0x1f:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0xbc9, 0xc2d, 0xc91, 0xcf5 });

                case 0x20:
                    return new DefaultCondition(ach, info, value);

                case 0x21:
                    return new HotSpingEnterCondition(ach, info, value);

                case 0x22:
                    return new UsingItemCondition(ach, info, 0x2724, value);

                case 0x23:
                    return new UsingItemCondition(ach, info, 0x2726, value);

                case 0x24:
                    return new DefaultCondition(ach, info, value);

                case 0x25:
                    return new DefaultCondition(ach, info, value);

                case 0x26:
                    return new GoldCollectionCondition(ach, info, value);

                case 0x27:
                    return new GiftTokenCollectionCondition(ach, info, value);

                case 40:
                    return new DefaultCondition(ach, info, value);

                case 0x29:
                    return new FightOneBloodIsWinCondition(ach, info, value);

                case 0x2a:
                    return new ItemEquipCondition(ach, info, value, 0x426a);

                case 0x2d:
                    return new DefaultCondition(ach, info, value);

                case 0x2f:
                    return new UseBigBugleCondition(ach, info, value);

                case 0x30:
                    return new UseSmaillBugleCondition(ach, info, value);

                case 50:
                    return new FightAddOfferCondition(ach, info, value);

                case 0x33:
                    return new DefaultCondition(ach, info, value);

                case 0x34:
                    return new DefaultCondition(ach, info, value);

                case 0x35:
                    return new DefaultCondition(ach, info, value);

                case 0x36:
                    return new DefaultCondition(ach, info, value);

                case 0x37:
                    return new DefaultCondition(ach, info, value);

                case 0x38:
                    return new DefaultCondition(ach, info, value);

                case 0x3b:
                    return new CompleteQuestGoodManCondtion(ach, info, value);

                case 60:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0x51c, 0x4b8, 0x454, 0x3f0 });

                case 0x3d:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0x517 });

                case 0x3e:
                    return new DefaultCondition(ach, info, value);

                case 0x41:
                    return new DefaultCondition(ach, info, value);

                case 0x42:
                    return new DefaultCondition(ach, info, value);

                case 0x43:
                    return new DefaultCondition(ach, info, value);

                case 0x44:
                    return new DefaultCondition(ach, info, value);

                case 0x48:
                    return new DefaultCondition(ach, info, value);

                case 0x49:
                    return new DefaultCondition(ach, info, value);

                case 0x4a:
                    return new GClass11(ach, info, value);

                case 0x4b:
                    return new DefaultCondition(ach, info, value);

                case 0x4c:
                    return new DefaultCondition(ach, info, value);

                case 0x4d:
                    return new DefaultCondition(ach, info, value);

                case 0x4e:
                    return new DefaultCondition(ach, info, value);

                case 0x4f:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0x1b77, 0x1bdb, 0x1c3f });

                case 80:
                    return new GameOverPassCondition(ach, info, 2, value);

                case 0x51:
                    return new DefaultCondition(ach, info, value);

                case 0x52:
                    return new GameOverPassCondition(ach, info, 1, value);

                case 0x53:
                    return new DefaultCondition(ach, info, value);

                case 0x54:
                    return new DefaultCondition(ach, info, value);

                case 0x55:
                    return new DefaultCondition(ach, info, value);

                case 0x56:
                    return new DefaultCondition(ach, info, value);

                case 0x57:
                    return new DefaultCondition(ach, info, value);

                case 0x58:
                    return new DefaultCondition(ach, info, value);

                case 0x59:
                    return new GameKillingBossCondition(ach, info, value, new int[] { 0x140b, 0x146f, 0x14d3 });

                case 90:
                    return new GameOverPassCondition(ach, info, 3, value);

                case 0x5b:
                    return new DefaultCondition(ach, info, value);

                case 0x5c:
                    return new DefaultCondition(ach, info, value);

                case 0x5d:
                    return new DefaultCondition(ach, info, value);

                case 0x5e:
                    return new DefaultCondition(ach, info, value);

                case 0x5f:
                    return new DefaultCondition(ach, info, value);
            }
            if (ilog_0.IsErrorEnabled)
            {
                ilog_0.Error(string.Format("Can't find achievement condition : {0}", info.CondictionType));
            }
            return null;
        }

        public virtual bool Finish(GamePlayer player)
        {
            return true;
        }

        public virtual bool IsCompleted(GamePlayer player)
        {
            return false;
        }

        public virtual void RemoveTrigger(GamePlayer player)
        {
        }

        public AchievementCondictionInfo Info
        {
            get
            {
                return this.m_info;
            }
        }

        public int Value
        {
            get
            {
                return this.int_0;
            }
            set
            {
                if (this.int_0 != value)
                {
                    this.int_0 = value;
                    this.baseAchievement_0.Update();
                }
            }
        }
    }
}


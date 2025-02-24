namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;

    public class BaseCondition
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected QuestConditionInfo m_info;
        private BaseQuest m_quest;
        private int m_value;

        public BaseCondition(BaseQuest quest, QuestConditionInfo info, int value)
        {
            this.m_quest = quest;
            this.m_info = info;
            this.m_value = value;
        }

        public virtual void AddTrigger(GamePlayer player)
        {
        }

        public virtual bool CancelFinish(GamePlayer player)
        {
            return true;
        }

        public static BaseCondition CreateCondition(BaseQuest quest, QuestConditionInfo info, int value)
        {
            switch (info.CondictionType)
            {
                case 1:
                    return new OwnGradeCondition(quest, info, value);

                case 2:
                    return new ItemMountingCondition(quest, info, value);

                case 3:
                    return new UsingItemCondition(quest, info, value);

                case 4:
                    return new GameKillByRoomCondition(quest, info, value);

                case 5:
                    return new GameFightByRoomCondition(quest, info, value);

                case 6:
                    return new GameOverByRoomCondition(quest, info, value);

                case 7:
                    return new GameCopyOverCondition(quest, info, value);

                case 8:
                    return new GameCopyPassCondition(quest, info, value);

                case 9:
                    return new ItemStrengthenCondition(quest, info, value);

                case 10:
                    return new Game.Server.Quests.ShopCondition(quest, info, value);

                case 11:
                    return new ItemFusionCondition(quest, info, value);

                case 12:
                    return new ItemMeltCondition(quest, info, value);

                case 13:
                    return new GameMonsterCondition(quest, info, value);

                case 14:
                    return new OwnPropertyCondition(quest, info, value);

                case 15:
                    return new TurnPropertyCondition(quest, info, value);

                case 0x10:
                    return new DirectFinishCondition(quest, info, value);

                case 0x11:
                    return new OwnMarryCondition(quest, info, value);

                case 0x12:
                    return new OwnConsortiaCondition(quest, info, value);

                case 0x13:
                    return new ItemComposeCondition(quest, info, value);

                case 20:
                    return new ClientModifyCondition(quest, info, value);

                case 0x15:
                    return new GameMissionOverCondition(quest, info, value);

                case 0x16:
                    return new GameKillByGameCondition(quest, info, value);

                case 0x17:
                    return new GameFightByGameCondition(quest, info, value);

                case 0x18:
                    return new GameOverByGameCondition(quest, info, value);

                case 0x19:
                    return new ItemInsertCondition(quest, info, value);

                case 0x1a:
                    return new MarryCondition(quest, info, value);

                case 0x1b:
                    return new EnterSpaCondition(quest, info, value);

                case 0x1c:
                    return new FightWifeHusbandCondition(quest, info, value);

                case 30:
                    return new AchievementCondition(quest, info, value);

                case 0x1f:
                    return new GameFightByGameCondition(quest, info, value);

                case 0x20:
                    return new SharePersonalStatusCondition(quest, info, value);

                case 0x21:
                    return new SendGiftForFriendCondition(quest, info, value);

                case 0x22:
                    return new GameFihgt2v2Condition(quest, info, value);

                case 0x23:
                    return new MasterApprenticeshipCondition(quest, info, value);

                case 0x24:
                    return new GameFightApprenticeshipCondition(quest, info, value);

                case 0x25:
                    return new GameFightMasterApprenticeshipCondition(quest, info, value);

                case 0x26:
                    return new CashCondition(quest, info, value);

                case 0x27:
                    return new NewGearCondition(quest, info, value);

                case 0x2a:
                    return new AccuontInfoCondition(quest, info, value);

                case 0x2b:
                    return new LoginMissionCondition(quest, info, value);

                case 0x2c:
                    return new SetPasswordTwoCondition(quest, info, value);

                case 0x2d:
                    return new FightWithPetCondition(quest, info, value);

                case 0x2e:
                    return new CombiePetFeedCondition(quest, info, value);

                case 0x2f:
                    return new FriendFarmCondition(quest, info, value);

                case 0x30:
                    return new AdoptPetCondition(quest, info, value);

                case 0x31:
                    return new CropPrimaryCondition(quest, info, value);

                case 50:
                    return new UpLevelPetCondition(quest, info, value);

                case 0x33:
                    return new SeedFoodPetCondition(quest, info, value);

                case 0x34:
                    return new UserSkillPetCondition(quest, info, value);

                case 0x36:
                    return new UserToemGemstoneCondition(quest, info, value);
            }
            return new UnknowQuestCondition(quest, info, value);
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

        public virtual void Reset(GamePlayer player)
        {
            this.m_value = this.m_info.Para2;
        }

        public QuestConditionInfo Info
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
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    this.m_value = value;
                    this.m_quest.Update();
                }
            }
        }
    }
}


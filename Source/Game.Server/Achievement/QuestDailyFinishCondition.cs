namespace Game.Server.Achievement
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class QuestDailyFinishCondition : BaseCondition
    {
        public QuestDailyFinishCondition(BaseAchievement quest, AchievementCondictionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.QuestFinishEvent += new GamePlayer.PlayerQuestFinish(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(QuestDataInfo questDataInfo_0, QuestInfo questInfo_0)
        {
            if (questInfo_0.QuestID == 2)
            {
                int num = base.Value;
                base.Value = num + 1;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.QuestFinishEvent -= new GamePlayer.PlayerQuestFinish(this.method_0);
        }
    }
}


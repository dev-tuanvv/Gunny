namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class ClickOpenCondition : BaseCondition
    {
        public ClickOpenCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return true;
        }
    }
}


namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class ClientModifyCondition : BaseCondition
    {
        public ClientModifyCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        public override void Reset(GamePlayer player)
        {
            base.Value = 1;
        }
    }
}


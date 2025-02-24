namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class OwnPropertyCondition : BaseCondition
    {
        public OwnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
        }

        public override bool IsCompleted(GamePlayer player)
        {
            if (player.GetItemCount(base.m_info.Para1) >= base.m_info.Para2)
            {
                base.Value = 0;
                return true;
            }
            return false;
        }

        private void player_OwnProperty()
        {
        }

        public override void RemoveTrigger(GamePlayer player)
        {
        }
    }
}


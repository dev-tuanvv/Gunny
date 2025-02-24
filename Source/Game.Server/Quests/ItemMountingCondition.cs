namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class ItemMountingCondition : BaseCondition
    {
        public ItemMountingCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (player.EquipBag.GetItemCount(0, base.m_info.Para1) >= base.m_info.Para2);
        }
    }
}


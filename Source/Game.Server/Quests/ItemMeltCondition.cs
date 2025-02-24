namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class ItemMeltCondition : BaseCondition
    {
        public ItemMeltCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.ItemMelt += new GamePlayer.PlayerItemMeltEventHandle(this.player_ItemMelt);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        private void player_ItemMelt(int categoryID)
        {
            if (categoryID == base.m_info.Para1)
            {
                base.Value = 0;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.ItemMelt -= new GamePlayer.PlayerItemMeltEventHandle(this.player_ItemMelt);
        }
    }
}


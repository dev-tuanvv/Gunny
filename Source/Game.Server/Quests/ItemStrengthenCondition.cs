namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class ItemStrengthenCondition : BaseCondition
    {
        public ItemStrengthenCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.ItemStrengthen += new GamePlayer.PlayerItemStrengthenEventHandle(this.player_ItemStrengthen);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return true;
        }

        private void player_ItemStrengthen(int categoryID, int level)
        {
            if ((base.m_info.Para1 == categoryID) && (base.m_info.Para2 <= level))
            {
                base.Value = 0;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.ItemStrengthen -= new GamePlayer.PlayerItemStrengthenEventHandle(this.player_ItemStrengthen);
        }
    }
}


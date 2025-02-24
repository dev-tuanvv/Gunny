namespace Game.Server.Achievement
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class ItemEquipCondition : BaseCondition
    {
        private int int_1;

        public ItemEquipCondition(BaseAchievement quest, AchievementCondictionInfo info, int value, int templateid) : base(quest, info, value)
        {
            this.int_1 = templateid;
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.NewGearEvent2 += new GamePlayer.PlayerNewGearEventHandle2(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(SqlDataProvider.Data.ItemInfo itemInfo_0)
        {
            if (itemInfo_0.TemplateID == this.int_1)
            {
                int num = base.Value;
                base.Value = num + 1;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.NewGearEvent2 -= new GamePlayer.PlayerNewGearEventHandle2(this.method_0);
        }
    }
}


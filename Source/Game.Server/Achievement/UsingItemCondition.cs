namespace Game.Server.Achievement
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class UsingItemCondition : BaseCondition
    {
        private int int_1;

        public UsingItemCondition(BaseAchievement quest, AchievementCondictionInfo info, int templateid, int value) : base(quest, info, value)
        {
            this.int_1 = templateid;
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.AfterUsingItem += new GamePlayer.PlayerItemPropertyEventHandle(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(int int_2)
        {
            if (this.int_1 == int_2)
            {
                int num = base.Value;
                base.Value = num + 1;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.AfterUsingItem -= new GamePlayer.PlayerItemPropertyEventHandle(this.method_0);
        }
    }
}


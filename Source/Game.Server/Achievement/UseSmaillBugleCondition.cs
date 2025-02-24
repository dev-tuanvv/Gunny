namespace Game.Server.Achievement
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class UseSmaillBugleCondition : BaseCondition
    {
        public UseSmaillBugleCondition(BaseAchievement quest, AchievementCondictionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.UseBugle += new GamePlayer.PlayerUseBugle(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(int int_1)
        {
            if (int_1 == 0x2b5d)
            {
                int num = base.Value;
                base.Value = num + 1;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.UseBugle -= new GamePlayer.PlayerUseBugle(this.method_0);
        }
    }
}


namespace Game.Server.Achievement
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class FightKillPlayerCondition : BaseCondition
    {
        public FightKillPlayerCondition(BaseAchievement quest, AchievementCondictionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.AfterKillingLiving += new GamePlayer.PlayerGameKillEventHandel(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(AbstractGame abstractGame_0, int int_1, int int_2, bool bool_0, int int_3)
        {
            if (!((int_1 != 1) || bool_0))
            {
                int num = base.Value;
                base.Value = num + 1;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.AfterKillingLiving -= new GamePlayer.PlayerGameKillEventHandel(this.method_0);
        }
    }
}


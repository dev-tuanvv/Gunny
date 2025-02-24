namespace Game.Server.Achievement
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;
    using System.Linq;

    public class GameKillingBossCondition : BaseCondition
    {
        private int[] int_1;

        public GameKillingBossCondition(BaseAchievement quest, AchievementCondictionInfo info, int value, int[] arrayId) : base(quest, info, value)
        {
            this.int_1 = arrayId;
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.AfterKillingBoss += new GamePlayer.PlayerGameKillBossEventHandel(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(AbstractGame abstractGame_0, NpcInfo npcInfo_0, int int_2)
        {
            if (this.int_1.Contains<int>(npcInfo_0.ID))
            {
                int num = base.Value;
                base.Value = num + 1;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.AfterKillingBoss -= new GamePlayer.PlayerGameKillBossEventHandel(this.method_0);
        }
    }
}


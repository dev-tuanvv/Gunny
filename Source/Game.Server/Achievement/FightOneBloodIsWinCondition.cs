namespace Game.Server.Achievement
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class FightOneBloodIsWinCondition : BaseCondition
    {
        public FightOneBloodIsWinCondition(BaseAchievement quest, AchievementCondictionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.FightOneBloodIsWin += new GamePlayer.PlayerFightOneBloodIsWin(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(eRoomType eRoomType_0)
        {
            int num = base.Value;
            base.Value = num + 1;
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.FightOneBloodIsWin -= new GamePlayer.PlayerFightOneBloodIsWin(this.method_0);
        }
    }
}


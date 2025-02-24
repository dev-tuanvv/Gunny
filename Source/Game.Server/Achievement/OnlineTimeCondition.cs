namespace Game.Server.Achievement
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class OnlineTimeCondition : BaseCondition
    {
        public OnlineTimeCondition(BaseAchievement quest, AchievementCondictionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.PingTimeOnline += new GamePlayer.PlayerEventHandle(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(GamePlayer gamePlayer_0)
        {
            int num = base.Value;
            base.Value = num + 1;
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.PingTimeOnline -= new GamePlayer.PlayerEventHandle(this.method_0);
        }
    }
}


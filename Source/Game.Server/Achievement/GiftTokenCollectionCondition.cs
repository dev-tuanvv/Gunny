namespace Game.Server.Achievement
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GiftTokenCollectionCondition : BaseCondition
    {
        public GiftTokenCollectionCondition(BaseAchievement quest, AchievementCondictionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.GiftTokenCollect += new GamePlayer.PlayerGiftTokenCollection(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(int int_1)
        {
            base.Value += int_1;
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.GiftTokenCollect -= new GamePlayer.PlayerGiftTokenCollection(this.method_0);
        }
    }
}


namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class SeedFoodPetCondition : BaseCondition
    {
        public SeedFoodPetCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.SeedFoodPetEvent += new GamePlayer.PlayerSeedFoodPetEventHandle(this.player_SeedFoodPet);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Para2);
        }

        private void player_SeedFoodPet()
        {
            if (base.Value < base.m_info.Para2)
            {
                base.Value++;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.SeedFoodPetEvent -= new GamePlayer.PlayerSeedFoodPetEventHandle(this.player_SeedFoodPet);
        }
    }
}


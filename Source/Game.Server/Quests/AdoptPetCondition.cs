namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class AdoptPetCondition : BaseCondition
    {
        public AdoptPetCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.AdoptPetEvent += new GamePlayer.PlayerAdoptPetEventHandle(this.player_AdoptPet);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Para2);
        }

        private void player_AdoptPet()
        {
            if (base.Value < base.m_info.Para2)
            {
                base.Value++;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.AdoptPetEvent -= new GamePlayer.PlayerAdoptPetEventHandle(this.player_AdoptPet);
        }
    }
}


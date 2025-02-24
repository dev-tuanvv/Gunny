namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class UpLevelPetCondition : BaseCondition
    {
        public UpLevelPetCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.UpLevelPetEvent += new GamePlayer.PlayerUpLevelPetEventHandle(this.player_MissionOver);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Para2);
        }

        private void player_MissionOver()
        {
            base.Value = base.m_info.Para2;
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.UpLevelPetEvent -= new GamePlayer.PlayerUpLevelPetEventHandle(this.player_MissionOver);
        }
    }
}


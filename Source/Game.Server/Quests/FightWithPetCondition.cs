﻿namespace Game.Server.Quests
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class FightWithPetCondition : BaseCondition
    {
        public FightWithPetCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        private void player_MissionOver(AbstractGame game, int missionId, int turnCount)
        {
        }

        public override void RemoveTrigger(GamePlayer player)
        {
        }
    }
}


﻿namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class OwnMarryCondition : BaseCondition
    {
        public OwnMarryCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
        }

        public override bool IsCompleted(GamePlayer player)
        {
            if (player.PlayerCharacter.IsMarried)
            {
                base.Value = 0;
                return true;
            }
            return false;
        }

        public override void RemoveTrigger(GamePlayer player)
        {
        }
    }
}


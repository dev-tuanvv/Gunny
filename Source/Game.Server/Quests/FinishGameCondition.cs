﻿namespace Game.Server.Quests
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class FinishGameCondition : BaseCondition
    {
        public FinishGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        private void player_GameOver(AbstractGame game, bool isWin, int gainXp)
        {
            if (base.Value < base.m_info.Para1)
            {
                base.Value++;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
        }
    }
}


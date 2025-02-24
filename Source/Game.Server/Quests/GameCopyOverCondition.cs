namespace Game.Server.Quests
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GameCopyOverCondition : BaseCondition
    {
        public GameCopyOverCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.MissionOver += new GamePlayer.PlayerMissionOverEventHandle(this.player_MissionOver);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        private void player_MissionOver(AbstractGame game, int missionId, bool isWin)
        {
            if (((missionId == base.m_info.Para1) || (base.m_info.Para1 == -1)) && (base.Value > 0))
            {
                base.Value--;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.MissionOver -= new GamePlayer.PlayerMissionOverEventHandle(this.player_MissionOver);
        }
    }
}


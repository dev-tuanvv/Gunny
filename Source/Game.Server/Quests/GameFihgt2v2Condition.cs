namespace Game.Server.Quests
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GameFihgt2v2Condition : BaseCondition
    {
        public GameFihgt2v2Condition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Para2);
        }

        private void player_GameOver(AbstractGame game, bool isWin, int gainXp)
        {
            eGameType gameType = game.GameType;
            switch (gameType)
            {
                case eGameType.Free:
                    if (((base.m_info.Para1 == 0) || (base.m_info.Para1 == -1)) && (base.Value < base.m_info.Para2))
                    {
                        base.Value++;
                    }
                    break;

                case eGameType.Guild:
                    if (((base.m_info.Para1 == 1) || (base.m_info.Para1 == -1)) && (base.Value < base.m_info.Para2))
                    {
                        base.Value++;
                    }
                    break;

                case eGameType.Training:
                case eGameType.Boss:
                    break;

                case eGameType.ALL:
                    if (((base.m_info.Para1 == 4) || (base.m_info.Para1 == -1)) && (base.Value < base.m_info.Para2))
                    {
                        base.Value++;
                    }
                    break;

                default:
                    if ((gameType == eGameType.Dungeon) && (((base.m_info.Para1 == 7) || (base.m_info.Para1 == -1)) && (base.Value < base.m_info.Para2)))
                    {
                        base.Value++;
                    }
                    break;
            }
            if (base.Value > base.m_info.Para2)
            {
                base.Value = base.m_info.Para2;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
        }
    }
}


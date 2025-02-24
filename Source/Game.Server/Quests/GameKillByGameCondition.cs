namespace Game.Server.Quests
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GameKillByGameCondition : BaseCondition
    {
        public GameKillByGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.AfterKillingLiving += new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
        {
            if (!isLiving && (type == 1))
            {
                switch (game.GameType)
                {
                    case eGameType.Free:
                        if (((base.m_info.Para1 == 0) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eGameType.Guild:
                        if (((base.m_info.Para1 == 1) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eGameType.Training:
                        if (((base.m_info.Para1 == 2) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eGameType.Boss:
                        if (((base.m_info.Para1 == 6) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eGameType.ALL:
                        if (((base.m_info.Para1 == 4) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eGameType.Exploration:
                        if (((base.m_info.Para1 == 5) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eGameType.Dungeon:
                        if (((base.m_info.Para1 == 7) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;
                }
                if (base.Value < 0)
                {
                    base.Value = 0;
                }
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.AfterKillingLiving -= new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
        }
    }
}


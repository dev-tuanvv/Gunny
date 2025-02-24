namespace Game.Server.Quests
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GameKillByRoomCondition : BaseCondition
    {
        public GameKillByRoomCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
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
            if (!isLiving)
            {
            }
            if (!isLiving && (type == 1))
            {
                switch (game.RoomType)
                {
                    case eRoomType.Match:
                        if (((base.m_info.Para1 == 0) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eRoomType.Freedom:
                        if (((base.m_info.Para1 == 1) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eRoomType.Exploration:
                        if (((base.m_info.Para1 == 2) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eRoomType.BattleRoom:
                        if (((base.m_info.Para1 == 3) || (base.m_info.Para1 == -1)) && (base.Value > 0))
                        {
                            base.Value--;
                        }
                        break;

                    case eRoomType.Dungeon:
                        if (((base.m_info.Para1 == 4) || (base.m_info.Para1 == -1)) && (base.Value > 0))
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


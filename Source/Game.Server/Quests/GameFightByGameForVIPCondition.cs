namespace Game.Server.Quests
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GameFightByGameForVIPCondition : BaseCondition
    {
        public GameFightByGameForVIPCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        private void method_0(AbstractGame abstractGame_0, bool bool_0, int int_1)
        {
            if (bool_0)
            {
                switch (abstractGame_0.GameType)
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
            player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.method_0);
        }
    }
}


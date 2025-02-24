namespace Game.Server.Achievement
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class FightMatchWinCondition : BaseCondition
    {
        public FightMatchWinCondition(BaseAchievement quest, AchievementCondictionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(AbstractGame abstractGame_0, bool bool_0, int int_1)
        {
            if ((abstractGame_0.GameType == eGameType.Free) && (abstractGame_0.RoomType == eRoomType.Match))
            {
                int num = base.Value;
                base.Value = num + 1;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.method_0);
        }
    }
}


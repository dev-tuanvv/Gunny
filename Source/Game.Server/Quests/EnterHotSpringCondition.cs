namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class EnterHotSpringCondition : BaseCondition
    {
        public EnterHotSpringCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.EnterHotSpringEvent += new GamePlayer.PlayerEnterHotSpring(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        private void method_0(GamePlayer gamePlayer_0)
        {
            int num = base.Value;
            base.Value = num - 1;
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.EnterHotSpringEvent -= new GamePlayer.PlayerEnterHotSpring(this.method_0);
        }
    }
}


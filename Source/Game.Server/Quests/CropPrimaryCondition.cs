namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class CropPrimaryCondition : BaseCondition
    {
        public CropPrimaryCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.CropPrimaryEvent += new GamePlayer.PlayerCropPrimaryEventHandle(this.player_CropPrimary);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Para2);
        }

        private void player_CropPrimary()
        {
            if (base.Value < base.m_info.Para2)
            {
                base.Value++;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.CropPrimaryEvent -= new GamePlayer.PlayerCropPrimaryEventHandle(this.player_CropPrimary);
        }
    }
}


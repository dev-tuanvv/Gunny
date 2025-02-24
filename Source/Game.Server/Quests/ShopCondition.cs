namespace Game.Server.Quests
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class ShopCondition : BaseCondition
    {
        public ShopCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.Paid += new GamePlayer.PlayerShopEventHandle(this.player_Shop);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value <= 0);
        }

        private void player_Shop(int money, int gold, int offer, int gifttoken, int medal, string payGoods)
        {
            if ((base.m_info.Para1 == -1) && (money > 0))
            {
                base.Value -= money;
            }
            if ((base.m_info.Para1 == -2) && (gold > 0))
            {
                base.Value -= gold;
            }
            if ((base.m_info.Para1 == -3) && (offer > 0))
            {
                base.Value -= offer;
            }
            if ((base.m_info.Para1 == -4) && (gifttoken > 0))
            {
                base.Value -= gifttoken;
            }
            string[] strArray = payGoods.Split(new char[] { ',' });
            foreach (string str in strArray)
            {
                if (str == base.m_info.Para1.ToString())
                {
                    base.Value--;
                }
            }
            if (base.Value < 0)
            {
                base.Value = 0;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.Paid -= new GamePlayer.PlayerShopEventHandle(this.player_Shop);
        }
    }
}


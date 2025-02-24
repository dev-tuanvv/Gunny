namespace Game.Server.Quests
{
    using Bussiness.Managers;
    using Game.Logic;
    using Game.Server.GameObjects;
    using Game.Server.Statics;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    public class TurnPropertyCondition : BaseCondition
    {
        private GamePlayer m_player;
        private BaseQuest m_quest;

        public TurnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {
            this.m_quest = quest;
        }

        public override void AddTrigger(GamePlayer player)
        {
            this.m_player = player;
            player.GameKillDrop += new GamePlayer.GameKillDropEventHandel(this.QuestDropItem);
            base.AddTrigger(player);
        }

        public override bool CancelFinish(GamePlayer player)
        {
            ItemTemplateInfo goods = ItemMgr.FindItemTemplate(base.m_info.Para1);
            if (goods != null)
            {
                SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, base.m_info.Para2, 0x75);
                return player.AddTemplate(cloneItem, eBageType.TempBag, base.m_info.Para2, eGameView.OtherTypeGet);
            }
            return false;
        }

        public override bool Finish(GamePlayer player)
        {
            return player.RemoveTemplate(base.m_info.Para1, base.m_info.Para2);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            bool flag = false;
            if (player.GetItemCount(base.m_info.Para1) >= base.m_info.Para2)
            {
                base.Value = 0;
                flag = true;
            }
            return flag;
        }

        private void QuestDropItem(AbstractGame game, int copyId, int npcId, bool playResult)
        {
            if (this.m_player.GetItemCount(base.m_info.Para1) < base.m_info.Para2)
            {
                List<SqlDataProvider.Data.ItemInfo> list = null;
                int gold = 0;
                int money = 0;
                int giftToken = 0;
                if (game is PVEGame)
                {
                    DropInventory.PvEQuestsDrop(npcId, ref list);
                }
                if (game is PVPGame)
                {
                    DropInventory.PvPQuestsDrop(game.RoomType, playResult, ref list);
                }
                if (list != null)
                {
                    foreach (SqlDataProvider.Data.ItemInfo info in list)
                    {
                        SqlDataProvider.Data.ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
                        if (info != null)
                        {
                            this.m_player.TempBag.AddTemplate(info, info.Count);
                        }
                    }
                    this.m_player.AddGold(gold);
                    this.m_player.AddGiftToken(giftToken);
                    this.m_player.AddMoney(money);
                    LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Drop, this.m_player.PlayerCharacter.ID, money, this.m_player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                }
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.GameKillDrop -= new GamePlayer.GameKillDropEventHandel(this.QuestDropItem);
            base.RemoveTrigger(player);
        }
    }
}


namespace Game.Server.Quests
{
    using Bussiness.Managers;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    public class BaseQuest
    {
        private QuestDataInfo m_data;
        private QuestInfo m_info;
        private List<BaseCondition> m_list;
        private DateTime m_oldFinishDate;
        private GamePlayer m_player;

        public BaseQuest(QuestInfo info, QuestDataInfo data)
        {
            this.m_info = info;
            this.m_data = data;
            this.m_data.QuestID = this.m_info.ID;
            this.m_list = new List<BaseCondition>();
            List<QuestConditionInfo> questCondiction = QuestMgr.GetQuestCondiction(info);
            int num = 0;
            foreach (QuestConditionInfo info2 in questCondiction)
            {
                BaseCondition item = BaseCondition.CreateCondition(this, info2, data.GetConditionValue(num++));
                if (item != null)
                {
                    this.m_list.Add(item);
                }
            }
        }

        public void AddToPlayer(GamePlayer player)
        {
            this.m_player = player;
            if (!this.m_data.IsComplete)
            {
                this.AddTrigger(player);
            }
        }

        private void AddTrigger(GamePlayer player)
        {
            foreach (BaseCondition condition in this.m_list)
            {
                condition.AddTrigger(player);
            }
        }

        public bool CancelFinish(GamePlayer player)
        {
            this.m_data.IsComplete = false;
            this.m_data.CompletedDate = this.m_oldFinishDate;
            foreach (BaseCondition condition in this.m_list)
            {
                condition.CancelFinish(player);
            }
            return true;
        }

        public bool CanCompleted(GamePlayer player)
        {
            if (this.m_data.IsComplete)
            {
                return false;
            }
            foreach (BaseCondition condition in this.m_list)
            {
                if (!condition.IsCompleted(player))
                {
                    return false;
                }
            }
            return true;
        }

        public bool Finish(GamePlayer player)
        {
            if (this.CanCompleted(player))
            {
                foreach (BaseCondition condition in this.m_list)
                {
                    if (!condition.Finish(player))
                    {
                        return false;
                    }
                }
                if (!this.Info.CanRepeat)
                {
                    this.m_data.IsComplete = true;
                    this.RemveTrigger(player);
                }
                this.m_oldFinishDate = this.m_data.CompletedDate;
                this.m_data.CompletedDate = DateTime.Now;
                return true;
            }
            return false;
        }

        public BaseCondition GetConditionById(int id)
        {
            foreach (BaseCondition condition in this.m_list)
            {
                if (condition.Info.CondictionID == id)
                {
                    return condition;
                }
            }
            return null;
        }

        public void RemoveFromPlayer(GamePlayer player)
        {
            if (!this.m_data.IsComplete)
            {
                this.RemveTrigger(player);
            }
            this.m_player = null;
        }

        private void RemveTrigger(GamePlayer player)
        {
            foreach (BaseCondition condition in this.m_list)
            {
                condition.RemoveTrigger(player);
            }
        }

        public void Reset(GamePlayer player, int rand)
        {
            this.m_data.QuestID = this.m_info.ID;
            this.m_data.UserID = player.PlayerId;
            this.m_data.IsComplete = false;
            this.m_data.IsExist = true;
            if (this.m_data.CompletedDate == DateTime.MinValue)
            {
                this.m_data.CompletedDate = DateTime.Now;
            }
            TimeSpan span = (TimeSpan) (DateTime.Now - this.m_data.CompletedDate);
            if (span.TotalDays >= this.m_info.RepeatInterval)
            {
                this.m_data.RepeatFinish = this.m_info.RepeatMax;
            }
            this.m_data.RepeatFinish--;
            this.m_data.RandDobule = rand;
            foreach (BaseCondition condition in this.m_list)
            {
                condition.Reset(player);
            }
            this.SaveData();
        }

        public void SaveData()
        {
            int num = 0;
            foreach (BaseCondition condition in this.m_list)
            {
                this.m_data.SaveConditionValue(num++, condition.Value);
            }
        }

        public void Update()
        {
            this.SaveData();
            if (this.m_data.IsDirty && (this.m_player != null))
            {
                this.m_player.QuestInventory.Update(this);
            }
        }

        public QuestDataInfo Data
        {
            get
            {
                return this.m_data;
            }
        }

        public QuestInfo Info
        {
            get
            {
                return this.m_info;
            }
        }
    }
}


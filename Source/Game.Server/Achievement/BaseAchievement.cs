namespace Game.Server.Achievement
{
    using Bussiness.Managers;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    public class BaseAchievement
    {
        private AchievementData achievementData_0;
        private AchievementInfo achievementInfo_0;
        private GamePlayer gamePlayer_0;
        private List<BaseCondition> list_0;

        public BaseAchievement(AchievementInfo info, AchievementData data)
        {
            this.CreateBaseAchievement(info, data, null);
        }

        public BaseAchievement(AchievementInfo info, AchievementData data, Dictionary<int, AchievementProcessInfo> processInfo)
        {
            this.CreateBaseAchievement(info, data, processInfo);
        }

        public void AddToPlayer(GamePlayer player)
        {
            this.gamePlayer_0 = player;
            this.achievementData_0.UserID = player.PlayerCharacter.ID;
            if (!this.achievementData_0.IsComplete)
            {
                this.method_0(player);
            }
        }

        public bool CanCompleted(GamePlayer player)
        {
            if (this.achievementData_0.IsComplete)
            {
                return false;
            }
            using (List<BaseCondition>.Enumerator enumerator = this.list_0.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!enumerator.Current.IsCompleted(player))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void CreateBaseAchievement(AchievementInfo info, AchievementData data, Dictionary<int, AchievementProcessInfo> processInfo)
        {
            this.achievementInfo_0 = info;
            this.achievementData_0 = data;
            this.achievementData_0.AchievementID = this.achievementInfo_0.ID;
            this.list_0 = new List<BaseCondition>();
            foreach (AchievementCondictionInfo info2 in QuestMgr.GetAchievementCondiction(info))
            {
                int num = 0;
                if ((processInfo != null) && processInfo.ContainsKey(info2.CondictionType))
                {
                    num = processInfo[info2.CondictionType].Value;
                }
                BaseCondition item = BaseCondition.CreateCondition(this, info2, num);
                if (item != null)
                {
                    this.list_0.Add(item);
                }
            }
        }

        public bool Finish(GamePlayer player)
        {
            if (!this.CanCompleted(player))
            {
                return false;
            }
            using (List<BaseCondition>.Enumerator enumerator = this.list_0.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!enumerator.Current.Finish(player))
                    {
                        return false;
                    }
                }
            }
            this.achievementData_0.IsComplete = true;
            this.method_1(player);
            this.achievementData_0.DateComplete = DateTime.Now;
            return true;
        }

        public BaseCondition GetConditionById(int id)
        {
            foreach (BaseCondition condition in this.list_0)
            {
                if (condition.Info.CondictionID == id)
                {
                    return condition;
                }
            }
            return null;
        }

        private void method_0(GamePlayer gamePlayer_1)
        {
            using (List<BaseCondition>.Enumerator enumerator = this.list_0.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.AddTrigger(gamePlayer_1);
                }
            }
        }

        private void method_1(GamePlayer gamePlayer_1)
        {
            using (List<BaseCondition>.Enumerator enumerator = this.list_0.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.RemoveTrigger(gamePlayer_1);
                }
            }
        }

        public void RemoveFromPlayer(GamePlayer player)
        {
            if (this.achievementData_0.IsComplete)
            {
                this.method_1(player);
            }
            this.gamePlayer_0 = null;
        }

        public void SaveData()
        {
            if (this.gamePlayer_0 != null)
            {
                foreach (BaseCondition condition in this.list_0)
                {
                    this.gamePlayer_0.AchievementInventory.UpdateProcess(condition);
                }
            }
        }

        public void Update()
        {
            this.SaveData();
            if (this.gamePlayer_0 != null)
            {
                this.gamePlayer_0.AchievementInventory.Update(this);
                if (this.CanCompleted(this.gamePlayer_0))
                {
                    this.gamePlayer_0.AchievementInventory.Finish(this);
                }
            }
        }

        public AchievementData Data
        {
            get
            {
                return this.achievementData_0;
            }
        }

        public AchievementInfo Info
        {
            get
            {
                return this.achievementInfo_0;
            }
        }
    }
}


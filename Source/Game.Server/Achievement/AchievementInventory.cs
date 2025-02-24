namespace Game.Server.Achievement
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    public class AchievementInventory
    {
        private Dictionary<int, AchievementData> dictionary_0;
        private Dictionary<int, AchievementProcessInfo> dictionary_1;
        private GamePlayer gamePlayer_0;
        private static readonly ILog ilog_0;
        private int int_0;
        protected List<AchievementProcessInfo> m_changedAchs;
        protected List<BaseAchievement> m_list;
        private object object_0;

        static AchievementInventory()
        {
            ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public AchievementInventory(GamePlayer player)
        {
            this.m_changedAchs = new List<AchievementProcessInfo>();
            this.gamePlayer_0 = player;
            this.object_0 = new object();
            this.m_list = new List<BaseAchievement>();
            this.dictionary_1 = new Dictionary<int, AchievementProcessInfo>();
            this.dictionary_0 = new Dictionary<int, AchievementData>();
        }

        public bool AddAchievement(AchievementInfo info)
        {
            try
            {
                if (((info == null) || (this.gamePlayer_0.PlayerCharacter.Grade < info.NeedMinLevel)) || (this.gamePlayer_0.PlayerCharacter.Grade > info.NeedMaxLevel))
                {
                    return false;
                }
                if (info.PreAchievementID != "0,")
                {
                    char[] separator = new char[] { ',' };
                    string[] strArray = info.PreAchievementID.Split(separator);
                    for (int i = 0; i < (strArray.Length - 1); i++)
                    {
                        if (!this.method_2(Convert.ToInt32(strArray[i])))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ilog_0.Info(exception.InnerException);
            }
            if (this.FindAchievement(info.ID) != null)
            {
                return false;
            }
            this.method_4();
            BaseAchievement achievement = new BaseAchievement(info, new AchievementData(), this.GetRealProcessAchievement());
            this.method_0(achievement);
            this.method_5();
            return true;
        }

        public void AddAchievementPre()
        {
            foreach (AchievementInfo info in QuestMgr.GetAllAchievements())
            {
                if (!((this.FindAchievement(info.ID) != null) || this.method_2(info.ID)))
                {
                    this.AddAchievement(info);
                }
            }
        }

        public void AddProcess(AchievementProcessInfo info)
        {
            object obj2 = this.object_0;
            lock (obj2)
            {
                this.method_4();
                if (!this.dictionary_1.ContainsKey(info.CondictionType))
                {
                    this.dictionary_1.Add(info.CondictionType, info);
                }
                this.method_5();
                this.gamePlayer_0.PlayerCharacter.AchievementProcess = this.method_7();
                this.OnAchievementsChanged(info);
            }
        }

        public BaseAchievement FindAchievement(int id)
        {
            foreach (BaseAchievement achievement in this.m_list)
            {
                if (achievement.Info.ID == id)
                {
                    return achievement;
                }
            }
            return null;
        }

        public bool Finish(BaseAchievement baseAch)
        {
            bool flag;
            AchievementInfo info = baseAch.Info;
            AchievementData d = baseAch.Data;
            this.gamePlayer_0.BeginAllChanges();
            try
            {
                if (!baseAch.Finish(this.gamePlayer_0))
                {
                    return true;
                }
                List<AchievementGoodsInfo> achievementGoods = QuestMgr.GetAchievementGoods(info);
                foreach (AchievementGoodsInfo info2 in achievementGoods)
                {
                    if (info2.RewardType == 1)
                    {
                        this.gamePlayer_0.Rank.AddRank(info2.RewardPara);
                    }
                }
                if (info.AchievementPoint != 0)
                {
                    this.gamePlayer_0.AddAchievementPoint(info.AchievementPoint);
                }
                this.gamePlayer_0.Out.SendAchievementSuccess(d);
                if (achievementGoods.Count > 0)
                {
                    this.gamePlayer_0.Out.SendUserRanks(this.gamePlayer_0.Rank.GetRank());
                }
                this.method_3(d);
                this.gamePlayer_0.OnAchievementFinish(d);
                this.method_1(d.AchievementID);
                this.RemoveAchievement(baseAch);
                flag = true;
            }
            catch (Exception exception)
            {
                if (ilog_0.IsErrorEnabled)
                {
                    ilog_0.Error("Achivement Finish：" + exception);
                }
                flag = false;
            }
            finally
            {
                this.gamePlayer_0.CommitAllChanges();
            }
            return flag;
        }

        public List<AchievementProcessInfo> GetProcessAchievement()
        {
            Dictionary<int, AchievementProcessInfo> dictionary = this.dictionary_1;
            lock (dictionary)
            {
                return this.dictionary_1.Values.ToList<AchievementProcessInfo>();
            }
        }

        public Dictionary<int, AchievementProcessInfo> GetRealProcessAchievement()
        {
            Dictionary<int, AchievementProcessInfo> dictionary = this.dictionary_1;
            lock (dictionary)
            {
                return this.dictionary_1;
            }
        }

        public List<AchievementData> GetSuccessAchievement()
        {
            Dictionary<int, AchievementData> dictionary = this.dictionary_0;
            lock (dictionary)
            {
                return this.dictionary_0.Values.ToList<AchievementData>();
            }
        }

        public AchievementData GetSuccessAchievement(int achid)
        {
            Dictionary<int, AchievementData> dictionary = this.dictionary_0;
            lock (dictionary)
            {
                if (this.dictionary_0.ContainsKey(achid))
                {
                    return this.dictionary_0[achid];
                }
                return null;
            }
        }

        public void LoadFromDatabase(int playerId)
        {
            object obj2 = this.object_0;
            lock (obj2)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    this.method_6(this.gamePlayer_0.PlayerCharacter.AchievementProcess);
                    foreach (AchievementData data in bussiness.GetUserAchievement(playerId))
                    {
                        if (data.IsComplete)
                        {
                            this.dictionary_0.Add(data.AchievementID, data);
                        }
                        else
                        {
                            AchievementInfo singleAchievement = QuestMgr.GetSingleAchievement(data.AchievementID);
                            if (singleAchievement != null)
                            {
                                this.method_0(new BaseAchievement(singleAchievement, data, this.GetRealProcessAchievement()));
                            }
                        }
                    }
                    this.AddAchievementPre();
                }
            }
        }

        private bool method_0(BaseAchievement baseAchievement_0)
        {
            List<BaseAchievement> list = this.m_list;
            lock (list)
            {
                this.m_list.Add(baseAchievement_0);
            }
            baseAchievement_0.AddToPlayer(this.gamePlayer_0);
            if (baseAchievement_0.CanCompleted(this.gamePlayer_0))
            {
                this.Finish(baseAchievement_0);
            }
            return true;
        }

        private void method_1(int int_1)
        {
            foreach (AchievementInfo info in QuestMgr.GetAllAchievements())
            {
                if (info.PreAchievementID != "0,")
                {
                    char[] separator = new char[] { ',' };
                    foreach (string str in info.PreAchievementID.Split(separator))
                    {
                        if ((str != null) && (str != ""))
                        {
                            AchievementInfo singleAchievement = QuestMgr.GetSingleAchievement(int.Parse(str));
                            if ((singleAchievement != null) && (singleAchievement.ID == int_1))
                            {
                                goto Label_00C2;
                            }
                        }
                    }
                }
                continue;
            Label_00C2:
                this.AddAchievement(info);
            }
        }

        private bool method_2(int int_1)
        {
            Dictionary<int, AchievementData> dictionary = this.dictionary_0;
            lock (dictionary)
            {
                return this.dictionary_0.ContainsKey(int_1);
            }
        }

        private void method_3(AchievementData achievementData_0)
        {
            Dictionary<int, AchievementData> dictionary = this.dictionary_0;
            lock (dictionary)
            {
                if (!this.dictionary_0.ContainsKey(achievementData_0.AchievementID))
                {
                    this.dictionary_0.Add(achievementData_0.AchievementID, achievementData_0);
                }
            }
        }

        private void method_4()
        {
            Interlocked.Increment(ref this.int_0);
        }

        private void method_5()
        {
            int num = Interlocked.Decrement(ref this.int_0);
            if (num < 0)
            {
                if (ilog_0.IsErrorEnabled)
                {
                    ilog_0.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
                }
                Thread.VolatileWrite(ref this.int_0, 0);
            }
            if ((num <= 0) && (this.m_changedAchs.Count > 0))
            {
                this.UpdateChangedAchievements();
            }
        }

        private void method_6(string string_0)
        {
            if ((string_0 != null) && (string_0 != ""))
            {
                Dictionary<int, AchievementProcessInfo> dictionary = this.dictionary_1;
                lock (dictionary)
                {
                    char[] separator = new char[] { '|' };
                    foreach (string str in string_0.Split(separator))
                    {
                        if ((str != null) && (str != ""))
                        {
                            char[] chArray2 = new char[] { ',' };
                            string[] strArray = str.Split(chArray2);
                            if (strArray.Length >= 2)
                            {
                                int key = int.Parse(strArray[0]);
                                int num2 = int.Parse(strArray[1]);
                                if (!this.dictionary_1.ContainsKey(key))
                                {
                                    AchievementProcessInfo info = new AchievementProcessInfo(key, num2);
                                    this.dictionary_1.Add(key, info);
                                }
                            }
                        }
                    }
                }
            }
        }

        private string method_7()
        {
            List<string> list = new List<string>();
            Dictionary<int, AchievementProcessInfo> dictionary = this.dictionary_1;
            lock (dictionary)
            {
                if (this.dictionary_1.Count > 0)
                {
                    foreach (AchievementProcessInfo info in this.dictionary_1.Values)
                    {
                        list.Add(info.CondictionType + "," + info.Value);
                    }
                    return string.Join("|", list.ToArray());
                }
                return "";
            }
        }

        protected void OnAchievementsChanged(AchievementProcessInfo ach)
        {
            if (!this.m_changedAchs.Contains(ach))
            {
                this.m_changedAchs.Add(ach);
            }
            if ((this.int_0 <= 0) && (this.m_changedAchs.Count > 0))
            {
                this.UpdateChangedAchievements();
            }
        }

        public bool RemoveAchievement(BaseAchievement ach)
        {
            ach.RemoveFromPlayer(this.gamePlayer_0);
            return true;
        }

        public void SaveToDatabase()
        {
            object obj2 = this.object_0;
            lock (obj2)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    foreach (AchievementData data in this.dictionary_0.Values)
                    {
                        if (data.IsDirty)
                        {
                            bussiness.UpdateDbAchievementDataInfo(data);
                        }
                    }
                }
            }
        }

        public void Update(BaseAchievement ach)
        {
        }

        public void UpdateChangedAchievements()
        {
            this.gamePlayer_0.Out.SendUpdateAchievementInfo(this.m_changedAchs.ToList<AchievementProcessInfo>());
            this.m_changedAchs.Clear();
        }

        public void UpdateProcess(BaseCondition info)
        {
            if (info != null)
            {
                AchievementProcessInfo info2 = new AchievementProcessInfo(info.Info.CondictionType, info.Value);
                this.UpdateProcess(info2);
            }
        }

        public void UpdateProcess(AchievementProcessInfo info)
        {
            object obj2 = this.object_0;
            lock (obj2)
            {
                bool flag = true;
                this.method_4();
                if (!this.dictionary_1.ContainsKey(info.CondictionType))
                {
                    this.dictionary_1.Add(info.CondictionType, info);
                }
                else if (this.dictionary_1[info.CondictionType].Value < info.Value)
                {
                    this.dictionary_1[info.CondictionType] = info;
                }
                else
                {
                    flag = false;
                }
                this.method_5();
                if (flag)
                {
                    this.gamePlayer_0.PlayerCharacter.AchievementProcess = this.method_7();
                    this.OnAchievementsChanged(info);
                }
            }
        }
    }
}


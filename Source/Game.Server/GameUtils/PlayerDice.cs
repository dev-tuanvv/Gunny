namespace Game.Server.GameUtils
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PlayerDice
    {
        public int bigDicePrice = GameProperties.BigDicePrice;
        public int commonDicePrice = GameProperties.CommonDicePrice;
        public int doubleDicePrice = GameProperties.DoubleDicePrice;
        public int[] IntegralPoint = new int[] { 100, 300, 700, 0x5dc, 0xc1c };
        private DiceDataInfo m_diceData;
        private Dictionary<int, List<DiceLevelAwardInfo>> m_LevelAward;
        protected object m_lock = new object();
        protected GamePlayer m_player;
        private int m_result;
        private List<EventAwardInfo> m_rewardItem;
        private string m_rewardName;
        private bool m_saveToDb;
        public int MAX_LEVEL = 5;
        public int refreshPrice = GameProperties.DiceRefreshPrice;
        public int smallDicePrice = GameProperties.SmallDicePrice;

        public PlayerDice(GamePlayer player, bool saveTodb)
        {
            this.m_player = player;
            this.m_saveToDb = saveTodb;
            this.m_result = 0;
            this.m_rewardName = "";
        }

        public void ConvertAwardArray()
        {
            if (this.m_rewardItem.Count > 0)
            {
                string str = "";
                foreach (EventAwardInfo info in this.m_rewardItem)
                {
                    str = str + string.Format("{0},{1},{2},{3},{4}|", new object[] { info.TemplateID, info.StrengthenLevel, info.Count, info.ValidDate, info.IsBinds.ToString() });
                }
                str = str.Substring(0, str.Length - 1);
                this.m_diceData.AwardArray = str;
            }
        }

        public void CreateDiceAward()
        {
            lock (this.m_lock)
            {
                this.m_rewardItem = new List<EventAwardInfo>();
                Dictionary<int, EventAwardInfo> dictionary = new Dictionary<int, EventAwardInfo>();
                for (int i = 0; this.m_rewardItem.Count < 0x13; i++)
                {
                    List<EventAwardInfo> diceAward = EventAwardMgr.GetDiceAward(eEventType.DICE);
                    if (diceAward.Count > 0)
                    {
                        EventAwardInfo info = diceAward[0];
                        if (!dictionary.Keys.Contains<int>(info.TemplateID))
                        {
                            dictionary.Add(info.TemplateID, info);
                            this.m_rewardItem.Add(info);
                        }
                    }
                }
            }
            this.ConvertAwardArray();
        }

        public void GetLevelAward()
        {
            StringBuilder builder = new StringBuilder();
            IList<DiceLevelAwardInfo> list = this.m_LevelAward[this.m_diceData.LuckIntegralLevel];
            foreach (DiceLevelAwardInfo info in list)
            {
                ItemTemplateInfo goods = ItemMgr.FindItemTemplate(info.TemplateID);
                if (goods != null)
                {
                    SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, info.Count, 0x67);
                    this.Player.AddTemplate(cloneItem);
                    builder.Append(string.Concat(new object[] { cloneItem.Template.Name, "x", cloneItem.Count, "; " }));
                }
            }
            if (this.m_diceData.LuckIntegralLevel > 3)
            {
                this.Player.SendMessage("Bạn nhận được " + builder.ToString());
            }
        }

        public bool IsDiceOpen()
        {
            Convert.ToDateTime(GameProperties.DiceBeginTime);
            DateTime time = Convert.ToDateTime(GameProperties.DiceEndTime);
            return (DateTime.Now.Date < time.Date);
        }

        public void LoadFromDatabase()
        {
            if (this.IsDiceOpen())
            {
                if (this.m_diceData == null)
                {
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        this.m_diceData = bussiness.GetSingleDiceData(this.Player.PlayerCharacter.ID);
                        if (this.m_diceData == null)
                        {
                            this.SetupDiceData();
                        }
                    }
                }
                this.ReceiveLevelAward();
            }
        }

        public void ReceiveData()
        {
            if (string.IsNullOrEmpty(this.m_diceData.AwardArray))
            {
                this.CreateDiceAward();
            }
            else
            {
                this.m_rewardItem = new List<EventAwardInfo>();
                foreach (string str in this.m_diceData.AwardArray.Split(new char[] { '|' }))
                {
                    string[] strArray3 = str.Split(new char[] { ',' });
                    EventAwardInfo item = new EventAwardInfo {
                        TemplateID = int.Parse(strArray3[0]),
                        StrengthenLevel = int.Parse(strArray3[1]),
                        Count = int.Parse(strArray3[2]),
                        ValidDate = int.Parse(strArray3[3]),
                        IsBinds = bool.Parse(strArray3[4])
                    };
                    this.m_rewardItem.Add(item);
                }
                if (this.m_rewardItem.Count < 0x13)
                {
                    this.CreateDiceAward();
                }
            }
        }

        public void ReceiveLevelAward()
        {
            lock (this.m_lock)
            {
                this.m_LevelAward = new Dictionary<int, List<DiceLevelAwardInfo>>();
                for (int i = 0; i < this.MAX_LEVEL; i++)
                {
                    List<DiceLevelAwardInfo> allDiceLevelAwardAward = DiceLevelAwardMgr.GetAllDiceLevelAwardAward(i + 1);
                    if (!this.m_LevelAward.ContainsKey(i))
                    {
                        this.m_LevelAward.Add(i, allDiceLevelAwardAward);
                    }
                    else
                    {
                        this.m_LevelAward[i] = allDiceLevelAwardAward;
                    }
                }
            }
        }

        public void Reset()
        {
            if (this.IsDiceOpen())
            {
                lock (this.m_lock)
                {
                    this.m_diceData.LuckIntegral = 0;
                    this.m_diceData.CurrentPosition = -1;
                    this.m_diceData.LuckIntegralLevel = -1;
                    this.m_diceData.UserFirstCell = false;
                    this.m_diceData.FreeCount = 3;
                    this.m_diceData.Level = 0;
                }
            }
        }

        public virtual void SaveToDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    lock (this.m_lock)
                    {
                        if ((this.m_diceData != null) && this.m_diceData.IsDirty)
                        {
                            if (this.m_diceData.ID > 0)
                            {
                                bussiness.UpdateDiceData(this.m_diceData);
                            }
                            else
                            {
                                bussiness.AddDiceData(this.m_diceData);
                            }
                        }
                    }
                }
            }
        }

        public void SendDiceActiveOpen()
        {
            if (this.IsDiceOpen())
            {
                this.Player.Out.SendDiceActiveOpen(this);
            }
        }

        private void SetupDiceData()
        {
            lock (this.m_lock)
            {
                this.m_diceData = new DiceDataInfo();
                this.m_diceData.UserID = this.Player.PlayerCharacter.ID;
                this.m_diceData.LuckIntegral = 0;
                this.m_diceData.CurrentPosition = -1;
                this.m_diceData.LuckIntegralLevel = -1;
                this.m_diceData.UserFirstCell = false;
                this.m_diceData.FreeCount = 3;
                this.m_diceData.Level = 0;
                this.m_diceData.AwardArray = "";
            }
        }

        public DiceDataInfo Data
        {
            get
            {
                return this.m_diceData;
            }
            set
            {
                this.m_diceData = value;
            }
        }

        public Dictionary<int, List<DiceLevelAwardInfo>> LevelAward
        {
            get
            {
                return this.m_LevelAward;
            }
            set
            {
                this.m_LevelAward = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
        }

        public int result
        {
            get
            {
                return this.m_result;
            }
            set
            {
                this.m_result = value;
            }
        }

        public List<EventAwardInfo> RewardItem
        {
            get
            {
                return this.m_rewardItem;
            }
            set
            {
                this.m_rewardItem = value;
            }
        }

        public string RewardName
        {
            get
            {
                return this.m_rewardName;
            }
            set
            {
                this.m_rewardName = value;
            }
        }
    }
}


namespace Game.Server.GameUtils
{
    using Bussiness;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;

    public class PlayerBattle
    {
        public readonly int Agility = 0x640;
        public readonly int Attack = 0x6a4;
        public readonly int Blood = 0x61a8;
        public readonly int Damage = 0x3e8;
        public readonly int Defend = 0x5dc;
        public readonly int Energy = 0x125;
        public readonly int fairBattleDayPrestige = 0x7d0;
        public readonly int Guard = 500;
        public readonly int LevelLimit = 15;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public readonly int Lucky = 0x5dc;
        protected object m_lock = new object();
        private UserMatchInfo m_matchInfo;
        protected GamePlayer m_player;
        private bool m_saveToDb;
        public readonly int maxCount = 30;

        public PlayerBattle(GamePlayer player, bool saveTodb)
        {
            this.m_player = player;
            this.m_saveToDb = saveTodb;
        }

        public void AddPrestige(bool isWin)
        {
            FairBattleRewardInfo battleDataByPrestige = FairBattleRewardMgr.GetBattleDataByPrestige(this.m_matchInfo.totalPrestige);
            if (battleDataByPrestige == null)
            {
                this.Player.SendMessage(LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg5", new object[0]));
            }
            else
            {
                int prestigeForWin = battleDataByPrestige.PrestigeForWin;
                string translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg3", new object[] { prestigeForWin });
                if (!isWin)
                {
                    prestigeForWin = battleDataByPrestige.PrestigeForLose;
                    translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg4", new object[] { prestigeForWin });
                }
                if (this.m_matchInfo.addDayPrestge < this.fairBattleDayPrestige)
                {
                    this.m_matchInfo.addDayPrestge += prestigeForWin;
                    this.m_matchInfo.totalPrestige += prestigeForWin;
                }
                this.Player.SendMessage(translation);
            }
        }

        public void CreateInfo(int UserID)
        {
            this.m_matchInfo = new UserMatchInfo();
            this.m_matchInfo.ID = 0;
            this.m_matchInfo.UserID = UserID;
            this.m_matchInfo.dailyScore = 0;
            this.m_matchInfo.dailyWinCount = 0;
            this.m_matchInfo.dailyGameCount = 0;
            this.m_matchInfo.DailyLeagueFirst = true;
            this.m_matchInfo.DailyLeagueLastScore = 0;
            this.m_matchInfo.weeklyScore = 0;
            this.m_matchInfo.weeklyGameCount = 0;
            this.m_matchInfo.weeklyRanking = 0x3e8;
            this.m_matchInfo.addDayPrestge = 0;
            this.m_matchInfo.totalPrestige = 0;
            this.m_matchInfo.restCount = 30;
            this.m_matchInfo.maxCount = this.maxCount;
        }

        public int GetRank()
        {
            UserMatchInfo info = RankMgr.FindRank(this.Player.PlayerCharacter.ID);
            if (info != null)
            {
                return info.rank;
            }
            return 0;
        }

        public virtual void LoadFromDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    this.m_matchInfo = bussiness.GetSingleUserMatchInfo(this.Player.PlayerCharacter.ID);
                    if (this.m_matchInfo == null)
                    {
                        this.CreateInfo(this.Player.PlayerCharacter.ID);
                    }
                    this.m_matchInfo.maxCount = this.maxCount;
                }
            }
        }

        public void Reset()
        {
            this.m_matchInfo.dailyScore = 0;
            this.m_matchInfo.addDayPrestge = 0;
            this.m_matchInfo.restCount = 30;
        }

        public virtual void SaveToDatabase()
        {
            if (this.m_saveToDb)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    lock (this.m_lock)
                    {
                        if ((this.m_matchInfo != null) && this.m_matchInfo.IsDirty)
                        {
                            if (this.m_matchInfo.ID > 0)
                            {
                                bussiness.UpdateUserMatchInfo(this.m_matchInfo);
                            }
                            else
                            {
                                bussiness.AddUserMatchInfo(this.m_matchInfo);
                            }
                        }
                    }
                }
            }
        }

        public void Update()
        {
            if (this.m_matchInfo.restCount > 0)
            {
                this.m_matchInfo.restCount--;
                this.Player.Out.SendLeagueNotice(this.Player.PlayerCharacter.ID, this.MatchInfo.restCount, this.maxCount, 3);
            }
        }

        public UserMatchInfo MatchInfo
        {
            get
            {
                return this.m_matchInfo;
            }
            set
            {
                this.m_matchInfo = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
        }
    }
}


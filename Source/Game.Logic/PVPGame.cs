namespace Game.Logic
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Maps;
    using Game.Logic.Phy.Object;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Configuration;
    using System.Reflection;

    public class PVPGame : BaseGame
    {
        private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly int DDT_MONEY_ACTIVE = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_ACTIVE"]);
        private static readonly int DDT_MONEY_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_MAX_RATE_LOSE"]);
        private static readonly int DDT_MONEY_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_MAX_RATE_WIN"]);
        private static readonly int DDT_MONEY_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_MIN_RATE_LOSE"]);
        private static readonly int DDT_MONEY_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_MIN_RATE_WIN"]);
        private static readonly int EXP_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["EXP_MAX_RATE_LOSE"]);
        private static readonly int EXP_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["EXP_MAX_RATE_WIN"]);
        private static readonly int EXP_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["EXP_MIN_RATE_LOSE"]);
        private static readonly int EXP_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["EXP_MIN_RATE_WIN"]);
        private static readonly int GOXU_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["GOXU_MAX_RATE_LOSE"]);
        private static readonly int GOXU_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["GOXU_MAX_RATE_WIN"]);
        private static readonly int GOXU_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["GOXU_MIN_RATE_LOSE"]);
        private static readonly int GOXU_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["GOXU_MIN_RATE_WIN"]);
        private static readonly double GP_RATE = int.Parse(ConfigurationManager.AppSettings["GP_RATE"]);
        private static readonly int LeagueMoney_Lose = int.Parse(ConfigurationManager.AppSettings["LeagueMoney_Lose"]);
        private static readonly int LeagueMoney_Win = int.Parse(ConfigurationManager.AppSettings["LeagueMoney_Win"]);
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private float m_blueAvgLevel;
        private List<Player> m_blueTeam;
        private float m_redAvgLevel;
        private List<Player> m_redTeam;
        private static readonly int MONEY_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["MONEY_MAX_RATE_LOSE"]);
        private static readonly int MONEY_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["MONEY_MAX_RATE_WIN"]);
        private static readonly int MONEY_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["MONEY_MIN_RATE_LOSE"]);
        private static readonly int MONEY_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["MONEY_MIN_RATE_WIN"]);
        private static readonly double MONEY_RATE = int.Parse(ConfigurationManager.AppSettings["MONEY_RATE"]);
        private string teamAStr;
        private string teamBStr;
        private int BeginPlayerCount;
        private DateTime beginTime;

        public PVPGame(int id, int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, Map map, eRoomType roomType, eGameType gameType, int timeType) : base(id, roomId, map, roomType, gameType, timeType)
        {
            this.m_redTeam = new List<Player>();
            this.m_blueTeam = new List<Player>();
            StringBuilder builder = new StringBuilder();
            this.m_redAvgLevel = 0f;
            foreach (IGamePlayer player in red)
            {
                Player fp = new Player(player, base.PhysicalId++, this, 1, player.PlayerCharacter.hp);
                builder.Append(player.PlayerCharacter.ID).Append(",");
                fp.Reset();
                fp.Direction = (base.m_random.Next(0, 1) == 0) ? 1 : -1;
                base.AddPlayer(player, fp);
                this.m_redTeam.Add(fp);
                this.m_redAvgLevel += player.PlayerCharacter.Grade;
            }
            this.m_redAvgLevel /= (float) this.m_redTeam.Count;
            this.teamAStr = builder.ToString();
            StringBuilder builder2 = new StringBuilder();
            this.m_blueAvgLevel = 0f;
            foreach (IGamePlayer player3 in blue)
            {
                Player player4 = new Player(player3, base.PhysicalId++, this, 2, player3.PlayerCharacter.hp);
                builder2.Append(player3.PlayerCharacter.ID).Append(",");
                player4.Reset();
                player4.Direction = (base.m_random.Next(0, 1) == 0) ? 1 : -1;
                base.AddPlayer(player3, player4);
                this.m_blueTeam.Add(player4);
                this.m_blueAvgLevel += player3.PlayerCharacter.Grade;
            }
            this.m_blueAvgLevel /= (float) blue.Count;
            this.teamBStr = builder2.ToString();
            this.BeginPlayerCount = this.m_redTeam.Count + this.m_blueTeam.Count;
            this.beginTime = DateTime.Now;
        }

        private int CalculateExperience(Player player, int winTeam, ref int reward)
        {
            int num = 1;
            if ((base.m_roomType == eRoomType.Match) || (base.RoomType == eRoomType.BattleRoom))
            {
                float num2 = (player.Team == 1) ? this.m_blueAvgLevel : this.m_redAvgLevel;
                float num3 = (player.Team == 1) ? ((float) this.m_blueTeam.Count) : ((float) this.m_redTeam.Count);
                Math.Abs((float) (num2 - player.PlayerDetail.PlayerCharacter.Grade));
                if (player.TotalHurt == 0)
                {
                    if ((((num2 - this.m_blueAvgLevel) < 5.0) && ((num2 - this.m_redAvgLevel) < 5.0)) || (base.TotalHurt <= 0))
                    {
                        return 1;
                    }
                    base.SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward", new object[0]), null, 2);
                    reward = 200;
                    return 0xc9;
                }
                float num4 = (player.Team == winTeam) ? 2f : 0f;
                double num5 = GP_RATE / 10.0;
                player.TotalShootCount = (player.TotalShootCount == 0) ? 1 : player.TotalShootCount;
                if (player.TotalShootCount < player.TotalHitTargetCount)
                {
                    player.TotalShootCount = player.TotalHitTargetCount;
                }
                int num6 = (player.Team == 1) ? ((int) ((this.m_blueTeam.Count * this.m_blueAvgLevel) * 300.0)) : ((int) ((this.m_redAvgLevel * this.m_redTeam.Count) * 300.0));
                int num7 = (player.TotalHurt > num6) ? num6 : player.TotalHurt;
                int gp = (int) Math.Ceiling((double) (((((num4 + (num7 * (0.019 + num5))) + (player.TotalKill * 0.5)) + ((player.TotalHitTargetCount / player.TotalShootCount) * 2)) * num2) * (0.9 + ((num3 - 1.0) * 0.3))));
                if ((((num2 - this.m_blueAvgLevel) >= 5.0) || ((num2 - this.m_redAvgLevel) >= 5.0)) && (base.TotalHurt > 0))
                {
                    base.SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward", new object[0]), null, 2);
                    reward = 200;
                    gp += 200;
                }
                num = this.GainCoupleGP(player, gp);
                if (num > 0x186a0)
                {
                    num = 0x186a0;
                }
            }
            if (base.m_roomType == eRoomType.FightFootballTime)
            {
                int num9 = 0;
                foreach (int num10 in player.ScoreArr)
                {
                    num9 += num10;
                }
                num = num9 * 50;
            }
            if (num >= 1)
            {
                return num;
            }
            return 1;
        }

        private int CalculateGuildMatchResult(List<Player> players, int winTeam)
        {
            if (base.RoomType == eRoomType.Match)
            {
                StringBuilder builder = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
                StringBuilder builder2 = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
                IGamePlayer playerDetail = null;
                IGamePlayer player2 = null;
                int num = 0;
                foreach (Player player3 in players)
                {
                    if (player3.Team == winTeam)
                    {
                        builder.Append(string.Format("[{0}]", player3.PlayerDetail.PlayerCharacter.NickName));
                        playerDetail = player3.PlayerDetail;
                    }
                    else
                    {
                        builder2.Append(string.Format("{0}", player3.PlayerDetail.PlayerCharacter.NickName));
                        player2 = player3.PlayerDetail;
                        num++;
                    }
                }
                if (player2 != null)
                {
                    builder.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1", new object[0]) + player2.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2", new object[0]));
                    builder2.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3", new object[0]) + playerDetail.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4", new object[0]));
                    int riches = 0;
                    if (base.GameType == eGameType.Guild)
                    {
                        riches = num + (base.TotalHurt / 0x7d0);
                    }
                    playerDetail.ConsortiaFight(playerDetail.PlayerCharacter.ConsortiaID, player2.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, base.TotalHurt, players.Count);
                    if (playerDetail.ServerID != player2.ServerID)
                    {
                        player2.ConsortiaFight(playerDetail.PlayerCharacter.ConsortiaID, player2.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, base.TotalHurt, players.Count);
                    }
                    if (base.GameType == eGameType.Guild)
                    {
                        playerDetail.SendConsortiaFight(playerDetail.PlayerCharacter.ConsortiaID, riches, builder.ToString());
                    }
                    return riches;
                }
            }
            return 0;
        }

        public bool CanGameOver()
        {
            if ((base.RoomType == eRoomType.FightFootballTime) && (base.TurnIndex > 7))
            {
                return true;
            }
            bool flag = true;
            bool flag2 = true;
            foreach (Player player in this.m_redTeam)
            {
                if (player.IsLiving)
                {
                    flag = false;
                    break;
                }
            }
            foreach (Player player2 in this.m_blueTeam)
            {
                if (player2.IsLiving)
                {
                    flag2 = false;
                    break;
                }
            }
            return (flag || flag2);
        }

        public void CreateNpc()
        {
            int[] array = new int[] { 0x2718, 0x2715, 0x2716, 0x2719, 0x2717 };
            int[] numArray2 = new int[] { 350, 500, 680 };
            base.Shuffer<int>(numArray2);
            base.Shuffer<int>(array);
            base.ClearAllNpc();
            int x = numArray2[base.Random.Next(numArray2.Length)];
            for (int i = 0; i < array.Length; i++)
            {
                int npcId = array[i];
                this.CreateNpc(npcId, x, 0x103, 1, -1);
                x += 210;
            }
        }

        public void CreateNpc(int npcId, int x, int y, int type, int direction)
        {
            NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpc living = new SimpleNpc(base.PhysicalId++, this, npcInfoById, type, direction);
            living.Reset();
            living.SetXY(x, y);
            this.AddLiving(living);
            living.StartMoving();
        }

        public override void CheckState(int delay)
        {
            base.AddAction(new CheckPVPGameStateAction(delay));
        }

        public int GainCoupleGP(Player player, int gp)
        {
            foreach (Player player2 in this.GetSameTeamPlayer(player))
            {
                if (player2.PlayerDetail.PlayerCharacter.SpouseID == player.PlayerDetail.PlayerCharacter.SpouseID)
                {
                    return (int) (gp * 1.2);
                }
            }
            return gp;
        }

        public void GameOver()
        {
            if (base.GameState == eGameState.Playing)
            {
                base.m_gameState = eGameState.GameOver;
                base.ClearWaitTimer();
                base.CurrentTurnTotalDamage = 0;
                List<Player> allFightPlayers = base.GetAllFightPlayers();
                int winTeam = -1;
                foreach (Player player in allFightPlayers)
                {
                    if (player.IsLiving)
                    {
                        winTeam = player.Team;
                        break;
                    }
                }
                if ((winTeam == -1) && (this.CurrentPlayer != null))
                {
                    winTeam = this.CurrentPlayer.Team;
                }
                int num2 = this.CalculateGuildMatchResult(allFightPlayers, winTeam);
                if ((base.RoomType == eRoomType.Match) && (base.GameType == eGameType.Guild))
                {
                    int num3 = 10;
                    int num4 = -10;
                    num3 += allFightPlayers.Count / 2;
                    num4 += (int) Math.Round((double) ((allFightPlayers.Count / 2) * 0.5));
                }
                int num5 = 0;
                int num6 = 0;
                foreach (Player player2 in allFightPlayers)
                {
                    if (player2.TotalHurt > 0)
                    {
                        if (player2.Team == 1)
                        {
                            num6 = 1;
                        }
                        else
                        {
                            num5 = 1;
                        }
                    }
                }
                GSPacketIn pkg = new GSPacketIn(0x5b);
                pkg.WriteByte(100);
                pkg.WriteInt(base.PlayerCount);
                int index = 0;
                foreach (Player player3 in allFightPlayers)
                {
                    bool flag;
                    float num9 = (player3.Team == 1) ? this.m_blueAvgLevel : this.m_redAvgLevel;
                    if (player3.Team != 1)
                    {
                        int count = this.m_redTeam.Count;
                    }
                    else
                    {
                        int num11 = this.m_blueTeam.Count;
                    }
                    float num12 = Math.Abs((float) (num9 - player3.PlayerDetail.PlayerCharacter.Grade));
                    int team = player3.Team;
                    int gp = 0;
                    int reward = 0;
                    if (player3.TotalShootCount != 0)
                    {
                        int totalShootCount = player3.TotalShootCount;
                    }
                    if (((base.RoomType == eRoomType.BattleRoom) || (base.m_roomType == eRoomType.Match)) || (num12 < 5f))
                    {
                        gp = this.CalculateExperience(player3, winTeam, ref reward);
                    }
                    gp = (gp == 0) ? 1 : gp;
                    string str = ". ";
                    player3.CanTakeOut = (player3.Team == 1) ? num6 : num5;
                    num2 += player3.GainOffer;
                    player3.CanTakeOut = 1;
                    this.TakeCard(player3, index);
                    index++;
                    if (base.RoomType == eRoomType.FightFootballTime)
                    {
                        flag = ((player3.Team == 1) && (base.blueScore > base.redScore)) || ((player3.Team == 2) && (base.blueScore < base.redScore));
                    }
                    else
                    {
                        flag = player3.Team == winTeam;
                    }
                    if ((base.RoomType == eRoomType.Match) || (base.RoomType == eRoomType.BattleRoom))
                    {
                        bool flag4;
                        int num17 = base.Random.Next(MONEY_MIN_RATE_LOSE, MONEY_MAX_RATE_LOSE);
                        int num18 = base.Random.Next(DDT_MONEY_MIN_RATE_LOSE, DDT_MONEY_MAX_RATE_LOSE);
                        int num19 = LeagueMoney_Lose;
                        int num20 = base.Random.Next(EXP_MIN_RATE_LOSE, EXP_MAX_RATE_LOSE);
                        int num21 = base.Random.Next(GOXU_MIN_RATE_LOSE, GOXU_MAX_RATE_LOSE);
                        Random random = new Random();
                        int num22 = 1;
                        int num23 = 0;
                        int num24 = 0;
                        if (((1 < DateTime.Now.Hour) && (DateTime.Now.Hour < 4)) || ((10 < DateTime.Now.Hour) && (DateTime.Now.Hour < 13)) || ((19 < DateTime.Now.Hour) && (DateTime.Now.Hour < 22)))
                        {
                            num22 = 2;
                            player3.PlayerDetail.SendMessage("Hiện tại bạn đang thi đấu trong giờ v\x00e0ng n\x00ean được x2 điểm kinh nghiệm v\x00e0 xu!");
                        }
                        if (flag)
                        {
                            flag4 = true;
                            num19 = LeagueMoney_Win;
                            num17 = (random.Next(95, 100) * num22) + num23;
                            num18 = base.Random.Next(DDT_MONEY_MIN_RATE_WIN, DDT_MONEY_MAX_RATE_WIN);
                            num20 = (base.Random.Next(EXP_MIN_RATE_WIN, EXP_MAX_RATE_WIN) * num22) + num24;
                            num21 = base.Random.Next(GOXU_MIN_RATE_WIN, GOXU_MAX_RATE_WIN);
                        }
                        else
                        {
                            flag4 = true;
                            num19 = LeagueMoney_Win;
                            num17 = (random.Next(45, 50) * num22) + num23;
                            num18 = base.Random.Next(DDT_MONEY_MIN_RATE_WIN, DDT_MONEY_MAX_RATE_WIN);
                            num20 = (base.Random.Next(EXP_MIN_RATE_WIN, EXP_MAX_RATE_WIN) * num22) + num24;
                            num21 = base.Random.Next(GOXU_MIN_RATE_WIN, GOXU_MAX_RATE_WIN);
                        }
                        player3.PlayerDetail.AddMoney(num17);
                        player3.PlayerDetail.AddActiveMoney(num17);
                        string translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg1", new object[] { num17 });
                        if (DDT_MONEY_ACTIVE > 0)
                        {
                            player3.PlayerDetail.AddGiftToken(num17);
                            translation = translation + string.Format(", {0} Xu kh\x00f3a", num18);
                        }
                        gp += num20;
                        if (player3.PlayerDetail.CanX2Exp)
                        {
                            gp *= 2;
                            str = str + "Bạn nhận x2 exp từ Event thuyền r\x00f4ng.";
                        }
                        if (player3.PlayerDetail.CanX3Exp)
                        {
                            gp *= 3;
                            str = str + "Bạn nhận x3 exp từ Event thuyền r\x00f4ng.";
                        }
                        player3.PlayerDetail.SendHideMessage(translation + str);
                        int restCount = player3.PlayerDetail.MatchInfo.restCount;
                        int maxCount = player3.PlayerDetail.MatchInfo.maxCount;
                        bool flag2 = base.PlayerCount == 4;
                        bool flag3 = player3.PlayerDetail.PlayerCharacter.Grade >= 20;
                        if ((flag2 && flag3) && (restCount > 0))
                        {
                            translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg6", new object[] { num19 });
                            if (!flag)
                            {
                                translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg7", new object[] { num19 });
                            }
                            player3.PlayerDetail.SendMessage(translation);
                            player3.PlayerDetail.AddLeagueMoney(num19);
                            player3.PlayerDetail.UpdateRestCount();
                        }
                        if (num21 > 0)
                        {
                        }
                    }
                    if (base.RoomType == eRoomType.BattleRoom)
                    {
                        player3.PlayerDetail.AddPrestige(flag);
                    }
                    if (player3.FightBuffers.ConsortionAddPercentGoldOrGP > 0)
                    {
                        gp += (gp * player3.FightBuffers.ConsortionAddPercentGoldOrGP) / 100;
                    }
                    if (player3.FightBuffers.ConsortionAddOfferRate > 0)
                    {
                        num2 *= player3.FightBuffers.ConsortionAddOfferRate;
                    }
                    player3.PlayerDetail.SendMessage("Tổng số EXP nhận được l\x00e0 " + gp + "EXP!");
                    player3.PlayerDetail.FootballTakeOut(flag);
                    player3.GainGP = player3.PlayerDetail.AddGP(gp);
                    player3.GainOffer = player3.PlayerDetail.AddOffer(num2);
                    pkg.WriteInt(player3.Id);
                    pkg.WriteBoolean(flag);
                    pkg.WriteInt(player3.Grade);
                    pkg.WriteInt(player3.PlayerDetail.PlayerCharacter.GP);
                    pkg.WriteInt(0);
                    pkg.WriteInt(gp);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(player3.GainGP);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(player3.GainOffer);
                    pkg.WriteInt(player3.CanTakeOut);
                }
                pkg.WriteInt(num2);
                this.SendToAll(pkg);
                new StringBuilder();
                foreach (Player player4 in allFightPlayers)
                {
                    player4.PlayerDetail.OnGameOver(this, player4.Team == winTeam, player4.GainGP);
                }
                string str3 = "";
                base.OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, this.beginTime, DateTime.Now, this.BeginPlayerCount, base.Map.Info.ID, this.teamAStr, this.teamBStr, str3, winTeam, base.BossWarField);
                base.WaitTime(0x3a98);
                base.OnGameOverred();
            }
        }

        public Player[] GetSameTeamPlayer(Player player)
        {
            List<Player> list = new List<Player>();
            foreach (Player player2 in base.GetAllFightPlayers())
            {
                if ((player2 != player) && (player2.Team == player.Team))
                {
                    list.Add(player2);
                }
            }
            return list.ToArray();
        }

        public void LoadFightFootballResources()
        {
            int[] numArray = new int[] { 0x2718, 0x2715, 0x2716, 0x2719, 0x2717 };
            foreach (int num2 in numArray)
            {
                NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(num2);
                if (npcInfoById == null)
                {
                    log.Error("LoadResources npcInfo resoure is not exits");
                }
                else
                {
                    base.AddLoadingFile(2, npcInfoById.ResourcesPath, npcInfoById.ModelID);
                }
            }
            base.AddLoadingFile(1, "bombs/24.swf", "tank.resource.bombs.Bomb24");
        }

        public void NextTurn()
        {
            if (base.GameState == eGameState.Playing)
            {
                base.ClearWaitTimer();
                base.ClearDiedPhysicals();
                base.CheckBox();
                base.m_turnIndex++;
                List<Box> newBoxes = base.CreateBox();
                List<Physics> allPhysicalSafe = base.m_map.GetAllPhysicalSafe();
                foreach (Physics physics in allPhysicalSafe)
                {
                    physics.PrepareNewTurn();
                }
                base.LastTurnLiving = base.m_currentLiving;
                if (base.RoomType == eRoomType.FightFootballTime)
                {
                    this.CreateNpc();
                    base.m_currentLiving = base.FindNextTurnedFightFootball();
                }
                else
                {
                    base.m_currentLiving = base.FindNextTurnedLiving();
                }
                if (base.m_currentLiving.VaneOpen)
                {
                    base.UpdateWind(base.GetNextWind(), false);
                }
                this.MinusDelays(base.m_currentLiving.Delay);
                base.m_currentLiving.PrepareSelfTurn();
                if (!base.CurrentLiving.IsFrost && base.m_currentLiving.IsLiving)
                {
                    base.m_currentLiving.StartAttacking();
                    base.SendGameNextTurn(base.m_currentLiving, this, newBoxes);
                    if (base.m_currentLiving.IsAttacking)
                    {
                        base.AddAction(new WaitLivingAttackingAction(base.m_currentLiving, base.m_turnIndex, (base.getTurnTime() + 20) * 0x3e8));
                    }
                }
                base.OnBeginNewTurn();
            }
        }

        public void Prepare()
        {
            if (base.GameState == eGameState.Inited)
            {
                base.SendCreateGame();
                base.m_gameState = eGameState.Prepared;
                this.CheckState(0);
            }
        }

        public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
        {
            Player player = base.RemovePlayer(gp, IsKick);
            if (((player != null) && player.IsLiving) && (base.GameState != eGameState.Loading))
            {
                gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
                string msg = null;
                string translation = null;
                if (base.RoomType == eRoomType.Match)
                {
                    if (base.GameType == eGameType.Guild)
                    {
                        msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[] { gp.PlayerCharacter.Grade * 12, 15 });
                        gp.RemoveOffer(15);
                        translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[] { gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 15 });
                    }
                    else if (base.GameType == eGameType.Free)
                    {
                        msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[] { gp.PlayerCharacter.Grade * 12, 5 });
                        gp.RemoveOffer(5);
                        translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[] { gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 5 });
                    }
                }
                else
                {
                    msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", new object[] { gp.PlayerCharacter.Grade * 12 });
                    translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", new object[] { gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12 });
                }
                base.SendMessage(gp, msg, translation, 3);
                if (base.GetSameTeam())
                {
                    base.CurrentLiving.StopAttacking();
                    this.CheckState(0);
                }
            }
            return player;
        }

        public void StartGame()
        {
            if (base.GameState == eGameState.Loading)
            {
                base.m_gameState = eGameState.Playing;
                base.ClearWaitTimer();
                base.SendSyncLifeTime();
                List<Player> allFightPlayers = base.GetAllFightPlayers();
                MapPoint mapRandomPos = MapMgr.GetMapRandomPos(base.m_map.Info.ID);
                GSPacketIn pkg = new GSPacketIn(0x5b);
                pkg.WriteByte(0x63);
                pkg.WriteInt(allFightPlayers.Count);
                foreach (Player player in allFightPlayers)
                {
                    if (!player.IsLiving)
                    {
                        this.AddLiving(player);
                    }
                    player.Reset();
                    Point playerPoint = base.GetPlayerPoint(mapRandomPos, player.Team);
                    player.SetXY(playerPoint);
                    base.m_map.AddPhysical(player);
                    player.StartMoving();
                    player.StartGame();
                    pkg.WriteInt(player.Id);
                    pkg.WriteInt(player.X);
                    pkg.WriteInt(player.Y);
                    if (playerPoint.X < 600)
                    {
                        player.Direction = 1;
                    }
                    else
                    {
                        player.Direction = -1;
                    }
                    pkg.WriteInt(player.Direction);
                    pkg.WriteInt(player.Blood);
                    pkg.WriteInt(player.Team);
                    pkg.WriteInt(player.Weapon.RefineryLevel);
                    pkg.WriteInt(50);
                    pkg.WriteInt(player.Dander);
                    pkg.WriteInt(player.PlayerDetail.FightBuffs.Count);
                    foreach (BufferInfo info in player.PlayerDetail.FightBuffs)
                    {
                        pkg.WriteInt(info.Type);
                        pkg.WriteInt(info.Value);
                    }
                }
                this.SendToAll(pkg);
                base.VaneLoading();
                base.WaitTime(allFightPlayers.Count * 0x3e8);
                base.OnGameStarted();
            }
        }

        public void StartLoading()
        {
            if (base.GameState == eGameState.Prepared)
            {
                if (base.RoomType == eRoomType.FightFootballTime)
                {
                    this.LoadFightFootballResources();
                }
                base.ClearWaitTimer();
                base.SendStartLoading(60);
                base.VaneLoading();
                if (base.RoomType == eRoomType.Encounter)
                {
                    List<Player> allFightPlayers = base.GetAllFightPlayers();
                    foreach (Player player in allFightPlayers)
                    {
                        base.SendSelectObject(player.Id);
                    }
                }
                base.AddAction(new WaitPlayerLoadingAction(this, 0xee48));
                base.m_gameState = eGameState.Loading;
            }
        }

        public override void Stop()
        {
            if (base.GameState == eGameState.GameOver)
            {
                base.m_gameState = eGameState.Stopped;
                List<Player> allFightPlayers = base.GetAllFightPlayers();
                foreach (Player player in allFightPlayers)
                {
                    if (((player.IsActive && !player.FinishTakeCard) && (player.CanTakeOut > 0)) && (base.RoomType != eRoomType.FightFootballTime))
                    {
                        this.TakeCard(player);
                    }
                }
                lock (base.m_players)
                {
                    base.m_players.Clear();
                }
                base.Stop();
            }
        }

        public override bool TakeCard(Player player)
        {
            int index = 0;
            for (int i = 0; i < base.Cards.Length; i++)
            {
                if (base.Cards[i] == 0)
                {
                    index = i;
                    break;
                }
            }
            return this.TakeCard(player, index);
        }

        public override bool TakeCard(Player player, int index)
        {
            if (((((player.CanTakeOut == 0) || !player.IsActive) || ((index < 0) || (index > base.Cards.Length))) || player.FinishTakeCard) || (base.Cards[index] > 0))
            {
                return false;
            }
            player.CanTakeOut--;
            int gold = 0;
            int money = 0;
            int giftToken = 0;
            int medal = 0;
            int honor = 0;
            int hardCurrency = 0;
            int token = 0;
            int dragonToken = 0;
            int magicStonePoint = 0;
            int templateID = 0;
            int count = 0;
            List<SqlDataProvider.Data.ItemInfo> list = null;
            if (DropInventory.CardDrop(base.RoomType, ref list) && (list != null))
            {
                foreach (SqlDataProvider.Data.ItemInfo info in list)
                {
                    templateID = info.TemplateID;
                    count = info.Count;
                    ShopMgr.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken, ref magicStonePoint);
                    if (templateID > 0)
                    {
                        player.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count, eGameView.BatleTypeGet);
                    }
                }
            }
            player.FinishTakeCard = true;
            base.Cards[index] = 1;
            player.PlayerDetail.AddGold(gold);
            player.PlayerDetail.AddMoney(money);
            player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_TakeCard, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
            player.PlayerDetail.AddGiftToken(giftToken);
            base.SendGamePlayerTakeCard(player, index, templateID, count);
            return true;
        }

        public Player CurrentPlayer
        {
            get
            {
                return (base.m_currentLiving as Player);
            }
        }
    }
}


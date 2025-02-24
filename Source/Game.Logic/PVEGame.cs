using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Game;
using Game.Logic.AI.Mission;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Logic
{
    public class PVEGame : BaseGame
    {
        private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private APVEGameControl m_gameAI;
        private AMissionControl m_missionAI;
        public int SessionId;
        public bool IsWin;
        public bool IsKillWorldBoss;
        public bool CanEnterGate;
        public bool CanShowBigBox;
        public int TotalMissionCount;
        public int TotalCount;
        public int TotalTurn;
        public int Param1;
        public int Param2;
        public int Param3;
        public int Param4;
        public string Pic;
        public int TotalKillCount;
        public double TotalNpcExperience;
        public double TotalNpcGrade;
        private int BeginPlayersCount;
        private PveInfo m_info;
        private List<string> m_gameOverResources;
        public Dictionary<int, MissionInfo> Misssions;
        private MapPoint mapPos;
        public int WantTryAgain;
        public long WorldbossBood;
        public long AllWorldDameBoss;
        private eHardLevel m_hardLevel;
        private DateTime beginTime;
        private string m_IsBossType;
        private MissionInfo m_missionInfo;
        private List<int> m_mapHistoryIds;
        public int[] BossCards;
        private int m_bossCardCount;
        private int m_pveGameDelay;
        public MissionInfo MissionInfo
        {
            get
            {
                return this.m_missionInfo;
            }
            set
            {
                this.m_missionInfo = value;
            }
        }
        public Player CurrentPlayer
        {
            get
            {
                return this.m_currentLiving as Player;
            }
        }
        public TurnedLiving CurrentTurnLiving
        {
            get
            {
                return this.m_currentLiving;
            }
        }
        public List<int> MapHistoryIds
        {
            get
            {
                return this.m_mapHistoryIds;
            }
            set
            {
                this.m_mapHistoryIds = value;
            }
        }
        public eHardLevel HandLevel
        {
            get
            {
                return this.m_hardLevel;
            }
        }
        public MapPoint MapPos
        {
            get
            {
                return this.mapPos;
            }
        }
        public string IsBossWar
        {
            get
            {
                return this.m_IsBossType;
            }
            set
            {
                this.m_IsBossType = value;
            }
        }
        public List<string> GameOverResources
        {
            get
            {
                return this.m_gameOverResources;
            }
        }
        public int BossCardCount
        {
            get
            {
                return this.m_bossCardCount;
            }
            set
            {
                if (value > 0)
                {
                    this.BossCards = new int[9];
                    this.m_bossCardCount = value;
                }
            }
        }
        public int PveGameDelay
        {
            get
            {
                return this.m_pveGameDelay;
            }
            set
            {
                this.m_pveGameDelay = value;
            }
        }
        public PVEGame(int id, int roomId, PveInfo info, List<IGamePlayer> players, Map map, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int currentFloor) : base(id, roomId, map, roomType, gameType, timeType)
        {
            foreach (IGamePlayer player in players)
            {
                base.AddPlayer(player, new Player(player, this.PhysicalId++, this, 1, player.PlayerCharacter.hp)
                {
                    Direction = (this.m_random.Next(0, 1) == 0) ? 1 : -1
                });
                this.WorldbossBood = player.WorldbossBood;
                this.AllWorldDameBoss = player.AllWorldDameBoss;
            }
            this.m_info = info;
            this.BeginPlayersCount = players.Count;
            this.TotalKillCount = 0;
            this.TotalNpcGrade = 0.0;
            this.TotalNpcExperience = 0.0;
            this.TotalHurt = 0;
            this.m_IsBossType = "";
            this.WantTryAgain = 0;
            if (currentFloor > 0)
            {
                this.SessionId = currentFloor - 1;
            }
            else
            {
                this.SessionId = 0;
            }
            this.m_gameOverResources = new List<string>();
            this.Misssions = new Dictionary<int, MissionInfo>();
            this.m_mapHistoryIds = new List<int>();
            this.m_hardLevel = hardLevel;
            string script = this.GetScript(info, hardLevel);
            this.m_gameAI = (ScriptMgr.CreateInstance(script) as APVEGameControl);
            if (this.m_gameAI == null)
            {
                PVEGame.log.ErrorFormat("Can't create game ai :{0}", script);
                this.m_gameAI = SimplePVEGameControl.Simple;
            }
            this.m_gameAI.Game = this;
            this.m_gameAI.OnCreated();
            this.m_missionAI = SimpleMissionControl.Simple;
            this.beginTime = DateTime.Now;
            this.m_bossCardCount = 0;
        }
        private string GetScript(PveInfo pveInfo, eHardLevel hardLevel)
        {
            string script = string.Empty;
            switch (hardLevel)
            {
                case eHardLevel.Easy:
                    script = pveInfo.SimpleGameScript;
                    break;
                case eHardLevel.Normal:
                    script = pveInfo.NormalGameScript;
                    break;
                case eHardLevel.Hard:
                    script = pveInfo.HardGameScript;
                    break;
                case eHardLevel.Terror:
                    script = pveInfo.TerrorGameScript;
                    break;
                case eHardLevel.Epic:
                    script = pveInfo.EpicGameScript;
                    break;
                default:
                    script = pveInfo.SimpleGameScript;
                    break;
            }
            return script;
        }
        public string GetMissionIdStr(string missionIds, int randomCount)
        {
            if (string.IsNullOrEmpty(missionIds))
            {
                return "";
            }
            string[] ids = missionIds.Split(new char[]
            {
                ','
            });
            if (ids.Length < randomCount)
            {
                return "";
            }
            List<string> idList = new List<string>();
            int seed = ids.Length;
            int i = 0;
            while (i < randomCount)
            {
                int rand = base.Random.Next(seed);
                string id = ids[rand];
                if (!idList.Contains(id))
                {
                    idList.Add(id);
                    i++;
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (string s in idList)
            {
                sb.Append(s).Append(",");
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }
        public void SetupMissions(string missionIds)
        {
            if (string.IsNullOrEmpty(missionIds))
            {
                return;
            }
            int i = 0;
            string[] ids = missionIds.Split(new char[]
            {
                ','
            });
            string[] array = ids;
            for (int j = 0; j < array.Length; j++)
            {
                string id = array[j];
                i++;
                MissionInfo mi = MissionInfoMgr.GetMissionInfo(int.Parse(id));
                this.Misssions.Add(i, mi);
            }
        }
        public LivingConfig BaseLivingConfig()
        {
            return new LivingConfig
            {
                isBotom = 1,
                IsTurn = true,
                isShowBlood = true,
                isShowSmallMapPoint = true,
                ReduceBloodStart = 1,
                CanTakeDamage = true
            };
        }
        public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction)
        {
            return this.CreateNpc(npcId, x, y, type, direction, this.BaseLivingConfig());
        }
        public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, LivingConfig config)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpc npc = new SimpleNpc(this.PhysicalId++, this, npcInfo, type, direction);
            if (config != null)
            {
                npc.Config = config;
            }
            if (npc.Config.ReduceBloodStart > 1)
            {
                npc.Blood = npcInfo.Blood / npc.Config.ReduceBloodStart;
            }
            else
            {
                npc.Reset();
            }
            npc.SetXY(x, y);
            this.AddLiving(npc);
            npc.StartMoving();
            return npc;
        }
        public SimpleNpc CreateNpc(int npcId, int type, int direction)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpc npc = new SimpleNpc(this.PhysicalId++, this, npcInfo, type, direction);
            Point pos = base.GetPlayerPoint(this.mapPos, npcInfo.Camp);
            npc.Reset();
            npc.SetXY(pos);
            this.AddLiving(npc);
            npc.StartMoving();
            return npc;
        }
        public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string action)
        {
            return this.CreateBoss(npcId, x, y, direction, type, action, this.BaseLivingConfig());
        }
        public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleBoss boss = new SimpleBoss(PhysicalId++, this, npcInfo, direction, type, "");
            boss.Reset();
            boss.SetXY(x, y);
            AddLiving(boss);
            boss.StartMoving();

            return boss;
        }
        public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string action, LivingConfig config)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleBoss boss = new SimpleBoss(this.PhysicalId++, this, npcInfo, direction, type, action);
            if (config != null)
            {
                boss.Config = config;
            }
            if (boss.Config.ReduceBloodStart > 1)
            {
                boss.Blood = npcInfo.Blood / boss.Config.ReduceBloodStart;
            }
            else
            {
                boss.Reset();
                if (boss.Config.IsWorldBoss && this.WorldbossBood < 2147483647L)
                {
                    boss.Blood = (int)this.WorldbossBood;
                }
                if (boss.Config.isConsortiaBoss)
                {
                    boss.Blood -= (int)this.AllWorldDameBoss;
                }
            }
            boss.SetXY(x, y);
            this.AddLiving(boss);
            boss.StartMoving();
            return boss;
        }
        public Box CreateBox(int x, int y, string model, ItemInfo item)
        {
            Box box = new Box(this.PhysicalId++, model, item);
            box.SetXY(x, y);
            this.m_map.AddPhysical(box);
            base.AddBox(box, true);
            return box;
        }
        public Ball CreateBall(int x, int y, string action)
        {
            Ball ball = new Ball(this.PhysicalId++, action);
            ball.SetXY(x, y);
            this.m_map.AddPhysical(ball);
            base.AddBall(ball, true);
            return ball;
        }
        public void SendGameFocus(Physics p, int delay, int finishTime)
        {
            base.AddAction(new FocusAction(p, 1, delay, finishTime));
        }
        public void SendGameFocus(int x, int y, int type, int delay, int finishTime)
        {
            Createlayer(x, y, "pic", "", "", 1, 0);
            SendGameObjectFocus(1, "pic", delay, finishTime);
        }
        public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            PhysicalObj obj = new PhysicalObj(this.PhysicalId++, name, model, defaultAction, scale, rotation, 0);
            obj.SetXY(x, y);
            this.AddPhysicalObj(obj, true);
            return obj;
        }
        public bool isDragonLair()
        {
            int nextSessionId = 1 + this.SessionId;
            return this.Misssions.ContainsKey(nextSessionId) && this.m_info.ID == 5;
        }
        public Layer Createlayer(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            Layer obj = new Layer(this.PhysicalId++, name, model, defaultAction, scale, rotation);
            obj.SetXY(x, y);
            this.AddPhysicalObj(obj, true);
            return obj;
        }
        public Layer CreateTip(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            Layer obj = new Layer(this.PhysicalId++, name, model, defaultAction, scale, rotation);
            obj.SetXY(x, y);
            this.AddPhysicalTip(obj, true);
            return obj;
        }
        public void CreateGate(bool isEnter)
        {
            this.CanEnterGate = isEnter;
        }
        public void ClearMissionData()
        {
            foreach (Living living in this.m_livings)
            {
                living.Dispose();
            }
            this.m_livings.Clear();
            List<TurnedLiving> temp = new List<TurnedLiving>();
            foreach (TurnedLiving tl in base.TurnQueue)
            {
                if (tl is Player)
                {
                    if (tl.IsLiving)
                    {
                        temp.Add(tl);
                    }
                }
                else
                {
                    tl.Dispose();
                }
            }
            base.TurnQueue.Clear();
            foreach (TurnedLiving tl2 in temp)
            {
                base.TurnQueue.Add(tl2);
            }
            if (this.m_map != null)
            {
                foreach (PhysicalObj obj in this.m_map.GetAllPhysicalObjSafe())
                {
                    obj.Dispose();
                }
            }
        }
        public void AddAllPlayerToTurn()
        {
            foreach (Player player in base.Players.Values)
            {
                base.TurnQueue.Add(player);
            }
        }
        public override void AddLiving(Living living)
        {
            base.AddLiving(living);
            living.Died += new LivingEventHandle(this.living_Died);
        }
        private void living_Died(Living living)
        {
            if (base.CurrentLiving != null && base.CurrentLiving is Player && !(living is Player) && living != base.CurrentLiving)
            {
                this.TotalKillCount++;
                this.TotalNpcExperience += (double)living.Experience;
                this.TotalNpcGrade += (double)living.Grade;
            }
        }
        public override void MissionStart(IGamePlayer host)
        {
            if (base.GameState == eGameState.SessionPrepared || base.GameState == eGameState.GameOver)
            {
                foreach (Player p in base.Players.Values)
                {
                    p.Ready = true;
                }
                this.CheckState(0);
            }
        }
        public override bool CanAddPlayer()
        {
            Dictionary<int, Player> players;
            Monitor.Enter(players = this.m_players);
            bool result;
            try
            {
                result = (base.GameState == eGameState.SessionPrepared && this.m_players.Count < 4);
            }
            finally
            {
                Monitor.Exit(players);
            }
            return result;
        }
        public override Player AddPlayer(IGamePlayer gp)
        {
            if (this.CanAddPlayer())
            {
                Player fp = new Player(gp, this.PhysicalId++, this, 1, gp.PlayerCharacter.hp);
                fp.Direction = ((this.m_random.Next(0, 1) == 0) ? 1 : -1);
                base.AddPlayer(gp, fp);
                this.SendCreateGameToSingle(this, gp);
                this.SendPlayerInfoInGame(this, gp, fp);
                return fp;
            }
            return null;
        }
        public override Player RemovePlayer(IGamePlayer gp, bool isKick)
        {
            Player player = base.GetPlayer(gp);
            if (player != null)
            {
                player.PlayerDetail.RemoveGP(gp.PlayerCharacter.Grade * 12);
                player.PlayerDetail.ClearFightBuffOneMatch();
                string msg = null;
                if (player.IsLiving && base.GameState == eGameState.Playing)
                {
                    msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", new object[]
                    {
                        gp.PlayerCharacter.Grade * 12
                    });
                    string msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", new object[]
                    {
                        gp.PlayerCharacter.NickName,
                        gp.PlayerCharacter.Grade * 12
                    });
                    base.SendMessage(gp, msg, msg2, 3);
                }
                else
                {
                    string msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg1", new object[]
                    {
                        gp.PlayerCharacter.NickName
                    });
                    base.SendMessage(gp, msg, msg2, 3);
                }
                base.RemovePlayer(gp, isKick);
            }
            return player;
        }
        public void LoadResources(int[] npcIds)
        {
            if (npcIds == null || npcIds.Length == 0)
            {
                return;
            }
            for (int i = 0; i < npcIds.Length; i++)
            {
                int npcId = npcIds[i];
                NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
                if (npcInfo == null)
                {
                    PVEGame.log.Error("LoadResources npcInfo resoure is not exits");
                }
                else
                {
                    base.AddLoadingFile(2, npcInfo.ResourcesPath, npcInfo.ModelID);
                }
            }
        }
        public void LoadNpcGameOverResources(int[] npcIds)
        {
            if (npcIds == null || npcIds.Length == 0)
            {
                return;
            }
            for (int i = 0; i < npcIds.Length; i++)
            {
                int npcId = npcIds[i];
                NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
                if (npcInfo == null)
                {
                    PVEGame.log.Error("LoadGameOverResources npcInfo resoure is not exits");
                }
                else
                {
                    this.m_gameOverResources.Add(npcInfo.ModelID);
                }
            }
        }
        public void Prepare()
        {
            if (base.GameState == eGameState.Inited)
            {
                this.m_gameState = eGameState.Prepared;
                base.SendCreateGame();
                this.CheckState(0);
                try
                {
                    this.m_gameAI.OnPrepated();
                }
                catch (Exception ex)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
                }
            }
        }
        public void PrepareNewSession()
        {
            if (base.GameState == eGameState.Prepared || base.GameState == eGameState.GameOver || base.GameState == eGameState.ALLSessionStopped)
            {
                this.m_gameState = eGameState.SessionPrepared;
                this.SessionId++;
                base.ClearLoadingFiles();
                this.ClearMissionData();
                this.m_gameOverResources.Clear();
                this.WantTryAgain = 0;
                this.m_missionInfo = this.Misssions[this.SessionId];
                this.m_pveGameDelay = this.m_missionInfo.Delay;
                this.TotalCount = this.m_missionInfo.TotalCount;
                this.TotalTurn = this.m_missionInfo.TotalTurn;
                this.Param1 = this.m_missionInfo.Param1;
                this.Param2 = this.m_missionInfo.Param2;
                this.Param3 = -1;
                this.Param4 = -1;
                this.Pic = string.Format("show{0}.jpg", this.SessionId);
                this.m_missionAI = (ScriptMgr.CreateInstance(this.m_missionInfo.Script) as AMissionControl);
                if (this.m_missionAI == null)
                {
                    PVEGame.log.ErrorFormat("Can't create game mission ai :{0}", this.m_missionInfo.Script);
                    this.m_missionAI = SimpleMissionControl.Simple;
                }
                if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.AcademyDungeon)
                {
                    List<Player> list = base.GetAllFightPlayers();
                    foreach (Player p in list)
                    {
                        p.PlayerDetail.UpdateBarrier(this.SessionId, this.Pic);
                    }
                }
                this.m_missionAI.Game = this;
                try
                {
                    this.m_missionAI.OnPrepareNewSession();
                }
                catch (Exception ex)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
                }
            }
        }
        public bool CanStartNewSession()
        {
            return base.m_turnIndex == 0 || this.IsAllReady();
        }
        public bool IsAllReady()
        {
            foreach (Player p in base.Players.Values)
            {
                if (!p.Ready)
                {
                    return false;
                }
            }
            return true;
        }
        public void StartLoading()
        {
            if (base.GameState == eGameState.SessionPrepared)
            {
                this.m_gameState = eGameState.Loading;
                base.m_turnIndex = 0;
                this.SendMissionInfo();
                base.SendStartLoading(60);
                base.VaneLoading();
                base.AddAction(new WaitPlayerLoadingAction(this, 61000));
            }
        }
        public void StartGameMovie()
        {
            if (base.GameState == eGameState.Loading)
            {
                try
                {
                    this.m_missionAI.OnStartMovie();
                }
                catch (Exception ex)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
                }
            }
        }
        public void StartGame()
        {
            if (base.GameState == eGameState.Loading)
            {
                this.m_gameState = eGameState.GameStart;
                base.SendSyncLifeTime();
                this.TotalKillCount = 0;
                this.TotalNpcGrade = 0.0;
                this.TotalNpcExperience = 0.0;
                this.TotalHurt = 0;
                this.m_bossCardCount = 0;
                this.BossCards = null;
                List<Player> list = base.GetAllFightPlayers();
                this.mapPos = MapMgr.GetPVEMapRandomPos(this.m_map.Info.ID);
                GSPacketIn pkg = new GSPacketIn(91);
                pkg.WriteByte(99);
                pkg.WriteInt(list.Count);
                foreach (Player p in list)
                {
                    if (!p.IsLiving)
                    {
                        this.AddLiving(p);
                    }
                    p.Reset();
                    Point pos = base.GetPlayerPoint(this.mapPos, p.Team);
                    p.SetXY(pos);
                    this.m_map.AddPhysical(p);
                    p.StartMoving();
                    p.StartGame();
                    pkg.WriteInt(p.Id);
                    pkg.WriteInt(p.X);
                    pkg.WriteInt(p.Y);
                    if (pos.X < 600)
                    {
                        p.Direction = 1;
                    }
                    else
                    {
                        p.Direction = -1;
                    }
                    pkg.WriteInt(p.Direction);
                    pkg.WriteInt(p.Blood);
                    //pkg.WriteInt(p.MaxBlood);
                    pkg.WriteInt(p.Team);
                    pkg.WriteInt(p.Weapon.RefineryLevel);
                    //pkg.WriteInt(p.deputyWeaponCount);
                    pkg.WriteInt(50);
                    pkg.WriteInt(p.Dander);
                    //pkg.WriteInt(0);
                    //pkg.WriteInt(0);
                    pkg.WriteInt(p.PlayerDetail.FightBuffs.Count);
                    foreach (BufferInfo info in p.PlayerDetail.FightBuffs)
                    {
                        pkg.WriteInt(info.Type);
                        pkg.WriteInt(info.Value);
                    }
                    //pkg.WriteInt(0);
                    //pkg.WriteBoolean(p.IsFrost);
                    //pkg.WriteBoolean(p.IsHide);
                    //pkg.WriteBoolean(p.IsNoHole);
                    //pkg.WriteBoolean(false);
                    //pkg.WriteInt(0);
                }
                //pkg.WriteInt(0);
                //pkg.WriteInt(0);
                //pkg.WriteDateTime(DateTime.Now);
                this.SendToAll(pkg);
                this.SendUpdateUiData();
                base.WaitTime(base.PlayerCount * 2500 + 1000);
                base.OnGameStarted();
            }
        }
        public void PrepareNewGame()
        {
            if (base.GameState == eGameState.GameStart)
            {
                this.m_gameState = eGameState.Playing;
                this.BossCardCount = 0;
                base.SendSyncLifeTime();
                base.WaitTime(base.PlayerCount * 1000);
                try
                {
                    this.m_missionAI.OnStartGame();
                }
                catch (Exception ex)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
                }
            }
        }
        public void NextTurn()
        {
            if (base.GameState == eGameState.Playing)
            {
                base.ClearWaitTimer();
                base.ClearDiedPhysicals();
                base.CheckBox();
                this.LivingRandSay();
                List<Physics> list = this.m_map.GetAllPhysicalSafe();
                foreach (Physics p in list)
                {
                    p.PrepareNewTurn();
                }
                List<Box> newBoxes = base.CreateBox();
                try
                {
                    this.m_missionAI.OnNewTurnStarted();
                }
                catch (Exception ex)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
                }
                this.LastTurnLiving = this.m_currentLiving;
                this.m_currentLiving = base.FindNextTurnedLiving();
                if (this.m_currentLiving != null)
                {
                    base.m_turnIndex++;
                    this.SendUpdateUiData();
                    List<Living> livedLivings = base.GetLivedLivingsHadTurn();
                    if (livedLivings.Count > 0 && this.m_currentLiving.Delay >= this.m_pveGameDelay)
                    {
                        this.MinusDelays(this.m_pveGameDelay);
                        foreach (Living living in this.m_livings)
                        {
                            living.PrepareSelfTurn();
                            if (!living.IsFrost)
                            {
                                living.StartAttacking();
                            }
                        }
                        base.SendGameNextTurn(livedLivings[0], this, newBoxes);
                        foreach (Living living2 in this.m_livings)
                        {
                            if (living2.IsAttacking)
                            {
                                living2.StopAttacking();
                            }
                        }
                        this.m_pveGameDelay += this.MissionInfo.IncrementDelay;
                        this.CheckState(0);
                    }
                    else
                    {
                        if (base.RoomType == eRoomType.ActivityDungeon && this.m_currentLiving is Player)
                        {
                            TurnedLiving[] lists = base.GetNextAllTurnedLiving();
                            base.UpdateWind(base.GetNextWind(), false);
                            this.CurrentTurnTotalDamage = 0;
                            TurnedLiving[] array = lists;
                            for (int i = 0; i < array.Length; i++)
                            {
                                TurnedLiving living3 = array[i];
                                this.MinusDelays(living3.Delay);
                                living3.PrepareSelfTurn();
                                living3.StartAttacking();
                                base.SendSyncLifeTime();
                                base.SendSigleNextTurn(living3, this, newBoxes);
                                base.SendFightStatus(living3, 1);
                                base.AddAction(new WaitLivingAttackingAction(living3, base.m_turnIndex, (this.m_timeType + 20) * 1000));
                            }
                        }
                        else
                        {
                            if (this.CanShowBigBox)
                            {
                                this.ShowBigBox();
                                this.CanEnterGate = true;
                            }
                            this.MinusDelays(this.m_currentLiving.Delay);
                            if (this.m_currentLiving is Player)
                            {
                                base.UpdateWind(base.GetNextWind(), false);
                            }
                            this.CurrentTurnTotalDamage = 0;
                            this.m_currentLiving.PrepareSelfTurn();
                            if (this.m_currentLiving.IsLiving && !this.m_currentLiving.IsFrost)
                            {
                                if (!m_currentLiving.BlockTurn)
                                {
                                    this.m_currentLiving.StartAttacking();
                                    base.SendSyncLifeTime();
                                    base.SendGameNextTurn(this.m_currentLiving, this, newBoxes);
                                    if (this.m_currentLiving.IsAttacking)
                                    {
                                        base.AddAction(new WaitLivingAttackingAction(this.m_currentLiving, base.m_turnIndex, (base.getTurnTime() + 20) * 1000));
                                    }
                                    Console.WriteLine(">?????????????????BlockTurn: " + m_currentLiving.BlockTurn);
                                }
                            }
                        }
                    }
                }
                base.OnBeginNewTurn();
                try
                {
                    this.m_missionAI.OnBeginNewTurn();
                }
                catch (Exception ex2)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex2);
                }
            }
        }
        public void LivingRandSay()
        {
            if (this.m_livings == null || this.m_livings.Count == 0)
            {
                return;
            }
            int livCount = this.m_livings.Count;
            foreach (Living living in this.m_livings)
            {
                living.IsSay = false;
            }
            if (base.TurnIndex % 2 == 0)
            {
                return;
            }
            int sayCount;
            if (livCount <= 5)
            {
                sayCount = base.Random.Next(0, 2);
            }
            else
            {
                if (livCount > 5 && livCount <= 10)
                {
                    sayCount = base.Random.Next(1, 3);
                }
                else
                {
                    sayCount = base.Random.Next(1, 4);
                }
            }
            if (sayCount > 0)
            {
                int i = 0;
                while (i < sayCount)
                {
                    int index = base.Random.Next(0, livCount);
                    if (!this.m_livings[index].IsSay)
                    {
                        this.m_livings[index].IsSay = true;
                        i++;
                    }
                }
            }
        }
        public override bool TakeCard(Player player)
        {
            int index = 0;
            for (int i = 0; i < this.Cards.Length; i++)
            {
                if (this.Cards[i] == 0)
                {
                    index = i;
                    break;
                }
            }
            return this.TakeCard(player, index);
        }
        public override bool TakeCard(Player player, int index)
        {
            if (player.CanTakeOut == 0)
            {
                return false;
            }
            if (!player.IsActive || index < 0 || index > this.Cards.Length || player.FinishTakeCard || this.Cards[index] > 0)
            {
                return false;
            }
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
            List<ItemInfo> infos = null;
            int copyId = this.m_missionInfo.Id;
            int value = 30;
            if (this.isDragonLair() && this.SessionId < 3)
            {
                value = 10;
            }
            if (player.PlayerDetail.MissionEnergyEmpty(value))
            {
                copyId = 0;
            }
            if (DropInventory.CopyDrop(copyId, 1, ref infos))
            {
                if (infos != null)
                {
                    foreach (ItemInfo info in infos)
                    {
                        ShopMgr.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken, ref magicStonePoint);
                        if (info != null && info.TemplateID > 0)
                        {
                            templateID = info.TemplateID;
                            count = info.Count;
                            player.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count, eGameView.dungeonTypeGet);
                        }
                    }
                }
                player.PlayerDetail.AddGold(gold);
                player.PlayerDetail.AddMoney(money);
                player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_TakeCard, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
                player.PlayerDetail.AddGiftToken(giftToken);
                if (templateID == 0 && gold > 0)
                {
                    templateID = -100;
                    count = gold;
                }
            }
            if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.SpecialActivityDungeon || base.RoomType == eRoomType.AcademyDungeon)
            {
                player.CanTakeOut--;
                if (player.CanTakeOut == 0)
                {
                    player.FinishTakeCard = true;
                }
            }
            else
            {
                player.FinishTakeCard = true;
            }
            this.Cards[index] = 1;
            base.SendGamePlayerTakeCard(player, index, templateID, count);
            return true;
        }
        public bool CanGameOver()
        {
            if (base.PlayerCount == 0)
            {
                return true;
            }
            if (base.GetDiedPlayerCount() == base.PlayerCount)
            {
                this.IsWin = false;
                return true;
            }
            try
            {
                return this.m_missionAI.CanGameOver();
            }
            catch (Exception ex)
            {
                PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
            }
            return true;
        }
        public void TakeSnow()
        {
            ItemTemplateInfo info = ItemMgr.FindItemTemplate(201144);
            if (info == null)
            {
                return;
            }
            ItemInfo item = ItemInfo.CreateFromTemplate(info, 1, 101);
            item.IsBinds = true;
            int count = base.Random.Next(3, 9);
            string msg = "";
            foreach (Player player in base.GetAllFightPlayers())
            {
                player.PlayerDetail.AddTemplate(item, item.Template.BagType, count, eGameView.dungeonTypeGet);
                msg += string.Format("Bạn nhận được {0} x{1}, trong lần tấn công này.", item.Template.Name, count);
                player.PlayerDetail.SendMessage(msg);
            }
        }
        public void TakeConsortiaBossAward(bool isWin)
        {
            List<Player> players = base.GetAllFightPlayers();
            foreach (Player p in players)
            {
                p.PlayerDetail.UpdatePveResult("consortiaboss", p.TotalDameLiving, isWin);
            }
        }
        public void ShowBigBox()
        {
            List<ItemInfo> infos = null;
            DropInventory.CopyDrop(this.m_missionInfo.Id, this.SessionId, ref infos);
            List<int> box = new List<int>();
            if (infos != null)
            {
                foreach (ItemInfo item in infos)
                {
                    box.Add(item.TemplateID);
                }
            }
            foreach (Player player in base.GetAllFightPlayers())
            {
                if (this.CanGetLabyrinthAward(player.PlayerDetail.ProcessLabyrinthAward))
                {
                    base.SendGameBigBox(player, box);
                    player.PlayerDetail.UpdateLabyrinth(this.SessionId, this.m_missionInfo.Id, true);
                }
            }
        }
        private bool CanGetLabyrinthAward(string param2)
        {
            bool _loc_3 = false;
            if (param2.Length > 0)
            {
                string[] _loc_4 = param2.Split(new char[]
                {
                    '-'
                });
                string[] array = _loc_4;
                for (int i = 0; i < array.Length; i++)
                {
                    string val = array[i];
                    string _loc_5 = val;
                    if (_loc_5 == this.SessionId.ToString())
                    {
                        _loc_3 = true;
                        break;
                    }
                }
            }
            return _loc_3;
        }
        public void GameOverMovie()
        {
            if (base.GameState == eGameState.Playing)
            {
                this.m_gameState = eGameState.GameOver;
                base.ClearWaitTimer();
                base.ClearDiedPhysicals();
                List<Player> players = base.GetAllFightPlayers();
                foreach (Player player in players)
                {
                    if (this.CanGetLabyrinthAward(player.PlayerDetail.ProcessLabyrinthAward))
                    {
                        player.PlayerDetail.UpdateLabyrinth(this.SessionId, this.m_missionInfo.Id, false);
                    }
                }
                try
                {
                    this.m_missionAI.OnGameOverMovie();
                }
                catch (Exception ex)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
                }
                bool hasNextSess = this.HasNextSession();
                if (!hasNextSess)
                {
                    GSPacketIn pkg = new GSPacketIn(91);
                    pkg.WriteByte(112);
                    pkg.WriteInt(0);
                    pkg.WriteBoolean(hasNextSess);
                    pkg.WriteBoolean(false);
                    pkg.WriteInt(base.PlayerCount);
                    foreach (Player p in players)
                    {
                        p.PlayerDetail.ClearFightBuffOneMatch();
                        if (this.IsLabyrinth())
                        {
                            p.PlayerDetail.OutLabyrinth(this.IsWin);
                        }
                        int experience = this.CalculateExperience(p);
                        int score = this.CalculateScore(p);
                        this.m_missionAI.CalculateScoreGrade(p.TotalAllScore);
                        if (p.CurrentIsHitTarget)
                        {
                            p.TotalHitTargetCount++;
                        }
                        this.CalculateHitRate(p.TotalHitTargetCount, p.TotalShootCount);
                        p.TotalAllHurt += p.TotalHurt;
                        p.TotalAllCure += p.TotalCure;
                        p.TotalAllHitTargetCount += p.TotalHitTargetCount;
                        p.TotalAllShootCount += p.TotalShootCount;
                        p.GainGP = p.PlayerDetail.AddGP(experience);
                        p.TotalAllExperience += p.GainGP;
                        p.TotalAllScore += score;
                        p.BossCardCount = this.BossCardCount;
                        pkg.WriteInt(p.PlayerDetail.PlayerCharacter.ID);
                        pkg.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
                        pkg.WriteInt(0);
                        pkg.WriteInt(p.GainGP);
                        pkg.WriteBoolean(this.IsWin);
                        pkg.WriteInt(p.BossCardCount);
                        pkg.WriteBoolean(false);
                        pkg.WriteBoolean(false);
                    }
                    if (this.BossCardCount > 0)
                    {
                        pkg.WriteInt(this.m_gameOverResources.Count);
                        foreach (string res in this.m_gameOverResources)
                        {
                            pkg.WriteString(res);
                        }
                    }
                    this.SendToAll(pkg);
                    base.OnGameStopped();
                    base.OnGameOverred();
                    return;
                }
                List<Physics> list = this.m_map.GetAllPhysicalSafe();
                foreach (Physics p2 in list)
                {
                    p2.PrepareNewTurn();
                }
                this.m_currentLiving = base.FindNextTurnedLiving();
                if (this.m_currentLiving != null && this.CanEnterGate)
                {
                    base.m_turnIndex++;
                    this.m_currentLiving.PrepareSelfTurn();
                    List<Box> newBoxes = new List<Box>();
                    base.SendGameNextTurn(this.m_currentLiving, this, newBoxes);
                    this.CanEnterGate = false;
                    this.CanShowBigBox = false;
                    this.EnterNextFloor();
                }
                base.OnBeginNewTurn();
                try
                {
                    this.m_missionAI.OnBeginNewTurn();
                }
                catch (Exception ex2)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex2);
                }
            }
        }
        public void EnterNextFloor()
        {
            int outMap = base.Map.Info.ForegroundWidth;
            Player target = base.FindRandomPlayer();
            int plus = 150;
            int disX = target.X;
            int disY = target.Y;
            if (disX + plus > outMap)
            {
                disX -= plus;
            }
            else
            {
                disX += plus;
            }
            Point p = this.m_map.FindYLineNotEmptyPoint(disX, disY);
            if (p == Point.Empty)
            {
                p = new Point(disX, base.Map.Bound.Height + 1);
            }
            this.CreatePhysicalObj(p.X, p.Y - 75, "transmitted", "asset.game.transmitted", "out", 1, 1);
        }
        public bool IsLabyrinth()
        {
            return base.RoomType == eRoomType.Lanbyrinth;
        }
        public void GameOver()
        {
            if (base.GameState == eGameState.Playing)
            {
                this.m_gameState = eGameState.GameOver;
                this.SendUpdateUiData();
                try
                {
                    this.m_missionAI.OnGameOver();
                }
                catch (Exception ex)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
                }
                List<Player> players = base.GetAllFightPlayers();
                this.CurrentTurnTotalDamage = 0;
                this.m_bossCardCount = 1;
                bool hasNextSess = this.HasNextSession();
                if (!this.IsWin || !hasNextSess)
                {
                    this.m_bossCardCount = 0;
                }
                if (this.IsWin && !hasNextSess && !base.isTrainer())
                {
                    this.m_bossCardCount = 2;
                }
                GSPacketIn pkg = new GSPacketIn(91);
                pkg.WriteByte(112);
                pkg.WriteInt(this.BossCardCount);
                if (hasNextSess)
                {
                    pkg.WriteBoolean(true);
                    pkg.WriteString(string.Format("show{0}.jpg", 1 + this.SessionId));
                    pkg.WriteBoolean(true);
                }
                else
                {
                    pkg.WriteBoolean(false);
                    pkg.WriteBoolean(false);
                }
                pkg.WriteInt(base.PlayerCount);
                foreach (Player p in players)
                {
                    p.PlayerDetail.ClearFightBuffOneMatch();
                    if (!this.IsWin && this.IsLabyrinth())
                    {
                        p.PlayerDetail.OutLabyrinth(this.IsWin);
                    }
                    int experience = this.CalculateExperience(p);
                    if (p.FightBuffers.ConsortionAddPercentGoldOrGP > 0)
                    {
                        experience += experience * p.FightBuffers.ConsortionAddPercentGoldOrGP / 100;
                    }
                    int score = this.CalculateScore(p);
                    this.m_missionAI.CalculateScoreGrade(p.TotalAllScore);
                    p.CanTakeOut = this.BossCardCount;
                    if (p.CurrentIsHitTarget)
                    {
                        p.TotalHitTargetCount++;
                    }
                    this.CalculateHitRate(p.TotalHitTargetCount, p.TotalShootCount);
                    p.TotalAllHurt += p.TotalHurt;
                    p.TotalAllCure += p.TotalCure;
                    p.TotalAllHitTargetCount += p.TotalHitTargetCount;
                    p.TotalAllShootCount += p.TotalShootCount;
                    p.GainGP = p.PlayerDetail.AddGP(experience);
                    p.TotalAllExperience += p.GainGP;
                    p.TotalAllScore += score;
                    p.BossCardCount = this.m_bossCardCount;
                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.ID);
                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
                    pkg.WriteInt(0);
                    pkg.WriteInt(p.GainGP);
                    pkg.WriteBoolean(this.IsWin);
                    pkg.WriteInt(p.BossCardCount);
                    pkg.WriteBoolean(false);
                    pkg.WriteBoolean(false);
                }
                if (this.BossCardCount > 0)
                {
                    pkg.WriteInt(this.m_gameOverResources.Count);
                    foreach (string res in this.m_gameOverResources)
                    {
                        pkg.WriteString(res);
                    }
                }
                this.SendToAll(pkg);
                StringBuilder sb = new StringBuilder();
                foreach (Player p2 in players)
                {
                    sb.Append(p2.PlayerDetail.PlayerCharacter.ID).Append(",");
                    p2.Ready = false;
                    p2.PlayerDetail.OnMissionOver(p2.Game, this.IsWin, this.MissionInfo.Id, p2.TurnNum);
                }
                int winTeam = this.IsWin ? 1 : 2;
                string teamAStr = sb.ToString();
                string teamBStr = "";
                string dropTemplateIdsStr = "";
                if (!this.IsWin)
                {
                    base.OnGameStopped();
                }
                StringBuilder BossWarRecord = new StringBuilder();
                if (this.IsWin && this.IsBossWar != "")
                {
                    BossWarRecord.Append(this.IsBossWar).Append(",");
                    foreach (Player p3 in players)
                    {
                        BossWarRecord.Append("PlayerCharacter ID: ").Append(p3.PlayerDetail.PlayerCharacter.ID).Append(",");
                        BossWarRecord.Append("Grade: ").Append(p3.PlayerDetail.PlayerCharacter.Grade).Append(",");
                        BossWarRecord.Append("TurnNum): ").Append(p3.TurnNum).Append(",");
                        BossWarRecord.Append("Attack: ").Append(p3.PlayerDetail.PlayerCharacter.Attack).Append(",");
                        BossWarRecord.Append("Defence: ").Append(p3.PlayerDetail.PlayerCharacter.Defence).Append(",");
                        BossWarRecord.Append("Agility: ").Append(p3.PlayerDetail.PlayerCharacter.Agility).Append(",");
                        BossWarRecord.Append("Luck: ").Append(p3.PlayerDetail.PlayerCharacter.Luck).Append(",");
                        BossWarRecord.Append("BaseAttack: ").Append(p3.PlayerDetail.GetBaseAttack()).Append(",");
                        BossWarRecord.Append("MaxBlood: ").Append(p3.MaxBlood).Append(",");
                        BossWarRecord.Append("BaseDefence: ").Append(p3.PlayerDetail.GetBaseDefence()).Append(",");
                        if (p3.PlayerDetail.SecondWeapon != null)
                        {
                            BossWarRecord.Append("SecondWeapon TemplateID: ").Append(p3.PlayerDetail.SecondWeapon.TemplateID).Append(",");
                            BossWarRecord.Append("SecondWeapon StrengthenLevel: ").Append(p3.PlayerDetail.SecondWeapon.StrengthenLevel).Append(".");
                        }
                    }
                }
                this.BossWarField = BossWarRecord.ToString();
                base.OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, this.beginTime, DateTime.Now, this.BeginPlayersCount, this.MissionInfo.Id, teamAStr, teamBStr, dropTemplateIdsStr, winTeam, this.BossWarField);
                base.OnGameOverred();
            }
        }
        public bool HasNextSession()
        {
            if (base.RoomType == eRoomType.ConsortiaBoss || this.isDragonLair())
            {
                return false;
            }
            if (base.PlayerCount == 0 || !this.IsWin)
            {
                return false;
            }
            int nextSessionId = 1 + this.SessionId;
            return this.Misssions.ContainsKey(nextSessionId);
        }
        public void GameOverAllSession()//__missionAllOver(event:CrazyTankSocketEvent) : void
        {
            if (base.GameState == eGameState.GameOver)
            {
                this.m_gameState = eGameState.ALLSessionStopped;
                try
                {
                    this.m_gameAI.OnGameOverAllSession();
                }
                catch (Exception ex)
                {
                    PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
                }
                List<Player> players = base.GetAllFightPlayers();
                GSPacketIn pkg = new GSPacketIn(91);
                pkg.WriteByte(115);
                int canTakeCards = 1;
                if (!this.IsWin)
                {
                    canTakeCards = 0;
                }
                else
                {
                    eRoomType roomType = base.RoomType;
                    if (roomType == eRoomType.Dungeon || roomType == eRoomType.SpecialActivityDungeon || base.RoomType == eRoomType.AcademyDungeon)
                    {
                        canTakeCards = 2;
                    }
                }
                pkg.WriteInt(base.PlayerCount);
                foreach (Player p in players)
                {
                    p.CanTakeOut = canTakeCards;
                    p.PlayerDetail.OnGameOver(this, this.IsWin, p.GainGP);
                    int value = 30;
                    if (this.isDragonLair() && this.SessionId < 3)
                    {
                        value = 10;
                    }
                    eRoomType roomType2 = base.RoomType;
                    if ((roomType2 == eRoomType.Dungeon || roomType2 == eRoomType.AcademyDungeon) && this.IsWin)
                    {
                        p.PlayerDetail.RemoveMissionEnergy(value);
                    }
                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.ID);
                    pkg.WriteInt(0);
                    pkg.WriteInt(p.GainGP);
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
                    pkg.WriteInt(p.TotalAllExperience);
                    //pkg.WriteInt(0);
                    pkg.WriteBoolean(this.IsWin);
                }
                pkg.WriteInt(this.m_gameOverResources.Count);
                foreach (string res in this.m_gameOverResources)
                {
                    pkg.WriteString(res);
                }
                this.SendToAll(pkg);
                if (this.isDragonLair())
                {
                    base.WaitTime(19000);
                }
                else
                {
                    base.WaitTime(25000);
                }
                this.CanStopGame();
            }
        }
        public void CanStopGame()
        {
            if (!this.IsWin)
            {
                if (base.GameType == eGameType.Dungeon)
                {
                    base.ClearWaitTimer();
                    return;
                }
            }
            else
            {
                int nextSessionId = 1 + this.SessionId;
                if (this.Misssions.ContainsKey(nextSessionId) && this.isDragonLair())
                {
                    this.WantTryAgain = 1;
                }
            }
        }
        public void ShowDragonLairCard()
        {
            if (base.GameState == eGameState.ALLSessionStopped && this.IsWin)
            {
                List<Player> players = base.GetAllFightPlayers();
                foreach (Player p in players)
                {
                    if (p.IsActive && p.CanTakeOut > 0)
                    {
                        p.HasPaymentTakeCard = true;
                        int left = p.CanTakeOut;
                        for (int i = 0; i < left; i++)
                        {
                            this.TakeCard(p);
                        }
                    }
                }
                this.SendShowCards();
            }
        }
        public override void Stop()
        {
            if (base.GameState == eGameState.ALLSessionStopped)
            {
                this.m_gameState = eGameState.Stopped;
                if (this.IsWin)
                {
                    List<Player> players = base.GetAllFightPlayers();
                    foreach (Player p in players)
                    {
                        if (p.IsActive && p.CanTakeOut > 0)
                        {
                            p.HasPaymentTakeCard = true;
                            int left = p.CanTakeOut;
                            for (int i = 0; i < left; i++)
                            {
                                this.TakeCard(p);
                            }
                        }
                    }
                    if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.SpecialActivityDungeon || base.RoomType == eRoomType.AcademyDungeon)
                    {
                        this.SendShowCards();
                    }
                    if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.AcademyDungeon)
                    {
                        foreach (Player p2 in players)
                        {
                            p2.PlayerDetail.SetPvePermission(this.m_info.ID, this.m_hardLevel);
                        }
                    }
                }
                Dictionary<int, Player> players2;
                Monitor.Enter(players2 = this.m_players);
                try
                {
                    this.m_players.Clear();
                }
                finally
                {
                    Monitor.Exit(players2);
                }
                base.OnGameStopped();
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (Living living in this.m_livings)
            {
                living.Dispose();
            }
            try
            {
                this.m_missionAI.Dispose();
            }
            catch (Exception ex)
            {
                PVEGame.log.ErrorFormat("game ai script m_missionAI.Dispose() error:{1}", ex);
            }
            try
            {
                this.m_gameAI.Dispose();
            }
            catch (Exception ex2)
            {
                PVEGame.log.ErrorFormat("game ai script m_gameAI.Dispose() error:{1}", ex2);
            }
        }
        public void DoOther()
        {
            try
            {
                this.m_missionAI.DoOther();
            }
            catch (Exception ex)
            {
                PVEGame.log.ErrorFormat("game ai script m_gameAI.DoOther() error:{1}", ex);
            }
        }
        internal void OnShooted()
        {
            try
            {
                this.m_missionAI.OnShooted();
            }
            catch (Exception ex)
            {
                PVEGame.log.ErrorFormat("game ai script m_gameAI.OnShooted() error:{1}", ex);
            }
        }
        private int CalculateExperience(Player p)
        {
            if (this.TotalKillCount == 0)
            {
                return 1;
            }
            double gradeGap = Math.Abs((double)p.Grade - this.TotalNpcGrade / (double)this.TotalKillCount);
            if (gradeGap >= 7.0)
            {
                return 1;
            }
            double behaveRevisal = 0.0;
            if (this.TotalKillCount > 0)
            {
                behaveRevisal += (double)p.TotalKill / (double)this.TotalKillCount * 0.4;
            }
            if (this.TotalHurt > 0)
            {
                behaveRevisal += (double)p.TotalHurt / (double)this.TotalHurt * 0.4;
            }
            if (p.IsLiving)
            {
                behaveRevisal += 0.4;
            }
            double gradeGapRevisal = 1.0;
            if (gradeGap >= 3.0 && gradeGap <= 4.0)
            {
                gradeGapRevisal = 0.7;
            }
            else
            {
                if (gradeGap >= 5.0 && gradeGap <= 6.0)
                {
                    gradeGapRevisal = 0.4;
                }
            }
            double playerCountRevisal = (0.9 + (double)(this.BeginPlayersCount - 1) * 0.4) / (double)base.PlayerCount;
            double experience = this.TotalNpcExperience * behaveRevisal * gradeGapRevisal * playerCountRevisal;
            experience = ((experience == 0.0) ? 1.0 : experience);
            return (int)experience;
        }
        private int CalculateScore(Player p)
        {
            int score = (200 - base.TurnIndex) * 5 + p.TotalKill * 5 + (int)((double)p.Blood / (double)p.MaxBlood) * 10;
            if (!this.IsWin)
            {
                score -= 400;
            }
            return score;
        }
        private int CalculateHitRate(int hitTargetCount, int shootCount)
        {
            double toHit = 0.0;
            if (shootCount > 0)
            {
                toHit = (double)hitTargetCount / (double)shootCount;
            }
            return (int)(toHit * 100.0);
        }
        public override void CheckState(int delay)
        {
            base.AddAction(new CheckPVEGameStateAction(delay));
        }
        public bool TakeBossCard(Player player)
        {
            int index = 0;
            for (int i = 0; i < this.BossCards.Length; i++)
            {
                if (this.Cards[i] == 0)
                {
                    index = i;
                    break;
                }
            }
            return this.TakeCard(player, index);
        }
        public bool TakeBossCard(Player player, int index)
        {
            if (!player.IsActive || player.BossCardCount <= 0 || index < 0 || index > this.BossCards.Length || this.BossCards[index] > 0)
            {
                return false;
            }
            List<ItemInfo> infos = null;
            int templateId = 0;
            int gold = 0;
            int money = 0;
            int giftToken = 0;
            int medal = 0;
            int count = 0;
            int honor = 0;
            int hardCurrency = 0;
            int magicStonePoint = 0;
            int token = 0;
            int dragonToken = 0;
            int missionId = int.Parse(this.IsBossWar);
            DropInventory.BossDrop(missionId, ref infos);
            if (infos != null)
            {
                foreach (ItemInfo info in infos)
                {
                    ShopMgr.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken, ref magicStonePoint);
                    if (info != null && info.TemplateID > 0)
                    {
                        player.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count, eGameView.dungeonTypeGet);
                        templateId = info.TemplateID;
                        count = info.Count;
                    }
                }
            }
            player.PlayerDetail.AddGold(gold);
            player.PlayerDetail.AddMoney(money);
            player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_BossDrop, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
            player.PlayerDetail.AddGiftToken(giftToken);
            if (templateId == 0 && gold > 0)
            {
                templateId = -100;
                count = gold;
            }
            player.BossCardCount--;
            this.BossCards[index] = 1;
            base.SendGamePlayerTakeCard(player, index, templateId, count);
            return true;
        }
        public void SendMissionInfo()
        {
            if (this.m_missionInfo == null)
            {
                return;
            }
            GSPacketIn pkg = new GSPacketIn(91);
            pkg.WriteByte(113);
            pkg.WriteString(this.m_missionInfo.Name);
            pkg.WriteString(this.m_missionInfo.Success);
            pkg.WriteString(this.m_missionInfo.Failure);
            pkg.WriteString(this.m_missionInfo.Description);
            pkg.WriteString(this.m_missionInfo.Title);
            pkg.WriteInt(this.TotalMissionCount);
            pkg.WriteInt(this.SessionId);
            pkg.WriteInt(this.TotalTurn);
            pkg.WriteInt(this.TotalCount);
            pkg.WriteInt(this.Param1);
            pkg.WriteInt(this.Param2);
            pkg.WriteInt(0);
            //pkg.WriteString(this.Pic);
            this.SendToAll(pkg);
        }
        public void SendUpdateUiData()
        {
            GSPacketIn pkg = new GSPacketIn(91);
            pkg.WriteByte(104);
            int count = 0;
            try
            {
                count = this.m_missionAI.UpdateUIData();
            }
            catch (Exception ex)
            {
                PVEGame.log.ErrorFormat("game ai script {0} error:{1}", string.Format("m_missionAI.UpdateUIData()", new object[0]), ex);
            }
            pkg.WriteInt(base.TurnIndex);
            pkg.WriteInt(count);
            pkg.WriteInt(this.Param3);
            pkg.WriteInt(this.Param4);
            this.SendToAll(pkg);
        }
        internal void SendShowCards()
        {
            GSPacketIn pkg = new GSPacketIn(91);
            pkg.WriteByte(89);
            int count = 0;
            List<int> cardIndexs = new List<int>();
            for (int i = 0; i < this.Cards.Length; i++)
            {
                if (this.Cards[i] == 0)
                {
                    cardIndexs.Add(i);
                    count++;
                }
            }
            int templateId = 0;
            int itemCount = 0;
            pkg.WriteInt(count);
            int copyId = this.m_missionInfo.Id;
            foreach (int index in cardIndexs)
            {
                List<ItemInfo> infos = DropInventory.CopySystemDrop(copyId, cardIndexs.Count);
                if (infos != null)
                {
                    foreach (ItemInfo info in infos)
                    {
                        templateId = info.TemplateID;
                        itemCount = info.Count;
                    }
                }
                pkg.WriteByte((byte)index);
                pkg.WriteInt(templateId);
                pkg.WriteInt(itemCount);
            }
            this.SendToAll(pkg);
        }
        public void SendGameObjectFocus(int type, string name, int delay, int finishTime)
        {
            Physics[] physics = base.FindPhysicalObjByName(name);
            Physics[] array = physics;
            for (int i = 0; i < array.Length; i++)
            {
                Physics p = array[i];
                base.AddAction(new FocusAction(p, type, delay, finishTime));
            }
        }
        private void SendCreateGameToSingle(PVEGame game, IGamePlayer gamePlayer)
        {
            GSPacketIn pkg = new GSPacketIn(91);
            pkg.WriteByte(121);
            pkg.WriteInt(game.Map.Info.ID);
            pkg.WriteInt((int)((byte)game.RoomType));
            pkg.WriteInt((int)((byte)game.GameType));
            pkg.WriteInt(game.TimeType);
            List<Player> players = game.GetAllFightPlayers();
            pkg.WriteInt(players.Count);
            foreach (Player p in players)
            {
                IGamePlayer gp = p.PlayerDetail;
                pkg.WriteInt(gp.PlayerCharacter.ID);
                pkg.WriteString(gp.PlayerCharacter.NickName);
                pkg.WriteBoolean(false);
                pkg.WriteByte(gp.PlayerCharacter.typeVIP);
                pkg.WriteInt(gp.PlayerCharacter.VIPLevel);
                pkg.WriteBoolean(gp.PlayerCharacter.Sex);
                pkg.WriteInt(gp.PlayerCharacter.Hide);
                pkg.WriteString(gp.PlayerCharacter.Style);
                pkg.WriteString(gp.PlayerCharacter.Colors);
                pkg.WriteString(gp.PlayerCharacter.Skin);
                pkg.WriteInt(gp.PlayerCharacter.Grade);
                pkg.WriteInt(gp.PlayerCharacter.Repute);
                if (gp.MainWeapon == null)
                {
                    pkg.WriteInt(0);
                }
                else
                {
                    pkg.WriteInt(gp.MainWeapon.TemplateID);
                    pkg.WriteInt(gp.MainWeapon.RefineryLevel);
                    pkg.WriteString(gp.MainWeapon.Name);
                    pkg.WriteDateTime(DateTime.MinValue);
                }
                if (gp.SecondWeapon == null)
                {
                    pkg.WriteInt(0);
                }
                else
                {
                    pkg.WriteInt(gp.SecondWeapon.TemplateID);
                }
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaID);
                pkg.WriteString(gp.PlayerCharacter.ConsortiaName);
                pkg.WriteInt(gp.PlayerCharacter.badgeID);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteBoolean(false);
                pkg.WriteInt(0);
                pkg.WriteInt(p.Team);
                pkg.WriteInt(p.Id);
                pkg.WriteInt(p.MaxBlood);
                pkg.WriteBoolean(p.Ready);
            }
            int index = game.SessionId;
            MissionInfo missionInfo = game.Misssions[index];
            pkg.WriteString(missionInfo.Name);
            pkg.WriteString(string.Format("show{0}.jpg", index));
            pkg.WriteString(missionInfo.Success);
            pkg.WriteString(missionInfo.Failure);
            pkg.WriteString(missionInfo.Description);
            pkg.WriteInt(game.TotalMissionCount);
            pkg.WriteInt(index);
            gamePlayer.SendTCP(pkg);
        }
        public void SendPlayerInfoInGame(PVEGame game, IGamePlayer gp, Player p)
        {
            GSPacketIn pkg = new GSPacketIn(91);
            pkg.Parameter2 = base.LifeTime;
            pkg.WriteByte(120);
            pkg.WriteInt(gp.ZoneId);
            pkg.WriteInt(gp.PlayerCharacter.ID);
            pkg.WriteInt(p.Team);
            pkg.WriteInt(p.Id);
            pkg.WriteInt(p.MaxBlood);
            pkg.WriteBoolean(p.Ready);
            game.SendToAll(pkg);
        }
        public void SendPlaySound(string playStr)
        {
            GSPacketIn pkg = new GSPacketIn(91);
            pkg.WriteByte(63);
            pkg.WriteString(playStr);
            this.SendToAll(pkg);
        }
        public void SendLoadResource(List<LoadingFileInfo> loadingFileInfos)
        {
            if (loadingFileInfos != null && loadingFileInfos.Count > 0)
            {
                GSPacketIn pkg = new GSPacketIn(91);
                pkg.WriteByte(67);
                pkg.WriteInt(loadingFileInfos.Count);
                foreach (LoadingFileInfo file in loadingFileInfos)
                {
                    pkg.WriteInt(file.Type);
                    pkg.WriteString(file.Path);
                    pkg.WriteString(file.ClassName);
                }
                this.SendToAll(pkg);
            }
        }
        public override void MinusDelays(int lowestDelay)
        {
            this.m_pveGameDelay -= lowestDelay;
            base.MinusDelays(lowestDelay);
        }
        public void Print(string str)
        {
            Console.WriteLine(str);
        }

        public HardBoss CreateHard(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            HardBoss boss = new HardBoss(base.PhysicalId++, this, npcInfo, direction, type);
            boss.Reset();
            boss.SetXY(x, y);
            this.AddLiving(boss);
            boss.StartMoving();
            return boss;
        }
        public NormalNpc CreateNormal(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            NormalNpc boss = new NormalNpc(base.PhysicalId++, this, npcInfo, direction, type);
            boss.Reset();
            boss.SetXY(x, y);
            AddLiving(boss);
            boss.StartMoving();
            AddLiving2(boss);
            return boss;
        }
        public void AddLiving2(Living living)
        {
            base.AddNormalBoss(living);
            living.Died += new LivingEventHandle(living_Died);
        }

        public NormalBoss CreateSimple(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            NormalBoss boss = new NormalBoss(base.PhysicalId++, this, npcInfo, direction, type);
            if (type != 2)
            {
                boss.Blood = npcInfo.Blood / 2;
                boss.BaseDamage = npcInfo.BaseDamage;
                boss.BaseGuard = npcInfo.BaseGuard;
                boss.Attack = npcInfo.Attack;
                boss.Defence = npcInfo.Defence;
                boss.Agility = npcInfo.Agility;
                boss.Lucky = npcInfo.Lucky;
                boss.Grade = npcInfo.Level;
                boss.Experience = npcInfo.Experience;
            }
            else
            {
                boss.Reset();
            }
            boss.SetRect(npcInfo.X, npcInfo.Y, npcInfo.Width, npcInfo.Height);
            boss.SetXY(x, y);
            AddLiving(boss);
            boss.StartMoving();
            AddLiving2(boss);
            return boss;
        }



        public SimpleBossFlying CreateBossFly(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleBossFlying boss = new SimpleBossFlying(PhysicalId++, this, npcInfo, direction, type, "");
            boss.Reset();
            boss.SetXY(x, y);
            AddLiving(boss);
            boss.StartMoving();

            return boss;
        }

        public SimpleNpcFire CreateNpcFire(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpcFire npc = new SimpleNpcFire(PhysicalId++, this, npcInfo, direction, type);
            npc.Reset();
            npc.SetXY(x, y);
            AddLiving(npc);

            npc.StartMoving();

            return npc;
        }

        public SimpleMyBoss CreateBoom(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleMyBoss boss = new SimpleMyBoss(this.PhysicalId++, this, npcInfo, direction, type);
            boss.Reset();
            boss.SetXY(x, y);
            this.AddLiving(boss);
            boss.StartMoving();
            return boss;
        }

        public KhoiTaoKhaNangConfig BaseLivingConfig(int dd)
        {
            return new KhoiTaoKhaNangConfig
            {
                IsBay = false,
                IsBrother = false,
                IsWorldBoss = false,
            };
        }
        //public SimpleNpc CreateNpc(int npcId, int x, int y, int direction)
        //{
        //    NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
        //    SimpleNpc npc = new SimpleNpc(PhysicalId++, this, npcInfo, direction, 0);
        //    npc.Reset();

        //    npc.SetXY(x, y);
        //    AddLiving(npc);
        //    npc.StartMoving();

        //    return npc;
        //}
        public SimpleNpc CreateNpc(int npcId, int x, int y, int type)
        {
            return this.CreateNpc(npcId, x, y, type, -1, "", this.BaseLivingConfig());
        }
        public SimpleNpc CreateNpc(int npcId, int x, int y, int direction, KhoiTaoKhaNangConfig config)
        {
            NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpc simpleNpc = new SimpleNpc(this.PhysicalId++, this, npcInfoById, direction, 0);
            if (config != null)
            {
                //simpleNpc.Config = config;
            }
            simpleNpc.Reset();
            simpleNpc.SetXY(x, y);
            this.AddLiving(simpleNpc);
            simpleNpc.StartMoving();
            simpleNpc.FallFrom(simpleNpc.X, simpleNpc.Y, "", 0, 0, 0x3e8, null);
            return simpleNpc;
        }

        public Layer Createlayerboss(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            Layer layer = new Layer(this.PhysicalId++, name, model, defaultAction, scale, rotation);
            layer.SetXY(x, y);
            this.AddPhysical(layer, true);
            return layer;
        }

        public virtual void AddPhysical(PhysicalObj phy, bool sendToClient)
        {
            this.m_map.AddPhysical(phy);
            phy.SetGame(this);
            if (sendToClient)
            {
                this.SendAddPhysical(phy);
            }
        }
        internal void SendAddPhysical(PhysicalObj obj)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x30);
            pkg.WriteInt(obj.Id);
            pkg.WriteInt(obj.Type);
            pkg.WriteInt(obj.X);
            pkg.WriteInt(obj.Y);
            pkg.WriteString(obj.Model);
            pkg.WriteString(obj.CurrentAction);
            pkg.WriteInt(obj.Scale);
            pkg.WriteInt(obj.Scale);
            pkg.WriteInt(obj.Rotation);
            pkg.WriteInt(1);
            pkg.WriteInt(0);
            this.SendToAll(pkg);
        }
        public SimpleBoss CreateBoss2(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleBoss boss = new SimpleBoss(PhysicalId++, this, npcInfo, direction, type);
            boss.Reset();
            boss.SetXY(x, y);
            AddLiving2(boss);
            boss.StartMoving();

            return boss;
        }

        public SimpleMyNpc CreateMyBoss(int npcId, int x, int y, int direction, int type)//MrPhuong
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleMyNpc boss = new SimpleMyNpc(PhysicalId++, this, npcInfo, direction, type);


            boss.Blood = npcInfo.Blood / 2;
            boss.BaseDamage = npcInfo.BaseDamage;
            boss.BaseGuard = npcInfo.BaseGuard;
            boss.TotalCure = 0;
            boss.Attack = npcInfo.Attack;
            boss.Defence = npcInfo.Defence;
            boss.Agility = npcInfo.Agility;
            boss.Lucky = npcInfo.Lucky;

            boss.Grade = npcInfo.Level;
            boss.Experience = npcInfo.Experience;
            boss.SetRect(npcInfo.X, npcInfo.Y, npcInfo.Width, npcInfo.Height);

            boss.SetXY(x, y);
            AddLiving(boss);
            boss.StartMoving();
            boss.FallFrom(boss.X, boss.Y, "", 0, 0, 1000, null);
            return boss;
        }

        public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string action, KhoiTaoKhaNangConfig config)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleBoss boss = new SimpleBoss(this.PhysicalId++, this, npcInfo, direction, type);
            if (config != null)
            {
                //boss.Config = config;
            }
            //if (boss.Config.ReduceBloodStart > 1)
            //{
            // boss.Blood = npcInfo.Blood / boss.Config.ReduceBloodStart;
            //}
            else
            {
                boss.Reset();
                //if (boss.Config.IsWorldBoss && this.WorldbossBood < 2147483647L)
                //{
                //    boss.Blood = (int)this.WorldbossBood;
                //}
                //if (boss.Config.isConsortiaBoss)
                //{
                //    boss.Blood -= (int)this.AllWorldDameBoss;
                //}
            }
            boss.SetXY(x, y);
            this.AddLiving(boss);
            boss.StartMoving();
            return boss;
        }

        public void Dichuyenmanhinh(int x, int y, int type, int delay, int finishTime)
        {
            AddAction(new DichuyenmanhinhAction(x, y, type, delay, finishTime));
        }

        public int tinhgoc(int Xp, int Yp, int Xnpc, int Ynpc)
        {
            double x;
            double y;
            double A = (Ynpc - Yp);
            double B = (Xnpc - Xp);
            x = A / B;
            y = Ynpc - (x * Xnpc);
            double BBphay = Xnpc;
            double CBphay = (y - Ynpc);
            if (CBphay < 0)
            {
                CBphay = -CBphay;
            }
            double goc = Math.Atan(BBphay / CBphay) * 180 / Math.PI;
            return (int)goc;
        }

        public void SendLivingActionMapping(Living liv, string source, string value)
        {
            if (liv != null)
            {
                base.method_25(liv.Id, source, value);
            }
        }

        public void SendHideBlood(Living living, int hide)
        {
            base.PedSuikAov(living, hide);
        }

        public void SendObjectFocus(Physics m_helper, int p1, int p2, int p3)
        {
            base.AddAction(new FocusAction2(m_helper, p1, p2, p3));
        }

        public void SendFreeFocus(int x, int y, int type, int delay, int finishTime)
        {
            base.AddAction(new FocusFreeAction(x, y, type, delay, finishTime));
        }

        public SimpleNpc[] GetNPCLivingWithID(int id)
        {
            List<SimpleNpc> list = new List<SimpleNpc>();
            foreach (Living living in this.m_livings)
            {
                if (((living is SimpleNpc) && living.IsLiving) && ((living as SimpleNpc).NpcInfo.ID == id))
                {
                    list.Add(living as SimpleNpc);
                }
            }
            return list.ToArray();
        }

        public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, string action)
        {
            return this.CreateNpc(npcId, x, y, type, direction, action, this.BaseLivingConfig());
        }

        public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, string action, LivingConfig config)
        {
            NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
            int physicalId = base.PhysicalId;
            base.PhysicalId = physicalId + 1;
            SimpleNpc living = new SimpleNpc(physicalId, this, npcInfoById, type, direction, action);
            if (config != null)
            {
                living.Config = new LivingConfig();
                living.Config.Clone(config);
            }
            living.Reset();
            if (living.Config.ReduceBloodStart > 1)
            {
                living.Blood = npcInfoById.Blood / living.Config.ReduceBloodStart;
            }
            living.SetXY(x, y);
            this.AddLiving(living);
            living.StartMoving();
            return living;
        }

        public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scale, int rotation, int typeEffect)
        {
            int physicalId = base.PhysicalId;
            base.PhysicalId = physicalId + 1;
            PhysicalObj phy = new PhysicalObj(physicalId, name, model, defaultAction, scale, rotation, typeEffect);
            phy.SetXY(x, y);
            this.AddPhysicalObj(phy, true);
            return phy;
        }

        public Ball CreateBall(int x, int y, string name, string defaultAction, int scale, int rotation)
        {
            int physicalId = base.PhysicalId;
            base.PhysicalId = physicalId + 1;
            Ball phy = new Ball(physicalId, name, defaultAction, scale, rotation);
            phy.SetXY(x, y);
            this.AddPhysicalObj(phy, true);
            base.method_25(phy.Id, "pick", name);
            return phy;
        }
        public void SendPlayersPicture(Living living, int type, bool state)
        {
            base.method_47(living, type, state);
        }
        public AMissionControl MissionAI
        {
            get
            {
                return this.m_missionAI;
            }
        }

        public LayerTop CreateLayerTop(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            int physicalId = base.PhysicalId;
            base.PhysicalId = physicalId + 1;
            LayerTop phy = new LayerTop(physicalId, name, model, defaultAction, scale, rotation);
            phy.SetXY(x, y);
            this.AddPhysicalObj(phy, true);
            return phy;
        }
        internal void method_52()
        {
            try
            {
                this.m_missionAI.OnShooted();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("game ai script m_gameAI.OnShooted() error:{0}", exception);
            }
        }
    }
}

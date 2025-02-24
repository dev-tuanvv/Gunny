namespace Game.Logic
{
    using Game.Base.Packets;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Maps;
    using Game.Logic.Phy.Object;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Reflection;

    public class BaseGame : AbstractGame
    {
        public int blueScore;
        public string BossWarField;
        public int[] Cards;
        public int ConsortiaAlly;
        public int CurrentActionCount;
        public int CurrentTurnTotalDamage;
        private List<PetSkillElementInfo> GameNeedPetSkillInfo;
        public TurnedLiving LastTurnLiving;
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private long long_1;
        private ArrayList m_actions;
        protected TurnedLiving m_currentLiving;
        protected eGameState m_gameState;
        private bool m_GetBlood;
        private int m_lifeTime;
        protected List<Living> m_livings;
        private List<LoadingFileInfo> m_loadingFiles;
        protected Game.Logic.Phy.Maps.Map m_map;
        protected int m_nextPlayerId;
        protected int m_nextWind;
        private long m_passTick;
        protected Dictionary<int, Player> m_players;
        protected System.Random m_random;
        private int m_roomId;
        private List<Ball> m_tempBall;
        private List<Box> m_tempBox;
        private List<Point> m_tempPoints;
        private List<TurnedLiving> m_turnQueue;
        private long m_waitTimer;
        public int PhysicalId;
        public int redScore;
        public int RichesRate;
        public int TotalCostGold;
        public int TotalCostMoney;
        public int TotalHurt;
        protected int turnIndex;

        public event GameEventHandle BeginNewTurn;

        public event GameNpcDieEventHandle GameNpcDie;

        public event GameOverLogEventHandle GameOverLog;

        public event GameEventHandle GameOverred;

        public BaseGame(int id, int roomId, Game.Logic.Phy.Maps.Map map, eRoomType roomType, eGameType gameType, int timeType) : base(id, roomType, gameType, timeType)
        {
            this.m_loadingFiles = new List<LoadingFileInfo>();
            this.GameNeedPetSkillInfo = new List<PetSkillElementInfo>();
            this.long_1 = 0L;
            this.m_roomId = roomId;
            this.m_players = new Dictionary<int, Player>();
            this.m_turnQueue = new List<TurnedLiving>();
            this.m_livings = new List<Living>();
            this.m_random = new System.Random();
            this.m_map = map;
            this.m_actions = new ArrayList();
            this.PhysicalId = 0;
            this.BossWarField = "";
            this.m_tempBox = new List<Box>();
            this.m_tempBall = new List<Ball>();
            this.m_tempPoints = new List<Point>();
            this.GameNeedPetSkillInfo = PetMgr.GameNeedPetSkill();
            if ((base.RoomType == eRoomType.Dungeon) || (base.RoomType == eRoomType.SpecialActivityDungeon))
            {
                this.Cards = new int[0x15];
            }
            else
            {
                this.Cards = new int[9];
            }
            this.m_gameState = eGameState.Inited;
        }

        public void AddAction(IAction action)
        {
            lock (this.m_actions)
            {
                this.m_actions.Add(action);
            }
        }

        public void AddAction(ArrayList actions)
        {
            lock (this.m_actions)
            {
                this.m_actions.AddRange(actions);
            }
        }

        public Ball AddBall(Ball ball, bool sendToClient)
        {
            this.m_tempBall.Add(ball);
            this.AddPhysicalObj(ball, sendToClient);
            return ball;
        }

        public Ball AddBall(Point pos, bool sendToClient)
        {
            Ball phy = new Ball(this.PhysicalId++, "1");
            phy.SetXY(pos);
            this.AddPhysicalObj(phy, sendToClient);
            return this.AddBall(phy, sendToClient);
        }

        public Box AddBox(Box box, bool sendToClient)
        {
            this.m_tempBox.Add(box);
            this.AddPhysicalObj(box, sendToClient);
            return box;
        }

        public Box AddBox(SqlDataProvider.Data.ItemInfo item, Point pos, bool sendToClient)
        {
            Box phy = new Box(this.PhysicalId++, "1", item);
            phy.SetXY(pos);
            this.AddPhysicalObj(phy, sendToClient);
            return this.AddBox(phy, sendToClient);
        }

        public virtual void AddLiving(Living living)
        {
            this.m_map.AddPhysical(living);
            if (living is Player)
            {
                Player player = living as Player;
                if (player.Weapon == null)
                {
                    return;
                }
            }
            if (living is TurnedLiving)
            {
                this.m_turnQueue.Add(living as TurnedLiving);
            }
            else
            {
                this.m_livings.Add(living);
            }
            this.SendAddLiving(living);
        }

        public void AddLoadingFile(int type, string file, string className)
        {
            if ((file != null) && (className != null))
            {
                this.m_loadingFiles.Add(new LoadingFileInfo(type, file, className));
            }
        }

        public virtual void AddNormalBoss(Living living)
        {
            this.m_map.AddPhysical(living);
            if (living is Player)
            {
                Player player = living as Player;
                if (player.Weapon == null)
                {
                    return;
                }
            }
            this.m_livings.Add(living);
            this.SendAddLiving(living);
        }

        protected void AddPlayer(IGamePlayer gp, Player fp)
        {
            lock (this.m_players)
            {
                this.m_players.Add(fp.Id, fp);
                if (fp.Weapon != null)
                {
                    this.m_turnQueue.Add(fp);
                }
            }
        }

        public virtual void AddPhysicalObj(PhysicalObj phy, bool sendToClient)
        {
            this.m_map.AddPhysical(phy);
            phy.SetGame(this);
            if (sendToClient)
            {
                this.SendAddPhysicalObj(phy);
            }
        }

        public virtual void AddPhysicalTip(PhysicalObj phy, bool sendToClient)
        {
            this.m_map.AddPhysical(phy);
            phy.SetGame(this);
            if (sendToClient)
            {
                this.SendAddPhysicalTip(phy);
            }
        }

        public void AddTempPoint(int x, int y)
        {
            this.m_tempPoints.Add(new Point(x, y));
        }

        public void AfterUseItem(SqlDataProvider.Data.ItemInfo item)
        {
        }

        internal void capnhattrangthai(Living player, string loai1, string loai2)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id);
            pkg.WriteByte(0x29);
            pkg.WriteString(loai1);
            pkg.WriteString(loai2);
            this.SendToAll(pkg);
        }

        public void ClearAllChild()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_livings)
            {
                if (living.IsLiving && (living is SimpleNpc))
                {
                    list.Add(living);
                }
            }
            foreach (Living living2 in list)
            {
                this.m_livings.Remove(living2);
                living2.Dispose();
                this.RemoveLiving(living2.Id);
            }
        }

        public void ClearAllNpc()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_livings)
            {
                if (living is SimpleNpc)
                {
                    list.Add(living);
                }
            }
            foreach (Living living2 in list)
            {
                this.m_livings.Remove(living2);
                living2.Dispose();
                this.SendRemoveLiving(living2.Id);
            }
            List<Physics> allPhysicalSafe = this.m_map.GetAllPhysicalSafe();
            foreach (Physics physics in allPhysicalSafe)
            {
                if (physics is SimpleNpc)
                {
                    this.m_map.RemovePhysical(physics);
                }
            }
        }

        public void ClearBall()
        {
            List<Ball> list = new List<Ball>();
            foreach (Ball ball in this.m_tempBall)
            {
                list.Add(ball);
            }
            foreach (Ball ball2 in list)
            {
                this.m_tempBall.Remove(ball2);
                this.RemovePhysicalObj(ball2, true);
            }
        }

        public void ClearDiedPhysicals()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_livings)
            {
                if (!living.IsLiving)
                {
                    list.Add(living);
                }
            }
            foreach (Living living2 in list)
            {
                this.m_livings.Remove(living2);
                living2.Dispose();
            }
            List<Living> list2 = new List<Living>();
            foreach (TurnedLiving living3 in this.m_turnQueue)
            {
                if (!living3.IsLiving)
                {
                    list2.Add(living3);
                }
            }
            foreach (TurnedLiving living4 in list2)
            {
                this.m_turnQueue.Remove(living4);
            }
            List<Physics> allPhysicalSafe = this.m_map.GetAllPhysicalSafe();
            foreach (Physics physics in allPhysicalSafe)
            {
                if (!(physics.IsLiving || (physics is Player)))
                {
                    this.m_map.RemovePhysical(physics);
                }
            }
        }

        public void ClearLoadingFiles()
        {
            this.m_loadingFiles.Clear();
        }

        public void ClearWaitTimer()
        {
            this.m_waitTimer = 0L;
        }

        public List<Box> CreateBox()
        {
            int num = this.m_players.Count + 2;
            int num2 = 0;
            List<SqlDataProvider.Data.ItemInfo> info = null;
            if (this.CurrentTurnTotalDamage > 0)
            {
                num2 = this.m_random.Next(1, 3);
                if ((this.m_tempBox.Count + num2) > num)
                {
                    num2 = num - this.m_tempBox.Count;
                }
                if (num2 > 0)
                {
                    DropInventory.BoxDrop(base.m_roomType, ref info);
                }
            }
            int diedPlayerCount = this.GetDiedPlayerCount();
            int num4 = 0;
            if (diedPlayerCount > 0)
            {
                num4 = this.m_random.Next(diedPlayerCount);
            }
            if (((this.m_tempBox.Count + num2) + num4) > num)
            {
                num4 = (num - this.m_tempBox.Count) - num2;
            }
            List<Box> list2 = new List<Box>();
            if (info != null)
            {
                for (int i = 0; i < this.m_tempPoints.Count; i++)
                {
                    int num6 = this.m_random.Next(this.m_tempPoints.Count);
                    Point point = this.m_tempPoints[num6];
                    this.m_tempPoints[num6] = this.m_tempPoints[i];
                    this.m_tempPoints[i] = point;
                }
                int num7 = Math.Min(info.Count, this.m_tempPoints.Count);
                for (int j = 0; j < num7; j++)
                {
                    list2.Add(this.AddBox(info[j], this.m_tempPoints[j], true));
                }
            }
            this.m_tempPoints.Clear();
            return list2;
        }

        public void CheckBox()
        {
            List<Box> list = new List<Box>();
            foreach (Box box in this.m_tempBox)
            {
                if (!box.IsLiving)
                {
                    list.Add(box);
                }
            }
            foreach (Box box2 in list)
            {
                this.m_tempBox.Remove(box2);
                this.RemovePhysicalObj(box2, true);
            }
        }

        public virtual void CheckState(int delay)
        {
        }

        internal void Dichuyenmanhinhmini(int x, int y, int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x3e);
            pkg.WriteInt(type);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            this.SendToAll(pkg);
        }

        public SimpleMyBoss[] FindAllBoom()
        {
            List<SimpleMyBoss> list = new List<SimpleMyBoss>();
            foreach (Living living in this.m_livings)
            {
                if (living is SimpleMyBoss)
                {
                    list.Add(living as SimpleMyBoss);
                    return list.ToArray();
                }
            }
            return null;
        }

        public SimpleBoss[] FindAllBoss()
        {
            List<SimpleBoss> list = new List<SimpleBoss>();
            foreach (Living living in this.m_livings)
            {
                if (living is SimpleBoss)
                {
                    list.Add(living as SimpleBoss);
                }
            }
            return list.ToArray();
        }

        public List<SimpleBoss> FindAllBossTurned()
        {
            List<SimpleBoss> list = new List<SimpleBoss>();
            foreach (TurnedLiving living in this.m_turnQueue)
            {
                if (living is SimpleBoss)
                {
                    list.Add(living as SimpleBoss);
                }
            }
            return list;
        }

        public SimpleFireHell[] FindAllFire()
        {
            List<SimpleFireHell> list = new List<SimpleFireHell>();
            foreach (Living living in this.m_livings)
            {
                if (living is SimpleFireHell)
                {
                    list.Add(living as SimpleFireHell);
                    return list.ToArray();
                }
            }
            return null;
        }

        public SimpleMyNpc[] FindAllMyNPC()
        {
            List<SimpleMyNpc> list = new List<SimpleMyNpc>();
            foreach (Living living in this.m_livings)
            {
                if (living is SimpleMyNpc)
                {
                    list.Add(living as SimpleMyNpc);
                }
            }
            return list.ToArray();
        }

        public List<NormalBoss> FindAllNormalBoss()
        {
            List<NormalBoss> list = new List<NormalBoss>();
            foreach (Living living in this.m_livings)
            {
                if (living is NormalBoss)
                {
                    list.Add(living as NormalBoss);
                }
            }
            return list;
        }

        public List<NormalNpc> FindAllNormalNPC()
        {
            List<NormalNpc> list = new List<NormalNpc>();
            foreach (Living living in this.m_livings)
            {
                if (living is NormalNpc)
                {
                    list.Add(living as NormalNpc);
                }
            }
            return list;
        }

        public SimpleNpc[] FindAllNpc()
        {
            List<SimpleNpc> list = new List<SimpleNpc>();
            foreach (Living living in this.m_livings)
            {
                if (living is SimpleNpc)
                {
                    list.Add(living as SimpleNpc);
                    return list.ToArray();
                }
            }
            return null;
        }

        public SimpleNpc[] FindAllNpcLiving()
        {
            List<SimpleNpc> list = new List<SimpleNpc>();
            foreach (Living living in this.m_livings)
            {
                if ((living is SimpleNpc) && living.IsLiving)
                {
                    list.Add(living as SimpleNpc);
                }
            }
            return list.ToArray();
        }

        public List<SimpleBoss> FindAllTurnBoss()
        {
            List<SimpleBoss> list = new List<SimpleBoss>();
            foreach (Living living in this.m_livings)
            {
                if (living is SimpleBoss)
                {
                    list.Add(living as SimpleBoss);
                }
            }
            return list;
        }

        public List<Living> FindAllTurnBossLiving()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_turnQueue)
            {
                if ((living is SimpleBoss) && living.IsLiving)
                {
                    list.Add(living);
                }
            }
            return list;
        }

        public Player FindFarPlayer(int x, int y)
        {
            Dictionary<int, Player> players = this.m_players;
            lock (players)
            {
                double minValue = double.MinValue;
                Player player = null;
                foreach (Player player2 in this.m_players.Values)
                {
                    if (player2.IsLiving)
                    {
                        double num2 = player2.Distance(x, y);
                        if (num2 > minValue)
                        {
                            minValue = num2;
                            player = player2;
                        }
                    }
                }
                return player;
            }
        }

        public int FindlivingbyDir(Living npc)
        {
            int num = 0;
            int num2 = 0;
            foreach (Player player in this.m_players.Values)
            {
                if (player.IsLiving)
                {
                    if (player.X > npc.X)
                    {
                        num2++;
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            if (num2 > num)
            {
                return 1;
            }
            if (num2 < num)
            {
                return -1;
            }
            return -npc.Direction;
        }

        public SimpleBoss[] FindLivingTurnBossWithID(int id)
        {
            List<SimpleBoss> list = new List<SimpleBoss>();
            foreach (Living living in this.m_turnQueue)
            {
                if (((living is SimpleBoss) && living.IsLiving) && ((living as SimpleBoss).NpcInfo.ID == id))
                {
                    list.Add(living as SimpleBoss);
                }
            }
            return list.ToArray();
        }

        public Living FindNearestHelper(int x, int y)
        {
            double maxValue = double.MaxValue;
            Living living = null;
            foreach (Living living2 in this.m_turnQueue)
            {
                if (living2.IsLiving && ((living2 is Player) || living2.Config.IsHelper))
                {
                    double num2 = living2.Distance(x, y);
                    if (num2 < maxValue)
                    {
                        maxValue = num2;
                        living = living2;
                    }
                }
            }
            return living;
        }

        public Player FindNearestPlayer(int x, int y)
        {
            double maxValue = double.MaxValue;
            Player player = null;
            foreach (Player player2 in this.m_players.Values)
            {
                if (player2.IsLiving)
                {
                    double num2 = player2.Distance(x, y);
                    if (num2 < maxValue)
                    {
                        maxValue = num2;
                        player = player2;
                    }
                }
            }
            return player;
        }

        public TurnedLiving FindNextTurnedFightFootball()
        {
            if (this.m_turnQueue.Count == 0)
            {
                return null;
            }
            int num = this.m_random.Next(this.m_turnQueue.Count - 1);
            TurnedLiving living = this.m_turnQueue[num];
            if (this.TurnIndex > 0)
            {
                for (int j = 0; j < this.m_turnQueue.Count; j++)
                {
                    if ((this.m_turnQueue[j] as Player).PlayerDetail.PlayerCharacter.ID == this.m_nextPlayerId)
                    {
                        living = this.m_turnQueue[j];
                        break;
                    }
                }
            }
            living.TurnNum++;
            for (int i = 0; i < this.m_turnQueue.Count; i++)
            {
                if ((this.m_turnQueue[i].Team != living.Team) && (this.m_turnQueue[i].TurnNum < living.TurnNum))
                {
                    this.m_nextPlayerId = (this.m_turnQueue[i] as Player).PlayerDetail.PlayerCharacter.ID;
                    return living;
                }
                num = this.m_random.Next(this.m_turnQueue.Count - 1);
                this.m_nextPlayerId = (this.m_turnQueue[num] as Player).PlayerDetail.PlayerCharacter.ID;
            }
            return living;
        }

        public TurnedLiving FindNextTurnedLiving()
        {
            if (this.m_turnQueue.Count == 0)
            {
                return null;
            }
            int num = this.m_random.Next(this.m_turnQueue.Count - 1);
            TurnedLiving living = this.m_turnQueue[num];
            int delay = living.Delay;
            for (int i = 0; i < this.m_turnQueue.Count; i++)
            {
                if ((this.m_turnQueue[i].Delay < delay) && this.m_turnQueue[i].IsLiving)
                {
                    delay = this.m_turnQueue[i].Delay;
                    living = this.m_turnQueue[i];
                }
            }
            living.TurnNum++;
            return living;
        }

        public Player FindPlayer(int id)
        {
            lock (this.m_players)
            {
                if (this.m_players.ContainsKey(id))
                {
                    return this.m_players[id];
                }
            }
            return null;
        }

        public PhysicalObj[] FindPhysicalObjByName(string name)
        {
            List<PhysicalObj> list = new List<PhysicalObj>();
            foreach (PhysicalObj obj2 in this.m_map.GetAllPhysicalObjSafe())
            {
                if (obj2.Name == name)
                {
                    list.Add(obj2);
                }
            }
            return list.ToArray();
        }

        public PhysicalObj[] FindPhysicalObjByName(string name, bool CanPenetrate)
        {
            List<PhysicalObj> list = new List<PhysicalObj>();
            foreach (PhysicalObj obj2 in this.m_map.GetAllPhysicalObjSafe())
            {
                if ((obj2.Name == name) && (obj2.CanPenetrate == CanPenetrate))
                {
                    list.Add(obj2);
                }
            }
            return list.ToArray();
        }

        public Living FindRandomLiving()
        {
            List<Living> list = new List<Living>();
            Living living = null;
            foreach (Living living2 in this.m_livings)
            {
                if (living2.IsLiving)
                {
                    list.Add(living2);
                }
            }
            int num = this.Random.Next(0, list.Count);
            if (list.Count > 0)
            {
                living = list[num];
            }
            return living;
        }

        public Player FindRandomPlayer()
        {
            List<Player> list = new List<Player>();
            Player player = null;
            foreach (Player player2 in this.m_players.Values)
            {
                if (player2.IsLiving)
                {
                    list.Add(player2);
                }
            }
            int num = this.Random.Next(0, list.Count);
            if (list.Count > 0)
            {
                player = list[num];
            }
            return player;
        }

        public Player FindRandomPlayerNotLock()
        {
            List<Player> source = new List<Player>();
            foreach (Player player in this.m_players.Values)
            {
                if (player.IsLiving && (player.State != 9))
                {
                    source.Add(player);
                }
            }
            if (source.Count > 0)
            {
                int index = this.Random.Next(0, source.Count);
                return source.ElementAt<Player>(index);
            }
            return null;
        }

        public List<Player> GetAllEnemyPlayers(Living living)
        {
            List<Player> list = new List<Player>();
            lock (this.m_players)
            {
                foreach (Player player in this.m_players.Values)
                {
                    if (player.Team != living.Team)
                    {
                        list.Add(player);
                    }
                }
            }
            return list;
        }

        public List<Player> GetAllFightPlayers()
        {
            List<Player> list = new List<Player>();
            lock (this.m_players)
            {
                list.AddRange(this.m_players.Values);
            }
            return list;
        }

        public List<Player> GetAllLivingPlayers()
        {
            List<Player> list = new List<Player>();
            lock (this.m_players)
            {
                foreach (Player player in this.m_players.Values)
                {
                    if (player.IsLiving)
                    {
                        list.Add(player);
                    }
                }
            }
            return list;
        }

        public Player[] GetAllPlayers()
        {
            return this.GetAllFightPlayers().ToArray();
        }

        public List<Player> GetAllTeamPlayers(Living living)
        {
            List<Player> list = new List<Player>();
            lock (this.m_players)
            {
                foreach (Player player in this.m_players.Values)
                {
                    if (player.Team == living.Team)
                    {
                        list.Add(player);
                    }
                }
            }
            return list;
        }

        public List<Living> GetBossLivings()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_livings)
            {
                if (living.IsLiving && (living is SimpleBoss))
                {
                    list.Add(living);
                }
            }
            return list;
        }

        public int GetDiedBossCount()
        {
            int num = 0;
            foreach (SimpleBoss boss in this.FindAllBoss())
            {
                if (!boss.IsLiving)
                {
                    num++;
                }
            }
            return num;
        }

        public int GetDiedCount()
        {
            return (this.GetDiedNPCCount() + this.GetDiedBossCount());
        }

        public int GetDiedNPCCount()
        {
            int num = 0;
            foreach (SimpleNpc npc in this.FindAllNpc())
            {
                if (!npc.IsLiving)
                {
                    num++;
                }
            }
            return num;
        }

        public int GetDiedPlayerCount()
        {
            int num = 0;
            foreach (Player player in this.m_players.Values)
            {
                if (!(!player.IsActive || player.IsLiving))
                {
                    num++;
                }
            }
            return num;
        }

        public List<Living> GetFightFootballLivings()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_livings)
            {
                if (living is SimpleNpc)
                {
                    list.Add(living);
                }
            }
            return list;
        }

        public Player GetFrostPlayerRadom()
        {
            List<Player> allFightPlayers = this.GetAllFightPlayers();
            List<Player> source = new List<Player>();
            foreach (Player player in allFightPlayers)
            {
                if (player.IsFrost)
                {
                    source.Add(player);
                }
            }
            if (source.Count > 0)
            {
                int index = this.Random.Next(0, source.Count);
                return source.ElementAt<Player>(index);
            }
            return null;
        }

        public int GetHighDelayTurn()
        {
            new List<Living>();
            int delay = -2147483648;
            foreach (TurnedLiving living in this.m_turnQueue)
            {
                if ((living != null) && (living.Delay > delay))
                {
                    delay = living.Delay;
                }
            }
            return delay;
        }

        public List<Living> GetLivedLivings()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_livings)
            {
                if (living.IsLiving)
                {
                    list.Add(living);
                }
            }
            return list;
        }

        public List<Living> GetLivedLivingsHadTurn()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_livings)
            {
                if ((living.IsLiving && (living is SimpleNpc)) && living.Config.IsTurn)
                {
                    list.Add(living);
                }
            }
            return list;
        }

        public List<Living> GetLivedNpcs(int npcId)
        {
            List<Living> list = new List<Living>();
            foreach (Living living in this.m_livings)
            {
                if ((living.IsLiving && (living is SimpleNpc)) && ((living as SimpleNpc).NpcInfo.ID == npcId))
                {
                    list.Add(living);
                }
            }
            return list;
        }

        public TurnedLiving[] GetNextAllTurnedLiving()
        {
            if (this.m_turnQueue.Count == 0)
            {
                return null;
            }
            List<TurnedLiving> list = new List<TurnedLiving>();
            for (int i = 0; i < this.m_turnQueue.Count; i++)
            {
                if (((this.m_turnQueue[i].IsLiving && !this.m_turnQueue[i].IsFrost) && !this.m_turnQueue[i].IsAttacking) && (this.m_turnQueue[i] is Player))
                {
                    TurnedLiving local1 = this.m_turnQueue[i];
                    local1.TurnNum++;
                    list.Add(this.m_turnQueue[i]);
                }
            }
            return list.ToArray();
        }

        public float GetNextWind()
        {
            int num2;
            int num = (int) (this.Wind * 10f);
            if (num > this.m_nextWind)
            {
                num2 = num - this.m_random.Next(11);
                if (num <= this.m_nextWind)
                {
                    this.m_nextWind = this.m_random.Next(-40, 40);
                }
            }
            else
            {
                num2 = num + this.m_random.Next(11);
                if (num >= this.m_nextWind)
                {
                    this.m_nextWind = this.m_random.Next(-40, 40);
                }
            }
            return (((float) num2) / 10f);
        }

        public Player GetNoHolePlayerRandom()
        {
            List<Player> allFightPlayers = this.GetAllFightPlayers();
            List<Player> source = new List<Player>();
            foreach (Player player in allFightPlayers)
            {
                if (player.IsNoHole)
                {
                    source.Add(player);
                }
            }
            if (source.Count > 0)
            {
                int index = this.Random.Next(0, source.Count);
                return source.ElementAt<Player>(index);
            }
            return null;
        }

        public Player GetPlayer(IGamePlayer gp)
        {
            Player player = null;
            lock (this.m_players)
            {
                foreach (Player player2 in this.m_players.Values)
                {
                    if (player2.PlayerDetail == gp)
                    {
                        return player2;
                    }
                }
                return player;
            }
            return player;
        }

        public Player GetPlayerByIndex(int index)
        {
            return this.m_players.ElementAt<KeyValuePair<int, Player>>(index).Value;
        }

        public int GetPlayerCount()
        {
            return this.GetAllFightPlayers().Count;
        }

        protected Point GetPlayerPoint(MapPoint mapPos, int team)
        {
            List<Point> list = (team == 1) ? mapPos.PosX : mapPos.PosX1;
            int num = this.m_random.Next(list.Count);
            Point item = list[num];
            list.Remove(item);
            return item;
        }

        public bool GetSameTeam()
        {
            bool flag = false;
            Player[] allPlayers = this.GetAllPlayers();
            foreach (Player player in allPlayers)
            {
                if (player.Team != allPlayers[0].Team)
                {
                    return false;
                }
                flag = true;
            }
            return flag;
        }

        public int getTurnTime()
        {
            switch (base.m_timeType)
            {
                case 1:
                    return 8;

                case 2:
                    return 10;

                case 3:
                    return 12;

                case 4:
                    return 0x10;

                case 5:
                    return 0x15;

                case 6:
                    return 0x1f;
            }
            return -1;
        }

        public int GetTurnWaitTime()
        {
            return base.m_timeType;
        }

        public byte GetVane(int Wind, int param)
        {
            int wind = Math.Abs(Wind);
            switch (param)
            {
                case 1:
                    return WindMgr.GetWindID(wind, 1);

                case 3:
                    return WindMgr.GetWindID(wind, 3);
            }
            return 0;
        }

        public long GetWaitTimer()
        {
            return this.m_waitTimer;
        }

        public int GetWaitTimerLeft()
        {
            if (this.long_1 <= 0L)
            {
                return 0;
            }
            long num = (TickHelper.GetTickCount() > this.long_1) ? (TickHelper.GetTickCount() - this.long_1) : (this.long_1 - TickHelper.GetTickCount());
            if (num > 0x2710L)
            {
                return 0x3e8;
            }
            return (int) num;
        }

        public bool IsAllComplete()
        {
            List<Player> allFightPlayers = this.GetAllFightPlayers();
            foreach (Player player in allFightPlayers)
            {
                if (player.LoadingProcess < 100)
                {
                    return false;
                }
            }
            return true;
        }

        internal bool isTrainer()
        {
            return (base.RoomType == eRoomType.Freshman);
        }

        internal void method_10(int int_3, int int_4, int int_5)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x3e);
            pkg.WriteInt(int_5);
            pkg.WriteInt(int_3);
            pkg.WriteInt(int_4);
            this.SendToAll(pkg);
        }

        internal void method_11(Physics physics_0, int int_4)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x3e);
            pkg.WriteInt(int_4);
            pkg.WriteInt(physics_0.X);
            pkg.WriteInt(physics_0.Y);
            this.SendToAll(pkg);
        }

        internal void method_12(PhysicalObj physicalObj_0)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x42);
            pkg.WriteInt(physicalObj_0.Id);
            pkg.WriteString(physicalObj_0.CurrentAction);
            this.SendToAll(pkg);
        }

        internal void method_14(int int_4)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x35);
            pkg.WriteInt(int_4);
            this.SendToAll(pkg);
        }

        internal void method_18(Living living_0, int int_4, int int_5, int int_6, int int_7, string string_0, string string_1, int int_8)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living_0.Id) {
                Parameter1 = living_0.Id
            };
            pkg.WriteByte(0x37);
            pkg.WriteInt(int_4);
            pkg.WriteInt(int_5);
            pkg.WriteInt(int_6);
            pkg.WriteInt(int_7);
            pkg.WriteInt(int_8);
            pkg.WriteString(!string.IsNullOrEmpty(string_0) ? string_0 : "");
            pkg.WriteString(!string.IsNullOrEmpty(string_1) ? string_1 : "");
            this.SendToAll(pkg);
        }

        internal void method_24(Living living_0)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living_0.Id) {
                Parameter1 = living_0.Id
            };
            pkg.WriteByte(0x48);
            pkg.WriteInt(living_0.X);
            pkg.WriteInt(living_0.Y);
            this.SendToAll(pkg);
        }

        internal void method_25(int int_4, string string_0, string string_1)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, int_4) {
                Parameter1 = int_4
            };
            pkg.WriteByte(0xdf);
            pkg.WriteInt(int_4);
            pkg.WriteString(string_0);
            pkg.WriteString(string_1);
            this.SendToAll(pkg);
        }

        internal void method_30(Living living_0)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living_0.Id) {
                Parameter1 = living_0.Id
            };
            pkg.WriteByte(0x21);
            pkg.WriteBoolean(living_0.IsFrost);
            this.SendToAll(pkg);
        }

        internal void method_34(Living living_0, string string_0, string string_1)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living_0.Id) {
                Parameter1 = living_0.Id
            };
            pkg.WriteByte(0x29);
            pkg.WriteString(string_0);
            pkg.WriteString(string_1);
            this.SendToAll(pkg);
        }

        internal void method_39(Player player_0, int int_4, int int_5, int int_6)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player_0.Id) {
                Parameter1 = player_0.Id
            };
            pkg.WriteByte(0x20);
            pkg.WriteByte((byte) int_4);
            pkg.WriteInt(int_5);
            pkg.WriteInt(int_6);
            pkg.WriteInt(player_0.Id);
            pkg.WriteBoolean(false);
            this.SendToAll(pkg);
        }

        internal void method_47(Living living_0, int int_4, bool bool_0)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = living_0.Id
            };
            pkg.WriteByte(0x80);
            pkg.WriteInt(int_4);
            pkg.WriteBoolean(bool_0);
            this.SendToAll(pkg);
        }

        public virtual void MinusDelays(int lowestDelay)
        {
            foreach (TurnedLiving living in this.m_turnQueue)
            {
                living.Delay -= lowestDelay;
            }
        }

        protected void OnBeginNewTurn()
        {
            if (this.BeginNewTurn != null)
            {
                this.BeginNewTurn(this);
            }
        }

        public void OnGameNpcDie(int Id)
        {
            if (this.GameNpcDie != null)
            {
                this.GameNpcDie(Id);
            }
        }

        public void OnGameOverLog(int _roomId, eRoomType _roomType, eGameType _fightType, int _changeTeam, DateTime _playBegin, DateTime _playEnd, int _userCount, int _mapId, string _teamA, string _teamB, string _playResult, int _winTeam, string BossWar)
        {
            if (this.GameOverLog != null)
            {
                this.GameOverLog(_roomId, _roomType, _fightType, _changeTeam, _playBegin, _playEnd, _userCount, _mapId, _teamA, _teamB, _playResult, _winTeam, this.BossWarField);
            }
        }

        protected void OnGameOverred()
        {
            if (this.GameOverred != null)
            {
                this.GameOverred(this);
            }
        }

        public override void Pause(int time)
        {
            this.m_passTick = Math.Max(this.m_passTick, TickHelper.GetTickCount() + time);
        }

        internal void PedSuikAov(Living living_0, int int_4)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = living_0.Id
            };
            pkg.WriteByte(80);
            pkg.WriteInt(living_0.Id);
            pkg.WriteInt(int_4);
            this.SendToAll(pkg);
        }

        public override void ProcessData(GSPacketIn packet)
        {
            if (this.m_players.ContainsKey(packet.Parameter1))
            {
                Player player = this.m_players[packet.Parameter1];
                this.AddAction(new ProcessPacketAction(player, packet));
            }
        }

        public void RemoveLiving(int id)
        {
            foreach (Living living in this.m_livings)
            {
                if (living.Id == id)
                {
                    this.m_map.RemovePhysical(living);
                    if (living is TurnedLiving)
                    {
                        this.m_turnQueue.Remove(living as TurnedLiving);
                    }
                    else
                    {
                        this.m_livings.Remove(living);
                    }
                }
            }
            this.SendRemoveLiving(id);
        }

        public void RemoveLiving(Living living, bool sendToClient)
        {
            this.m_map.RemovePhysical(living);
            if (sendToClient)
            {
                this.method_14(living.Id);
            }
        }

        public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
        {
            Player player = null;
            lock (this.m_players)
            {
                foreach (Player player2 in this.m_players.Values)
                {
                    if (player2.PlayerDetail == gp)
                    {
                        player = player2;
                        this.m_players.Remove(player2.Id);
                        goto Label_0086;
                    }
                }
            }
        Label_0086:
            if (player != null)
            {
                this.AddAction(new RemovePlayerAction(player));
            }
            return player;
        }

        public void RemovePhysicalObj(PhysicalObj phy, bool sendToClient)
        {
            this.m_map.RemovePhysical(phy);
            phy.SetGame(null);
            if (sendToClient)
            {
                this.SendRemovePhysicalObj(phy);
            }
        }

        public override void Resume()
        {
            this.m_passTick = 0L;
        }

        public void SelectObject(int id, int zoneId)
        {
            lock (this.m_players)
            {
            }
        }

        internal void SendAddAnimation(Player player, int type, bool states)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x4f);
            pkg.WriteInt(type);
            pkg.WriteBoolean(states);
            pkg.WriteInt(player.Id);
            if (type == 1)
            {
                pkg.WriteDateTime(DateTime.Now.AddMilliseconds(9000.0));
            }
            this.SendToAll(pkg);
        }

        internal void SendAddLiving(Living living)
        {
            if (!(living is Player))
            {
                GSPacketIn pkg = new GSPacketIn(0x5b) {
                    Parameter1 = living.Id
                };
                pkg.WriteByte(0x40);
                pkg.WriteByte((byte) living.Type);
                pkg.WriteInt(living.Id);
                pkg.WriteString(living.Name);
                pkg.WriteString(living.ModelId);
                pkg.WriteString(living.ActionStr);
                pkg.WriteInt(living.X);
                pkg.WriteInt(living.Y);
                pkg.WriteInt(living.Blood);
                pkg.WriteInt(living.MaxBlood);
                pkg.WriteInt(living.Team);
                pkg.WriteByte((byte) living.Direction);
                pkg.WriteByte(living.Config.isBotom);
                pkg.WriteBoolean(living.Config.isShowBlood);
                pkg.WriteBoolean(living.Config.isShowSmallMapPoint);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteBoolean(living.IsFrost);
                pkg.WriteBoolean(living.IsHide);
                pkg.WriteBoolean(living.IsNoHole);
                pkg.WriteBoolean(false);
                pkg.WriteInt(0);
                if ((base.RoomType == eRoomType.ActivityDungeon) && (living is SimpleBoss))
                {
                    pkg.WriteInt((living as SimpleBoss).NpcInfo.ID);
                }
                this.SendToAll(pkg);
            }
        }

        internal void SendAddPhysicalObj(PhysicalObj obj)
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
            pkg.WriteInt(obj.phyBringToFront);
            pkg.WriteInt(obj.typeEffect);
            pkg.WriteInt(obj.ActionMapping.Count);
            foreach (string str in obj.ActionMapping.Keys)
            {
                pkg.WriteString(str);
                pkg.WriteString(obj.ActionMapping[str]);
            }
            this.SendToAll(pkg);
        }

        internal void SendAddPhysicalTip(PhysicalObj obj)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x44);
            pkg.WriteInt(obj.Id);
            pkg.WriteInt(obj.Type);
            pkg.WriteInt(obj.X);
            pkg.WriteInt(obj.Y);
            pkg.WriteString(obj.Model);
            pkg.WriteString(obj.CurrentAction);
            pkg.WriteInt(obj.Scale);
            pkg.WriteInt(obj.Rotation);
            this.SendToAll(pkg);
        }

        internal void SendAttackEffect(Living player, int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x81);
            pkg.WriteBoolean(true);
            pkg.WriteInt(type);
            this.SendToAll(pkg);
        }

        internal void SendCreateGame()
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x65);
            pkg.WriteInt((int) base.m_roomType);
            pkg.WriteInt((int) base.m_gameType);
            pkg.WriteInt(base.m_timeType);
            List<Player> allFightPlayers = this.GetAllFightPlayers();
            bool flag = base.m_roomType == eRoomType.FightFootballTime;
            pkg.WriteInt(allFightPlayers.Count);
            foreach (Player player in allFightPlayers)
            {
                IGamePlayer playerDetail = player.PlayerDetail;
                pkg.WriteInt(playerDetail.ZoneId);
                pkg.WriteString(playerDetail.ZoneName);
                pkg.WriteInt(playerDetail.PlayerCharacter.ID);
                pkg.WriteString(playerDetail.PlayerCharacter.NickName);
                pkg.WriteBoolean(false);
                pkg.WriteByte(playerDetail.PlayerCharacter.typeVIP);
                pkg.WriteInt(playerDetail.PlayerCharacter.VIPLevel);
                pkg.WriteBoolean(playerDetail.PlayerCharacter.Sex);
                pkg.WriteInt(playerDetail.PlayerCharacter.Hide);
                if (flag)
                {
                    pkg.WriteString(playerDetail.GetFightFootballStyle(player.Team));
                }
                else
                {
                    pkg.WriteString(playerDetail.PlayerCharacter.Style);
                }
                pkg.WriteString(playerDetail.PlayerCharacter.Colors);
                pkg.WriteString(playerDetail.PlayerCharacter.Skin);
                pkg.WriteInt(playerDetail.PlayerCharacter.Grade);
                pkg.WriteInt(playerDetail.PlayerCharacter.Repute);
                if (flag)
                {
                    pkg.WriteInt(0x112fc);
                }
                else
                {
                    pkg.WriteInt(playerDetail.MainWeapon.TemplateID);
                }
                pkg.WriteInt(playerDetail.MainWeapon.RefineryLevel);
                pkg.WriteString(playerDetail.MainWeapon.Name);
                pkg.WriteDateTime(DateTime.Now);
                if (playerDetail.SecondWeapon == null)
                {
                    pkg.WriteInt(0);
                }
                else
                {
                    pkg.WriteInt(playerDetail.SecondWeapon.TemplateID);
                }
                pkg.WriteInt(playerDetail.PlayerCharacter.Nimbus);
                pkg.WriteInt(playerDetail.PlayerCharacter.ConsortiaID);
                pkg.WriteString(playerDetail.PlayerCharacter.ConsortiaName);
                pkg.WriteInt(playerDetail.PlayerCharacter.badgeID);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(playerDetail.PlayerCharacter.Win);
                pkg.WriteInt(playerDetail.PlayerCharacter.Total);
                pkg.WriteInt(playerDetail.PlayerCharacter.FightPower);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteString("");
                pkg.WriteInt(playerDetail.PlayerCharacter.AchievementPoint);
                pkg.WriteString(playerDetail.PlayerCharacter.Honor);
                pkg.WriteInt(playerDetail.PlayerCharacter.Offer);
                pkg.WriteBoolean(playerDetail.PlayerCharacter.IsMarried);
                if (playerDetail.PlayerCharacter.IsMarried)
                {
                    pkg.WriteInt(playerDetail.PlayerCharacter.SpouseID);
                    pkg.WriteString(playerDetail.PlayerCharacter.SpouseName);
                }
                pkg.WriteInt(5);
                pkg.WriteInt(5);
                pkg.WriteInt(5);
                pkg.WriteInt(5);
                pkg.WriteInt(5);
                pkg.WriteInt(5);
                pkg.WriteInt(player.Team);
                pkg.WriteInt(player.Id);
                pkg.WriteInt(player.MaxBlood);
            }
            this.SendToAll(pkg);
        }

        internal void SendEquipEffect(Living player, string buffer)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(3);
            pkg.WriteString(buffer);
            this.SendToAll(pkg);
        }

        internal void SendFightAchievement(Living living, int achievID, int dis, int delay)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0xee);
            pkg.WriteInt(achievID);
            pkg.WriteInt(dis);
            pkg.WriteInt(delay);
            this.SendToAll(pkg);
        }

        internal void SendFightStatus(Living player, int status)
        {
            if (base.RoomType == eRoomType.ActivityDungeon)
            {
                GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                    Parameter1 = player.Id
                };
                pkg.WriteByte(0x4c);
                pkg.WriteInt(status);
                this.SendToAll(pkg);
            }
        }

        internal void SendGameActionMapping(Living player, Ball ball)
        {
            string currentAction = ball.CurrentAction;
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id);
            pkg.WriteByte(0xdf);
            pkg.WriteInt(ball.Id);
            pkg.WriteString(currentAction);
            pkg.WriteString(ball.ActionMapping[currentAction]);
            this.SendToAll(pkg);
        }

        internal void SendGameBigBox(Living player, List<int> listTemplate)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id);
            pkg.WriteByte(0x88);
            pkg.WriteInt(listTemplate.Count);
            foreach (int num in listTemplate)
            {
                pkg.WriteInt(num);
            }
            this.SendToAll(pkg);
        }

        internal void SendGameNextTurn(Living living, BaseGame game, List<Box> newBoxes)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(6);
            int val = (int) (this.m_map.wind * 10f);
            pkg.WriteInt(val);
            pkg.WriteBoolean(val > 0);
            pkg.WriteByte(this.GetVane(val, 1));
            pkg.WriteByte(this.GetVane(val, 2));
            pkg.WriteByte(this.GetVane(val, 3));
            pkg.WriteBoolean(living.IsHide);
            pkg.WriteInt(this.getTurnTime());
            pkg.WriteInt(newBoxes.Count);
            foreach (Box box in newBoxes)
            {
                pkg.WriteInt(box.Id);
                pkg.WriteInt(box.X);
                pkg.WriteInt(box.Y);
                pkg.WriteInt(box.Type);
            }
            if (living is TurnedLiving)
            {
                List<Player> allFightPlayers = game.GetAllFightPlayers();
                pkg.WriteInt(allFightPlayers.Count);
                foreach (Player player in allFightPlayers)
                {
                    pkg.WriteInt(player.Id);
                    pkg.WriteBoolean(player.IsLiving);
                    pkg.WriteInt(player.X);
                    pkg.WriteInt(player.Y);
                    pkg.WriteInt(player.Blood);
                    pkg.WriteBoolean(player.IsNoHole);
                    pkg.WriteInt(player.Energy);
                    pkg.WriteInt(player.psychic);
                    pkg.WriteInt(player.Dander);
                    pkg.WriteInt(player.ShootCount);
                }
                pkg.WriteInt(game.TurnIndex);
                pkg.WriteBoolean(false);
            }
            this.SendToAll(pkg);
        }

        internal void SendGamePickBox(Living player, int index, int arkType, string goods)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id);
            pkg.WriteByte(0x31);
            pkg.WriteByte((byte) index);
            pkg.WriteByte((byte) arkType);
            pkg.WriteString(goods);
            this.SendToAll(pkg);
        }

        internal void SendGamePlayerProperty(Living living, string type, string state)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x29);
            pkg.WriteString(type);
            pkg.WriteString(state);
            this.SendToAll(pkg);
        }

        internal void SendGamePlayerTakeCard(Player player, int index, int templateID, int count)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x62);
            pkg.WriteBoolean(false);
            pkg.WriteByte((byte) index);
            pkg.WriteInt(templateID);
            pkg.WriteInt(count);
            pkg.WriteBoolean(false);
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateBall(Player player, bool Special)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(20);
            pkg.WriteBoolean(Special);
            pkg.WriteInt(player.CurrentBall.ID);
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateDander(TurnedLiving player)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(14);
            pkg.WriteInt(player.Dander);
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateFrozenState(Living player)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x21);
            pkg.WriteBoolean(player.IsFrost);
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateHealth(Living living, int type, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(11);
            pkg.WriteByte((byte) type);
            pkg.WriteInt(living.Blood);
            pkg.WriteInt(value);
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateHideState(Living player)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x23);
            pkg.WriteBoolean(player.IsHide);
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateNoHoleState(Living player)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x52);
            pkg.WriteBoolean(player.IsNoHole);
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateSealState(Living player)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x12);
            pkg.WriteBoolean(player.GetSealState());
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateShootCount(Player player)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x2e);
            pkg.WriteByte((byte) player.ShootCount);
            this.SendToAll(pkg);
        }

        internal void SendGameUpdateWind(float wind)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x26);
            int val = (int) (wind * 10f);
            pkg.WriteInt(val);
            pkg.WriteBoolean(val > 0);
            pkg.WriteByte(this.GetVane(val, 1));
            pkg.WriteByte(this.GetVane(val, 2));
            pkg.WriteByte(this.GetVane(val, 3));
            this.SendToAll(pkg);
        }

        internal void SendGameWindPic(byte windId, byte[] windpic)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0xf1);
            pkg.WriteByte(windId);
            pkg.Write(windpic);
            this.SendToAll(pkg);
        }

        internal void SendIsLastMission(bool isLast)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(160);
            pkg.WriteBoolean(isLast);
            this.SendToAll(pkg);
        }

        internal void SendLivingBeat(Living living, Living target, int totalDemageAmount, string action, int livingCount, int attackEffect)
        {
            int val = 0;
            if (target is Player)
            {
                Player player = target as Player;
                val = player.Dander;
            }
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x3a);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");
            pkg.WriteInt(livingCount);
            for (int i = 1; i <= livingCount; i++)
            {
                pkg.WriteInt(target.Id);
                pkg.WriteInt(totalDemageAmount);
                pkg.WriteInt(target.Blood);
                pkg.WriteInt(val);
                pkg.WriteInt(attackEffect);
            }
            this.SendToAll(pkg);
        }

        internal void SendLivingBoltmove(Player player, int x, int y)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x48);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            this.SendToAll(pkg);
        }

        internal void SendLivingFall(Living living, int toX, int toY, int speed, string action, int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x38);
            pkg.WriteInt(toX);
            pkg.WriteInt(toY);
            pkg.WriteInt(speed);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");
            pkg.WriteInt(type);
            this.SendToAll(pkg);
        }

        internal void SendLivingJump(Living living, int toX, int toY, int speed, string action, int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x39);
            pkg.WriteInt(toX);
            pkg.WriteInt(toY);
            pkg.WriteInt(speed);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");
            pkg.WriteInt(type);
            this.SendToAll(pkg);
        }

        internal void SendLivingMoveTo(Living living, int fromX, int fromY, int toX, int toY, string action, int speed)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x37);
            pkg.WriteInt(fromX);
            pkg.WriteInt(fromY);
            pkg.WriteInt(toX);
            pkg.WriteInt(toY);
            pkg.WriteInt(speed);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");
            pkg.WriteString("");
            this.SendToAll(pkg);
        }

        internal void SendLivingMoveTo(Living living, int fromX, int fromY, int toX, int toY, string action, int speed, string sAction)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x37);
            pkg.WriteInt(fromX);
            pkg.WriteInt(fromY);
            pkg.WriteInt(toX);
            pkg.WriteInt(toY);
            pkg.WriteInt(speed);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");
            pkg.WriteString(sAction);
            this.SendToAll(pkg);
        }

        internal void SendLivingPlayMovie(Living living, string action)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(60);
            pkg.WriteString(action);
            this.SendToAll(pkg);
        }

        internal void SendLivingSay(Living living, string msg, int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, living.Id) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x3b);
            pkg.WriteString(msg);
            pkg.WriteInt(type);
            this.SendToAll(pkg);
        }

        internal void SendLivingShowBlood(Player player, int isShow)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id);
            pkg.WriteByte(80);
            pkg.WriteInt(player.Id);
            pkg.WriteInt(isShow);
            this.SendToAll(pkg);
        }

        internal void SendLivingShowBlood(Player player, long blood, int x, int y)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id);
            pkg.WriteByte(0x27);
            pkg.WriteInt(player.Id);
            pkg.WriteLong(blood);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            this.SendToAll(pkg);
        }

        internal void SendLivingTurnRotation(Player player, int rotation, int speed, string endPlay)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x55);
            pkg.WriteInt(rotation);
            pkg.WriteInt(speed);
            pkg.WriteString(endPlay);
            this.SendToAll(pkg);
        }

        internal void SendLivingUpdateAngryState(Living living)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x76);
            pkg.WriteInt(living.State);
            this.SendToAll(pkg);
        }

        internal void SendLivingUpdateDirection(Living living)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(7);
            pkg.WriteInt(living.Direction);
            this.SendToAll(pkg);
        }

        internal void SendMessage(IGamePlayer player, string msg, string msg1, int type)
        {
            if (msg != null)
            {
                GSPacketIn pkg = new GSPacketIn(3);
                pkg.WriteInt(type);
                pkg.WriteString(msg);
                player.SendTCP(pkg);
            }
            if (msg1 != null)
            {
                GSPacketIn in2 = new GSPacketIn(3);
                in2.WriteInt(type);
                in2.WriteString(msg1);
                this.SendToAll(in2, player);
            }
        }

        internal void SendMissionTryAgain(int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x77);
            pkg.WriteInt(type);
            this.SendToAll(pkg);
        }

        internal void SendOpenSelectLeaderWindow(int maxTime)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x66);
            pkg.WriteInt(maxTime);
            this.SendToAll(pkg);
        }

        internal void SendPetBuff(Living player, PetSkillElementInfo info, bool isActive)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x91);
            pkg.WriteInt(info.ID);
            pkg.WriteString("");
            pkg.WriteString("");
            pkg.WriteString(info.Pic.ToString());
            pkg.WriteString(info.EffectPic);
            pkg.WriteBoolean(isActive);
            this.SendToAll(pkg);
        }

        internal void SendPetSkillCd(Living player, int skillInfoID, int ColdDown)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x93);
            pkg.WriteInt(skillInfoID);
            pkg.WriteInt(ColdDown);
            (player as Player).PlayerDetail.SendTCP(pkg);
        }

        internal void SendPetUseKill(Player player)
        {
            this.SendPetUseKill(player, player.PetEffects.CurrentUseSkill, player.PetEffects.IsPetUseSkill);
        }

        internal void SendPetUseKill(Player player, int killId, bool isUse)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x90);
            pkg.WriteInt(killId);
            pkg.WriteBoolean(isUse);
            this.SendToAll(pkg);
        }

        internal void SendPlayerMove(Player player, int type, int x, int y, byte dir)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(9);
            pkg.WriteByte((byte) type);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            pkg.WriteByte(dir);
            pkg.WriteBoolean(player.IsLiving);
            if (type == 2)
            {
                pkg.WriteInt(this.m_tempBox.Count);
                foreach (Box box in this.m_tempBox)
                {
                    pkg.WriteInt(box.X);
                    pkg.WriteInt(box.Y);
                }
            }
            this.SendToAll(pkg, player.PlayerDetail);
        }

        internal void SendPlayerMove(Player player, int type, int x, int y, byte dir, bool isLiving)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(9);
            pkg.WriteByte((byte) type);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            pkg.WriteByte(dir);
            pkg.WriteBoolean(isLiving);
            if (type == 2)
            {
                pkg.WriteInt(this.m_tempBox.Count);
                foreach (Box box in this.m_tempBox)
                {
                    pkg.WriteInt(box.X);
                    pkg.WriteInt(box.Y);
                }
            }
            this.SendToAll(pkg);
        }

        internal void SendPlayerMove(Player player, int type, int x, int y, byte dir, bool isLiving, string action)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(9);
            pkg.WriteByte((byte) type);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            pkg.WriteByte(dir);
            pkg.WriteBoolean(isLiving);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "move");
            this.SendToAll(pkg);
        }

        internal void SendPlayerMove2(Player player, int type, int x, int y, byte dir, bool isLiving)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(9);
            pkg.WriteByte((byte) type);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            pkg.WriteByte(dir);
            pkg.WriteBoolean(isLiving);
            if (type == 2)
            {
                pkg.WriteInt(this.m_tempBox.Count);
                foreach (Box box in this.m_tempBox)
                {
                    pkg.WriteInt(box.X);
                    pkg.WriteInt(box.Y);
                }
            }
            this.SendToAll(pkg);
        }

        internal void SendPlayerPicture(Living living, int type, bool state)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = living.Id
            };
            pkg.WriteByte(0x80);
            pkg.WriteInt(type);
            pkg.WriteBoolean(state);
            this.SendToAll(pkg);
        }

        internal void SendPlayerRemove(Player player)
        {
            GSPacketIn pkg = new GSPacketIn(0x5e, player.PlayerDetail.PlayerCharacter.ID);
            pkg.WriteByte(5);
            pkg.WriteInt(player.PlayerDetail.ZoneId);
            this.SendToAll(pkg);
        }

        internal void SendPlayerUseProp(Player player, int type, int place, int templateID)
        {
            this.SendPlayerUseProp(player, type, place, templateID, player);
        }

        internal void SendPlayerUseProp(Living player, int type, int place, int templateID, Player p)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x20);
            pkg.WriteByte((byte) type);
            pkg.WriteInt(place);
            pkg.WriteInt(templateID);
            pkg.WriteInt(p.Id);
            pkg.WriteBoolean(templateID == 0x2721);
            this.SendToAll(pkg);
        }

        internal void SendPhysicalObjFocus(Physics obj, int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x3e);
            pkg.WriteInt(type);
            pkg.WriteInt(obj.X);
            pkg.WriteInt(obj.Y);
            this.SendToAll(pkg);
        }

        internal void SendPhysicalObjPlayAction(PhysicalObj obj)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x42);
            pkg.WriteInt(obj.Id);
            pkg.WriteString(obj.CurrentAction);
            this.SendToAll(pkg);
        }

        internal void SendRemoveLiving(int id)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x35);
            pkg.WriteInt(id);
            this.SendToAll(pkg);
        }

        internal void SendRemovePhysicalObj(PhysicalObj obj)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x35);
            pkg.WriteInt(obj.Id);
            this.SendToAll(pkg);
        }

        internal void SendSelectObject(int playerId)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, playerId) {
                Parameter1 = playerId
            };
            pkg.WriteByte(0x8a);
            int count = this.Players.Count;
            pkg.WriteInt(count);
            for (int i = 0; i < count; i++)
            {
                pkg.WriteInt(this.Players[i].PlayerDetail.PlayerCharacter.ID);
                pkg.WriteInt(this.Players[i].PlayerDetail.ZoneId);
                pkg.WriteInt(this.Players[i].Team);
                pkg.WriteInt(this.Players[i].Id);
                pkg.WriteInt(this.Players[i].PlayerDetail.ZoneId);
            }
            this.SendToAll(pkg);
        }

        internal void SendSigleNextTurn(Living p, BaseGame game, List<Box> newBoxes)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, p.Id) {
                Parameter1 = p.Id
            };
            pkg.WriteByte(6);
            int wind = (int) (this.m_map.wind * 10f);
            pkg.WriteBoolean(wind > 0);
            pkg.WriteByte(this.GetVane(wind, 1));
            pkg.WriteByte(this.GetVane(wind, 2));
            pkg.WriteByte(this.GetVane(wind, 3));
            pkg.WriteBoolean(p.IsHide);
            pkg.WriteInt(this.getTurnTime());
            pkg.WriteInt(newBoxes.Count);
            foreach (Box box in newBoxes)
            {
                pkg.WriteInt(box.Id);
                pkg.WriteInt(box.X);
                pkg.WriteInt(box.Y);
                pkg.WriteInt(box.Type);
            }
            pkg.WriteInt(1);
            pkg.WriteInt(p.Id);
            pkg.WriteBoolean(p.IsLiving);
            pkg.WriteInt(p.X);
            pkg.WriteInt(p.Y);
            pkg.WriteInt(p.Blood);
            pkg.WriteBoolean(p.IsNoHole);
            pkg.WriteInt(((Player) p).Energy);
            pkg.WriteInt(((Player) p).psychic);
            pkg.WriteInt(((Player) p).Dander);
            if (((Player) p).Pet == null)
            {
                pkg.WriteInt(0);
                pkg.WriteInt(0);
            }
            else
            {
                pkg.WriteInt(((Player) p).PetMaxMP);
                pkg.WriteInt(((Player) p).PetMP);
            }
            pkg.WriteInt(((Player) p).ShootCount);
            pkg.WriteInt(((Player) p).flyCount);
            pkg.WriteInt(game.TurnIndex);
            pkg.WriteBoolean(false);
            ((Player) p).PlayerDetail.SendTCP(pkg);
        }

        internal void SendSkipNext(Player player)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(12);
            player.PlayerDetail.SendTCP(pkg);
        }

        internal void SendStartLoading(int maxTime)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x67);
            pkg.WriteInt(maxTime);
            pkg.WriteInt(this.m_map.Info.ID);
            pkg.WriteInt(this.m_loadingFiles.Count);
            foreach (LoadingFileInfo info in this.m_loadingFiles)
            {
                pkg.WriteInt(info.Type);
                pkg.WriteString(info.Path);
                pkg.WriteString(info.ClassName);
            }
            this.SendToAll(pkg);
        }

        internal void SendSyncLifeTime()
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(0x83);
            pkg.WriteInt(this.m_lifeTime);
            this.SendToAll(pkg);
        }

        public virtual void SendToAll(GSPacketIn pkg)
        {
            this.SendToAll(pkg, null);
        }

        public virtual void SendToAll(GSPacketIn pkg, IGamePlayer except)
        {
            if (pkg.Parameter2 == 0)
            {
                pkg.Parameter2 = this.LifeTime;
            }
            List<Player> allFightPlayers = this.GetAllFightPlayers();
            foreach (Player player in allFightPlayers)
            {
                if (player.IsActive && (player.PlayerDetail != except))
                {
                    player.PlayerDetail.SendTCP(pkg);
                }
            }
        }

        public virtual void SendToTeam(GSPacketIn pkg, int team)
        {
            this.SendToTeam(pkg, team, null);
        }

        public virtual void SendToTeam(GSPacketIn pkg, int team, IGamePlayer except)
        {
            List<Player> allFightPlayers = this.GetAllFightPlayers();
            foreach (Player player in allFightPlayers)
            {
                if ((player.IsActive && (player.PlayerDetail != except)) && (player.Team == team))
                {
                    player.PlayerDetail.SendTCP(pkg);
                }
            }
        }

        internal void SendUseDeputyWeapon(Player player, int ResCount)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, player.Id) {
                Parameter1 = player.Id
            };
            pkg.WriteByte(0x54);
            pkg.WriteInt(ResCount);
            player.PlayerDetail.SendTCP(pkg);
        }

        public bool SetMap(int mapId)
        {
            if (this.GameState != eGameState.Playing)
            {
                Game.Logic.Phy.Maps.Map map = MapMgr.CloneMap(mapId);
                if (map != null)
                {
                    this.m_map = map;
                    return true;
                }
            }
            return false;
        }

        public void SetWind(int wind)
        {
            this.m_map.wind = wind;
        }

        public void Shuffer<T>(T[] array)
        {
            for (int i = array.Length; i > 1; i--)
            {
                int index = this.Random.Next(i);
                T local = array[index];
                array[index] = array[i - 1];
                array[i - 1] = local;
            }
        }

        public virtual bool TakeCard(Player player)
        {
            return false;
        }

        public virtual bool TakeCard(Player player, int index)
        {
            return false;
        }

        public Player Timnguoichoibikobinhot()
        {
            List<Player> source = new List<Player>();
            foreach (Player player in this.m_players.Values)
            {
                if (!(!player.IsLiving || player.BiKhoa))
                {
                    source.Add(player);
                }
            }
            int index = this.Random.Next(0, source.Count);
            if (source.Count > 0)
            {
                return source.ElementAt<Player>(index);
            }
            return null;
        }

        public Player Timnguoichoibinhot()
        {
            List<Player> source = new List<Player>();
            foreach (Player player in this.m_players.Values)
            {
                if (player.IsLiving && player.BiKhoa)
                {
                    source.Add(player);
                }
            }
            int index = this.Random.Next(0, source.Count);
            if (source.Count > 0)
            {
                return source.ElementAt<Player>(index);
            }
            return null;
        }

        public Player Timnguoichoigannhat()
        {
            List<Player> list = new List<Player>();
            List<int> list2 = new List<int>();
            foreach (Player player in this.m_players.Values)
            {
                if (player.IsLiving)
                {
                    list.Add(player);
                    list2.Add(player.X);
                }
            }
            int num = ((IEnumerable<int>) list2).Max();
            foreach (Player player2 in list)
            {
                if (player2.X == num)
                {
                    return player2;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("Id:{0},player:{1},state:{2},current:{3},turnIndex:{4},actions:{5}", new object[] { base.Id, this.PlayerCount, this.GameState, this.CurrentLiving, this.m_turnIndex, this.m_actions.Count });
        }

        public void Update(long tick)
        {
            if (this.m_passTick < tick)
            {
                ArrayList list2;
                this.m_lifeTime++;
                lock (this.m_actions)
                {
                    list2 = (ArrayList) this.m_actions.Clone();
                    this.m_actions.Clear();
                }
                if ((list2 != null) && (this.GameState != eGameState.Stopped))
                {
                    this.CurrentActionCount = list2.Count;
                    if (list2.Count > 0)
                    {
                        ArrayList actions = new ArrayList();
                        foreach (IAction action in list2)
                        {
                            try
                            {
                                action.Execute(this, tick);
                                if (!action.IsFinished(tick))
                                {
                                    actions.Add(action);
                                }
                            }
                            catch (Exception exception)
                            {
                                log.Error("Map update error:", exception);
                            }
                        }
                        this.AddAction(actions);
                    }
                    else if (this.m_waitTimer < tick)
                    {
                        this.CheckState(0);
                    }
                }
            }
        }

        public void UpdateWind(float wind, bool sendToClient)
        {
            if (this.m_map.wind != wind)
            {
                this.m_map.wind = wind;
                if (sendToClient)
                {
                    this.SendGameUpdateWind(wind);
                }
            }
        }

        public void VaneLoading()
        {
            List<WindInfo> wind = WindMgr.GetWind();
            foreach (WindInfo info in wind)
            {
                this.SendGameWindPic((byte) info.WindID, info.WindPic);
            }
        }

        public void WaitTime(int delay)
        {
            this.m_waitTimer = Math.Max(this.m_waitTimer, TickHelper.GetTickCount() + delay);
            this.long_1 = this.m_waitTimer;
        }

        public TurnedLiving CurrentLiving
        {
            get
            {
                return this.m_currentLiving;
            }
        }

        public eGameState GameState
        {
            get
            {
                return this.m_gameState;
            }
        }

        public bool GetBlood
        {
            get
            {
                return this.m_GetBlood;
            }
            set
            {
                this.m_GetBlood = value;
            }
        }

        public bool HasPlayer
        {
            get
            {
                return (this.m_players.Count > 0);
            }
        }

        public int LifeTime
        {
            get
            {
                return this.m_lifeTime;
            }
        }

        protected int m_turnIndex
        {
            get
            {
                return this.turnIndex;
            }
            set
            {
                this.turnIndex = value;
            }
        }

        public Game.Logic.Phy.Maps.Map Map
        {
            get
            {
                return this.m_map;
            }
        }

        public int nextPlayerId
        {
            get
            {
                return this.m_nextPlayerId;
            }
            set
            {
                this.m_nextPlayerId = value;
            }
        }

        public int PlayerCount
        {
            get
            {
                lock (this.m_players)
                {
                    return this.m_players.Count;
                }
            }
        }

        public Dictionary<int, Player> Players
        {
            get
            {
                return this.m_players;
            }
        }

        public System.Random Random
        {
            get
            {
                return this.m_random;
            }
        }

        public int RoomId
        {
            get
            {
                return this.m_roomId;
            }
        }

        public int TurnIndex
        {
            get
            {
                return this.m_turnIndex;
            }
            set
            {
                this.m_turnIndex = value;
            }
        }

        public List<TurnedLiving> TurnQueue
        {
            get
            {
                return this.m_turnQueue;
            }
        }

        public float Wind
        {
            get
            {
                return this.m_map.wind;
            }
        }

        public delegate void GameNpcDieEventHandle(int NpcId);

        public delegate void GameOverLogEventHandle(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar);

        internal void SendLivingWalkTo(Living m_living, int p1, int p2, int p3, int p4, string m_action, int m_speed)
        {
            throw new NotImplementedException();
        }
    }
}


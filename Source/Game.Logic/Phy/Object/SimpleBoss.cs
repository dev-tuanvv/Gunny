namespace Game.Logic.Phy.Object
{
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.AI;
    using Game.Logic.AI.Npc;
    using Game.Server.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;

    public class SimpleBoss : TurnedLiving
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ABrain m_ai;
        private bool m_BallType;
        private List<SimpleBoss> m_boss;
        private List<SimpleNpc> m_child;
        private List<SimpleNpc> m_fire;
        private Dictionary<Player, int> m_mostHateful;
        private SqlDataProvider.Data.NpcInfo m_npcInfo;
        public int TotalCure;

        public SimpleBoss(int id, BaseGame game, SqlDataProvider.Data.NpcInfo npcInfo, int direction, int type) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            this.m_child = new List<SimpleNpc>();
            this.m_boss = new List<SimpleBoss>();
            this.m_fire = new List<SimpleNpc>();
            if (type == 0)
            {
                base.Type = eLivingType.SimpleBoss;
            }
            else
            {
                base.Type = eLivingType.SimpleBoss1;
            }
            this.m_mostHateful = new Dictionary<Player, int>();
            this.m_npcInfo = npcInfo;
            this.m_ai = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
            if (this.m_ai == null)
            {
                log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
                this.m_ai = SimpleBrain.Simple;
            }
            this.m_ai.Game = base.m_game;
            this.m_ai.Body = this;
            try
            {
                this.m_ai.OnCreated();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss Created error:{1}", exception);
            }
        }

        public SimpleBoss(int id, BaseGame game, SqlDataProvider.Data.NpcInfo npcInfo, int direction, int type, string actions) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            this.m_child = new List<SimpleNpc>();
            this.m_boss = new List<SimpleBoss>();
            this.m_fire = new List<SimpleNpc>();
            switch (type)
            {
                case 0:
                    base.Type = eLivingType.SimpleBoss;
                    break;

                case 1:
                    base.Type = eLivingType.ClearEnemy;
                    break;

                default:
                    base.Type = (eLivingType) type;
                    break;
            }
            base.ActionStr = actions;
            this.m_mostHateful = new Dictionary<Player, int>();
            this.m_npcInfo = npcInfo;
            this.m_ai = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
            if (this.m_ai == null)
            {
                log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
                this.m_ai = SimpleBrain.Simple;
            }
            this.m_ai.Game = base.m_game;
            this.m_ai.Body = this;
            try
            {
                this.m_ai.OnCreated();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss Created error:{1}", exception);
            }
        }

        public SimpleNpc CreateBoss(int id, int x, int y, int direction, int type)
        {
            SimpleNpc item = ((PVEGame) base.Game).CreateNpc(id, x, y, type, direction);
            this.Child.Add(item);
            return item;
        }

        public void CreateBoss(int id, int x, int y, int direction, int disToSecond, int maxCount)
        {
            if (this.CurrentLivingBossNum < maxCount)
            {
                if ((maxCount - this.CurrentLivingNpcNum) >= 2)
                {
                    this.Boss.Add(((PVEGame) base.Game).CreateBoss(id, x + disToSecond, y, direction, 0));
                    this.Boss.Add(((PVEGame) base.Game).CreateBoss(id, x, y, direction, 0));
                }
                else if ((maxCount - this.CurrentLivingBossNum) == 1)
                {
                    this.Boss.Add(((PVEGame) base.Game).CreateBoss(id, x, y, direction, 0));
                }
            }
        }

        public void CreateBoss(int id, int x, int y, int direction, int disToSecond, int maxCount, string action)
        {
            this.CreateBoss(id, x, y, direction, 1, disToSecond, maxCount, action);
        }

        public void CreateBoss(int id, int x, int y, int direction, int type, int disToSecond, int maxCount, string action)
        {
            if (this.CurrentLivingBossNum < maxCount)
            {
                if ((maxCount - this.CurrentLivingNpcNum) >= 2)
                {
                    this.Boss.Add(((PVEGame) base.Game).CreateBoss(id, x + disToSecond, y, direction, type, action));
                    this.Boss.Add(((PVEGame) base.Game).CreateBoss(id, x, y, direction, type, action));
                }
                else if ((maxCount - this.CurrentLivingBossNum) == 1)
                {
                    this.Boss.Add(((PVEGame) base.Game).CreateBoss(id, x, y, direction, type, action));
                }
            }
        }

        public SimpleNpc CreateChild(int id, int x, int y, bool showBlood, LivingConfig config)
        {
            return this.CreateChild(id, x, y, 1, -1, showBlood, config);
        }

        public void CreateChild(int id, int x, int y, int direction, int maxCount)
        {
            if (this.CurrentLivingNpcNum < maxCount)
            {
                if ((maxCount - this.CurrentLivingNpcNum) >= 2)
                {
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, direction, 1));
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, direction, 1));
                }
                else if ((maxCount - this.CurrentLivingNpcNum) == 1)
                {
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, direction, 1));
                }
            }
        }

        public SimpleNpc CreateChild(int id, int x, int y, int dir, bool showBlood, LivingConfig config)
        {
            return this.CreateChild(id, x, y, 1, dir, showBlood, config);
        }

        public void CreateChild(int id, int x, int y, int disToSecond, int maxCount, int direction)
        {
            if (this.CurrentLivingNpcNum < maxCount)
            {
                if ((maxCount - this.CurrentLivingNpcNum) >= 2)
                {
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x + disToSecond, y, 1, direction));
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, 1, direction));
                }
                else if ((maxCount - this.CurrentLivingNpcNum) == 1)
                {
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, 1, direction));
                }
            }
        }

        public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type, int direction)
        {
            int num = base.Game.Random.Next(0, maxCountForOnce);
            for (int i = 0; i < num; i++)
            {
                int index = base.Game.Random.Next(0, brithPoint.Length);
                this.CreateChild(id, brithPoint[index].X, brithPoint[index].Y, 4, maxCount, direction);
            }
        }

        public SimpleNpc CreateChild(int id, int x, int y, int type, int dir, bool showBlood, LivingConfig config)
        {
            SimpleNpc item = ((PVEGame) base.Game).CreateNpc(id, x, y, type, dir, config);
            this.Child.Add(item);
            if (!showBlood)
            {
                base.Game.PedSuikAov(item, 0);
            }
            return item;
        }

        public void CreateFire(int id, int x, int y, int disToSecond, int maxCount)
        {
            if (this.CurrentLivingFireNum < maxCount)
            {
                KhoiTaoKhaNangConfig config = ((PVEGame) base.Game).BaseLivingConfig(9);
                config.IsBay = true;
                if ((maxCount - this.CurrentLivingFireNum) >= 2)
                {
                    this.Fire.Add(((PVEGame) base.Game).CreateNpc(id, x + disToSecond, y, 1, config));
                    this.Fire.Add(((PVEGame) base.Game).CreateNpc(id, x, y, 1, config));
                }
                else if ((maxCount - this.CurrentLivingFireNum) == 1)
                {
                    this.Fire.Add(((PVEGame) base.Game).CreateNpc(id, x, y, 1, config));
                }
            }
        }

        public void CreatePhaLe(int id, int x, int y, int direction, int disToSecond, int maxCount)
        {
            this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, direction, 1));
        }

        public override void Die()
        {
            base.Die();
        }

        public override void Die(int delay)
        {
            base.Die(delay);
        }

        public override void Dispose()
        {
            base.Dispose();
            try
            {
                this.m_ai.Dispose();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss Dispose error:{1}", exception);
            }
        }

        public List<SimpleNpc> FindChildLiving(int npcId)
        {
            List<SimpleNpc> list = new List<SimpleNpc>();
            foreach (SimpleNpc npc in this.m_child)
            {
                if (((npc != null) && npc.IsLiving) && (npc.NpcInfo.ID == npcId))
                {
                    list.Add(npc);
                }
            }
            return list;
        }

        public Player FindMostHatefulPlayer()
        {
            if (this.m_mostHateful.Count > 0)
            {
                KeyValuePair<Player, int> pair = this.m_mostHateful.ElementAt<KeyValuePair<Player, int>>(0);
                foreach (KeyValuePair<Player, int> pair2 in this.m_mostHateful)
                {
                    if (pair.Value < pair2.Value)
                    {
                        pair = pair2;
                    }
                }
                return pair.Key;
            }
            return null;
        }

        public override void OnAfterTakedBomb()
        {
            try
            {
                this.m_ai.OnAfterTakedBomb();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss OnAfterTakedBomb error:{1}", exception);
            }
        }

        public override void OnAfterTakedFrozen()
        {
            try
            {
                this.m_ai.OnAfterTakedFrozen();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss OnAfterTakedFrozen error:{1}", exception);
            }
        }

        public override void PrepareNewTurn()
        {
            base.PrepareNewTurn();
            try
            {
                this.m_ai.OnBeginNewTurn();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss BeginNewTurn error:{1}", exception);
            }
        }

        public override void PrepareSelfTurn()
        {
            base.PrepareSelfTurn();
            base.DefaultDelay = base.m_delay;
            base.AddDelay(this.m_npcInfo.Delay);
            try
            {
                this.m_ai.OnBeginSelfTurn();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss BeginSelfTurn error:{1}", exception);
            }
        }

        public void RandomSay(string[] msg, int type, int delay, int finishTime)
        {
            if (base.Game.Random.Next(0, 2) == 1)
            {
                int index = base.Game.Random.Next(0, msg.Count<string>());
                string str = msg[index];
                base.m_game.AddAction(new LivingSayAction(this, str, type, delay, finishTime));
            }
        }

        public void RemoveAllChild()
        {
            foreach (SimpleNpc npc in this.Child)
            {
                if (npc.IsLiving)
                {
                    npc.Die();
                }
            }
            this.m_child = new List<SimpleNpc>();
        }

        public override void Reset()
        {
            if (base.Config.IsWorldBoss)
            {
                base.m_maxBlood = 0x7fffffff;
            }
            else
            {
                base.m_maxBlood = this.m_npcInfo.Blood;
            }
            base.BaseDamage = this.m_npcInfo.BaseDamage;
            base.BaseGuard = this.m_npcInfo.BaseGuard;
            base.Attack = this.m_npcInfo.Attack;
            base.Defence = this.m_npcInfo.Defence;
            base.Agility = this.m_npcInfo.Agility;
            base.Lucky = this.m_npcInfo.Lucky;
            base.Grade = this.m_npcInfo.Level;
            base.Experience = this.m_npcInfo.Experience;
            base.m_delay = this.m_npcInfo.Agility;
            base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
            base.SetRelateDemagemRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
            base.Reset();
        }

        public override void StartAttacking()
        {
            base.StartAttacking();
            try
            {
                this.m_ai.OnStartAttacking();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss StartAttacking error:{1}", exception);
            }
            if (base.IsAttacking)
            {
                this.StopAttacking();
            }
        }

        public override void StopAttacking()
        {
            base.StopAttacking();
            try
            {
                this.m_ai.OnStopAttacking();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss StopAttacking error:{1}", exception);
            }
        }

        public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            bool flag = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
            if (source is Player)
            {
                Player key = source as Player;
                int num = damageAmount + criticalAmount;
                if (this.m_mostHateful.ContainsKey(key))
                {
                    this.m_mostHateful[key] += num;
                    return flag;
                }
                this.m_mostHateful.Add(key, num);
            }
            return flag;
        }

        public bool BallType
        {
            get
            {
                return this.m_BallType;
            }
            set
            {
                this.m_BallType = value;
            }
        }

        public List<SimpleBoss> Boss
        {
            get
            {
                return this.m_boss;
            }
        }

        public int CurrentLivingBossNum
        {
            get
            {
                int num = 0;
                foreach (SimpleBoss boss in this.Boss)
                {
                    if (!boss.IsLiving)
                    {
                        num++;
                    }
                }
                return (this.Boss.Count - num);
            }
        }

        public int CurrentLivingFireNum
        {
            get
            {
                int num = 0;
                foreach (SimpleNpc npc in this.Fire)
                {
                    if (!npc.IsLiving)
                    {
                        num++;
                    }
                }
                return (this.Fire.Count - num);
            }
        }

        public int CurrentLivingNpcNum
        {
            get
            {
                int num = 0;
                foreach (SimpleNpc npc in this.Child)
                {
                    if (!npc.IsLiving)
                    {
                        num++;
                    }
                }
                return (this.Child.Count - num);
            }
        }

        public List<SimpleNpc> Child
        {
            get
            {
                return this.m_child;
            }
        }

        public List<SimpleNpc> Fire
        {
            get
            {
                return this.m_fire;
            }
        }

        public SqlDataProvider.Data.NpcInfo NpcInfo
        {
            get
            {
                return this.m_npcInfo;
            }
        }
    }
}


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

    public class SimpleMyNpc : TurnedLiving
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ABrain m_ai;
        private List<SimpleBoss> m_boss;
        private List<SimpleNpc> m_child;
        private Dictionary<Player, int> m_mostHateful;
        private SqlDataProvider.Data.NpcInfo m_npcInfo;
        public int TotalCure;

        public SimpleMyNpc(int id, BaseGame game, SqlDataProvider.Data.NpcInfo npcInfo, int direction, int type) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            this.m_child = new List<SimpleNpc>();
            this.m_boss = new List<SimpleBoss>();
            if (type == 0)
            {
                base.Type = eLivingType.SimpleMyNpc;
            }
            else
            {
                base.Type = eLivingType.SimpleMyNpc1;
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
            try
            {
                this.m_ai.OnCreated();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleBoss Created error:{1}", exception);
            }
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

        public void CreateChild(int id, int x, int y, int direction, int disToSecond, int maxCount)
        {
            if (this.CurrentLivingNpcNum < maxCount)
            {
                if ((maxCount - this.CurrentLivingNpcNum) >= 2)
                {
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x + disToSecond, y, direction, 1));
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, direction, 1));
                }
                else if ((maxCount - this.CurrentLivingNpcNum) == 1)
                {
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, direction, 1));
                }
            }
        }

        public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int direction, int type)
        {
            int index = 0;
            int num2 = base.Game.Random.Next(0, maxCountForOnce);
            for (int i = 0; i < num2; i++)
            {
                index = base.Game.Random.Next(0, brithPoint.Length);
                this.CreateChild(id, brithPoint[index].X, brithPoint[index].Y, 4, direction, maxCount);
            }
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
            string[] source = msg;
            string str = null;
            if (base.Game.Random.Next(0, 2) == 1)
            {
                int index = base.Game.Random.Next(0, source.Count<string>());
                str = source[index];
                base.m_game.AddAction(new LivingSayAction(this, str, type, delay, finishTime));
            }
        }

        public override void Reset()
        {
            base.m_maxBlood = this.m_npcInfo.Blood;
            base.BaseDamage = this.m_npcInfo.BaseDamage;
            base.BaseGuard = this.m_npcInfo.BaseGuard;
            this.TotalCure = 0;
            base.Attack = this.m_npcInfo.Attack;
            base.Defence = this.m_npcInfo.Defence;
            base.Agility = this.m_npcInfo.Agility;
            base.Lucky = this.m_npcInfo.Lucky;
            base.Grade = this.m_npcInfo.Level;
            base.Experience = this.m_npcInfo.Experience;
            base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
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

        public SqlDataProvider.Data.NpcInfo NpcInfo
        {
            get
            {
                return this.m_npcInfo;
            }
        }
    }
}


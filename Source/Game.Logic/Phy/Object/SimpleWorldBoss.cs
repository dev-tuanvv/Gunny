using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using Bussiness;

namespace Game.Logic.Phy.Object
{
    public class SimpleWorldBoss : TurnedLiving
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private NpcInfo m_npcInfo;
        private ABrain m_ai;
        private Player target;
        private List<SimpleNpc> m_child = new List<SimpleNpc>();
        private List<SimpleFireHell> m_fire = new List<SimpleFireHell>();
        private Dictionary<Player, int> m_mostHateful;

        public NpcInfo NpcInfo
        {
            get
            {
                return this.m_npcInfo;
            }
        }

        public List<SimpleNpc> Child
        {
            get
            {
                return this.m_child;
            }
        }

        public List<SimpleFireHell> Child2
        {
            get
            {
                return this.m_fire;
            }
        }

        public int CurrentLivingNpcNum
        {
            get
            {
                int count = 0;
                foreach (SimpleNpc child in this.Child)
                {
                    if (!child.IsLiving)
                    {
                        count++;
                    }
                }
                return this.Child.Count - count;
            }
        }

        public SimpleWorldBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type, string actions)
            : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            if (type == 0)
            {
                Type = eLivingType.SimpleBossSpecial;
            }
            else
            {
                Type = eLivingType.SimpleBossHard;
            }
            base.ActionStr = actions;
            this.m_mostHateful = new Dictionary<Player, int>();
            this.m_npcInfo = npcInfo;
            this.m_ai = (ScriptMgr.CreateInstance(npcInfo.Script) as ABrain);
            if (this.m_ai == null)
            {
                SimpleWorldBoss.log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
                this.m_ai = SimpleBrain.Simple;
            }
            this.m_ai.Game = this.m_game;
            this.m_ai.Body = this;
            try
            {
                this.m_ai.OnCreated();
            }
            catch (Exception ex)
            {
                SimpleWorldBoss.log.ErrorFormat("SimpleWorldBoss Created error:{1}", ex);
            }
        }

        public override void Reset()
        {
            base.m_maxBlood = this.m_npcInfo.Blood;            
            this.BaseDamage = (double)this.m_npcInfo.BaseDamage;
            this.BaseGuard = (double)this.m_npcInfo.BaseGuard;
            this.Attack = (double)this.m_npcInfo.Attack;
            this.Defence = (double)this.m_npcInfo.Defence;
            this.Agility = (double)this.m_npcInfo.Agility;
            this.Lucky = (double)this.m_npcInfo.Lucky;
            this.Grade = this.m_npcInfo.Level;
            this.Experience = this.m_npcInfo.Experience;
            base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
            base.Reset();
        }

        public override void Die()
        {
            base.Die();
        }

        public override void Die(int delay)
        {
            base.Die(delay);
        }

        public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            bool result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
            if (source is Player)
            {
                Player p = source as Player;
                int damage = damageAmount + criticalAmount;
                if (this.m_mostHateful.ContainsKey(p))
                {
                    this.m_mostHateful[p] = this.m_mostHateful[p] + damage;
                }
                else
                {
                    this.m_mostHateful.Add(p, damage);
                }
            }
            return result;
        }

        protected int MakeCritical(Living name, int baseDamage)
        {
            double lucky = target.Lucky;

            //Random rd = new Random();
            bool canHit = lucky * 75 / (800 + lucky) > ThreadSafeRandom.NextStatic(100);
            if (canHit)
            {
                return (int)((0.5 + lucky * 0.0003) * baseDamage);
            }
            else
            {
                return 0;
            }
        }

        public Player FindMostHatefulPlayer()
        {
            Player result;
            if (this.m_mostHateful.Count > 0)
            {
                KeyValuePair<Player, int> i = this.m_mostHateful.ElementAt(0);
                foreach (KeyValuePair<Player, int> kvp in this.m_mostHateful)
                {
                    if (i.Value < kvp.Value)
                    {
                        i = kvp;
                    }
                }
                result = i.Key;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public void CreateChild(int id, int x, int y, int disToSecond, int maxCount)
        {
            if (this.CurrentLivingNpcNum < maxCount)
            {
                if (maxCount - this.CurrentLivingNpcNum >= 2)
                {
                    this.Child.Add(((PVEGame)base.Game).CreateNpc(id, x + disToSecond, y, 1));
                    this.Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, 1));
                }
                else
                {
                    if (maxCount - this.CurrentLivingNpcNum == 1)
                    {
                        this.Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, 1));
                    }
                }
            }
        }

        public void CreateChildFire(int id, int x, int y, int disToSecond, int maxCount)
        {
            //if (this.CurrentLivingNpcNum < maxCount)
            //{
            //    if (maxCount - this.CurrentLivingNpcNum >= 2)
            //    {
            //        Child2.Add(((PVEGame)base.Game).CreateFireHell(id, x + disToSecond, y, 1));
            //        Child2.Add(((PVEGame)base.Game).CreateFireHell(id, x, y, 1));
            //    }
            //    else
            //    {
            //        if (maxCount - this.CurrentLivingNpcNum == 1)
            //        {
            //            Child2.Add(((PVEGame)base.Game).CreateFireHell(id, x, y, 1));
            //        }
            //    }
            //}
        }

        public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type)
        {
            int length = base.Game.Random.Next(0, maxCountForOnce);
            for (int i = 0; i < length; i++)
            {
                int index = base.Game.Random.Next(0, brithPoint.Length);
                this.CreateChild(id, brithPoint[index].X, brithPoint[index].Y, 4, maxCount);
            }
        }

        public void RandomSay(string[] msg, int type, int delay, int finishTime)
        {
            int IsSay = base.Game.Random.Next(0, 2);
            if (IsSay == 1)
            {
                int index = base.Game.Random.Next(0, msg.Count<string>());
                string text = msg[index];
                this.m_game.AddAction(new LivingSayAction(this, text, type, delay, finishTime));
            }
        }

        public override void PrepareNewTurn()
        {
            base.PrepareNewTurn();
            try
            {
                this.m_ai.OnBeginNewTurn();
            }
            catch (Exception ex)
            {
                SimpleWorldBoss.log.ErrorFormat("SimpleWorldBoss BeginNewTurn error:{1}", ex);
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
            catch (Exception ex)
            {
                SimpleWorldBoss.log.ErrorFormat("SimpleWorldBoss BeginSelfTurn error:{1}", ex);
            }
        }

        public override void StartAttacking()
        {
            base.StartAttacking();
            try
            {
                this.m_ai.OnStartAttacking();
            }
            catch (Exception ex)
            {
                SimpleWorldBoss.log.ErrorFormat("SimpleWorldBoss StartAttacking error:{1}", ex);
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
            catch (Exception ex)
            {
                SimpleWorldBoss.log.ErrorFormat("SimpleWorldBoss StopAttacking error:{1}", ex);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            try
            {
                this.m_ai.Dispose();
            }
            catch (Exception ex)
            {
                SimpleWorldBoss.log.ErrorFormat("SimpleWorldBoss Dispose error:{1}", ex);
            }
        }
    }
}
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
    public class SimpleSearchBoss : Living
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private NpcInfo m_npcInfo;
        private ABrain m_ai;
        private List<SimpleNpc> m_child = new List<SimpleNpc>();
        private List<SimpleFireHell> m_fire = new List<SimpleFireHell>();
        public int TotalCure;
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

        public SimpleSearchBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type, string action)
            : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            if (type == 0)
            {
                base.Type = eLivingType.SimpleBossSpecial;
            }
            else
            {
                base.Type = eLivingType.SimpleBossHard;
            }
            ActionStr = action;
            this.m_npcInfo = npcInfo;
            this.m_mostHateful = new Dictionary<Player, int>();
            this.m_ai = (ScriptMgr.CreateInstance(npcInfo.Script) as ABrain);
            if (this.m_ai == null)
            {
                SimpleSearchBoss.log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
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
                SimpleSearchBoss.log.ErrorFormat("SimpleSearchBoss Created error:{1}", ex);
            }
        }

        public override void Reset()
        {
            this.Agility = (double)this.m_npcInfo.Agility;
            this.Attack = (double)this.m_npcInfo.Attack;
            this.BaseDamage = (double)this.m_npcInfo.BaseDamage;
            this.BaseGuard = (double)this.m_npcInfo.BaseGuard;
            this.Lucky = (double)this.m_npcInfo.Lucky;
            this.Grade = this.m_npcInfo.Level;
            this.Experience = this.m_npcInfo.Experience;
            base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
            base.Reset();
        }

        public void GetDropItemInfo()
        {
            if (this.m_game.CurrentLiving is Player)
            {
                Player p = this.m_game.CurrentLiving as Player;
                List<ItemInfo> infos = null;
                int gold = 0;
                int money = 0;
                int gifttoken = 0;
                int medal = 0;
                DropInventory.NPCDrop(this.m_npcInfo.DropId, ref infos);
                if (infos != null)
                {
                    foreach (ItemInfo info in infos)
                    {
                        ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref gifttoken);
                        if (info != null)
                        {
                            if (info.Template.CategoryID == 10)
                            {
                                //p.PlayerDetail.AddTemplate(info, eBageType.FightBag, info.Count);
                            }
                            else
                            {
                                //p.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count);
                            }
                        }
                    }
                    p.PlayerDetail.AddGold(gold);
                    p.PlayerDetail.AddMoney(money);
                    p.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_Drop, p.PlayerDetail.PlayerCharacter.ID, money, p.PlayerDetail.PlayerCharacter.Money);
                    p.PlayerDetail.AddGiftToken(gifttoken);
                }
            }
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

        public override void Die()
        {
            this.GetDropItemInfo();
            base.Die();
        }

        public override void Die(int delay)
        {
            this.GetDropItemInfo();
            base.Die(delay);
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
                SimpleSearchBoss.log.ErrorFormat("SimpleSearchBoss BeginNewTurn error:{1}", ex);
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
                SimpleSearchBoss.log.ErrorFormat("SimpleSearchBoss StartAttacking error:{1}", ex);
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
                SimpleSearchBoss.log.ErrorFormat("SimpleSearchBoss Dispose error:{1}", ex);
            }
        }
    }
}
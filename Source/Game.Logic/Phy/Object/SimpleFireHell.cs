namespace Game.Logic.Phy.Object
{
    using Game.Logic;
    using Game.Logic.AI;
    using Game.Logic.AI.Npc;
    using Game.Server.Managers;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class SimpleFireHell : Living
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ABrain m_ai;
        private List<SimpleNpc> m_child;
        private SqlDataProvider.Data.NpcInfo m_npcInfo;

        public SimpleFireHell(int id, BaseGame game, SqlDataProvider.Data.NpcInfo npcInfo, int type) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, -1)
        {
            this.m_child = new List<SimpleNpc>();
            if (type == 0)
            {
                base.Type = eLivingType.SimpleNpc;
            }
            if (type == 10)
            {
                base.Type = eLivingType.SimpleNpc1;
            }
            else
            {
                base.Type = eLivingType.SimpleNpcNormal;
            }
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
                log.ErrorFormat("SimpleFireHell Created error:{1}", exception);
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

        public override void Dispose()
        {
            base.Dispose();
            try
            {
                this.m_ai.Dispose();
            }
            catch (Exception exception)
            {
                log.ErrorFormat("SimpleFireHell Dispose error:{1}", exception);
            }
        }

        public void GetDropItemInfo()
        {
            if (base.m_game.CurrentLiving is Player)
            {
                Player currentLiving = base.m_game.CurrentLiving as Player;
                List<SqlDataProvider.Data.ItemInfo> list = null;
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                DropInventory.NPCDrop(this.m_npcInfo.DropId, ref list);
                if (list != null)
                {
                    foreach (SqlDataProvider.Data.ItemInfo info in list)
                    {
                        if ((info != null) && (info.Template.CategoryID == 10))
                        {
                        }
                    }
                    currentLiving.PlayerDetail.AddGold(num);
                    currentLiving.PlayerDetail.AddMoney(num2);
                    currentLiving.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_Drop, currentLiving.PlayerDetail.PlayerCharacter.ID, num2, currentLiving.PlayerDetail.PlayerCharacter.Money);
                    currentLiving.PlayerDetail.AddGiftToken(num3);
                }
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
                log.ErrorFormat("SimpleFireHell BeginNewTurn error:{1}", exception);
            }
        }

        public override void Reset()
        {
            base.Agility = this.m_npcInfo.Agility;
            base.Attack = this.m_npcInfo.Attack;
            base.BaseDamage = this.m_npcInfo.BaseDamage;
            base.BaseGuard = this.m_npcInfo.BaseGuard;
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
                log.ErrorFormat("SimpleFireHell StartAttacking error:{1}", exception);
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


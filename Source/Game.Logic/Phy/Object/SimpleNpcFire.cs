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

    public class SimpleNpcFire : Living
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ABrain m_ai;
        private SqlDataProvider.Data.NpcInfo m_npcInfo;

        public SimpleNpcFire(int id, BaseGame game, SqlDataProvider.Data.NpcInfo npcInfo, int direction, int type) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            if (type == 0)
            {
                base.Type = eLivingType.SimpleNpc;
            }
            else
            {
                base.Type = eLivingType.SimpleNpc;
            }
            this.m_npcInfo = npcInfo;
            this.m_ai = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
            if (this.m_ai == null)
            {
                log.WarnFormat("Can't create abrain :{0}", npcInfo.Script);
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
                log.Warn("SimpleNpcFire Created error:{1}", exception);
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
                log.Warn("SimpleNpcFire Dispose error:{1}", exception);
            }
        }

        public void GetDropItemInfo()
        {
            if (base.m_game.CurrentLiving is Player)
            {
                Player currentLiving = base.m_game.CurrentLiving as Player;
                List<SqlDataProvider.Data.ItemInfo> list = null;
                int gold = 0;
                int money = 0;
                int giftToken = 0;
                DropInventory.NPCDrop(this.m_npcInfo.DropId, ref list);
                if (list != null)
                {
                    foreach (SqlDataProvider.Data.ItemInfo info in list)
                    {
                        SqlDataProvider.Data.ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
                        if (info != null)
                        {
                            if (info.Template.CategoryID == 10)
                            {
                                currentLiving.PlayerDetail.AddTemplate(info, eBageType.FightBag, info.Count, eGameView.dungeonTypeGet);
                            }
                            else
                            {
                                currentLiving.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count, eGameView.dungeonTypeGet);
                            }
                        }
                    }
                    currentLiving.PlayerDetail.AddGold(gold);
                    currentLiving.PlayerDetail.AddMoney(money);
                    currentLiving.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_Drop, currentLiving.PlayerDetail.PlayerCharacter.ID, money, currentLiving.PlayerDetail.PlayerCharacter.Money);
                    currentLiving.PlayerDetail.AddGiftToken(giftToken);
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
                log.Warn("SimpleNpcFire BeginNewTurn error:{1}", exception);
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
                log.Warn("SimpleNpcFire StartAttacking error:{1}", exception);
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


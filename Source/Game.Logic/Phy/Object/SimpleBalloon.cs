using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object
{
    public class SimpleBalloon : Living
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private NpcInfo m_npcInfo;
        private ABrain m_ai;

        public NpcInfo NpcInfo
        {
            get
            {
                return this.m_npcInfo;
            }
        }

        public SimpleBalloon(int id, BaseGame game, NpcInfo npcInfo, int type, string action)
            : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, type)
        {
            if (type == 0)
            {
                base.Type = eLivingType.SimpleNpc;
            }
            else
            {
                base.Type = eLivingType.SimpleNpc;
            }
            ActionStr = action;
            this.m_npcInfo = npcInfo;
            this.m_ai = (ScriptMgr.CreateInstance(npcInfo.Script) as ABrain);
            if (this.m_ai == null)
            {
                SimpleBalloon.log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
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
                SimpleBalloon.log.ErrorFormat("SimpleBalloon Created error:{1}", ex);
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
                SimpleBalloon.log.ErrorFormat("SimpleBalloon BeginNewTurn error:{1}", ex);
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
                SimpleBalloon.log.ErrorFormat("SimpleBalloon StartAttacking error:{1}", ex);
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
                SimpleBalloon.log.ErrorFormat("SimpleBalloon Dispose error:{1}", ex);
            }
        }
    }
}
namespace Game.Logic.Actions
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class NpcRangeAttackingAction : BaseAction
    {
        private string m_action;
        private int m_fx;
        private Living m_living;
        private List<NormalNpc> m_npc;
        private int m_tx;

        public NpcRangeAttackingAction(Living living, int fx, int tx, string action, int delay, List<NormalNpc> players) : base(delay, 0x3e8)
        {
            this.m_living = living;
            this.m_npc = players;
            this.m_fx = fx;
            this.m_tx = tx;
            this.m_action = action;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, this.m_living.Id) {
                Parameter1 = this.m_living.Id
            };
            pkg.WriteByte(0x3d);
            List<NormalNpc> list = game.Map.FindNpcIds(this.m_fx, this.m_tx, this.m_npc);
            int count = list.Count;
            foreach (NormalNpc npc in list)
            {
                if (this.m_living.IsFriendly(npc))
                {
                    count--;
                }
            }
            pkg.WriteInt(count);
            this.m_living.SyncAtTime = false;
            try
            {
                foreach (NormalNpc npc in list)
                {
                    if (npc.IsFrost)
                    {
                        npc.IsFrost = false;
                    }
                    else
                    {
                        npc.SyncAtTime = false;
                        if (!this.m_living.IsFriendly(npc))
                        {
                            int val = 0;
                            npc.IsFrost = false;
                            game.SendGameUpdateFrozenState(npc);
                            int baseDamage = this.MakeDamage(npc);
                            int criticalAmount = this.MakeCriticalDamage(npc, baseDamage);
                            int num5 = 0;
                            if (npc.TakeDamage(this.m_living, ref baseDamage, ref criticalAmount, "Dame"))
                            {
                                num5 = baseDamage + criticalAmount;
                            }
                            pkg.WriteInt(npc.Id);
                            pkg.WriteInt(num5);
                            pkg.WriteInt(npc.Blood);
                            pkg.WriteInt(val);
                            pkg.WriteInt(new Random().Next(2));
                        }
                    }
                }
                game.SendToAll(pkg);
                base.Finish(tick);
            }
            finally
            {
                this.m_living.SyncAtTime = true;
                foreach (NormalNpc npc in list)
                {
                    npc.SyncAtTime = true;
                }
            }
        }

        private int MakeCriticalDamage(NormalNpc p, int baseDamage)
        {
            double lucky = this.m_living.Lucky;
            Random random = new Random();
            if (((75000.0 * lucky) / (lucky + 800.0)) > random.Next(0x186a0))
            {
                return (int) ((0.5 + (lucky * 0.0003)) * baseDamage);
            }
            return 0;
        }

        private int MakeDamage(NormalNpc p)
        {
            double baseDamage = this.m_living.BaseDamage;
            double baseGuard = p.BaseGuard;
            double defence = p.Defence;
            double attack = this.m_living.Attack;
            if (this.m_living.IgnoreArmor)
            {
                baseGuard = 0.0;
                defence = 0.0;
            }
            float currentDamagePlus = this.m_living.CurrentDamagePlus;
            float currentShootMinus = this.m_living.CurrentShootMinus;
            double num7 = (0.95 * (p.BaseGuard - (3 * this.m_living.Grade))) / ((500.0 + p.BaseGuard) - (3 * this.m_living.Grade));
            double num8 = 0.0;
            if ((p.Defence - this.m_living.Lucky) < 0.0)
            {
                num8 = 0.0;
            }
            else
            {
                num8 = (0.95 * (p.Defence - this.m_living.Lucky)) / ((600.0 + p.Defence) - this.m_living.Lucky);
            }
            double num9 = (((baseDamage * (1.0 + (attack * 0.001))) * (1.0 - ((num7 + num8) - (num7 * num8)))) * currentDamagePlus) * currentShootMinus;
            Rectangle directDemageRect = p.GetDirectDemageRect();
            double num10 = Math.Sqrt((double) (((directDemageRect.X - this.m_living.X) * (directDemageRect.X - this.m_living.X)) + ((directDemageRect.Y - this.m_living.Y) * (directDemageRect.Y - this.m_living.Y))));
            num9 *= 1.0 - ((num10 / ((double) Math.Abs((int) (this.m_tx - this.m_fx)))) / 4.0);
            if (num9 < 0.0)
            {
                return 1;
            }
            return (int) num9;
        }
    }
}


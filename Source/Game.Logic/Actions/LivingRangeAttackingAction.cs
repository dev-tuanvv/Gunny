namespace Game.Logic.Actions
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class LivingRangeAttackingAction : BaseAction
    {
        private string m_action;
        private int m_fx;
        private Living m_living;
        private List<Player> m_players;
        private int m_tx;

        public LivingRangeAttackingAction(Living living, int fx, int tx, string action, int delay, List<Player> players) : base(delay, 0x3e8)
        {
            this.m_living = living;
            this.m_players = players;
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
            List<Living> list = game.Map.FindPlayers(this.m_fx, this.m_tx, this.m_players);
            int count = list.Count;
            foreach (Living living in list)
            {
                if (this.m_living.IsFriendly(living))
                {
                    count--;
                }
            }
            pkg.WriteInt(count);
            this.m_living.SyncAtTime = false;
            try
            {
                foreach (Living living2 in list)
                {
                    living2.SyncAtTime = false;
                    if (!this.m_living.IsFriendly(living2))
                    {
                        int val = 0;
                        living2.IsFrost = false;
                        game.SendGameUpdateFrozenState(living2);
                        int baseDamage = this.MakeDamage(living2);
                        int criticalAmount = this.MakeCriticalDamage(living2, baseDamage);
                        int num5 = 0;
                        if (living2.TakeDamage(this.m_living, ref baseDamage, ref criticalAmount, "范围攻击"))
                        {
                            num5 = baseDamage + criticalAmount;
                            if (living2 is Player)
                            {
                                Player player = living2 as Player;
                                val = player.Dander;
                            }
                        }
                        pkg.WriteInt(living2.Id);
                        pkg.WriteInt(num5);
                        pkg.WriteInt(living2.Blood);
                        pkg.WriteInt(val);
                        pkg.WriteInt(1);
                    }
                }
                game.SendToAll(pkg);
                base.Finish(tick);
            }
            finally
            {
                this.m_living.SyncAtTime = true;
                foreach (Living living3 in list)
                {
                    living3.SyncAtTime = true;
                }
            }
        }

        private int MakeCriticalDamage(Living p, int baseDamage)
        {
            double lucky = this.m_living.Lucky;
            Random random = new Random();
            if (((75000.0 * lucky) / (lucky + 800.0)) > random.Next(0x186a0))
            {
                int num2 = p.ReduceCritFisrtGem + p.ReduceCritSecondGem;
                int num3 = (int) ((0.5 + (lucky * 0.0003)) * baseDamage);
                return ((num3 * (100 - num2)) / 100);
            }
            return 0;
        }

        private int MakeDamage(Living p)
        {
            double num9;
            double baseDamage = this.m_living.BaseDamage;
            double baseGuard = p.BaseGuard;
            double defence = p.Defence;
            double attack = this.m_living.Attack;
            if (p.AddArmor && ((p as Player).DeputyWeapon != null))
            {
                int num5 = (p as Player).DeputyWeapon.Template.Property7 + ((int) Math.Pow(1.1, (double) (p as Player).DeputyWeapon.StrengthenLevel));
                baseGuard += num5;
                defence += num5;
            }
            if (this.m_living.IgnoreArmor)
            {
                baseGuard = 0.0;
                defence = 0.0;
            }
            float currentDamagePlus = this.m_living.CurrentDamagePlus;
            float currentShootMinus = this.m_living.CurrentShootMinus;
            double num8 = (0.95 * (baseGuard - (3 * this.m_living.Grade))) / ((500.0 + baseGuard) - (3 * this.m_living.Grade));
            if ((defence - this.m_living.Lucky) < 0.0)
            {
                num9 = 0.0;
            }
            else
            {
                num9 = (0.95 * (defence - this.m_living.Lucky)) / ((600.0 + defence) - this.m_living.Lucky);
            }
            double num10 = (((baseDamage * (1.0 + (attack * 0.001))) * (1.0 - ((num8 + num9) - (num8 * num9)))) * currentDamagePlus) * currentShootMinus;
            Rectangle directDemageRect = p.GetDirectDemageRect();
            double num11 = Math.Sqrt((double) (((directDemageRect.X - this.m_living.X) * (directDemageRect.X - this.m_living.X)) + ((directDemageRect.Y - this.m_living.Y) * (directDemageRect.Y - this.m_living.Y))));
            num10 *= 1.0 - ((num11 / ((double) Math.Abs((int) (this.m_tx - this.m_fx)))) / 4.0);
            if (num10 < 0.0)
            {
                return 1;
            }
            return (int) num10;
        }
    }
}


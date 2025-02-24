namespace Game.Logic.Actions
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class LivingRangeAttackingAction2 : BaseAction
    {
        private bool bool_0;
        private bool bool_1;
        private int int_x;
        private int int_y;
        private List<Player> list_player;
        private Living living_0;
        private string string_0;

        public LivingRangeAttackingAction2(Living living, int fx, int tx, string action, int delay, bool removeFrost, bool directDamage, List<Player> players) : base(delay, 0x3e8)
        {
            this.living_0 = living;
            this.list_player = players;
            this.int_x = fx;
            this.int_y = tx;
            this.string_0 = action;
            this.bool_0 = removeFrost;
            this.bool_1 = directDamage;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b, this.living_0.Id) {
                Parameter1 = this.living_0.Id
            };
            pkg.WriteByte(0x3d);
            List<Living> list = game.Map.FindPlayers(this.int_x, this.int_y, this.list_player);
            int count = list.Count;
            foreach (Living living in list)
            {
                if (this.living_0.IsFriendly(living) || (((living is SimpleBoss) || (living is SimpleNpc)) && !living.Config.CanTakeDamage))
                {
                    count--;
                }
            }
            pkg.WriteInt(count);
            this.living_0.SyncAtTime = false;
            try
            {
                foreach (Living living2 in list)
                {
                    living2.SyncAtTime = false;
                    if ((!this.living_0.IsFriendly(living2) && ((!(living2 is SimpleBoss) && !(living2 is SimpleNpc)) || living2.Config.CanTakeDamage)) || living2.Config.IsHelper)
                    {
                        int val = 0;
                        if (living2.IsFrost)
                        {
                            living2.IsFrost = false;
                            game.method_30(living2);
                            base.Finish(tick);
                            return;
                        }
                        int num3 = this.method_0(living2);
                        int criticalAmount = this.method_1(living2, num3);
                        int num5 = 0;
                        if (living2.TakeDamage(this.living_0, ref num3, ref criticalAmount, "范围攻击"))
                        {
                            num5 = num3 + criticalAmount;
                            if (living2 is Player)
                            {
                                val = (living2 as Player).Dander;
                            }
                        }
                        pkg.WriteInt(living2.Id);
                        pkg.WriteInt(num5);
                        pkg.WriteInt(living2.Blood);
                        pkg.WriteInt(val);
                        pkg.WriteInt(living2.IsLiving ? 1 : 6);
                    }
                }
                game.SendToAll(pkg);
                base.Finish(tick);
            }
            finally
            {
                this.living_0.SyncAtTime = true;
                using (List<Living>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.SyncAtTime = true;
                    }
                }
            }
        }

        private int method_0(Living living_1)
        {
            double baseGuard = living_1.BaseGuard;
            double defence = living_1.Defence;
            double attack = this.living_0.Attack;
            if (living_1.AddArmor && ((living_1 as Player).DeputyWeapon != null))
            {
                int num4 = (living_1 as Player).DeputyWeapon.Template.Property7 + ((int) Math.Pow(1.1, (double) (living_1 as Player).DeputyWeapon.StrengthenLevel));
                baseGuard += num4;
                defence += num4;
            }
            if (this.living_0.IgnoreArmor)
            {
                baseGuard = 0.0;
                defence = 0.0;
            }
            float currentDamagePlus = this.living_0.CurrentDamagePlus;
            float currentShootMinus = this.living_0.CurrentShootMinus;
            double num7 = (0.95 * (living_1.BaseGuard - (3 * this.living_0.Grade))) / ((500.0 + living_1.BaseGuard) - (3 * this.living_0.Grade));
            double num8 = 0.0;
            if ((living_1.Defence - this.living_0.Lucky) < 0.0)
            {
                num8 = 0.0;
            }
            else
            {
                num8 = (0.95 * (living_1.Defence - this.living_0.Lucky)) / ((600.0 + living_1.Defence) - this.living_0.Lucky);
            }
            double num9 = (((this.living_0.BaseDamage * (1.0 + (attack * 0.001))) * (1.0 - ((num7 + num8) - (num7 * num8)))) * currentDamagePlus) * currentShootMinus;
            if (!this.bool_1)
            {
                Rectangle directDemageRect = living_1.GetDirectDemageRect();
                double num10 = Math.Sqrt((double) (((directDemageRect.X - this.living_0.X) * (directDemageRect.X - this.living_0.X)) + ((directDemageRect.Y - this.living_0.Y) * (directDemageRect.Y - this.living_0.Y))));
                num9 *= 1.0 - ((num10 / ((double) Math.Abs((int) (this.int_y - this.int_x)))) / 4.0);
            }
            if (num9 <= 0.0)
            {
                return 1;
            }
            return (int) num9;
        }

        private int method_1(Living living_1, int int_2)
        {
            double lucky = this.living_0.Lucky;
            Random random = new Random();
            if (((75000.0 * lucky) / (lucky + 800.0)) > random.Next(0x186a0))
            {
                int num2 = living_1.ReduceCritFisrtGem + living_1.ReduceCritSecondGem;
                return ((((int) ((0.5 + (lucky * 0.0003)) * int_2)) * (100 - num2)) / 100);
            }
            return 0;
        }
    }
}


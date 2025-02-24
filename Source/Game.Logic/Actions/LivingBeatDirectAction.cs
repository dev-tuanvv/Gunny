namespace Game.Logic.Actions
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingBeatDirectAction : BaseAction
    {
        private int int_0;
        private int int_1;
        private Living living_0;
        private Living living_1;
        private string string_0;

        public LivingBeatDirectAction(Living living, Living target, string action, int delay, int livingCount, int attackEffect) : base(delay)
        {
            this.living_0 = living;
            this.living_1 = target;
            this.string_0 = action;
            this.int_0 = livingCount;
            this.int_1 = attackEffect;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            this.living_1.SyncAtTime = false;
            try
            {
                GSPacketIn pkg = new GSPacketIn(0x5b, this.living_0.Id) {
                    Parameter1 = this.living_0.Id
                };
                pkg.WriteByte(0x3a);
                pkg.WriteString(!string.IsNullOrEmpty(this.string_0) ? this.string_0 : "");
                pkg.WriteInt(this.int_0);
                for (int i = 1; i <= this.int_0; i++)
                {
                    int baseDamage = this.living_0.MakeDamage(this.living_1, false);
                    int criticalAmount = this.MakeCriticalDamage(baseDamage);
                    int val = 0;
                    if (this.living_1 is Player)
                    {
                        val = (this.living_1 as Player).Dander;
                    }
                    if (this.living_1.IsFrost)
                    {
                        this.living_1.IsFrost = false;
                        game.method_30(this.living_1);
                    }
                    if (!this.living_1.TakeDamage(this.living_0, ref baseDamage, ref criticalAmount, "小怪伤血"))
                    {
                        Console.WriteLine("//error beat direct damage");
                    }
                    pkg.WriteInt(this.living_1.Id);
                    pkg.WriteInt(baseDamage + criticalAmount);
                    pkg.WriteInt(this.living_1.Blood);
                    pkg.WriteInt(val);
                    pkg.WriteInt(this.int_1);
                }
                game.SendToAll(pkg);
                base.Finish(tick);
            }
            finally
            {
                this.living_1.SyncAtTime = true;
            }
        }

        protected int MakeCriticalDamage(int baseDamage)
        {
            double lucky = this.living_0.Lucky;
            if (((lucky * 45.0) / (800.0 + lucky)) <= ThreadSafeRandom.NextStatic(100))
            {
                return 0;
            }
            int num2 = this.living_1.ReduceCritFisrtGem + this.living_1.ReduceCritSecondGem;
            int num3 = (int) ((0.5 + (lucky * 0.00015)) * baseDamage);
            num3 = (num3 * (100 - num2)) / 100;
            if (this.living_0.FightBuffers.ConsortionAddCritical > 0)
            {
                num3 += this.living_0.FightBuffers.ConsortionAddCritical;
            }
            return num3;
        }
    }
}


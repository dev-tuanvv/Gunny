namespace Game.Logic.Effects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class ContinueReduceGreenBloodEffect : AbstractEffect
    {
        private int int_0;
        private int int_1;
        private Living yktowyBbie;

        public ContinueReduceGreenBloodEffect(int count, int blood, Living liv) : base(eEffectType.ContinueReduceGreenBloodEffect)
        {
            this.int_0 = count;
            this.int_1 = blood;
            this.yktowyBbie = liv;
        }

        private void method_0(Living living_0)
        {
            this.int_0--;
            if (this.int_0 < 0)
            {
                this.Stop();
            }
            else
            {
                living_0.AddBlood(-this.int_1, 1);
                if (living_0.Blood <= 0)
                {
                    living_0.Die();
                    if ((this.yktowyBbie != null) && (this.yktowyBbie is Player))
                    {
                        int type = 2;
                        if (living_0 is Player)
                        {
                            type = 1;
                        }
                        (this.yktowyBbie as Player).PlayerDetail.OnKillingLiving(this.yktowyBbie.Game, type, living_0.Id, living_0.IsLiving, this.int_1);
                    }
                }
            }
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(this.method_0);
            living.Game.method_47(living, 0x1c, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.method_0);
            living.Game.method_47(living, 0x1c, false);
        }

        public override bool Start(Living living)
        {
            ContinueReduceGreenBloodEffect ofType = living.EffectList.GetOfType(eEffectType.ContinueReduceGreenBloodEffect) as ContinueReduceGreenBloodEffect;
            if (ofType != null)
            {
                ofType.int_0 = this.int_0;
                return true;
            }
            return base.Start(living);
        }
    }
}


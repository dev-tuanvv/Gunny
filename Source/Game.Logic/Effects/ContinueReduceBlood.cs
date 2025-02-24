namespace Game.Logic.Effects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class ContinueReduceBlood : AbstractEffect
    {
        private int m_blood;
        private int m_count;
        private Living m_liv;

        public ContinueReduceBlood(int count, int blood, Living liv) : base(eEffectType.ContinueReduceBlood)
        {
            this.m_count = count;
            this.m_blood = blood;
            this.m_liv = liv;
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 0x1c, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 0x1c, false);
        }

        private void player_BeginFitting(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
            else
            {
                living.AddBlood(-this.m_blood, 1);
                if (living.Blood <= 0)
                {
                    living.Die();
                    if ((this.m_liv != null) && (this.m_liv is Player))
                    {
                        (this.m_liv as Player).PlayerDetail.OnKillingLiving(this.m_liv.Game, 2, living.Id, living.IsLiving, this.m_blood);
                    }
                }
            }
        }

        public override bool Start(Living living)
        {
            ContinueReduceBlood ofType = living.EffectList.GetOfType(eEffectType.ContinueReduceBlood) as ContinueReduceBlood;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


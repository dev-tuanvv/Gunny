namespace Game.Logic.Effects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class InvinciblyEffect : AbstractEffect
    {
        private int m_count;

        public InvinciblyEffect(int count) : base(eEffectType.InvinciblyEffect)
        {
            this.m_count = count;
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
        }

        private void player_BeginFitting(Living player)
        {
            this.m_count--;
            if (this.m_count <= 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            InvinciblyEffect ofType = living.EffectList.GetOfType(eEffectType.InvinciblyEffect) as InvinciblyEffect;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


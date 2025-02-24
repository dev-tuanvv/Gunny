namespace Game.Logic.Effects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class GuardEffect : AbstractEffect
    {
        private int m_count;

        public GuardEffect(int count) : base(eEffectType.GuardEffect)
        {
            this.m_count = count;
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 30, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 30, false);
        }

        private void player_BeginFitting(Living living)
        {
            this.m_count--;
            if (this.m_count <= 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            GuardEffect ofType = living.EffectList.GetOfType(eEffectType.GuardEffect) as GuardEffect;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


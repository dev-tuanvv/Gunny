namespace Game.Logic.Effects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Runtime.InteropServices;

    public class ReduceStrengthEffect : AbstractEffect
    {
        private int m_count;
        private int m_reduce;

        public ReduceStrengthEffect(int count, int reduce = 0) : base(eEffectType.ReduceStrengthEffect)
        {
            this.m_count = count;
            this.m_reduce = reduce;
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 1, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 1, false);
        }

        private void player_BeginFitting(Living living)
        {
            this.m_count--;
            if (living is Player)
            {
                Player player1 = living as Player;
                player1.Energy -= this.m_reduce;
            }
            if (this.m_count < 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            ReduceStrengthEffect ofType = living.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) as ReduceStrengthEffect;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


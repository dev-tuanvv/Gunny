namespace Game.Logic.Effects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class Phongan : AbstractEffect
    {
        private int m_count;

        public Phongan(int count) : base(eEffectType.PhongAn)
        {
            this.m_count = count;
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 2, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 2, false);
        }

        private void player_BeginFitting(Living living)
        {
            Player player = null;
            this.m_count--;
            if (living is Player)
            {
                player = living as Player;
                player.capnhatstate("silencedSpecial", "true");
                player.IconPicture(eMirariType.Lockstate, true);
                player.State = 9;
            }
            if (this.m_count < 0)
            {
                if (player != null)
                {
                    player.capnhatstate("silencedSpecial", "false");
                    player.IconPicture(eMirariType.ReversePlayer, true);
                    player.State = 0;
                }
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            Phongan ofType = living.EffectList.GetOfType(eEffectType.PhongAn) as Phongan;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetFatalEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetFatalEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.FatalEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x88:
                case 0x89:
                    this.m_count = 1;
                    break;
            }
        }

        public void ChangeProperty(Living living)
        {
            base.IsTrigger = false;
            if ((base.rand.Next(0x2710) < this.m_probability) && (living.PetEffects.CurrentUseSkill == this.m_currentId))
            {
                base.IsTrigger = true;
                living.PetEffectTrigger = true;
                living.PetEffects.PetDelay = this.m_delay;
                if (living.PetEffects.IsPetUseSkill)
                {
                    living.PetEffects.IsPetUseSkill = false;
                    (living as Player).ControlBall = true;
                }
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
            player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
            player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
        }

        private void player_AfterPlayerShooted(Player player)
        {
            base.IsTrigger = false;
            player.ControlBall = false;
            player.EffectTrigger = false;
        }

        public override bool Start(Living living)
        {
            PetFatalEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.FatalEffect) as PetFatalEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


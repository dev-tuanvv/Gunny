namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetPlusDameEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetPlusDameEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetPlusDameEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x45:
                    this.m_value = 15;
                    break;

                case 70:
                    this.m_value = 30;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginSelfTurn -= new LivingEventHandle(this.player_AfterKilledByLiving);
        }

        private void player_AfterKilledByLiving(Living living)
        {
            if ((base.rand.Next(0x2710) < this.m_probability) && (living.PetEffects.AddDameValue < (this.m_value * 5)))
            {
                Player player1 = living as Player;
                player1.BaseDamage += this.m_value;
                PetEffectInfo petEffects = living.PetEffects;
                petEffects.AddDameValue += this.m_value;
            }
        }

        public override bool Start(Living living)
        {
            PetPlusDameEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetPlusDameEquipEffect) as PetPlusDameEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


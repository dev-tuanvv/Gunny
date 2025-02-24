namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddHighMPEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetAddHighMPEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetAddHighMPEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            if (skillId == 0x60)
            {
                this.m_value = 50;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_BeginSelfTurn);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginSelfTurn -= new LivingEventHandle(this.player_BeginSelfTurn);
        }

        private void player_BeginSelfTurn(Living living)
        {
            if ((base.rand.Next(0x2710) < this.m_probability) && (0x60 == this.m_currentId))
            {
                ((Player) living).AddPetMP(this.m_value);
            }
        }

        public override bool Start(Living living)
        {
            PetAddHighMPEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddHighMPEquipEffect) as PetAddHighMPEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


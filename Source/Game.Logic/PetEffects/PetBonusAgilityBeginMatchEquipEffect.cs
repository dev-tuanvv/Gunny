namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetBonusAgilityBeginMatchEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetBonusAgilityBeginMatchEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetBonusAgilityBeginMatchEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x86:
                    this.m_value = 300;
                    break;

                case 0x87:
                    this.m_value = 500;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            PetEffectInfo petEffects = player.PetEffects;
            petEffects.BonusAgility += this.m_value;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            PetEffectInfo petEffects = player.PetEffects;
            petEffects.BonusAgility -= this.m_value;
        }

        public override bool Start(Living living)
        {
            PetBonusAgilityBeginMatchEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetBonusAgilityBeginMatchEquipEffect) as PetBonusAgilityBeginMatchEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


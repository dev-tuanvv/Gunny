namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetBonusGuardBeginMatchEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetBonusGuardBeginMatchEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetBonusGuardBeginMatchEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x98:
                    this.m_value = 100;
                    break;

                case 0x99:
                    this.m_value = 200;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            PetEffectInfo petEffects = player.PetEffects;
            petEffects.BonusGuard += this.m_value;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            PetEffectInfo petEffects = player.PetEffects;
            petEffects.BonusGuard -= this.m_value;
        }

        public override bool Start(Living living)
        {
            PetBonusGuardBeginMatchEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetBonusGuardBeginMatchEquipEffect) as PetBonusGuardBeginMatchEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


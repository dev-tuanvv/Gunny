namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetBonusAttackBeginMatchEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetBonusAttackBeginMatchEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetBonusAttackBeginMatchEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x70:
                    this.m_value = 80;
                    break;

                case 0x71:
                    this.m_value = 100;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            PetEffectInfo petEffects = player.PetEffects;
            petEffects.BonusAttack += this.m_value;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            PetEffectInfo petEffects = player.PetEffects;
            petEffects.BonusAttack -= this.m_value;
        }

        public override bool Start(Living living)
        {
            PetBonusAttackBeginMatchEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetBonusAttackBeginMatchEquipEffect) as PetBonusAttackBeginMatchEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


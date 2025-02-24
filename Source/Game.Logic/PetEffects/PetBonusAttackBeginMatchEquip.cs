namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetBonusAttackBeginMatchEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_value;

        public PetBonusAttackBeginMatchEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetBonusAttackBeginMatchEquip, elementID)
        {
            this.m_count = count;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x86:
                    this.m_value = 100;
                    break;

                case 0x87:
                    this.m_value = 300;
                    break;
            }
        }

        public override void OnAttached(Living player)
        {
            PetEffectInfo petEffects = player.PetEffects;
            petEffects.BonusAttack += this.m_value;
        }

        public override void OnRemoved(Living player)
        {
            PetEffectInfo petEffects = player.PetEffects;
            petEffects.BonusAttack -= this.m_value;
        }

        public override bool Start(Living living)
        {
            PetBonusAttackBeginMatchEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetBonusAttackBeginMatchEquip) as PetBonusAttackBeginMatchEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


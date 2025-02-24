namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddCritRateEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetAddCritRateEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAddCritRateEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x88:
                    this.m_value = 30;
                    break;

                case 0x89:
                case 0x97:
                    this.m_value = 50;
                    break;

                case 150:
                    this.m_value = 20;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            PetEffectInfo petEffects = (living as Player).PetEffects;
            petEffects.CritRate += this.m_value;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            PetEffectInfo petEffects = (living as Player).PetEffects;
            petEffects.CritRate -= this.m_value;
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
        }

        private void player_BeginFitting(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetAddCritRateEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddCritRateEquip) as PetAddCritRateEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


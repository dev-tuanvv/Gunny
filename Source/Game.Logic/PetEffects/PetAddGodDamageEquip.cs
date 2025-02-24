namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddGodDamageEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_probability;
        private int m_value;

        public PetAddGodDamageEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAddGodDamageEquip, elementID)
        {
            this.m_count = count;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0xbb:
                    this.m_value = 100;
                    break;

                case 0xbc:
                    this.m_value = 300;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.BaseDamage += this.m_value;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.BaseDamage -= this.m_value;
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
        }

        private void player_BeginFitting(Living living)
        {
        }

        public override bool Start(Living living)
        {
            PetAddGodDamageEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddGodDamageEquip) as PetAddGodDamageEquip;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


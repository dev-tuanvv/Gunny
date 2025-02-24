namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetReduceDefendEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetReduceDefendEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetReduceDefendEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x92:
                    this.m_value = 300;
                    break;

                case 0x93:
                    this.m_value = 500;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.BaseDamage -= this.m_value;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.BaseDamage += this.m_value;
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
            PetReduceDefendEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceDefendEquip) as PetReduceDefendEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetReduceAttackEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetReduceAttackEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetReduceAttackEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x25:
                case 0x26:
                case 0x27:
                case 0xb2:
                    this.m_value = 100;
                    break;

                case 0x92:
                case 180:
                    this.m_value = 300;
                    break;

                case 0x93:
                    this.m_value = 500;
                    break;

                case 0xb3:
                    this.m_value = 200;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.Attack -= this.m_value;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.Attack += this.m_value;
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
            PetReduceAttackEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceAttackEquip) as PetReduceAttackEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetReduceBaseGuardEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetReduceBaseGuardEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetReduceBaseGuardEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x6a:
                    this.m_value = 70;
                    break;

                case 0x6b:
                    this.m_value = 120;
                    break;

                case 0x9a:
                    this.m_value = 100;
                    break;

                case 0x9b:
                    this.m_value = 200;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.BaseGuard -= this.m_value;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.BaseGuard += this.m_value;
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
            PetReduceBaseGuardEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceBaseGuardEquip) as PetReduceBaseGuardEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetBuffGuardEquip : AbstractPetEffect
    {
        private int m_added;
        private int m_count;

        public PetBuffGuardEquip(int count, int skilId, string elementID) : base(ePetEffectType.PetBuffGuardEquip, elementID)
        {
            this.m_count = count;
            switch (skilId)
            {
                case 0x7f:
                    this.m_added = 100;
                    break;

                case 0x80:
                    this.m_added = 200;
                    break;

                case 0x81:
                    this.m_added = 300;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.BaseGuard += this.m_added;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.BaseGuard -= this.m_added;
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
            PetBuffGuardEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetBuffGuardEquip) as PetBuffGuardEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


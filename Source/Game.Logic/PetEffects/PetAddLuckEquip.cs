namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddLuckEquip : AbstractPetEffect
    {
        private int m_added;
        private int m_count;

        public PetAddLuckEquip(int count, int skilId, string elementID) : base(ePetEffectType.PetAddLuckEquip, elementID)
        {
            this.m_count = count;
            switch (skilId)
            {
                case 0x1c:
                case 0x59:
                    this.m_added = 100;
                    break;

                case 0x1d:
                case 90:
                    this.m_added = 300;
                    break;

                case 30:
                    this.m_added = 500;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.Lucky += this.m_added;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.Lucky -= this.m_added;
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
            PetAddLuckEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddLuckEquip) as PetAddLuckEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


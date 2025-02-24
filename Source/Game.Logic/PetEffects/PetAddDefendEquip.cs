namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddDefendEquip : AbstractPetEffect
    {
        private int m_added;
        private int m_count;

        public PetAddDefendEquip(int count, int skilId, string elementID) : base(ePetEffectType.AddDefenceEquip, elementID)
        {
            this.m_count = count;
            switch (skilId)
            {
                case 0x22:
                case 0x59:
                    this.m_added = 100;
                    break;

                case 0x23:
                case 0x25:
                case 0x4a:
                case 90:
                    this.m_added = 300;
                    break;

                case 0x24:
                case 0x27:
                case 0x4b:
                    this.m_added = 500;
                    break;

                case 0x26:
                    this.m_added = 400;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.Defence += this.m_added;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.Defence -= this.m_added;
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
            PetAddDefendEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.AddDefenceEquip) as PetAddDefendEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


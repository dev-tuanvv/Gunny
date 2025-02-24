namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddAttackEquip : AbstractPetEffect
    {
        private int m_added;
        private int m_count;

        public PetAddAttackEquip(int count, int skilId, string elementID) : base(ePetEffectType.PetAddAttackEquip, elementID)
        {
            this.m_count = count;
            switch (skilId)
            {
                case 0x59:
                case 0x7c:
                    this.m_added = 100;
                    break;

                case 90:
                case 0x7d:
                case 0x89:
                    this.m_added = 300;
                    break;

                case 0x7e:
                    this.m_added = 500;
                    break;

                case 0x88:
                    this.m_added = 200;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.Attack += this.m_added;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.Attack -= this.m_added;
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
        }

        private void player_BeginFitting(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                living.Game.SendPetBuff(living, base.Info, false);
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetAddAttackEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddAttackEquip) as PetAddAttackEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


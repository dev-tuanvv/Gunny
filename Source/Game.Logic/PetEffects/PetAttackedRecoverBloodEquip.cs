namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAttackedRecoverBloodEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_value;

        public PetAttackedRecoverBloodEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAttackedRecoverBloodEquip, elementID)
        {
            this.m_count = count;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 110:
                    this.m_value = 800;
                    break;

                case 0x6f:
                    this.m_value = 0x3e8;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
            living.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
            living.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
        }

        private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (base.rand.Next(0x2710) < 0xdac)
            {
                living.SyncAtTime = true;
                living.AddBlood(this.m_value);
                living.SyncAtTime = false;
            }
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
            PetAttackedRecoverBloodEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAttackedRecoverBloodEquip) as PetAttackedRecoverBloodEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


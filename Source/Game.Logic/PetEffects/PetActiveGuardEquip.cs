namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetActiveGuardEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetActiveGuardEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetActiveGuardEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 40:
                    this.m_value = 400;
                    break;

                case 0x29:
                    this.m_value = 300;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
            living.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
        }

        private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (living.PetEffects.AddGuardValue < this.m_value)
            {
                Player player1 = living as Player;
                player1.BaseGuard += this.m_value;
                living.PetEffects.AddGuardValue = this.m_value;
            }
        }

        private void player_BeginFitting(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                Player player1 = living as Player;
                player1.BaseGuard -= living.PetEffects.AddGuardValue;
                living.PetEffects.AddGuardValue = 0;
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetActiveGuardEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetActiveGuardEquip) as PetActiveGuardEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


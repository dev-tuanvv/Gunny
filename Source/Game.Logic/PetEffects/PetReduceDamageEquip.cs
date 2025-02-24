namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetReduceDamageEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetReduceDamageEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetReduceDamageEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 50:
                case 0x35:
                    this.m_value = 100;
                    break;

                case 0x33:
                case 0x36:
                case 0xa3:
                    this.m_value = 200;
                    break;

                case 0x34:
                case 0x37:
                    this.m_value = 300;
                    break;

                case 0xa4:
                    this.m_value = 250;
                    break;

                case 0xa5:
                    this.m_value = 350;
                    break;
            }
        }

        public override void OnAttached(Living player)
        {
            player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        public override void OnRemoved(Living player)
        {
            player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            damageAmount -= this.m_value;
        }

        private void player_SelfTurn(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetReduceDamageEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceDamageEquip) as PetReduceDamageEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


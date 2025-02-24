namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetReduceTakeDamageEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_value;

        public PetReduceTakeDamageEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetReduceTakeDamageEquip, elementID)
        {
            this.m_count = count;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 50:
                case 0x33:
                case 0x34:
                case 0xa3:
                    this.m_value = 500;
                    break;

                case 0xa4:
                    this.m_value = 650;
                    break;

                case 0xa5:
                    this.m_value = 750;
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
            PetReduceTakeDamageEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceTakeDamageEquip) as PetReduceTakeDamageEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


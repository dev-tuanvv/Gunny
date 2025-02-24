namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetLuckMakeDamageEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetLuckMakeDamageEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetLuckMakeDamageEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x54:
                    this.m_value = 300;
                    break;

                case 0x55:
                case 170:
                    this.m_value = 500;
                    break;

                case 0xab:
                    this.m_value = 800;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.AfterKilledByLiving += new KillLivingEventHanlde(this.player_BeforeTakeDamage);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.AfterKilledByLiving -= new KillLivingEventHanlde(this.player_BeforeTakeDamage);
        }

        private void player_BeforeTakeDamage(Living living, Living source, int damageAmount, int criticalAmount)
        {
            if ((base.rand.Next(0x2710) < this.m_probability) && (source != living))
            {
                source.SyncAtTime = true;
                source.AddBlood(-this.m_value, 1);
                source.SyncAtTime = false;
                if (source.Blood <= 0)
                {
                    source.Die();
                }
            }
        }

        public override bool Start(Living living)
        {
            PetLuckMakeDamageEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetLuckMakeDamageEquipEffect) as PetLuckMakeDamageEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


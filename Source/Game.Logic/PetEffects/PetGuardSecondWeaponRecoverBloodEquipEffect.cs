namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetGuardSecondWeaponRecoverBloodEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetGuardSecondWeaponRecoverBloodEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetGuardSecondWeaponRecoverBloodEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 80:
                    this.m_value = 500;
                    break;

                case 0x51:
                    this.m_value = 0x3e8;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerGuard += new PlayerEventHandle(this.player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerGuard -= new PlayerEventHandle(this.player_AfterKilledByLiving);
        }

        private void player_AfterKilledByLiving(Living living)
        {
            if (base.rand.Next(0x2710) < this.m_probability)
            {
                living.SyncAtTime = true;
                living.AddBlood(this.m_value);
                living.SyncAtTime = false;
            }
        }

        public override bool Start(Living living)
        {
            PetGuardSecondWeaponRecoverBloodEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetGuardSecondWeaponRecoverBloodEquipEffect) as PetGuardSecondWeaponRecoverBloodEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


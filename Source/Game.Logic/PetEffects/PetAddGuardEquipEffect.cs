namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddGuardEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetAddGuardEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetPlusGuardEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x4c:
                    this.m_value = 70;
                    break;

                case 0x4d:
                    this.m_value = 100;
                    break;

                case 0xa8:
                    this.m_value = 90;
                    break;

                case 0xa9:
                    this.m_value = 130;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginSelfTurn -= new LivingEventHandle(this.player_AfterKilledByLiving);
        }

        private void player_AfterKilledByLiving(Living living)
        {
            if ((base.rand.Next(0x2710) < this.m_probability) && (living.PetEffects.AddGuardValue < this.m_value))
            {
                Player player1 = living as Player;
                player1.BaseGuard += this.m_value;
                PetEffectInfo petEffects = living.PetEffects;
                petEffects.AddGuardValue += this.m_value;
            }
        }

        public override bool Start(Living living)
        {
            PetAddGuardEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetPlusGuardEquipEffect) as PetAddGuardEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetClearPlusGuardEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetClearPlusGuardEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetClearPlusGuardEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBeginMoving += new PlayerEventHandle(this.player_WhenMoving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBeginMoving -= new PlayerEventHandle(this.player_WhenMoving);
        }

        private void player_WhenMoving(Player player)
        {
            if (player.PetEffects.AddGuardValue > 0)
            {
                player.BaseGuard -= player.PetEffects.AddGuardValue;
                player.PetEffects.AddGuardValue = 0;
            }
        }

        public override bool Start(Living living)
        {
            PetClearPlusGuardEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetClearPlusGuardEquipEffect) as PetClearPlusGuardEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


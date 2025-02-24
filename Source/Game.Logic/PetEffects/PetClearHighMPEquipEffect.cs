namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetClearHighMPEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetClearHighMPEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetClearHighMPEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_BeginSelfTurn);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginSelfTurn -= new LivingEventHandle(this.player_BeginSelfTurn);
        }

        private void player_BeginSelfTurn(Living living)
        {
            if (base.rand.Next(0x2710) < this.m_probability)
            {
                PetAddHighMPEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddHighMPEquipEffect) as PetAddHighMPEquipEffect;
                if ((ofType != null) && (((Player) living).TurnNum > 0))
                {
                    ofType.Stop();
                }
            }
        }

        public override bool Start(Living living)
        {
            PetClearHighMPEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetClearHighMPEquipEffect) as PetClearHighMPEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddDefendByCureEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetAddDefendByCureEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetAddDefendByCureEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerCure += new PlayerEventHandle(this.player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerCure -= new PlayerEventHandle(this.player_AfterKilledByLiving);
        }

        private void player_AfterKilledByLiving(Living living)
        {
            if (base.rand.Next(0x2710) < this.m_probability)
            {
                living.PetEffectTrigger = true;
                new PetAddDefendEquip(this.m_count, this.m_currentId, base.Info.ID.ToString()).Start(living);
            }
        }

        public override bool Start(Living living)
        {
            PetAddDefendByCureEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddDefendByCureEquipEffect) as PetAddDefendByCureEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


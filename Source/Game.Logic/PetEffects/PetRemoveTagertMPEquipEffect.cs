namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetRemoveTagertMPEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetRemoveTagertMPEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetRemoveTagertMPEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0xb7:
                    this.m_value = 10;
                    break;

                case 0xb8:
                    this.m_value = 20;
                    break;
            }
        }

        private void ChangeProperty(Player player)
        {
            base.IsTrigger = false;
            if ((base.rand.Next(100) < this.m_probability) && (player.PetEffects.CurrentUseSkill == this.m_currentId))
            {
                base.IsTrigger = true;
                player.PetEffectTrigger = true;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBuffSkillPet += new PlayerEventHandle(this.ChangeProperty);
            player.AfterKillingLiving += new KillLivingEventHanlde(this.player_AfterKillingLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(this.ChangeProperty);
            player.AfterKillingLiving -= new KillLivingEventHanlde(this.player_AfterKillingLiving);
        }

        private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if ((base.IsTrigger && (living != target)) && (target is Player))
            {
                ((Player) target).RemovePetMP(this.m_value);
                base.IsTrigger = false;
            }
        }

        public override bool Start(Living living)
        {
            PetRemoveTagertMPEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetRemoveTagertMPEquipEffect) as PetRemoveTagertMPEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


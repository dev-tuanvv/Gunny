namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetBurningBloodTargetEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetBurningBloodTargetEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetBurningBloodTargetEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
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
                target.AddPetEffect(new PetReduceBloodAllBattleEquip(this.m_count, this.m_currentId, base.Info.ID.ToString()), 0);
                base.IsTrigger = false;
            }
        }

        public override bool Start(Living living)
        {
            PetBurningBloodTargetEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetBurningBloodTargetEquipEffect) as PetBurningBloodTargetEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


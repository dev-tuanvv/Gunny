namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetMakeDamageEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetMakeDamageEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetMakeDamageEquipEffect, elementID)
        {
            this.m_count = count - 1;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBuffSkillPet += new PlayerEventHandle(this.player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(this.player_AfterKilledByLiving);
        }

        private void player_AfterKilledByLiving(Living living)
        {
            if ((base.rand.Next(0x2710) < this.m_probability) && (living.PetEffects.CurrentUseSkill == this.m_currentId))
            {
                living.PetEffectTrigger = true;
                living.Game.SendPetBuff(living, base.Info, true);
                new PetMakeDamageEquip(this.m_count, this.m_currentId, base.Info.ID.ToString()).Start(living);
            }
        }

        public override bool Start(Living living)
        {
            PetMakeDamageEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetMakeDamageEquipEffect) as PetMakeDamageEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetReduceDefendEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetReduceDefendEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetReduceDefendEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x56:
                    this.m_value = 0x3e8;
                    break;

                case 0x57:
                    this.m_value = 800;
                    break;
            }
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
            if (((base.rand.Next(0x2710) < this.m_probability) && (living.PetEffects.CurrentUseSkill == this.m_currentId)) && (living.PetEffects.ReduceDefendValue < this.m_value))
            {
                double defence = (living as Player).Defence;
                if ((living as Player).Defence < this.m_value)
                {
                    this.m_value -= this.m_value - ((int) defence);
                }
                Player player1 = living as Player;
                player1.Defence -= this.m_value;
                PetEffectInfo petEffects = living.PetEffects;
                petEffects.ReduceDefendValue += this.m_value;
            }
        }

        public override bool Start(Living living)
        {
            PetReduceDefendEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceDefendEquipEffect) as PetReduceDefendEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


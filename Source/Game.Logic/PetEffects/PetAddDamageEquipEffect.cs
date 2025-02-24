namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddDamageEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetAddDamageEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetAddDamageEquipEffect, elementID)
        {
            this.m_count = count - 1;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x6a:
                    this.m_value = 50;
                    break;

                case 0x6b:
                case 0x88:
                    this.m_value = 100;
                    break;

                case 0x89:
                    this.m_value = 150;
                    break;

                case 160:
                    this.m_value = 0x2d;
                    break;

                case 0xa1:
                    this.m_value = 0x4b;
                    break;

                case 0xa2:
                    this.m_value = 0x73;
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
            if (((base.rand.Next(0x2710) < this.m_probability) && (living.PetEffects.CurrentUseSkill == this.m_currentId)) && (living.PetEffects.AddDameValue < this.m_value))
            {
                living.PetEffectTrigger = true;
                Player player1 = living as Player;
                player1.BaseDamage += this.m_value;
                living.PetEffects.AddDameValue = this.m_value;
                new PetAddDamageEquip(this.m_count, this.m_currentId, base.Info.ID.ToString()).Start(living);
            }
        }

        public override bool Start(Living living)
        {
            PetAddDamageEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddDamageEquipEffect) as PetAddDamageEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


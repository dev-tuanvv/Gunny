namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetRecoverMPForTeamEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetRecoverMPForTeamEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetRecoverMPForTeamEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0xb7:
                    this.m_value = 5;
                    break;

                case 0xb8:
                    this.m_value = 10;
                    break;
            }
        }

        private void ChangeProperty(Player player)
        {
            if ((base.rand.Next(100) < this.m_probability) && (player.PetEffects.CurrentUseSkill == this.m_currentId))
            {
                List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
                foreach (Player player2 in allTeamPlayers)
                {
                    if (player2 != player)
                    {
                        player2.AddPetMP(this.m_value);
                    }
                }
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBuffSkillPet += new PlayerEventHandle(this.ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(this.ChangeProperty);
        }

        public override bool Start(Living living)
        {
            PetRecoverMPForTeamEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetRecoverMPForTeamEquipEffect) as PetRecoverMPForTeamEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetRecoverBloodForTeamInMapEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetRecoverBloodForTeamInMapEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetRecoverBloodForTeamInMapEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x3b:
                    this.m_value = 0x5dc;
                    break;

                case 60:
                    this.m_value = 0xbb8;
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
            if ((base.rand.Next(0x2710) < this.m_probability) && (living.PetEffects.CurrentUseSkill == this.m_currentId))
            {
                List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
                foreach (Player player in allTeamPlayers)
                {
                    player.SyncAtTime = true;
                    player.AddBlood(this.m_value);
                    player.SyncAtTime = false;
                }
            }
        }

        public override bool Start(Living living)
        {
            PetRecoverBloodForTeamInMapEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetRecoverBloodForTeamInMapEquipEffect) as PetRecoverBloodForTeamInMapEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetReduceMpAllEnemyEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetReduceMpAllEnemyEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetReduceMpAllEnemyEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x94:
                    this.m_value = 10;
                    break;

                case 0x95:
                    this.m_value = 30;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBuffSkillPet += new PlayerEventHandle(this.player_AfterBuffSkill);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(this.player_AfterBuffSkill);
        }

        private void player_AfterBuffSkill(Living living)
        {
            if ((base.rand.Next(0x2710) < this.m_probability) && (living.PetEffects.CurrentUseSkill == this.m_currentId))
            {
                List<Player> allEnemyPlayers = living.Game.GetAllEnemyPlayers(living);
                foreach (Player player in allEnemyPlayers)
                {
                    player.RemovePetMP(this.m_value);
                }
            }
        }

        public override bool Start(Living living)
        {
            PetReduceMpAllEnemyEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceMpAllEnemyEquipEffect) as PetReduceMpAllEnemyEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


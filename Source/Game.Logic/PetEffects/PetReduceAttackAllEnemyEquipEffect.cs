namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetReduceAttackAllEnemyEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetReduceAttackAllEnemyEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetReduceAttackAllEnemyEquipEffect, elementID)
        {
            this.m_count = count;
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
                List<Player> allEnemyPlayers = living.Game.GetAllEnemyPlayers(living);
                foreach (Player player in allEnemyPlayers)
                {
                    player.AddPetEffect(new PetReduceAttackEquip(this.m_count, this.m_currentId, base.Info.ID.ToString()), 0);
                }
            }
        }

        public override bool Start(Living living)
        {
            PetReduceAttackAllEnemyEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceAttackAllEnemyEquipEffect) as PetReduceAttackAllEnemyEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


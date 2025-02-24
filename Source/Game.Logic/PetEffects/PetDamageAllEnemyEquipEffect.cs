namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetDamageAllEnemyEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetDamageAllEnemyEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetDamageAllEnemyEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x72:
                    this.m_value = 0x7d0;
                    break;

                case 0x73:
                    this.m_value = 0xfa0;
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
                    player.SyncAtTime = true;
                    player.AddBlood(-this.m_value, 1);
                    player.SyncAtTime = false;
                    if (player.Blood <= 0)
                    {
                        player.Die();
                    }
                }
            }
        }

        public override bool Start(Living living)
        {
            PetDamageAllEnemyEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetDamageAllEnemyEquipEffect) as PetDamageAllEnemyEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


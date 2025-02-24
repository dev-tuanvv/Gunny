namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetRevertBloodAllPlayerAroundEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetRevertBloodAllPlayerAroundEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetRevertBloodAllPlayerAroundEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x3d:
                    this.m_value = 0x3e8;
                    break;

                case 0x3e:
                    this.m_value = 0x7d0;
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
                living.PetEffectTrigger = true;
                living.SyncAtTime = true;
                living.AddBlood(this.m_value);
                living.SyncAtTime = false;
                List<Living> list = living.Game.Map.FindAllNearestSameTeam(living.X, living.Y, 150.0, living);
                foreach (Living living2 in list)
                {
                    if (living2 != living)
                    {
                        living2.SyncAtTime = true;
                        living2.AddBlood(this.m_value);
                        living2.SyncAtTime = false;
                    }
                }
            }
        }

        public override bool Start(Living living)
        {
            PetRevertBloodAllPlayerAroundEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetRevertBloodAllPlayerAroundEquipEffect) as PetRevertBloodAllPlayerAroundEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


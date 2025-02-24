namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetAddGodDamageEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetAddGodDamageEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetAddGodDamageEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginSelfTurn -= new LivingEventHandle(this.player_AfterKilledByLiving);
        }

        private void player_AfterKilledByLiving(Living living)
        {
            if ((base.rand.Next(0x2710) < this.m_probability) && ((0xbb == this.m_currentId) || (0xbc == this.m_currentId)))
            {
                List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
                foreach (Player player in allTeamPlayers)
                {
                    if (player != living)
                    {
                        player.AddPetEffect(new PetAddGodDamageEquip(this.m_count, this.m_currentId, base.Info.ID.ToString()), 0);
                    }
                }
            }
        }

        public override bool Start(Living living)
        {
            PetAddGodDamageEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddGodDamageEquipEffect) as PetAddGodDamageEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


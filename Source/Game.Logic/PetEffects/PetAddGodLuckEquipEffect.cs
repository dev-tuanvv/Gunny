namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddGodLuckEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetAddGodLuckEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetAddGodLuckEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0xb9:
                    this.m_value = 300;
                    break;

                case 0xba:
                    this.m_value = 500;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_BeginSelfTurn);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginSelfTurn -= new LivingEventHandle(this.player_BeginSelfTurn);
        }

        private void player_BeginSelfTurn(Living living)
        {
            if (((base.rand.Next(0x2710) < this.m_probability) && ((0xb9 == this.m_currentId) || (0xba == this.m_currentId))) && (living.PetEffects.AddLuckValue < this.m_value))
            {
                Player player1 = living as Player;
                player1.Lucky += this.m_value;
                PetEffectInfo petEffects = living.PetEffects;
                petEffects.AddLuckValue += this.m_value;
            }
        }

        public override bool Start(Living living)
        {
            PetAddGodLuckEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddGodLuckEquipEffect) as PetAddGodLuckEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetClearHellIceEquipEffectcs : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetClearHellIceEquipEffectcs(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetClearHellIceEquipEffectcs, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBeginMoving += new PlayerEventHandle(this.player_BeginMoving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBeginMoving -= new PlayerEventHandle(this.player_BeginMoving);
        }

        private void player_BeginMoving(Living living)
        {
            PetReduceBloodAllBattleEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceBloodAllBattleEquip) as PetReduceBloodAllBattleEquip;
            if (ofType != null)
            {
                ofType.Stop();
            }
            PetAddGuardEquip equip2 = living.PetEffectList.GetOfType(ePetEffectType.PetAddGuardEquip) as PetAddGuardEquip;
            if (equip2 != null)
            {
                equip2.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetClearHellIceEquipEffectcs ofType = living.PetEffectList.GetOfType(ePetEffectType.PetClearHellIceEquipEffectcs) as PetClearHellIceEquipEffectcs;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


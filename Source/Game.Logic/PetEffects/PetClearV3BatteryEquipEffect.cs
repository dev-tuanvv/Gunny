namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetClearV3BatteryEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;

        public PetClearV3BatteryEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetClearV3BatteryEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBeginMoving += new PlayerEventHandle(this.player_WhenMoving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBeginMoving += new PlayerEventHandle(this.player_WhenMoving);
        }

        private void player_WhenMoving(Living living)
        {
            if (((living.PetEffects.AddAttackValue > 0) && (living.PetEffects.AddLuckValue > 0)) && (living.PetEffects.ReduceDefendValue > 0))
            {
                living.IsNoHole = false;
                PetAddAttackEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddAttackEquipEffect) as PetAddAttackEquipEffect;
                if (ofType != null)
                {
                    living.Game.SendPetBuff(living, ofType.Info, false);
                }
                Player player1 = living as Player;
                player1.Attack -= living.PetEffects.AddAttackValue;
                Player player2 = living as Player;
                player2.Lucky -= living.PetEffects.AddLuckValue;
                Player player3 = living as Player;
                player3.Defence += living.PetEffects.ReduceDefendValue;
                living.PetEffects.AddAttackValue = 0;
                living.PetEffects.AddLuckValue = 0;
                living.PetEffects.ReduceDefendValue = 0;
            }
        }

        public override bool Start(Living living)
        {
            PetClearV3BatteryEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetClearV3BatteryEquipEffect) as PetClearV3BatteryEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


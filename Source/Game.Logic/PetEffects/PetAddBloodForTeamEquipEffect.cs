namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetAddBloodForTeamEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetAddBloodForTeamEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetAddBloodForTeamEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x52:
                    this.m_value = 300;
                    break;

                case 0x53:
                    this.m_value = 600;
                    break;
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerShoot += new PlayerEventHandle(this.player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(this.player_AfterKilledByLiving);
        }

        private void player_AfterKilledByLiving(Player living)
        {
            if ((base.rand.Next(0x2710) < this.m_probability) && living.IsCure())
            {
                living.PetEffectTrigger = true;
                List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
                foreach (Player player in allTeamPlayers)
                {
                    if (player != living)
                    {
                        player.SyncAtTime = true;
                        player.AddBlood(this.m_value);
                        player.SyncAtTime = false;
                    }
                }
            }
        }

        public override bool Start(Living living)
        {
            PetAddBloodForTeamEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodForTeamEquipEffect) as PetAddBloodForTeamEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


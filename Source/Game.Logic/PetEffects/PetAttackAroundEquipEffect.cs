namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetAttackAroundEquipEffect : BasePetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_delay;
        private int m_probability;
        private int m_type;
        private int m_value;

        public PetAttackAroundEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.PetPlusAllTwoMpEquipEffect, elementID)
        {
            this.m_count = count;
            this.m_probability = (probability == -1) ? 0x2710 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 0x4e:
                    this.m_value = 200;
                    break;

                case 0x4f:
                    this.m_value = 400;
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

        private void player_AfterKilledByLiving(Living living)
        {
            if (base.rand.Next(0x2710) < this.m_probability)
            {
                List<Living> list = living.Game.Map.FindAllNearestEnemy(living.X, living.Y, 110.0, living);
                foreach (Living living2 in list)
                {
                    living2.SyncAtTime = true;
                    living2.AddBlood(-this.m_value, 1);
                    living2.SyncAtTime = false;
                    if (living2.Blood <= 0)
                    {
                        living2.Die();
                    }
                }
            }
        }

        public override bool Start(Living living)
        {
            PetAttackAroundEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetPlusAllTwoMpEquipEffect) as PetAttackAroundEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetBurningBloodShootingEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_currentId;
        private int m_value;

        public PetBurningBloodShootingEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetBurningBloodShootingEquip, elementID)
        {
            this.m_count = count;
            this.m_currentId = skillId;
            switch (skillId)
            {
                case 110:
                    this.m_value = 800;
                    break;

                case 0x6f:
                    this.m_value = 0x3e8;
                    break;
            }
        }

        private void ChangeProperty(Player living)
        {
            if (living.ShootCount == 1)
            {
                living.SyncAtTime = true;
                living.AddBlood(-this.m_value, 1);
                living.SyncAtTime = false;
                if (living.Blood <= 0)
                {
                    living.Die();
                }
            }
        }

        public override void OnAttached(Living living)
        {
            (living as Player).PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            (living as Player).PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
        }

        private void player_BeginFitting(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetBurningBloodShootingEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetBurningBloodShootingEquip) as PetBurningBloodShootingEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


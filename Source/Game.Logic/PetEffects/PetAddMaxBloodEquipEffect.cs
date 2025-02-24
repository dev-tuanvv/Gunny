namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddMaxBloodEquipEffect : AbstractPetEffect
    {
        private int m_added;
        private int m_count;

        public PetAddMaxBloodEquipEffect(int count, int skilId, string elementID) : base(ePetEffectType.PetAddMaxBloodEquipEffect, elementID)
        {
            this.m_count = count;
            switch (skilId)
            {
                case 0x59:
                    this.m_added = 0x3e8;
                    break;

                case 90:
                    this.m_added = 0x7d0;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            Player player1 = living as Player;
            player1.MaxBlood += this.m_added;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            Player player1 = living as Player;
            player1.MaxBlood -= this.m_added;
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
            PetAddMaxBloodEquipEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddMaxBloodEquipEffect) as PetAddMaxBloodEquipEffect;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


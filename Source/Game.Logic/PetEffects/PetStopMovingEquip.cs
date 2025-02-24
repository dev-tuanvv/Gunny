namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetStopMovingEquip : AbstractPetEffect
    {
        private int m_count;

        public PetStopMovingEquip(int count, string elementID) : base(ePetEffectType.PetStopMovingEquip, elementID)
        {
            this.m_count = count;
        }

        public override void OnAttached(Living living)
        {
            living.SpeedMultX(0);
            living.PetEffects.StopMoving = true;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.SpeedMultX(3);
            living.PetEffects.StopMoving = false;
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
            PetStopMovingEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetStopMovingEquip) as PetStopMovingEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


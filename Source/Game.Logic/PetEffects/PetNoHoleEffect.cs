namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetNoHoleEffect : AbstractPetEffect
    {
        private int m_count;

        public PetNoHoleEffect(int count, string elementID) : base(ePetEffectType.NoHoleEquipEffect, elementID)
        {
            this.m_count = count;
        }

        public override void OnAttached(Living living)
        {
            living.IsNoHole = true;
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
            living.Game.SendPlayerPicture(living, 5, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
            living.IsNoHole = false;
            living.Game.SendPlayerPicture(living, 5, false);
        }

        private void player_BeginFitting(Living player)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetNoHoleEffect ofType = living.PetEffectList.GetOfType(ePetEffectType.NoHoleEquipEffect) as PetNoHoleEffect;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


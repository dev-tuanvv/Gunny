namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class BasePetEffect : AbstractPetEffect
    {
        public BasePetEffect(ePetEffectType type, string elementId) : base(type, elementId)
        {
        }

        public sealed override void OnAttached(Living living)
        {
            if (living is Player)
            {
                this.OnAttachedToPlayer(living as Player);
            }
        }

        protected virtual void OnAttachedToPlayer(Player player)
        {
        }

        public sealed override void OnRemoved(Living living)
        {
            if (living is Player)
            {
                this.OnRemovedFromPlayer(living as Player);
            }
        }

        protected virtual void OnRemovedFromPlayer(Player player)
        {
        }

        public override bool Start(Living living)
        {
            return ((living is Player) && base.Start(living));
        }
    }
}


namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddDamageEquip : AbstractPetEffect
    {
        private int m_count;

        public PetAddDamageEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAddDamageEquip, elementID)
        {
            this.m_count = count;
        }

        public override void OnAttached(Living player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        public override void OnRemoved(Living player)
        {
            player.BeginSelfTurn -= new LivingEventHandle(this.player_SelfTurn);
        }

        private void player_SelfTurn(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                Player player1 = living as Player;
                player1.BaseDamage -= living.PetEffects.AddDameValue;
                living.PetEffects.AddDameValue = 0;
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetAddDamageEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddDamageEquip) as PetAddDamageEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


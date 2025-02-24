namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetRemoveDamageEquip : AbstractPetEffect
    {
        private int m_count;

        public PetRemoveDamageEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetRemoveDamageEquip, elementID)
        {
            this.m_count = count;
        }

        public override void OnAttached(Living player)
        {
            player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        public override void OnRemoved(Living player)
        {
            player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (((living.Game.RoomType == eRoomType.Match) || (living.Game.RoomType == eRoomType.Freedom)) || (living.Game.RoomType == eRoomType.RingStation))
            {
                damageAmount = 0;
                criticalAmount = 0;
            }
        }

        private void player_SelfTurn(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetRemoveDamageEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetRemoveDamageEquip) as PetRemoveDamageEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


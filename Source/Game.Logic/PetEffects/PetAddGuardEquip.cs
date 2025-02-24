namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddGuardEquip : AbstractPetEffect
    {
        private int m_added;
        private int m_count;

        public PetAddGuardEquip(int count, int skilId, string elementID) : base(ePetEffectType.PetAddGuardEquip, elementID)
        {
            this.m_count = count;
            switch (skilId)
            {
                case 0xb5:
                    this.m_added = 100;
                    break;

                case 0xb6:
                    this.m_added = 200;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            if (((living.Game.RoomType == eRoomType.Match) || (living.Game.RoomType == eRoomType.Freedom)) || (living.Game.RoomType == eRoomType.RingStation))
            {
                living.BaseGuard += this.m_added;
            }
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            if (((living.Game.RoomType == eRoomType.Match) || (living.Game.RoomType == eRoomType.Freedom)) || (living.Game.RoomType == eRoomType.RingStation))
            {
                living.BaseGuard -= this.m_added;
            }
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
            PetAddGuardEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddGuardEquip) as PetAddGuardEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


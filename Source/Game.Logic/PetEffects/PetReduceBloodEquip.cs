namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetReduceBloodEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetReduceBloodEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetReduceBloodEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0xbd:
                    this.m_value = 800;
                    break;

                case 190:
                    this.m_value = 0x3e8;
                    break;
            }
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
        }

        private void player_BeginFitting(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
            else if (((living.Game.RoomType == eRoomType.Match) || (living.Game.RoomType == eRoomType.Freedom)) || (living.Game.RoomType == eRoomType.RingStation))
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

        public override bool Start(Living living)
        {
            PetReduceBloodEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceBloodEquip) as PetReduceBloodEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


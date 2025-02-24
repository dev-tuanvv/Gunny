namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddBloodSelfEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetAddBloodSelfEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAddBloodSelfEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0xb5:
                    this.m_value = 800;
                    break;

                case 0xb6:
                    this.m_value = 0x3e8;
                    break;
            }
        }

        public override void OnAttached(Living player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        public override void OnRemoved(Living player)
        {
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        private void player_SelfTurn(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
            else if (((living.Game.RoomType == eRoomType.Match) || (living.Game.RoomType == eRoomType.Freedom)) || (living.Game.RoomType == eRoomType.RingStation))
            {
                living.SyncAtTime = true;
                living.AddBlood(this.m_value);
                living.SyncAtTime = false;
            }
        }

        public override bool Start(Living living)
        {
            PetAddBloodSelfEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodSelfEquip) as PetAddBloodSelfEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


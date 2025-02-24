namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddBloodEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetAddBloodEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAddBloodEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x5d:
                    this.m_value = 500;
                    break;

                case 0x5e:
                    this.m_value = 0x3e8;
                    break;

                case 0xac:
                    this.m_value = 0x5dc;
                    break;

                case 0xad:
                    this.m_value = 0x9c4;
                    break;
            }
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
                living.SyncAtTime = true;
                living.AddBlood(this.m_value);
                living.SyncAtTime = false;
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
            PetAddBloodEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodEquip) as PetAddBloodEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


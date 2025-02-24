namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetUnlimitAddBloodEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetUnlimitAddBloodEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAddBloodEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x1f:
                    this.m_value = 500;
                    break;

                case 0x20:
                    this.m_value = 700;
                    break;

                case 0x21:
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
            else
            {
                living.SyncAtTime = true;
                living.AddBlood(this.m_value);
                living.SyncAtTime = false;
            }
        }

        public override bool Start(Living living)
        {
            PetUnlimitAddBloodEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetUnlimitAddBloodEquip) as PetUnlimitAddBloodEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


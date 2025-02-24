namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetAddBloodForSelfEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetAddBloodForSelfEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAddBloodForSelfEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x3d:
                    this.m_value = 500;
                    break;

                case 0x3e:
                    this.m_value = 800;
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
                living.Game.SendPetBuff(living, base.Info, false);
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
            PetAddBloodForSelfEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodForSelfEquip) as PetAddBloodForSelfEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


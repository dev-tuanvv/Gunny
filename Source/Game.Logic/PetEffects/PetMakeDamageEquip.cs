namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetMakeDamageEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetMakeDamageEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetMakeDamageEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x41:
                    this.m_value = 400;
                    break;

                case 0x42:
                    this.m_value = 800;
                    break;

                case 0xa6:
                    this.m_value = 600;
                    break;

                case 0xa7:
                    this.m_value = 0x4b0;
                    break;
            }
        }

        public override void OnAttached(Living player)
        {
            player.AfterKilledByLiving += new KillLivingEventHanlde(this.player_BeforeTakeDamage);
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        public override void OnRemoved(Living player)
        {
            player.AfterKilledByLiving -= new KillLivingEventHanlde(this.player_BeforeTakeDamage);
            player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
        }

        private void player_BeforeTakeDamage(Living living, Living source, int damageAmount, int criticalAmount)
        {
            if (source != living)
            {
                source.SyncAtTime = true;
                source.AddBlood(-this.m_value, 1);
                source.SyncAtTime = false;
                if (source.Blood <= 0)
                {
                    source.Die();
                }
            }
        }

        private void player_SelfTurn(Living living)
        {
            this.m_count--;
            if (this.m_count < 0)
            {
                living.Game.SendPetBuff(living, base.Info, false);
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetMakeDamageEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetMakeDamageEquip) as PetMakeDamageEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


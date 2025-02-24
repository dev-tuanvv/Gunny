namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class PetReduceBloodAllBattleEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetReduceBloodAllBattleEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetReduceBloodAllBattleEquip, elementID)
        {
            this.m_count = count;
            if (skillId > 0x83)
            {
                switch (skillId)
                {
                    case 150:
                        goto Label_00C3;

                    case 0x97:
                    case 190:
                        this.m_value = 0x3e8;
                        break;

                    case 0xbd:
                        goto Label_00D0;
                }
            }
            else
            {
                switch (skillId)
                {
                    case 0x67:
                        this.m_value = 200;
                        break;

                    case 0x68:
                        this.m_value = 300;
                        break;

                    case 0x69:
                    case 130:
                        goto Label_00C3;

                    case 0x83:
                        goto Label_00D0;
                }
            }
            return;
        Label_00C3:
            this.m_value = 500;
            return;
        Label_00D0:
            this.m_value = 800;
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
            else
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
            PetReduceBloodAllBattleEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetReduceBloodAllBattleEquip) as PetReduceBloodAllBattleEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


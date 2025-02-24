namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetAddBloodAllPlayerAroundEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_value;

        public PetAddBloodAllPlayerAroundEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetAddBloodAllPlayerAroundEquip, elementID)
        {
            this.m_count = count;
            switch (skillId)
            {
                case 0x2b:
                    this.m_value = 300;
                    break;

                case 0x2d:
                    this.m_value = 500;
                    break;

                case 0x2e:
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
                this.Stop();
            }
            else
            {
                living.SyncAtTime = true;
                living.AddBlood(this.m_value);
                living.SyncAtTime = false;
                List<Living> list = living.Game.Map.FindAllNearestSameTeam(living.X, living.Y, 150.0, living);
                foreach (Living living2 in list)
                {
                    if (living2 != living)
                    {
                        living2.SyncAtTime = true;
                        living2.AddBlood(this.m_value);
                        living2.SyncAtTime = false;
                    }
                }
            }
        }

        public override bool Start(Living living)
        {
            PetAddBloodAllPlayerAroundEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodAllPlayerAroundEquip) as PetAddBloodAllPlayerAroundEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


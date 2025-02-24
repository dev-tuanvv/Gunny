namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class RecoverBloodEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;
        private int m_type;

        public RecoverBloodEffect(int count, int probability, int type) : base(eEffectType.RecoverBloodEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
            this.m_type = type;
        }

        public void ChangeProperty(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (living.IsLiving)
            {
                base.IsTrigger = false;
                if ((base.rand.Next(100) < this.m_probability) && (living.DefendActiveGem == this.m_type))
                {
                    base.IsTrigger = true;
                    living.EffectTrigger = true;
                    living.SyncAtTime = true;
                    living.AddBlood(this.m_count);
                    living.SyncAtTime = false;
                    living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success", new object[0]));
                    living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("RecoverBloodEffect.msg", new object[0]), 9, 0, 0x3e8));
                }
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.AfterKilledByLiving += new KillLivingEventHanlde(this.ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.AfterKilledByLiving -= new KillLivingEventHanlde(this.ChangeProperty);
        }

        public override bool Start(Living living)
        {
            RecoverBloodEffect ofType = living.EffectList.GetOfType(eEffectType.RecoverBloodEffect) as RecoverBloodEffect;
            if (ofType == null)
            {
                return base.Start(living);
            }
            this.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
            return true;
        }
    }
}


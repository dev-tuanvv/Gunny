namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class ReflexDamageEquipEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;

        public ReflexDamageEquipEffect(int count, int probability) : base(eEffectType.ReflexDamageEquipEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
        }

        public void ChangeProperty(Living living)
        {
            base.IsTrigger = false;
            if (base.rand.Next(100) < this.m_probability)
            {
                base.IsTrigger = true;
                living.EffectTrigger = true;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("ReflexDamageEquipEffect.Success", new object[] { this.m_count }));
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginAttacked += new LivingEventHandle(this.ChangeProperty);
            player.AfterKilledByLiving += new KillLivingEventHanlde(this.player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginAttacked -= new LivingEventHandle(this.ChangeProperty);
        }

        private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (base.IsTrigger)
            {
                target.AddBlood(-this.m_count);
            }
        }

        public override bool Start(Living living)
        {
            ReflexDamageEquipEffect ofType = living.EffectList.GetOfType(eEffectType.ReflexDamageEquipEffect) as ReflexDamageEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class AvoidDamageEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;
        private int m_type;

        public AvoidDamageEffect(int count, int probability, int type) : base(eEffectType.AvoidDamageEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
            this.m_type = type;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
        }

        private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            base.IsTrigger = false;
            if ((base.rand.Next(100) < this.m_probability) && (living.DefendActiveGem == this.m_type))
            {
                base.IsTrigger = true;
                living.EffectTrigger = true;
                damageAmount = (damageAmount * (100 - this.m_count)) / 100;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success", new object[0]));
                living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AvoidDamageEffect.msg", new object[0]), 9, 0, 0x3e8));
            }
        }

        public override bool Start(Living living)
        {
            AvoidDamageEffect ofType = living.EffectList.GetOfType(eEffectType.AvoidDamageEffect) as AvoidDamageEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


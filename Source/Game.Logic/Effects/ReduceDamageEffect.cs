namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class ReduceDamageEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;
        private int m_type;

        public ReduceDamageEffect(int count, int probability, int type) : base(eEffectType.ReduceDamageEffect)
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
                damageAmount -= this.m_count;
                if ((damageAmount -= this.m_count) <= 0)
                {
                    damageAmount = 1;
                }
                living.EffectTrigger = true;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success", new object[0]));
                living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("ReduceDamageEffect.msg", new object[0]), 9, 0, 0x3e8));
            }
        }

        public override bool Start(Living living)
        {
            ReduceDamageEffect ofType = living.EffectList.GetOfType(eEffectType.ReduceDamageEffect) as ReduceDamageEffect;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}


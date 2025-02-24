namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class MakeCriticalEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;

        public MakeCriticalEffect(int count, int probability) : base(eEffectType.MakeCriticalEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
        }

        private void player_AfterPlayerShooted(Player player)
        {
            player.FlyingPartical = 0;
        }

        private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (!(living as Player).CurrentBall.IsSpecial())
            {
                base.IsTrigger = false;
                if ((base.rand.Next(100) < this.m_probability) && (living.AttackGemLimit == 0))
                {
                    living.AttackGemLimit = 4;
                    base.IsTrigger = true;
                    living.EffectTrigger = true;
                    criticalAmount = (int) (0.5 + ((living.Lucky * 0.0005) * ((double) damageAmount)));
                    living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
                    living.FlyingPartical = 0x41;
                    living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("MakeCriticalEffect.msg", new object[0]), 9, 0, 0x3e8));
                }
            }
        }

        public override bool Start(Living living)
        {
            MakeCriticalEffect ofType = living.EffectList.GetOfType(eEffectType.MakeCriticalEffect) as MakeCriticalEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


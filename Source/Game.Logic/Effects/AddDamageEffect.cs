namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class AddDamageEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;

        public AddDamageEffect(int count, int probability) : base(eEffectType.AddDamageEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            player.PlayerShoot += new PlayerEventHandle(this.playerShot);
            player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
            player.PlayerShoot -= new PlayerEventHandle(this.playerShot);
            player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
        }

        private void player_AfterPlayerShooted(Player player)
        {
            player.FlyingPartical = 0;
        }

        private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (base.IsTrigger)
            {
                damageAmount += this.m_count;
            }
        }

        private void playerShot(Player player)
        {
            if (!player.CurrentBall.IsSpecial())
            {
                base.IsTrigger = false;
                if ((base.rand.Next(100) < this.m_probability) && (player.AttackGemLimit == 0))
                {
                    player.AttackGemLimit = 4;
                    base.IsTrigger = true;
                    player.FlyingPartical = 0x41;
                    player.EffectTrigger = true;
                    player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
                    player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddDamageEffect.msg", new object[0]), 9, 0, 0x3e8));
                }
            }
        }

        public override bool Start(Living living)
        {
            AddDamageEffect ofType = living.EffectList.GetOfType(eEffectType.AddDamageEffect) as AddDamageEffect;
            if (ofType != null)
            {
                this.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


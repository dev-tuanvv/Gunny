namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class AddLuckyEffect : BasePlayerEffect
    {
        private int m_added;
        private int m_count;
        private int m_probability;

        public AddLuckyEffect(int count, int probability) : base(eEffectType.AddLuckyEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
            this.m_added = 0;
        }

        private void ChangeProperty(Player player)
        {
            if (!player.CurrentBall.IsSpecial())
            {
                player.Lucky -= this.m_added;
                this.m_added = 0;
                base.IsTrigger = false;
                if ((base.rand.Next(100) < this.m_probability) && (player.AttackGemLimit == 0))
                {
                    player.AttackGemLimit = 4;
                    player.FlyingPartical = 0x41;
                    base.IsTrigger = true;
                    player.Lucky += this.m_count;
                    player.EffectTrigger = true;
                    this.m_added = this.m_count;
                    player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
                    player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddLuckyEffect.msg", new object[0]), 9, 0, 0x3e8));
                }
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeforePlayerShoot += new PlayerEventHandle(this.ChangeProperty);
            player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeforePlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
            player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
        }

        private void player_AfterPlayerShooted(Player player)
        {
            player.FlyingPartical = 0;
        }

        public override bool Start(Living living)
        {
            AddLuckyEffect ofType = living.EffectList.GetOfType(eEffectType.AddLuckyEffect) as AddLuckyEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


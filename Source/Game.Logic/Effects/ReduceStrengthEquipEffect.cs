namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class ReduceStrengthEquipEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;

        public ReduceStrengthEquipEffect(int count, int probability) : base(eEffectType.ReduceStrengthEquipEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
        }

        private void ChangeProperty(Player player)
        {
            if (!player.CurrentBall.IsSpecial())
            {
                base.IsTrigger = false;
                if ((base.rand.Next(100) < this.m_probability) && (player.AttackGemLimit == 0))
                {
                    player.AttackGemLimit = 4;
                    base.IsTrigger = true;
                    player.EffectTrigger = true;
                    player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
                    player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("ReduceStrengthEquipEffect.msg", new object[0]), 9, 0, 0x3e8));
                }
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
            player.AfterKillingLiving += new KillLivingEventHanlde(this.player_AfterKillingLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
            player.AfterKillingLiving -= new KillLivingEventHanlde(this.player_AfterKillingLiving);
        }

        private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (base.IsTrigger)
            {
                target.AddEffect(new ReduceStrengthEffect(2, this.m_count), 0);
            }
        }

        public override bool Start(Living living)
        {
            ReduceStrengthEquipEffect ofType = living.EffectList.GetOfType(eEffectType.ReduceStrengthEquipEffect) as ReduceStrengthEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


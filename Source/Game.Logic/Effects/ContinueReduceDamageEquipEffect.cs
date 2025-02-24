namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class ContinueReduceDamageEquipEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;

        public ContinueReduceDamageEquipEffect(int count, int probability) : base(eEffectType.ContinueReduceBaseDamageEquipEffect)
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
                    player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("ContinueReduceBaseDamageEquipEffect.msg", new object[0]), 9, 0, 0x3e8));
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
                target.AddEffect(new ContinueReduceDamageEffect(2), 0);
            }
        }

        public override bool Start(Living living)
        {
            ContinueReduceDamageEquipEffect ofType = living.EffectList.GetOfType(eEffectType.ContinueReduceBaseDamageEquipEffect) as ContinueReduceDamageEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


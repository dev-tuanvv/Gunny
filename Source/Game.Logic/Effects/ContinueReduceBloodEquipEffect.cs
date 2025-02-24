namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class ContinueReduceBloodEquipEffect : BasePlayerEffect
    {
        private int m_blood;
        private int m_probability;

        public ContinueReduceBloodEquipEffect(int blood, int probability) : base(eEffectType.ContinueReduceBloodEquipEffect)
        {
            this.m_blood = blood;
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
                    player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("ContinueReduceBloodEquipEffect.msg", new object[0]), 9, 0, 0x3e8));
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

        protected void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (base.IsTrigger)
            {
                target.AddEffect(new ContinueReduceBloodEffect(2, this.m_blood, living), 0);
            }
        }

        public override bool Start(Living living)
        {
            ContinueReduceBloodEquipEffect ofType = living.EffectList.GetOfType(eEffectType.ContinueReduceBloodEquipEffect) as ContinueReduceBloodEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


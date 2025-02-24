namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class AddBombEquipEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;
        private bool m_show;

        public AddBombEquipEffect(int count, int probability) : base(eEffectType.AddBombEquipEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
        }

        private void ChangeProperty(Living living)
        {
            if (!(living as Player).CurrentBall.IsSpecial())
            {
                base.IsTrigger = false;
                if ((base.rand.Next(100) < this.m_probability) && (living.AttackGemLimit == 0))
                {
                    this.m_show = true;
                    living.AttackGemLimit = 4;
                    base.IsTrigger = true;
                    Player player1 = living as Player;
                    player1.ShootCount += this.m_count;
                    living.EffectTrigger = true;
                }
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerShoot += new PlayerEventHandle(this.playerShot);
            player.BeginAttacking += new LivingEventHandle(this.ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(this.playerShot);
            player.BeginAttacking -= new LivingEventHandle(this.ChangeProperty);
        }

        private void playerShot(Player player)
        {
            if (base.IsTrigger && this.m_show)
            {
                player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddBombEquipEffect.msg", new object[0]), 9, 0, 0x3e8));
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
                this.m_show = false;
            }
        }

        public override bool Start(Living living)
        {
            AddBombEquipEffect ofType = living.EffectList.GetOfType(eEffectType.AddBombEquipEffect) as AddBombEquipEffect;
            if (ofType != null)
            {
                this.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class AtomBombEquipEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;

        public AtomBombEquipEffect(int count, int probability) : base(eEffectType.AtomBomb)
        {
            this.m_count = count;
            this.m_probability = probability;
        }

        private void ChangeProperty(Player player)
        {
            if (!player.CurrentBall.IsSpecial() && ((base.rand.Next(100) < this.m_probability) && (player.AttackGemLimit == 0)))
            {
                player.SetBall(4);
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
                player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AtomBombEquipEffect.msg", new object[0]), 9, 0, 0x3e8));
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
        }

        public override bool Start(Living living)
        {
            AtomBombEquipEffect ofType = living.EffectList.GetOfType(eEffectType.AtomBomb) as AtomBombEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


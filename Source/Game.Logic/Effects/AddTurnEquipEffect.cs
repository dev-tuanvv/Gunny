namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class AddTurnEquipEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;
        private int m_templateID;

        public AddTurnEquipEffect(int count, int probability, int templateID) : base(eEffectType.AddTurnEquipEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
            this.m_templateID = templateID;
        }

        private void ChangeProperty(Player player)
        {
            if (!player.CurrentBall.IsSpecial() && ((base.rand.Next(100) < this.m_probability) && (player.AttackGemLimit == 0)))
            {
                player.AttackGemLimit = 4;
                player.Delay = player.DefaultDelay;
                base.IsTrigger = true;
                player.EffectTrigger = true;
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
                player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddTurnEquipEffect.msg", new object[0]), 9, 0, 0x3e8));
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
            player.BeginNextTurn += new LivingEventHandle(this.player_BeginSelfTurn);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
            player.BeginNextTurn -= new LivingEventHandle(this.player_BeginSelfTurn);
        }

        public void player_BeginSelfTurn(Living living)
        {
            if (base.IsTrigger && (living is Player))
            {
                int num = 0;
                int templateID = this.m_templateID;
                if (templateID <= 0x4c010)
                {
                    if (templateID <= 0x4bf59)
                    {
                        switch (templateID)
                        {
                            case 0x4bf48:
                                num = 130;
                                break;

                            case 0x4bf59:
                                num = 0x91;
                                break;
                        }
                    }
                    else if (templateID == 0x4bfac)
                    {
                        num = 160;
                    }
                    else if (templateID == 0x4bfbd)
                    {
                        num = 0xaf;
                    }
                    else if (templateID == 0x4c010)
                    {
                        num = 190;
                    }
                }
                else
                {
                    switch (templateID)
                    {
                        case 0x4c021:
                            num = 0xcd;
                            break;

                        case 0x4c074:
                            num = 220;
                            break;

                        case 0x4c085:
                            num = 0xf5;
                            break;

                        case 0x4c0d8:
                            num = 260;
                            break;

                        case 0x4c0e9:
                            num = 0x109;
                            break;
                    }
                }
                Player player1 = living as Player;
                player1.Delay += ((living as Player).Delay * this.m_count) / 100;
                (living as Player).Energy = num;
                base.IsTrigger = false;
            }
        }

        public override bool Start(Living living)
        {
            AddTurnEquipEffect ofType = living.EffectList.GetOfType(eEffectType.AddTurnEquipEffect) as AddTurnEquipEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


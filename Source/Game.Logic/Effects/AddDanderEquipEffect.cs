namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class AddDanderEquipEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;
        private int m_type;

        public AddDanderEquipEffect(int count, int probability, int type) : base(eEffectType.AddDander)
        {
            this.m_count = count;
            this.m_probability = probability;
            this.m_type = type;
        }

        private void ChangeProperty(Living living)
        {
            base.IsTrigger = false;
            if ((base.rand.Next(100) < this.m_probability) && (living.DefendActiveGem == this.m_type))
            {
                base.IsTrigger = true;
                if (living is Player)
                {
                    (living as Player).AddDander(this.m_count);
                }
                living.EffectTrigger = true;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success", new object[0]));
                living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AddDanderEquipEffect.msg", new object[0]), 9, 0, 0x3e8));
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginAttacked += new LivingEventHandle(this.ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginAttacked -= new LivingEventHandle(this.ChangeProperty);
        }

        public override bool Start(Living living)
        {
            AddDanderEquipEffect ofType = living.EffectList.GetOfType(eEffectType.AddDander) as AddDanderEquipEffect;
            if (ofType != null)
            {
                this.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


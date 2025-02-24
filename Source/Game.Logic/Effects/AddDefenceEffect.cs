namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using System;

    public class AddDefenceEffect : BasePlayerEffect
    {
        private int m_added;
        private int m_count;
        private int m_probability;
        private int m_type;

        public AddDefenceEffect(int count, int probability, int type) : base(eEffectType.AddDefenceEffect)
        {
            this.m_count = count;
            this.m_probability = probability;
            this.m_added = 0;
            this.m_type = type;
        }

        public void ChangeProperty(Living living)
        {
            living.Defence -= this.m_added;
            this.m_added = 0;
            base.IsTrigger = false;
            if ((base.rand.Next(100) < this.m_probability) && (living.DefendActiveGem == this.m_type))
            {
                base.IsTrigger = true;
                living.Defence += this.m_count;
                this.m_added = this.m_count;
                living.EffectTrigger = true;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success", new object[0]));
                living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AddDefenceEffect.msg", new object[0]), 9, 0x3e8, 0x3e8));
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
            AddDefenceEffect ofType = living.EffectList.GetOfType(eEffectType.AddDefenceEffect) as AddDefenceEffect;
            if (ofType != null)
            {
                ofType.m_probability = (this.m_probability > ofType.m_probability) ? this.m_probability : ofType.m_probability;
                return true;
            }
            return base.Start(living);
        }
    }
}


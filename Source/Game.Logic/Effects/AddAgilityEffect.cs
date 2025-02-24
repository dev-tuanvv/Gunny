namespace Game.Logic.Effects
{
    using Bussiness;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class AddAgilityEffect : BasePlayerEffect
    {
        private int m_added;
        private int m_count;
        private int m_probablity;

        public AddAgilityEffect(int count, int probability) : base(eEffectType.AddAgilityEffect)
        {
            this.m_count = count;
            this.m_probablity = probability;
        }

        private void ChangeProperty(Living living)
        {
            living.Agility -= this.m_added;
            this.m_added = 0;
            base.IsTrigger = false;
            if (base.rand.Next(100) < this.m_probablity)
            {
                living.EffectTrigger = true;
                base.IsTrigger = true;
                living.Agility += this.m_count;
                this.m_added = this.m_count;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddAgilityEffect.Success", new object[] { this.m_count }));
            }
        }

        private void DefaultProperty(Living living)
        {
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginAttacking += new LivingEventHandle(this.ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginAttacking -= new LivingEventHandle(this.ChangeProperty);
        }

        public override bool Start(Living living)
        {
            AddAgilityEffect ofType = living.EffectList.GetOfType(eEffectType.AddAgilityEffect) as AddAgilityEffect;
            if (ofType != null)
            {
                this.m_probablity = (this.m_probablity > ofType.m_probablity) ? this.m_probablity : ofType.m_probablity;
                return true;
            }
            return base.Start(living);
        }
    }
}


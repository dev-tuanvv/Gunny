namespace Game.Logic.Effects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public abstract class AbstractEffect
    {
        public bool IsTrigger;
        protected Living m_living;
        private eEffectType m_type;
        protected Random rand = new Random();

        public AbstractEffect(eEffectType type)
        {
            this.m_type = type;
        }

        public virtual void OnAttached(Living living)
        {
        }

        public virtual void OnRemoved(Living living)
        {
        }

        public virtual bool Start(Living living)
        {
            this.m_living = living;
            return this.m_living.EffectList.Add(this);
        }

        public virtual bool Stop()
        {
            return ((this.m_living != null) && this.m_living.EffectList.Remove(this));
        }

        public eEffectType Type
        {
            get
            {
                return this.m_type;
            }
        }

        public int TypeValue
        {
            get
            {
                return (int) this.m_type;
            }
        }
    }
}


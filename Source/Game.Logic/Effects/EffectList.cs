namespace Game.Logic.Effects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using log4net;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Reflection;

    public class EffectList
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected volatile sbyte m_changesCount;
        protected ArrayList m_effects;
        protected int m_immunity;
        protected readonly Living m_owner;

        public EffectList(Living owner, int immunity)
        {
            this.m_owner = owner;
            this.m_effects = new ArrayList(3);
            this.m_immunity = immunity;
        }

        public virtual bool Add(AbstractEffect effect)
        {
            if (this.CanAddEffect(effect.TypeValue))
            {
                lock (this.m_effects)
                {
                    this.m_effects.Add(effect);
                }
                effect.OnAttached(this.m_owner);
                this.OnEffectsChanged(effect);
                return true;
            }
            if ((effect.TypeValue == 9) && (this.m_owner is SimpleBoss))
            {
                this.m_owner.State = 0;
            }
            return false;
        }

        public void BeginChanges()
        {
            this.m_changesCount = (sbyte) (this.m_changesCount + 1);
        }

        public bool CanAddEffect(int id)
        {
            return (((id > 40) || (id < 0)) || (((((int) 1) << (id - 1)) & this.m_immunity) == 0));
        }

        public virtual void CommitChanges()
        {
            if ((this.m_changesCount = (sbyte) (this.m_changesCount - 1)) < 0)
            {
                if (log.IsWarnEnabled)
                {
                    log.Warn("changes count is less than zero, forgot BeginChanges()?\n" + Environment.StackTrace);
                }
                this.m_changesCount = 0;
            }
            if (this.m_changesCount == 0)
            {
                this.UpdateChangedEffects();
            }
        }

        public virtual IList GetAllOfType(Type effectType)
        {
            ArrayList list = new ArrayList();
            lock (this.m_effects)
            {
                foreach (AbstractEffect effect in this.m_effects)
                {
                    if (effect.GetType().Equals(effectType))
                    {
                        list.Add(effect);
                    }
                }
            }
            return list;
        }

        public virtual AbstractEffect GetOfType(eEffectType effectType)
        {
            lock (this.m_effects)
            {
                foreach (AbstractEffect effect in this.m_effects)
                {
                    if (effect.Type == effectType)
                    {
                        return effect;
                    }
                }
            }
            return null;
        }

        public virtual void OnEffectsChanged(AbstractEffect changedEffect)
        {
            if (this.m_changesCount <= 0)
            {
                this.UpdateChangedEffects();
            }
        }

        public virtual bool Remove(AbstractEffect effect)
        {
            int index = -1;
            lock (this.m_effects)
            {
                index = this.m_effects.IndexOf(effect);
                if (index < 0)
                {
                    return false;
                }
                this.m_effects.RemoveAt(index);
            }
            if (index != -1)
            {
                effect.OnRemoved(this.m_owner);
                this.OnEffectsChanged(effect);
                return true;
            }
            return false;
        }

        public void StopAllEffect()
        {
            if (this.m_effects.Count > 0)
            {
                AbstractEffect[] array = new AbstractEffect[this.m_effects.Count];
                this.m_effects.CopyTo(array);
                foreach (AbstractEffect effect in array)
                {
                    effect.Stop();
                }
                this.m_effects.Clear();
            }
        }

        public void StopEffect(Type effectType)
        {
            IList allOfType = this.GetAllOfType(effectType);
            this.BeginChanges();
            foreach (AbstractEffect effect in allOfType)
            {
                effect.Stop();
            }
            this.CommitChanges();
        }

        protected virtual void UpdateChangedEffects()
        {
        }

        public ArrayList List
        {
            get
            {
                return this.m_effects;
            }
        }
    }
}


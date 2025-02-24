namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using SqlDataProvider.Data;
    using System;

    public abstract class AbstractPetEffect
    {
        public bool IsTrigger;
        private PetSkillElementInfo m_info;
        protected Living m_living;
        private ePetEffectType m_type;
        protected Random rand = new Random();

        public AbstractPetEffect(ePetEffectType type, string ElementID)
        {
            this.m_type = type;
            this.m_info = PetMgr.FindPetSkillElement(int.Parse(ElementID));
            if (this.m_info == null)
            {
                this.m_info = new PetSkillElementInfo();
                this.m_info.EffectPic = "";
                this.m_info.Pic = -1;
                this.m_info.Value = 1;
            }
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
            return this.m_living.PetEffectList.Add(this);
        }

        public virtual bool Stop()
        {
            return ((this.m_living != null) && this.m_living.PetEffectList.Remove(this));
        }

        public PetSkillElementInfo Info
        {
            get
            {
                return this.m_info;
            }
        }

        public ePetEffectType Type
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


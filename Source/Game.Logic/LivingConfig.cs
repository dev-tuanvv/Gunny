namespace Game.Logic
{
    using System;
    using System.Runtime.CompilerServices;

    public class LivingConfig
    {
        public bool HaveShield;
        private bool m_DamageForzen;
        private byte m_isBotom;
        private bool m_isConsortiaBoss;
        private bool m_isChristmasBoss;
        private bool m_isFly;
        private bool m_isHelper;
        private bool m_isShowBlood;
        private bool m_isShowSmallMapPoint;
        private bool m_isTurn;
        private bool m_isWorldBoss;
        private int m_reduceBloodStart;

        public void Clone(LivingConfig _clone)
        {
            this.m_isHelper = _clone.IsHelper;
            this.CanTakeDamage = _clone.CanTakeDamage;
            this.HaveShield = _clone.HaveShield;
            this.isBotom = _clone.isBotom;
            this.isConsortiaBoss = _clone.isConsortiaBoss;
            this.IsFly = _clone.IsFly;
            this.isShowBlood = _clone.isShowBlood;
            this.isShowSmallMapPoint = _clone.isShowSmallMapPoint;
            this.IsTurn = _clone.IsTurn;
            this.ReduceBloodStart = _clone.ReduceBloodStart;
            this.DamageForzen = _clone.DamageForzen;
        }

        public bool CanTakeDamage { get; set; }

        public bool DamageForzen
        {
            get
            {
                return this.m_DamageForzen;
            }
            set
            {
                this.m_DamageForzen = value;
            }
        }

        public byte isBotom
        {
            get
            {
                return this.m_isBotom;
            }
            set
            {
                this.m_isBotom = value;
            }
        }

        public bool isConsortiaBoss
        {
            get
            {
                return this.m_isConsortiaBoss;
            }
            set
            {
                this.m_isConsortiaBoss = value;
            }
        }

        public bool IsChristmasBoss
        {
            get
            {
                return this.m_isChristmasBoss;
            }
            set
            {
                this.m_isChristmasBoss = value;
            }
        }

        public bool IsFly
        {
            get
            {
                return this.m_isFly;
            }
            set
            {
                this.m_isFly = value;
            }
        }

        public bool IsHelper
        {
            get
            {
                return this.m_isHelper;
            }
            set
            {
                this.m_isHelper = value;
            }
        }

        public bool isShowBlood
        {
            get
            {
                return this.m_isShowBlood;
            }
            set
            {
                this.m_isShowBlood = value;
            }
        }

        public bool isShowSmallMapPoint
        {
            get
            {
                return this.m_isShowSmallMapPoint;
            }
            set
            {
                this.m_isShowSmallMapPoint = value;
            }
        }

        public bool IsTurn
        {
            get
            {
                return this.m_isTurn;
            }
            set
            {
                this.m_isTurn = value;
            }
        }

        public bool IsWorldBoss
        {
            get
            {
                return this.m_isWorldBoss;
            }
            set
            {
                this.m_isWorldBoss = value;
            }
        }

        public int ReduceBloodStart
        {
            get
            {
                return this.m_reduceBloodStart;
            }
            set
            {
                this.m_reduceBloodStart = value;
            }
        }
    }
}


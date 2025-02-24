namespace Game.Logic
{
    using System;

    public class KhoiTaoKhaNangConfig
    {
        private bool m_giuBong;
        private bool m_isBay;
        private bool m_isBrother;
        private bool m_isWorldBoss;
        private bool m_luomBong;

        public bool GiuBong
        {
            get
            {
                return this.m_giuBong;
            }
            set
            {
                this.m_giuBong = value;
            }
        }

        public bool IsBay
        {
            get
            {
                return this.m_isBay;
            }
            set
            {
                this.m_isBay = value;
            }
        }

        public bool IsBrother
        {
            get
            {
                return this.m_isBrother;
            }
            set
            {
                this.m_isBrother = value;
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

        public bool LuomBong
        {
            get
            {
                return this.m_luomBong;
            }
            set
            {
                this.m_luomBong = value;
            }
        }
    }
}


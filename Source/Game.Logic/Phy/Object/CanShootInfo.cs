namespace Game.Logic.Phy.Object
{
    using System;

    public class CanShootInfo
    {
        private int m_angle;
        private bool m_canShoot;
        private int m_force;

        public CanShootInfo(bool canShoot, int force, int angle)
        {
            this.m_canShoot = canShoot;
            this.m_force = force;
            this.m_angle = angle;
        }

        public int Angle
        {
            get
            {
                return this.m_angle;
            }
        }

        public bool CanShoot
        {
            get
            {
                return this.m_canShoot;
            }
        }

        public int Force
        {
            get
            {
                return this.m_force;
            }
        }
    }
}


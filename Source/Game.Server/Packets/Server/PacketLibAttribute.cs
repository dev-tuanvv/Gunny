namespace Game.Base.Packets
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class PacketLibAttribute : Attribute
    {
        private int m_rawVersion;

        public PacketLibAttribute(int rawVersion)
        {
            this.m_rawVersion = rawVersion;
        }

        public int RawVersion
        {
            get
            {
                return this.m_rawVersion;
            }
        }
    }
}


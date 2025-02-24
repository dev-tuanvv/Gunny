namespace Game.Server.HotSpringRooms.TankHandle
{
    using System;
    using System.Runtime.CompilerServices;

    public class HotSpringCommandAttbute : Attribute
    {
        [CompilerGenerated]
        private byte byte_0;

        public HotSpringCommandAttbute(byte code)
        {
            this.Code = code;
        }

        public byte Code
        {
            [CompilerGenerated]
            get
            {
                return this.byte_0;
            }
            [CompilerGenerated]
            private set
            {
                this.byte_0 = value;
            }
        }
    }
}


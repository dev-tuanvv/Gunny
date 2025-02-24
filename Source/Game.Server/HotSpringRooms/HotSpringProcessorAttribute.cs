namespace Game.Server.HotSpringRooms
{
    using System;

    public class HotSpringProcessorAttribute : Attribute
    {
        private byte byte_0;
        private string string_0;

        public HotSpringProcessorAttribute(byte code, string description)
        {
            this.byte_0 = code;
            this.string_0 = description;
        }

        public byte Code
        {
            get
            {
                return this.byte_0;
            }
        }

        public string Description
        {
            get
            {
                return this.string_0;
            }
        }
    }
}


namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class ConsortiaBufferInfo
    {
        public DateTime BeginDate { get; set; }

        public int BufferID { get; set; }

        public int ConsortiaID { get; set; }

        public bool IsOpen { get; set; }

        public int Type { get; set; }

        public int ValidDate { get; set; }

        public int Value { get; set; }
    }
}


namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class ConsortiaEventInfo
    {
        public int ConsortiaID { get; set; }

        public DateTime Date { get; set; }

        public int ID { get; set; }

        public bool IsExist { get; set; }

        public string Remark { get; set; }

        public int Type { get; set; }
    }
}


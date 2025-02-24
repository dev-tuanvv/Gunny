namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class QuestConditionInfo
    {
        public int Tagert()
        {
            if ((this.CondictionType == 20) && (this.Para1 != 3))
            {
                return 0;
            }
            return this.Para2;
        }

        public int CondictionID { get; set; }

        public string CondictionTitle { get; set; }

        public int CondictionType { get; set; }

        public bool isOpitional { get; set; }

        public int Para1 { get; set; }

        public int Para2 { get; set; }

        public int QuestID { get; set; }
    }
}


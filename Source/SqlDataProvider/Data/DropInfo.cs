namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class DropInfo
    {
        public DropInfo(int id, int time, int count, int maxCount)
        {
            this.ID = id;
            this.Time = time;
            this.Count = count;
            this.MaxCount = maxCount;
        }

        public int Count { get; set; }

        public int ID { get; set; }

        public int MaxCount { get; set; }

        public int Time { get; set; }
    }
}


namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class EventRewardInfo
    {
        public int ActivityType { get; set; }

        public List<EventRewardGoodsInfo> AwardLists { get; set; }

        public int Condition { get; set; }

        public int SubActivityType { get; set; }
    }
}


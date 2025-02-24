namespace SqlDataProvider.Data
{
    using System;

    public class EventRewardProcessInfo : DataObject
    {
        private int int_0;
        private int int_1;
        private int int_2;
        private int int_3;

        public int ActiveType
        {
            get
            {
                return this.int_1;
            }
            set
            {
                this.int_1 = value;
                base._isDirty = true;
            }
        }

        public int AwardGot
        {
            get
            {
                return this.int_3;
            }
            set
            {
                this.int_3 = value;
                base._isDirty = true;
            }
        }

        public int Conditions
        {
            get
            {
                return this.int_2;
            }
            set
            {
                this.int_2 = value;
                base._isDirty = true;
            }
        }

        public int UserID
        {
            get
            {
                return this.int_0;
            }
            set
            {
                this.int_0 = value;
                base._isDirty = true;
            }
        }
    }
}


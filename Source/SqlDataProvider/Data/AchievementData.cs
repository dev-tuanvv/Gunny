namespace SqlDataProvider.Data
{
    using System;

    public class AchievementData : DataObject
    {
        private bool bool_0;
        private DateTime dateTime_0;
        private int int_0;
        private int int_1;

        public AchievementData()
        {
            this.UserID = 0;
            this.AchievementID = 0;
            this.IsComplete = false;
            this.DateComplete = DateTime.Now;
        }

        public int AchievementID
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

        public DateTime DateComplete
        {
            get
            {
                return this.dateTime_0;
            }
            set
            {
                this.dateTime_0 = value;
                base._isDirty = true;
            }
        }

        public bool IsComplete
        {
            get
            {
                return this.bool_0;
            }
            set
            {
                this.bool_0 = value;
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


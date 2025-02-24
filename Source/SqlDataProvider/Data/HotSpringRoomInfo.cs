namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class HotSpringRoomInfo
    {
        [CompilerGenerated]
        private DateTime dateTime_0;
        [CompilerGenerated]
        private DateTime dateTime_1;
        [CompilerGenerated]
        private int FxiFhyuqdi8;
        [CompilerGenerated]
        private int int_0;
        [CompilerGenerated]
        private int int_1;
        [CompilerGenerated]
        private int int_2;
        [CompilerGenerated]
        private int int_3;
        [CompilerGenerated]
        private int int_4;
        [CompilerGenerated]
        private int int_5;
        [CompilerGenerated]
        private string string_0;
        [CompilerGenerated]
        private string string_1;
        [CompilerGenerated]
        private string string_2;
        [CompilerGenerated]
        private string string_3;

        public bool CanEnter()
        {
            return (this.curCount < this.maxCount);
        }

        public int curCount
        {
            [CompilerGenerated]
            get
            {
                return this.int_3;
            }
            [CompilerGenerated]
            set
            {
                this.int_3 = value;
            }
        }

        public int effectiveTime
        {
            [CompilerGenerated]
            get
            {
                return this.int_2;
            }
            [CompilerGenerated]
            set
            {
                this.int_2 = value;
            }
        }

        public DateTime endTime
        {
            [CompilerGenerated]
            get
            {
                return this.dateTime_1;
            }
            [CompilerGenerated]
            set
            {
                this.dateTime_1 = value;
            }
        }

        public int maxCount
        {
            [CompilerGenerated]
            get
            {
                return this.int_5;
            }
            [CompilerGenerated]
            set
            {
                this.int_5 = value;
            }
        }

        public int playerID
        {
            [CompilerGenerated]
            get
            {
                return this.int_4;
            }
            [CompilerGenerated]
            set
            {
                this.int_4 = value;
            }
        }

        public string playerName
        {
            [CompilerGenerated]
            get
            {
                return this.string_2;
            }
            [CompilerGenerated]
            set
            {
                this.string_2 = value;
            }
        }

        public int roomID
        {
            [CompilerGenerated]
            get
            {
                return this.int_1;
            }
            [CompilerGenerated]
            set
            {
                this.int_1 = value;
            }
        }

        public string roomIntroduction
        {
            [CompilerGenerated]
            get
            {
                return this.string_3;
            }
            [CompilerGenerated]
            set
            {
                this.string_3 = value;
            }
        }

        public string roomName
        {
            [CompilerGenerated]
            get
            {
                return this.string_0;
            }
            [CompilerGenerated]
            set
            {
                this.string_0 = value;
            }
        }

        public int roomNumber
        {
            [CompilerGenerated]
            get
            {
                return this.int_0;
            }
            [CompilerGenerated]
            set
            {
                this.int_0 = value;
            }
        }

        public string roomPassword
        {
            [CompilerGenerated]
            get
            {
                return this.string_1;
            }
            [CompilerGenerated]
            set
            {
                this.string_1 = value;
            }
        }

        public int roomType
        {
            [CompilerGenerated]
            get
            {
                return this.FxiFhyuqdi8;
            }
            [CompilerGenerated]
            set
            {
                this.FxiFhyuqdi8 = value;
            }
        }

        public DateTime startTime
        {
            [CompilerGenerated]
            get
            {
                return this.dateTime_0;
            }
            [CompilerGenerated]
            set
            {
                this.dateTime_0 = value;
            }
        }
    }
}


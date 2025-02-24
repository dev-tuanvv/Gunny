namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class RuneTemplateInfo
    {
        public bool IsAttack()
        {
            switch (((this.Type1 == 0x25) ? this.Type2 : this.Type1))
            {
                case 1:
                case 4:
                case 5:
                case 8:
                case 9:
                case 11:
                case 12:
                case 14:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x15:
                case 0x16:
                case 0x17:
                case 0x18:
                case 0x19:
                    return true;
            }
            return false;
        }

        public bool IsDefend()
        {
            int num2 = (this.Type1 == 0x27) ? this.Type2 : this.Type1;
            if (num2 <= 10)
            {
                if (((num2 != 2) && (num2 != 6)) && (num2 != 10))
                {
                    return false;
                }
            }
            else
            {
                switch (num2)
                {
                    case 13:
                    case 15:
                        goto Label_0080;

                    case 14:
                        return false;
                }
                if ((num2 != 0x13) && (num2 != 0x1a))
                {
                    return false;
                }
            }
        Label_0080:
            return true;
        }

        public bool IsProp()
        {
            switch (this.Type1)
            {
                case 0x1f:
                case 0x20:
                case 0x21:
                case 0x22:
                case 0x23:
                case 0x24:
                    return true;
            }
            return false;
        }

        public string Attribute1 { get; set; }

        public string Attribute2 { get; set; }

        public string Attribute3 { get; set; }

        public int BaseLevel { get; set; }

        public int MaxLevel { get; set; }

        public string Name { get; set; }

        public int NextTemplateID { get; set; }

        public int Rate1 { get; set; }

        public int Rate2 { get; set; }

        public int Rate3 { get; set; }

        public int TemplateID { get; set; }

        public int Turn1 { get; set; }

        public int Turn2 { get; set; }

        public int Turn3 { get; set; }

        public int Type1 { get; set; }

        public int Type2 { get; set; }

        public int Type3 { get; set; }
    }
}


namespace SqlDataProvider.Data
{
    using System;

    public class Equip
    {
        public static bool isAvatar(ItemTemplateInfo info)
        {
            switch (info.TemplateID)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 13:
                case 15:
                    return true;
            }
            return false;
        }

        public static bool isDress(ItemTemplateInfo info)
        {
            return false;
        }

        public static bool isMagicStone(ItemTemplateInfo info)
        {
            bool flag = false;
            if (info.CategoryID == 0x3d)
            {
                flag = true;
            }
            return flag;
        }

        public static bool isShowImp(ItemTemplateInfo info)
        {
            switch (info.CategoryID)
            {
                case 5:
                case 7:
                    return true;

                case 1:
                    return true;
            }
            return false;
        }

        public static bool isWeddingRing(ItemTemplateInfo info)
        {
            int templateID = info.TemplateID;
            if (templateID <= 0x2406)
            {
                if (((templateID != 0x233e) && (templateID != 0x23a2)) && (templateID != 0x2406))
                {
                    return false;
                }
            }
            else if (((((templateID != 0x246a) && (templateID != 0x24ce)) && ((templateID != 0x2532) && (templateID != 0x2596))) && ((templateID != 0x25fa) && (templateID != 0x265e))) && (templateID != 0x26c2))
            {
                return false;
            }
            return true;
        }
    }
}


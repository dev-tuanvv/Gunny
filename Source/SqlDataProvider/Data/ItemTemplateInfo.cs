namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class ItemTemplateInfo : DataObject
    {
        public string AddTime { get; set; }

        public int Agility { get; set; }

        public int Attack { get; set; }

        public eBageType BagType
        {
            get
            {
                int categoryID = this.CategoryID;
                if (categoryID <= 20)
                {
                    switch (categoryID)
                    {
                        case 10:
                        case 11:
                        case 12:
                            goto Label_007D;
                    }
                    if (categoryID != 20)
                    {
                        return eBageType.EquipBag;
                    }
                }
                else
                {
                    switch (categoryID)
                    {
                        case 0x20:
                            return eBageType.FarmBag;

                        case 0x21:
                            return eBageType.EquipBag;

                        case 0x22:
                        case 0x23:
                            goto Label_007D;
                    }
                    if (categoryID != 0x3d)
                    {
                        return eBageType.EquipBag;
                    }
                    return eBageType.MagicStone;
                }
            Label_007D:
                if ((this.Property1 == 0x1f) && (this.CategoryID == 11))
                {
                    return eBageType.BeadBag;
                }
                return eBageType.PropBag;
            }
        }

        public int BindType { get; set; }

        public bool CanCompose { get; set; }

        public bool CanDelete { get; set; }

        public bool CanDrop { get; set; }

        public bool CanEquip { get; set; }

        public int CanRecycle { get; set; }

        public bool CanStrengthen { get; set; }

        public bool CanUse { get; set; }

        public int CategoryID { get; set; }

        public string Colors { get; set; }

        public string Data { get; set; }

        public int Defence { get; set; }

        public string Description { get; set; }

        public int FloorPrice { get; set; }

        public int FusionNeedRate { get; set; }

        public int FusionRate { get; set; }

        public int FusionType { get; set; }

        public string Hole { get; set; }

        public int Level { get; set; }

        public int Luck { get; set; }

        public int MaxCount { get; set; }

        public string Name { get; set; }

        public int NeedLevel { get; set; }

        public int NeedSex { get; set; }

        public string Pic { get; set; }

        public int Property1 { get; set; }

        public int Property2 { get; set; }

        public int Property3 { get; set; }

        public int Property4 { get; set; }

        public int Property5 { get; set; }

        public int Property6 { get; set; }

        public int Property7 { get; set; }

        public int Property8 { get; set; }

        public int Quality { get; set; }

        public int ReclaimType { get; set; }

        public int ReclaimValue { get; set; }

        public int RefineryLevel { get; set; }

        public int RefineryType { get; set; }

        public string Script { get; set; }

        public int SuitId { get; set; }

        public int TemplateID { get; set; }
    }
}


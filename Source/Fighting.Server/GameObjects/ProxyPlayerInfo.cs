namespace Fighting.Server.GameObjects
{
    using Bussiness.Managers;
    using SqlDataProvider.Data;
    using System;
    using System.Runtime.CompilerServices;

    public class ProxyPlayerInfo
    {
        public SqlDataProvider.Data.ItemInfo GetHealstone()
        {
            SqlDataProvider.Data.ItemInfo info = null;
            if (this.Healstone != 0)
            {
                info = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(this.Healstone), 1, 1);
                info.Count = this.HealstoneCount;
            }
            return info;
        }

        public SqlDataProvider.Data.ItemInfo GetItemInfo()
        {
            SqlDataProvider.Data.ItemInfo info = null;
            if (this.SecondWeapon != 0)
            {
                info = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(this.SecondWeapon), 1, 1);
                info.StrengthenLevel = this.StrengthLevel;
            }
            return info;
        }

        public ItemTemplateInfo GetItemTemplateInfo()
        {
            return ItemMgr.FindItemTemplate(this.TemplateId);
        }

        public double AntiAddictionRate { get; set; }

        public float AuncherExperienceRate { get; set; }

        public float AuncherOfferRate { get; set; }

        public float AuncherRichesRate { get; set; }

        public double BaseAgility { get; set; }

        public double BaseAttack { get; set; }

        public double BaseBlood { get; set; }

        public double BaseDefence { get; set; }

        public bool CanUserProp { get; set; }

        public bool CanX2Exp { get; set; }

        public bool CanX3Exp { get; set; }

        public string FightFootballStyle { get; set; }

        public float GMExperienceRate { get; set; }

        public float GMOfferRate { get; set; }

        public float GMRichesRate { get; set; }

        public double GPAddPlus { get; set; }

        public int Healstone { get; set; }

        public int HealstoneCount { get; set; }

        public double OfferAddPlus { get; set; }

        public int SecondWeapon { get; set; }

        public int ServerId { get; set; }

        public int StrengthLevel { get; set; }

        public int TemplateId { get; set; }

        public int ZoneId { get; set; }

        public string ZoneName { get; set; }
    }
}


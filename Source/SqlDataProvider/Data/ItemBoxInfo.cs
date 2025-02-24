﻿namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class ItemBoxInfo : DataObject
    {
        public int AgilityCompose { get; set; }

        public int AttackCompose { get; set; }

        public int DataId { get; set; }

        public int DefendCompose { get; set; }

        public int Id { get; set; }

        public bool IsBind { get; set; }

        public bool IsLogs { get; set; }

        public bool IsSelect { get; set; }

        public int IsTips { get; set; }

        public int ItemCount { get; set; }

        public int ItemValid { get; set; }

        public int LuckCompose { get; set; }

        public int MagicAttack { get; set; }

        public int MagicDefence { get; set; }

        public int Random { get; set; }

        public int StrengthenLevel { get; set; }

        public int TemplateId { get; set; }
    }
}


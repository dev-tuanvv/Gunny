﻿namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class LoadUserBoxInfo : DataObject
    {
        public int Condition { get; set; }

        public int ID { get; set; }

        public int Level { get; set; }

        public int TemplateID { get; set; }

        public int Type { get; set; }
    }
}


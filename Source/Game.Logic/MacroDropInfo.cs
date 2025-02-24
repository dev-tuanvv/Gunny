namespace Game.Logic
{
    using System;
    using System.Runtime.CompilerServices;

    public class MacroDropInfo
    {
        public MacroDropInfo(int dropCount, int maxDropCount)
        {
            this.DropCount = dropCount;
            this.MaxDropCount = maxDropCount;
        }

        public int DropCount { get; set; }

        public int MaxDropCount { get; set; }

        public int SelfDropCount { get; set; }
    }
}


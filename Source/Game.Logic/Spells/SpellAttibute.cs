namespace Game.Logic.Spells
{
    using System;
    using System.Runtime.CompilerServices;

    public class SpellAttibute : Attribute
    {
        public SpellAttibute(int type)
        {
            this.Type = type;
        }

        public int Type { get; private set; }
    }
}


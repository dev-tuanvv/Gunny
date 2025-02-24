namespace Game.Logic.Phy.Object
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void LivingTakedDamageEventHandle(Living living, Living source, ref int damageAmount, ref int criticalAmount);
}


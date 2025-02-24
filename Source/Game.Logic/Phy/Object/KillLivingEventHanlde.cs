namespace Game.Logic.Phy.Object
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void KillLivingEventHanlde(Living living, Living target, int damageAmount, int criticalAmount);
}


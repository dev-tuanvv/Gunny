namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x54, "副武器")]
    public class SecondWeaponCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            player.UseSecondWeapon();
        }
    }
}


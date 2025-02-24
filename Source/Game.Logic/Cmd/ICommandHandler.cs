namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public interface ICommandHandler
    {
        void HandleCommand(BaseGame game, Player player, GSPacketIn packet);
    }
}


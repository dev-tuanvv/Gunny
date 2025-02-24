namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x95, "使用道具")]
    public class GameTrusteeshipCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if ((game.GameState == eGameState.Playing) && !player.GetSealState())
            {
                packet.ReadBoolean();
            }
        }
    }
}


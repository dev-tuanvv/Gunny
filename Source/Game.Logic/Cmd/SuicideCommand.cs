namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x11, "自杀")]
    public class SuicideCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsLiving && (game.GameState == eGameState.Playing))
            {
                packet.Parameter1 = player.Id;
                game.SendToAll(packet);
                player.Die();
                game.CheckState(0);
            }
        }
    }
}


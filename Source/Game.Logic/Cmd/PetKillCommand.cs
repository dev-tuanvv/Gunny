namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x90, "使用道具")]
    public class PetKillCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if ((game.GameState == eGameState.Playing) && !player.GetSealState())
            {
                int skillID = packet.ReadInt();
                player.PetUseKill(skillID);
            }
        }
    }
}


namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x74, "关卡准备")]
    public class MissionPrepareCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if ((game.GameState == eGameState.SessionPrepared) || (game.GameState == eGameState.GameOver))
            {
                bool flag = packet.ReadBoolean();
                if (player.Ready != flag)
                {
                    player.Ready = flag;
                    game.SendToAll(packet);
                }
            }
        }
    }
}


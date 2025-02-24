namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x10, "游戏加载进度")]
    public class LoadCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game.GameState == eGameState.Loading)
            {
                player.LoadingProcess = packet.ReadInt();
                if (player.LoadingProcess >= 100)
                {
                    game.CheckState(0);
                }
                if (player.LoadingProcess < 100)
                {
                }
                GSPacketIn pkg = new GSPacketIn(0x5b);
                pkg.WriteByte(0x10);
                pkg.WriteInt(player.LoadingProcess);
                pkg.WriteInt(player.PlayerDetail.ZoneId);
                pkg.WriteInt(player.PlayerDetail.PlayerCharacter.ID);
                game.SendToAll(pkg);
            }
        }
    }
}


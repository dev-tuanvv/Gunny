namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x60, "准备开炮")]
    public class FireTagCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                packet.Parameter1 = player.Id;
                game.SendToAll(packet);
                game.SendSyncLifeTime();
                if (packet.ReadBoolean())
                {
                    byte speedTime = packet.ReadByte();
                    player.PrepareShoot(speedTime);
                }
            }
        }
    }
}


namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(9, "开始移动")]
    public class MoveStartCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                byte type = packet.ReadByte();
                int x = packet.ReadInt();
                int y = packet.ReadInt();
                byte dir = packet.ReadByte();
                bool isLiving = packet.ReadBoolean();
                short num5 = packet.ReadShort();
                int turnIndex = game.TurnIndex;
                game.SendPlayerMove(player, type, x, y, dir);
                switch (type)
                {
                    case 0:
                    case 1:
                        player.SetXY(x, y);
                        player.StartMoving();
                        if (((player.Y - y) > 1) || (player.IsLiving != isLiving))
                        {
                            game.SendPlayerMove(player, 3, player.X, player.Y, dir, isLiving);
                        }
                        break;
                }
            }
        }
    }
}


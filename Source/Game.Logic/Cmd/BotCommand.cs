using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.BOT_COMMAND, "战胜关卡中Boss翻牌")]
    public class BotCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game is PVPGame)
            {
                PVPGame pvp = game as PVPGame;
                Player[] players = pvp.GetAllPlayers();
                List<Player> enemies = new List<Player>();
                foreach (Player child in players)
                {
                    if (child.Team != player.Team)
                    {
                        enemies.Add(child);
                    }
                }
                Random rand = new Random();
                int next = rand.Next(0, enemies.Count);
                Player target = enemies.ElementAt(next);

                if (target.X > player.X)
                {
                    player.ChangeDirection(1, 500);
                }
                else
                {
                    player.ChangeDirection(-1, 500);
                }
                player.ShootPoint(target.X, target.Y, player.CurrentBall.ID, 1001, 10001, 1, 1.5f, 2000);
                if (player.IsAttacking)
                    player.StopAttacking();
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
                pkg.WriteByte((byte)eTankCmdType.BOT_COMMAND);
                game.SendToAll(pkg);
                Console.WriteLine("BombId {0}", player.CurrentBall.ID);
            }
        }
    }
}
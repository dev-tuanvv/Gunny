namespace Game.Server.Commands.Admin
{
    using Game.Base;
    using Game.Logic;
    using Game.Server;
    using Game.Server.Battle;
    using Game.Server.GameObjects;
    using Game.Server.Games;
    using Game.Server.Managers;
    using Game.Server.Rooms;
    using System;
    using System.Collections.Generic;

    [Cmd("&list", ePrivLevel.Player, "List the objects info in game", new string[] { "   /list [Option1][Option2] ...", "eg:    /list -g :list all game objects", "       /list -c :list all client objects", "       /list -p :list all gameplaye objects", "       /list -r :list all room objects", "       /list -b :list all battle servers" })]
    public class ListObjectsCommand : AbstractCommandHandler, ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "-c":
                    {
                        Console.WriteLine("client list:");
                        Console.WriteLine("-------------------------------");
                        GameClient[] allClients = GameServer.Instance.GetAllClients();
                        foreach (GameClient client2 in allClients)
                        {
                            Console.WriteLine(client2.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", allClients.Length));
                        return true;
                    }
                    case "-p":
                    {
                        Console.WriteLine("player list:");
                        Console.WriteLine("-------------------------------");
                        GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
                        foreach (GamePlayer player in allPlayers)
                        {
                            Console.WriteLine(player.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", allPlayers.Length));
                        return true;
                    }
                    case "-r":
                    {
                        Console.WriteLine("room list:");
                        Console.WriteLine("-------------------------------");
                        List<BaseRoom> allUsingRoom = RoomMgr.GetAllUsingRoom();
                        foreach (BaseRoom room in allUsingRoom)
                        {
                            Console.WriteLine(room.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", allUsingRoom.Count));
                        return true;
                    }
                    case "-g":
                    {
                        Console.WriteLine("game list:");
                        Console.WriteLine("-------------------------------");
                        List<BaseGame> allGame = GameMgr.GetAllGame();
                        foreach (BaseGame game in allGame)
                        {
                            Console.WriteLine(game.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", allGame.Count));
                        return true;
                    }
                    case "-b":
                    {
                        Console.WriteLine("battle list:");
                        Console.WriteLine("-------------------------------");
                        List<BattleServer> allBattles = BattleMgr.GetAllBattles();
                        foreach (BattleServer server in allBattles)
                        {
                            Console.WriteLine(server.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", allBattles.Count));
                        return true;
                    }
                }
                this.DisplaySyntax(client);
            }
            else
            {
                this.DisplaySyntax(client);
            }
            return true;
        }
    }
}


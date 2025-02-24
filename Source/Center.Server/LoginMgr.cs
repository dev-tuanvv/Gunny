namespace Center.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LoginMgr
    {
        private static Dictionary<int, Player> m_players = new Dictionary<int, Player>();
        private static object syc_obj = new object();

        public static void CreatePlayer(Player player)
        {
            Player playerByName = null;
            lock (syc_obj)
            {
                player.LastTime = DateTime.Now.Ticks;
                if (m_players.ContainsKey(player.Id))
                {
                    playerByName = m_players[player.Id];
                    player.State = playerByName.State;
                    player.CurrentServer = playerByName.CurrentServer;
                    m_players[player.Id] = player;
                }
                else
                {
                    playerByName = GetPlayerByName(player.Name);
                    if ((playerByName != null) && m_players.ContainsKey(playerByName.Id))
                    {
                        m_players.Remove(playerByName.Id);
                    }
                    player.State = ePlayerState.NotLogin;
                    m_players.Add(player.Id, player);
                }
            }
            if ((playerByName != null) && (playerByName.CurrentServer != null))
            {
                playerByName.CurrentServer.SendKitoffUser(playerByName.Id);
            }
        }

        public static Player[] GetAllPlayer()
        {
            lock (syc_obj)
            {
                return m_players.Values.ToArray<Player>();
            }
        }

        public static int GetOnlineCount()
        {
            Player[] allPlayer = GetAllPlayer();
            int num = 0;
            foreach (Player player in allPlayer)
            {
                if (player.State != ePlayerState.NotLogin)
                {
                    num++;
                }
            }
            return num;
        }

        public static Dictionary<int, int> GetOnlineForLine()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            foreach (Player player in GetAllPlayer())
            {
                if (player.CurrentServer != null)
                {
                    if (dictionary.ContainsKey(player.CurrentServer.Info.ID))
                    {
                        Dictionary<int, int> dictionary2;
                        int num2;
                        (dictionary2 = dictionary)[num2 = player.CurrentServer.Info.ID] = dictionary2[num2] + 1;
                    }
                    else
                    {
                        dictionary.Add(player.CurrentServer.Info.ID, 1);
                    }
                }
            }
            return dictionary;
        }

        public static Player GetPlayer(int playerId)
        {
            lock (syc_obj)
            {
                if (m_players.ContainsKey(playerId))
                {
                    return m_players[playerId];
                }
            }
            return null;
        }

        public static Player GetPlayerByName(string name)
        {
            Player[] allPlayer = GetAllPlayer();
            if (allPlayer != null)
            {
                foreach (Player player in allPlayer)
                {
                    if (player.Name == name)
                    {
                        return player;
                    }
                }
            }
            return null;
        }

        public static ServerClient GetServerClient(int playerId)
        {
            Player player = GetPlayer(playerId);
            if (player != null)
            {
                return player.CurrentServer;
            }
            return null;
        }

        public static List<Player> GetServerPlayers(ServerClient server)
        {
            List<Player> list = new List<Player>();
            foreach (Player player in GetAllPlayer())
            {
                if (player.CurrentServer == server)
                {
                    list.Add(player);
                }
            }
            return list;
        }

        public static void PlayerLogined(int id, ServerClient server)
        {
            lock (syc_obj)
            {
                if (m_players.ContainsKey(id))
                {
                    Player player = m_players[id];
                    if (player != null)
                    {
                        player.CurrentServer = server;
                        player.State = ePlayerState.Play;
                    }
                }
            }
        }

        public static void PlayerLoginOut(int id, ServerClient server)
        {
            lock (syc_obj)
            {
                if (m_players.ContainsKey(id))
                {
                    Player player = m_players[id];
                    if ((player != null) && (player.CurrentServer == server))
                    {
                        player.CurrentServer = null;
                        player.State = ePlayerState.NotLogin;
                    }
                }
            }
        }

        public static void RemovePlayer(int playerId)
        {
            lock (syc_obj)
            {
                if (m_players.ContainsKey(playerId))
                {
                    m_players.Remove(playerId);
                }
            }
        }

        public static void RemovePlayer(List<Player> players)
        {
            lock (syc_obj)
            {
                foreach (Player player in players)
                {
                    m_players.Remove(player.Id);
                }
            }
        }

        public static bool TryLoginPlayer(int id, ServerClient server)
        {
            lock (syc_obj)
            {
                if (m_players.ContainsKey(id))
                {
                    Player player = m_players[id];
                    if (player.CurrentServer == null)
                    {
                        player.CurrentServer = server;
                        player.State = ePlayerState.Logining;
                        return true;
                    }
                    if (player.State == ePlayerState.Play)
                    {
                        player.CurrentServer.SendKitoffUser(id);
                    }
                    return false;
                }
                return false;
            }
        }
    }
}


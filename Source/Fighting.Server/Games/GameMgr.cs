namespace Fighting.Server.Games
{
    using Bussiness;
    using Fighting.Server.GameObjects;
    using Fighting.Server.Rooms;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Maps;
    using Game.Logic.Phy.Object;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class GameMgr
    {
        private static readonly int CLEAR_GAME_INTERVAL = 0xea60;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static int m_boxBroadcastLevel;
        private static long m_clearGamesTimer;
        private static int m_gameId;
        private static Dictionary<int, BaseGame> m_games;
        private static bool m_running;
        private static int m_serverId;
        private static Thread m_thread;
        public static readonly long THREAD_INTERVAL = 40L;

        public static bool CanUseBuff(eGameType gameType)
        {
            if (gameType <= eGameType.ALL)
            {
                if ((gameType != eGameType.Free) && (gameType != eGameType.ALL))
                {
                    return false;
                }
            }
            else if ((gameType != eGameType.Encounter) && (gameType != eGameType.BattleGame))
            {
                return false;
            }
            return true;
        }

        private static void ClearStoppedGames(long tick)
        {
            if (m_clearGamesTimer < 1)
            {
                m_clearGamesTimer += CLEAR_GAME_INTERVAL;
                ArrayList list = new ArrayList();
                lock (m_games)
                {
                    foreach (BaseGame game in m_games.Values)
                    {
                        if (game.GameState == eGameState.Stopped)
                        {
                            list.Add(game);
                        }
                    }
                    foreach (BaseGame game2 in list)
                    {
                        m_games.Remove(game2.Id);
                        try
                        {
                            game2.Dispose();
                        }
                        catch (Exception exception)
                        {
                            log.Error("game dispose error:", exception);
                        }
                    }
                }
            }
        }

        public static BaseGame FindGame(int id)
        {
            lock (m_games)
            {
                if (m_games.ContainsKey(id))
                {
                    return m_games[id];
                }
            }
            return null;
        }

        private static void GameThread()
        {
            long num = 0L;
            m_clearGamesTimer = TickHelper.GetTickCount();
            while (m_running)
            {
                long tickCount = TickHelper.GetTickCount();
                try
                {
                    UpdateGames(tickCount);
                    ClearStoppedGames(tickCount);
                }
                catch (Exception exception)
                {
                    log.Error("Room Mgr Thread Error:", exception);
                }
                long num3 = TickHelper.GetTickCount();
                num += THREAD_INTERVAL - (num3 - tickCount);
                if (num > 0L)
                {
                    Thread.Sleep((int) num);
                    num = 0L;
                }
                else if (num < -1000L)
                {
                    log.WarnFormat("Room Mgr is delay {0} ms!", num);
                    num += 0x3e8L;
                }
            }
        }

        public static List<BaseGame> GetGames()
        {
            List<BaseGame> list = new List<BaseGame>();
            lock (m_games)
            {
                list.AddRange(m_games.Values);
            }
            return list;
        }

        public static GSPacketIn SendBufferList(Player player, List<BufferInfo> infos)
        {
            GSPacketIn @in = new GSPacketIn(0xba, player.Id);
            @in.WriteInt(infos.Count);
            foreach (BufferInfo info in infos)
            {
                @in.WriteInt(info.Type);
                @in.WriteBoolean(info.IsExist);
                @in.WriteDateTime(info.BeginDate);
                @in.WriteInt(info.ValidDate);
                @in.WriteInt(info.Value);
            }
            return @in;
        }

        public static void SendStartMessage(BattleGame game)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(2);
            if (CanUseBuff(game.GameType))
            {
                foreach (Player player in game.GetAllFightPlayers())
                {
                    (player.PlayerDetail as ProxyPlayer).m_antiAddictionRate = 1.0;
                    GSPacketIn in2 = SendBufferList(player, (player.PlayerDetail as ProxyPlayer).Buffers);
                    game.SendToAll(in2);
                }
                pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.free", new object[0]));
            }
            else if (game.RoomType == eRoomType.FightFootballTime)
            {
                pkg.WriteString(LanguageMgr.GetTranslation("Gh\x00e9p cặp th\x00e0nh c\x00f4ng, quyết chiến bắt đầu.", new object[0]));
            }
            else
            {
                pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.free", new object[0]));
                log.Warn(LanguageMgr.GetTranslation("CanUseBuff = false,  code:-121212", new object[0]));
            }
            game.SendToAll(pkg, null);
            Console.WriteLine(string.Format("Create {0} with {1} player, createID {2}", game.RoomType, game.PlayerCount, game.Id));
        }

        public static bool Setup(int serverId, int boxBroadcastLevel)
        {
            m_thread = new Thread(new ThreadStart(GameMgr.GameThread));
            m_games = new Dictionary<int, BaseGame>();
            m_serverId = serverId;
            m_boxBroadcastLevel = boxBroadcastLevel;
            m_gameId = 0;
            return true;
        }

        public static void Start()
        {
            if (!m_running)
            {
                m_running = true;
                m_thread.Start();
            }
        }

        public static BattleGame StartBattleGame(List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
        {
            try
            {
                Map map = MapMgr.CloneMap(MapMgr.GetMapIndex(mapIndex, (byte) roomType, m_serverId));
                if (map != null)
                {
                    int num2;
                    for (num2 = 0; num2 < red.Count; num2++)
                    {
                        PlayerInfo playerCharacter = red[num2].PlayerCharacter;
                        playerCharacter.Defence += Convert.ToInt16((int) (red[num2].PlayerCharacter.Defence / 5));
                    }
                    for (num2 = 0; num2 < blue.Count; num2++)
                    {
                        PlayerInfo info2 = blue[num2].PlayerCharacter;
                        info2.Defence += Convert.ToInt16((int) (blue[num2].PlayerCharacter.Defence / 5));
                    }
                    BattleGame game2 = new BattleGame(m_gameId++, red, roomRed, blue, roomBlue, map, roomType, gameType, timeType);
                    lock (m_games)
                    {
                        m_games.Add(game2.Id, game2);
                    }
                    game2.Prepare();
                    SendStartMessage(game2);
                    return game2;
                }
                return null;
            }
            catch (Exception exception)
            {
                log.Error("Create battle game error:", exception);
                return null;
            }
        }

        public static BaseGame StartPVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
        {
            try
            {
                Map map = MapMgr.CloneMap(MapMgr.GetMapIndex(mapIndex, (byte) roomType, m_serverId));
                if (map != null)
                {
                    PVPGame game2 = new PVPGame(m_gameId++, 0, red, blue, map, roomType, gameType, timeType);
                    lock (m_games)
                    {
                        m_games.Add(game2.Id, game2);
                    }
                    return game2;
                }
                return null;
            }
            catch (Exception exception)
            {
                log.Error("Create game error:", exception);
                return null;
            }
        }

        public static void Stop()
        {
            if (m_running)
            {
                m_running = false;
                m_thread.Join();
            }
        }

        private static void UpdateGames(long tick)
        {
            IList games = GetGames();
            if (games != null)
            {
                foreach (BaseGame game in games)
                {
                    try
                    {
                        game.Update(tick);
                    }
                    catch (Exception exception)
                    {
                        log.Error("Game  updated error:", exception);
                    }
                }
            }
        }

        public static int BoxBroadcastLevel
        {
            get
            {
                return m_boxBroadcastLevel;
            }
        }
    }
}


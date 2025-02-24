namespace Fighting.Server.Rooms
{
    using Fighting.Server.Games;
    using Game.Logic;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class ProxyRoomMgr
    {
        private static List<string> botList;
        public static readonly int CLEAR_ROOM_INTERVAL = 0x3e8;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Queue<IAction> m_actionQueue = new Queue<IAction>();
        private static long m_nextClearTick = 0L;
        private static long m_nextPickTick = 0L;
        private static Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();
        private static bool m_running = false;
        private static int m_serverId = 1;
        private static Thread m_thread;
        private static int NpcIndex = 0;
        public static readonly int PICK_UP_INTERVAL = 0x2710;
        private static int RoomIndex = 0;
        public static readonly int THREAD_INTERVAL = 40;

        public static void AddAction(IAction action)
        {
            lock (m_actionQueue)
            {
                m_actionQueue.Enqueue(action);
            }
        }

        public static void AddRoom(ProxyRoom room)
        {
            AddAction(new AddRoomAction(room));
        }

        public static bool AddRoomUnsafe(ProxyRoom room)
        {
            if (!m_rooms.ContainsKey(room.RoomId))
            {
                m_rooms.Add(room.RoomId, room);
                return true;
            }
            return false;
        }

        private static int CalculateScore(ProxyRoom red, ProxyRoom blue)
        {
            return Math.Abs((int)(red.FightPower - blue.FightPower));
        }

        private static void ClearRooms(long tick)
        {
            List<ProxyRoom> list = new List<ProxyRoom>();
            foreach (ProxyRoom room in m_rooms.Values)
            {
                if (!(room.IsPlaying || (room.Game == null)))
                {
                    list.Add(room);
                }
            }
            foreach (ProxyRoom room2 in list)
            {
                m_rooms.Remove(room2.RoomId);
                try
                {
                    room2.Dispose();
                }
                catch (Exception exception)
                {
                    log.Error("Room dispose error:", exception);
                }
            }
        }

        public static ProxyRoom CloneTeam(ProxyRoom red)
        {
            return null;
        }

        private static void ExecuteActions()
        {
            IAction[] array = null;
            lock (m_actionQueue)
            {
                if (m_actionQueue.Count > 0)
                {
                    array = new IAction[m_actionQueue.Count];
                    m_actionQueue.CopyTo(array, 0);
                    m_actionQueue.Clear();
                }
            }
            if (array != null)
            {
                foreach (IAction action in array)
                {
                    try
                    {
                        action.Execute();
                    }
                    catch (Exception exception)
                    {
                        log.Error("RoomMgr execute action error:", exception);
                    }
                }
            }
        }

        public static ProxyRoom[] GetAllRoom()
        {
            lock (m_rooms)
            {
                return GetAllRoomUnsafe();
            }
        }

        public static ProxyRoom[] GetAllRoomUnsafe()
        {
            ProxyRoom[] array = new ProxyRoom[m_rooms.Values.Count];
            m_rooms.Values.CopyTo(array, 0);
            return array;
        }

        public static ProxyRoom GetRoomUnsafe(int roomId)
        {
            if (m_rooms.ContainsKey(roomId))
            {
                return m_rooms[roomId];
            }
            return null;
        }

        public static List<ProxyRoom> GetWaitMatchRoomUnsafe()
        {
            List<ProxyRoom> list = new List<ProxyRoom>();
            foreach (ProxyRoom room in m_rooms.Values)
            {
                if (!(room.IsPlaying || (room.Game != null)))
                {
                    list.Add(room);
                }
            }
            return list;
        }

        public static int NextRoomId()
        {
            return Interlocked.Increment(ref RoomIndex);
        }

        private static void PickUpRooms(long tick)
        {
            List<ProxyRoom> waitMatchRoomUnsafe = GetWaitMatchRoomUnsafe();
            foreach (ProxyRoom room in waitMatchRoomUnsafe)
            {
                int num = 0x5f5e100;
                int num2 = 2;
                ProxyRoom blue = null;
                if (room.IsPlaying)
                {
                    break;
                }
                eRoomType roomType = room.RoomType;
                if (roomType > eRoomType.Encounter)
                {
                    go Label_0312;
                }
                if (roomType == eRoomType.Match)
                {
                    if (room.GameType == eGameType.Guild)
                    {
                        foreach (ProxyRoom room5 in waitMatchRoomUnsafe)
                        {
                            if ((((((room5.GuildId == 0) || (room5.GuildId != room.GuildId)) && ((room5 != room) && (room5.PlayerCount == room.PlayerCount))) && !room5.IsPlaying) && (room5.GameType == eGameType.Guild)) && ((CalculateScore(room, room5) < num) || (room.PickUpCount > num2)))
                            {
                                blue = room5;
                            }
                        }
                    }
                    else
                    {
                        foreach (ProxyRoom room6 in waitMatchRoomUnsafe)
                        {
                            if ((((((room6 != room) && !room.startWithNpc) && (!room.isAutoBot && (room6.PlayerCount == room.PlayerCount))) && !room6.IsPlaying) && ((room6.GameType == eGameType.ALL) || (room6.GameType == eGameType.Free))) && !(((CalculateScore(room, room6) >= num) || (room.PickUpCount <= num2)) || room6.isAutoBot))
                            {
                                blue = room6;
                                break;
                            }
                        }
                    }
                }
                else if (roomType == eRoomType.BattleRoom)
                {
                    foreach (ProxyRoom room4 in waitMatchRoomUnsafe)
                    {
                        if ((((room4 != room) && (room4.RoomType == eRoomType.BattleRoom)) && !room4.IsPlaying) && (room4.PlayerCount == room.PlayerCount))
                        {
                            blue = room4;
                        }
                    }
                }
                else if (roomType == eRoomType.Encounter)
                {
                    foreach (ProxyRoom room3 in waitMatchRoomUnsafe)
                    {
                        if ((((room3 != room) && (room3.RoomType == eRoomType.Encounter)) && !room3.IsPlaying) && (room3.PlayerCount == room.PlayerCount))
                        {
                            blue = room3;
                        }
                    }
                }
                while (blue != null)
                {
                    if (blue != null)
                    {
                        StartMatchGame(room, blue);
                        continue;
                    }
                Label_0312:
                    if (roomType <= eRoomType.SingleBattle)
                    {
                        switch (roomType)
                        {
                            case eRoomType.ConsortiaBattle:
                                foreach (ProxyRoom room8 in waitMatchRoomUnsafe)
                                {
                                    if ((((room8 != room) && (room8.RoomType == eRoomType.ConsortiaBattle)) && !room8.IsPlaying) && (room8.PlayerCount == room.PlayerCount))
                                    {
                                        blue = room8;
                                    }
                                }
                                break;

                            case eRoomType.SingleBattle:
                                foreach (ProxyRoom room7 in waitMatchRoomUnsafe)
                                {
                                    if ((((room7 != room) && (room7.RoomType == eRoomType.SingleBattle)) && !room7.IsPlaying) && (room7.PlayerCount == room.PlayerCount))
                                    {
                                        blue = room7;
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (roomType)
                        {
                            case eRoomType.EntertainmentRoom:
                                foreach (ProxyRoom room9 in waitMatchRoomUnsafe)
                                {
                                    if ((((room9 != room) && (room9.RoomType == eRoomType.EntertainmentRoom)) && !room9.IsPlaying) && (room9.PlayerCount == room.PlayerCount))
                                    {
                                        blue = room9;
                                    }
                                }
                                break;

                            case eRoomType.EntertainmentRoomPK:
                                foreach (ProxyRoom room10 in waitMatchRoomUnsafe)
                                {
                                    if ((((room10 != room) && (room10.RoomType == eRoomType.EntertainmentRoomPK)) && !room10.IsPlaying) && (room10.PlayerCount == room.PlayerCount))
                                    {
                                        blue = room10;
                                    }
                                }
                                break;

                            case eRoomType.FightFootballTime:
                                foreach (ProxyRoom room11 in waitMatchRoomUnsafe)
                                {
                                    if ((((room11 != room) && (room11.RoomType == eRoomType.FightFootballTime)) && !room11.IsPlaying) && (room11.PlayerCount == room.PlayerCount))
                                    {
                                        blue = room11;
                                        break;
                                    }
                                }
                                break;
                        }
                        continue;
                    }
                }
                if ((((room.PickUpCount >= 2) && !room.startWithNpc) && !room.isAutoBot) && (room.RoomType == eRoomType.Match))
                {
                    room.startWithNpc = true;
                    room.Client.SendBeginFightNpc(room.selfId, (int)room.RoomType, (int)room.GameType, room.NpcId);
                    Console.WriteLine("Call AutoBot No.{0}", room.NpcId);
                }
                if ((((room.PickUpCount >= 2) && room.startWithNpc) && !room.isAutoBot) && (room.RoomType == eRoomType.Match))
                {
                    foreach (ProxyRoom room10 in waitMatchRoomUnsafe)
                    {
                        if (((((room10 != room) && (room10.PlayerCount == room.PlayerCount)) && (!room10.IsPlaying && room10.isAutoBot)) && room10.IsFreedom) && (room.NpcId == room10.NpcId))
                        {
                            Console.WriteLine("Start fight with AutoBot No.{0}", room.NpcId);
                            StartMatchGame(room, room10);
                        }
                    }
                }
                if (room.RoomType == eRoomType.Match)
                {
                    if (!room.isAutoBot)
                    {
                        room.PickUpCount++;
                    }
                    else
                    {
                        room.PickUpCount--;
                    }
                }
            }
        }

        public static void RemoveRoom(ProxyRoom room)
        {
            AddAction(new RemoveRoomAction(room));
        }

        public static bool RemoveRoomUnsafe(int roomId)
        {
            if (m_rooms.ContainsKey(roomId))
            {
                m_rooms.Remove(roomId);
                return true;
            }
            return false;
        }

        private static void RoomThread()
        {
            long num = 0L;
            m_nextClearTick = TickHelper.GetTickCount();
            m_nextPickTick = TickHelper.GetTickCount();
            while (m_running)
            {
                long num3;
                long tickCount = TickHelper.GetTickCount();
                try
                {
                    ExecuteActions();
                    if (m_nextPickTick <= tickCount)
                    {
                        m_nextPickTick += PICK_UP_INTERVAL;
                        PickUpRooms(tickCount);
                    }
                    if (m_nextClearTick <= tickCount)
                    {
                        m_nextClearTick += CLEAR_ROOM_INTERVAL;
                        ClearRooms(tickCount);
                    }
                }
                catch (Exception exception)
                {
                    log.Error("Room Mgr Thread Error:", exception);
                }
                goto Label_009F;
            Label_0092:
                Thread.Sleep((int)num);
                num = 0L;
                continue;
            Label_009F:
                num3 = TickHelper.GetTickCount();
                num += THREAD_INTERVAL - (num3 - tickCount);
                if (num > 0L)
                {
                    goto Label_0092;
                }
                if (num < -1000L)
                {
                    log.WarnFormat("Room Mgr is delay {0} ms!", num);
                    num += 0x3e8L;
                }
            }
        }

        public static bool Setup()
        {
            m_thread = new Thread(new ThreadStart(ProxyRoomMgr.RoomThread));
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

        private static void StartAutoBotGame(ProxyRoom red)
        {
            int mapIndex = MapMgr.GetMapIndex(0, 0, m_serverId);
            eGameType free = eGameType.Free;
            eRoomType match = eRoomType.Match;
            BaseGame game = GameMgr.StartBattleGame(red.GetPlayers(), red, null, null, mapIndex, match, free, 2);
            if (game != null)
            {
                red.StartGame(game);
            }
        }

        private static void StartMatchGame(ProxyRoom red, ProxyRoom blue)
        {
            int mapIndex = MapMgr.GetMapIndex(0, 0, m_serverId);
            eGameType free = eGameType.Free;
            eRoomType match = eRoomType.Match;
            if (red.GameType == blue.GameType)
            {
                free = red.GameType;
                match = red.RoomType;
            }
            if (red.RoomType == eRoomType.FightFootballTime)
            {
                mapIndex = 0x521;
            }
            BaseGame game = GameMgr.StartBattleGame(red.GetPlayers(), red, blue.GetPlayers(), blue, mapIndex, match, free, 2);
            if (game != null)
            {
                blue.StartGame(game);
                red.StartGame(game);
            }
            if (game.GameType == eGameType.Guild)
            {
                red.Client.SendConsortiaAlly(red.GetPlayers()[0].PlayerCharacter.ConsortiaID, blue.GetPlayers()[0].PlayerCharacter.ConsortiaID, game.Id);
            }
        }

        public static void StartWithNpcUnsafe(ProxyRoom room)
        {
            int npcId = room.NpcId;
            ProxyRoom roomUnsafe = GetRoomUnsafe(room.RoomId);
            List<ProxyRoom> waitMatchRoomUnsafe = GetWaitMatchRoomUnsafe();
            foreach (ProxyRoom room3 in waitMatchRoomUnsafe)
            {
                if (((room3.isAutoBot && !room3.IsPlaying) && (room3.Game == null)) && (room3.NpcId == npcId))
                {
                    Console.WriteLine("Start fight with AutoBot No.{0}", npcId);
                    StartMatchGame(roomUnsafe, room3);
                }
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
    }
}


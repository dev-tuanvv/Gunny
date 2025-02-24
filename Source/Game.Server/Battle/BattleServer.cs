namespace Game.Server.Battle
{
    using Game.Base;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Rooms;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class BattleServer
    {
        private string m_ip;
        private DateTime m_lastRetryTime;
        private string m_loginKey;
        private int m_port;
        private int m_retryCount;
        private Dictionary<int, BaseRoom> m_rooms;
        private FightServerConnector m_server;
        private int m_serverId;

        public event EventHandler Disconnected;

        public BattleServer(int serverId, string ip, int port, string loginKey)
        {
            this.m_serverId = serverId;
            this.m_ip = ip;
            this.m_port = port;
            this.m_loginKey = loginKey;
            this.m_retryCount = 0;
            this.m_lastRetryTime = DateTime.Now;
            this.m_server = new FightServerConnector(this, ip, port, loginKey);
            this.m_rooms = new Dictionary<int, BaseRoom>();
            this.m_server.Disconnected += new ClientEventHandle(this.m_server_Disconnected);
            this.m_server.Connected += new ClientEventHandle(this.m_server_Connected);
        }

        public bool AddRoom(BaseRoom room)
        {
            bool flag = false;
            BaseRoom room2 = null;
            lock (this.m_rooms)
            {
                if (this.m_rooms.ContainsKey(room.RoomId))
                {
                    room2 = this.m_rooms[room.RoomId];
                    this.m_rooms.Remove(room.RoomId);
                }
            }
            if ((room2 != null) && (room2.Game != null))
            {
                room2.Game.Stop();
            }
            lock (this.m_rooms)
            {
                if (!this.m_rooms.ContainsKey(room.RoomId))
                {
                    this.m_rooms.Add(room.RoomId, room);
                    flag = true;
                }
            }
            if (flag)
            {
                this.m_server.SendAddRoom(room);
            }
            return flag;
        }

        public BattleServer Clone()
        {
            return new BattleServer(this.m_serverId, this.m_ip, this.m_port, this.m_loginKey);
        }

        public BaseRoom FindRoom(int roomId)
        {
            BaseRoom room = null;
            lock (this.m_rooms)
            {
                if (this.m_rooms.ContainsKey(roomId))
                {
                    room = this.m_rooms[roomId];
                }
            }
            return room;
        }

        private void InvokeDisconnect(object state)
        {
            this.m_server_Disconnected(this.m_server);
        }

        private void m_server_Connected(BaseClient client)
        {
        }

        private void m_server_Disconnected(BaseClient client)
        {
            this.RemoveAllRoom();
            if (this.Disconnected != null)
            {
                this.Disconnected(this, null);
            }
        }

        public void RemoveAllRoom()
        {
            BaseRoom[] roomArray = null;
            lock (this.m_rooms)
            {
                roomArray = this.m_rooms.Values.ToArray<BaseRoom>();
                this.m_rooms.Clear();
            }
            foreach (BaseRoom room in roomArray)
            {
                if (room != null)
                {
                    room.RemoveAllPlayer();
                    RoomMgr.StopProxyGame(room);
                }
            }
        }

        public bool RemoveRoom(BaseRoom room)
        {
            bool flag = false;
            lock (this.m_rooms)
            {
                flag = this.m_rooms.ContainsKey(room.RoomId);
            }
            if (flag)
            {
                this.m_server.SendRemoveRoom(room);
            }
            return flag;
        }

        public void RemoveRoomImp(int roomId)
        {
            BaseRoom room = null;
            lock (this.m_rooms)
            {
                if (this.m_rooms.ContainsKey(roomId))
                {
                    room = this.m_rooms[roomId];
                    this.m_rooms.Remove(roomId);
                }
            }
            if (room != null)
            {
                if (room.IsPlaying && (room.Game == null))
                {
                    RoomMgr.CancelPickup(this, room);
                }
                else
                {
                    RoomMgr.StopProxyGame(room);
                }
            }
        }

        public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
        {
            BaseRoom room = this.FindRoom(roomId);
            if (room != null)
            {
                if (exceptId != 0)
                {
                    GamePlayer playerById = WorldMgr.GetPlayerById(exceptId);
                    if (playerById != null)
                    {
                        if (playerById.GamePlayerId == exceptGameId)
                        {
                            room.SendToAll(pkg, playerById);
                        }
                        else
                        {
                            room.SendToAll(pkg);
                        }
                    }
                }
                else
                {
                    room.SendToAll(pkg);
                }
            }
        }

        public void SendToUser(int playerid, GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(playerid);
            if (playerById != null)
            {
                playerById.SendTCP(pkg);
            }
        }

        public void Start()
        {
            if (!this.m_server.Connect())
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.InvokeDisconnect));
            }
        }

        public void StartGame(int roomId, ProxyGame game)
        {
            BaseRoom room = this.FindRoom(roomId);
            if (room != null)
            {
                RoomMgr.StartProxyGame(room, game);
            }
        }

        public void StopGame(int roomId, int gameId, int fightRoomId)
        {
            BaseRoom room = this.FindRoom(roomId);
            if ((room != null) && (fightRoomId == room.FightRoomID))
            {
                RoomMgr.StopProxyGame(room);
                lock (this.m_rooms)
                {
                    this.m_rooms.Remove(roomId);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("ServerID:{0},Ip:{1},Port:{2},IsConnected:{3},RoomCount:{4}", new object[] { this.m_serverId, this.m_server.RemoteEP.Address, this.m_server.RemoteEP.Port, this.m_server.IsConnected, this.m_rooms.Count });
        }

        public void UpdatePlayerGameId(int playerid, int gamePlayerId)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(playerid);
            if (playerById != null)
            {
                playerById.GamePlayerId = gamePlayerId;
            }
        }

        internal void UpdateRoomId(int roomId, int fightRoomId)
        {
            BaseRoom room = this.FindRoom(roomId);
            if (room != null)
            {
                room.FightRoomID = fightRoomId;
            }
        }

        public FightServerConnector Connector
        {
            get
            {
                return this.m_server;
            }
        }

        public string Ip
        {
            get
            {
                return this.m_ip;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.m_server.IsConnected;
            }
        }

        public DateTime LastRetryTime
        {
            get
            {
                return this.m_lastRetryTime;
            }
            set
            {
                this.m_lastRetryTime = value;
            }
        }

        public string LoginKey
        {
            get
            {
                return this.m_loginKey;
            }
        }

        public int Port
        {
            get
            {
                return this.m_port;
            }
        }

        public int RetryCount
        {
            get
            {
                return this.m_retryCount;
            }
            set
            {
                this.m_retryCount = value;
            }
        }

        public FightServerConnector Server
        {
            get
            {
                return this.m_server;
            }
        }

        public int ServerId
        {
            get
            {
                return this.m_serverId;
            }
        }
    }
}


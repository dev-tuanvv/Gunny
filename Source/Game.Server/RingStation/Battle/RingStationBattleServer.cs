namespace Game.Server.RingStation.Battle
{
    using Game.Base;
    using Game.Base.Packets;
    using Game.Server.RingStation;
    using System;
    using System.Collections.Generic;

    public class RingStationBattleServer
    {
        private string m_ip;
        private int m_port;
        private Dictionary<int, BaseRoomRingStation> m_rooms;
        private RingStationFightConnector m_server;
        private int m_serverId;

        public RingStationBattleServer(int serverId, string ip, int port, string loginKey)
        {
            this.m_serverId = serverId;
            this.m_ip = ip;
            this.m_port = port;
            this.m_server = new RingStationFightConnector(this, ip, port, loginKey);
            this.m_rooms = new Dictionary<int, BaseRoomRingStation>();
            this.m_server.Disconnected += new ClientEventHandle(this.m_server_Disconnected);
            this.m_server.Connected += new ClientEventHandle(this.m_server_Connected);
        }

        public bool AddRoom(BaseRoomRingStation room)
        {
            Dictionary<int, BaseRoomRingStation> dictionary;
            bool flag = false;
            BaseRoomRingStation station = null;
            lock ((dictionary = this.m_rooms))
            {
                if (this.m_rooms.ContainsKey(room.RoomId))
                {
                    station = this.m_rooms[room.RoomId];
                    this.m_rooms.Remove(room.RoomId);
                }
            }
            if ((station != null) && (station.Game != null))
            {
                station.Game.Stop();
            }
            lock ((dictionary = this.m_rooms))
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
            room.BattleServer = this;
            return flag;
        }

        private BaseRoomRingStation FindRoom(int roomId)
        {
            BaseRoomRingStation station = null;
            lock (this.m_rooms)
            {
                if (this.m_rooms.ContainsKey(roomId))
                {
                    station = this.m_rooms[roomId];
                }
            }
            return station;
        }

        private void m_server_Connected(BaseClient client)
        {
        }

        private void m_server_Disconnected(BaseClient client)
        {
        }

        public bool RemoveRoom(BaseRoomRingStation room)
        {
            bool flag = false;
            lock (this.m_rooms)
            {
                flag = this.m_rooms.ContainsKey(room.RoomId);
                if (flag)
                {
                    this.m_server.SendRemoveRoom(room);
                }
            }
            return flag;
        }

        public void RemoveRoomImp(int roomId)
        {
        }

        public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
        {
            BaseRoomRingStation station = this.FindRoom(roomId);
            if (station != null)
            {
                if (exceptId != 0)
                {
                    RingStationGamePlayer playerById = RingStationMgr.GetPlayerById(exceptId);
                    if (playerById != null)
                    {
                        if (playerById.GamePlayerId == exceptGameId)
                        {
                            station.SendToAll(pkg, playerById);
                        }
                        else
                        {
                            station.SendToAll(pkg);
                        }
                    }
                }
                else
                {
                    station.SendToAll(pkg);
                }
            }
        }

        public void SendToUser(int playerid, GSPacketIn pkg)
        {
        }

        public bool Start()
        {
            return this.m_server.Connect();
        }

        public void StartGame(int roomId, ProxyRingStationGame game)
        {
            BaseRoomRingStation station = this.FindRoom(roomId);
            if (station != null)
            {
                station.StartGame(game);
            }
        }

        public void StopGame(int roomId, int gameId)
        {
            if (this.FindRoom(roomId) != null)
            {
                lock (this.m_rooms)
                {
                    this.m_rooms.Remove(roomId);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("ServerID:{0},Ip:{1},Port:{2},IsConnected:{3},RoomCount:", new object[] { this.m_serverId, this.m_server.RemoteEP.Address, this.m_server.RemoteEP.Port, this.m_server.IsConnected });
        }

        public void UpdatePlayerGameId(int playerid, int gamePlayerId)
        {
            RingStationGamePlayer playerById = RingStationMgr.GetPlayerById(playerid);
            if (playerById != null)
            {
                playerById.GamePlayerId = gamePlayerId;
            }
        }

        public void UpdateRoomId(int roomId, int fightRoomId)
        {
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

        public int Port
        {
            get
            {
                return this.m_port;
            }
        }

        public RingStationFightConnector Server
        {
            get
            {
                return this.m_server;
            }
        }
    }
}


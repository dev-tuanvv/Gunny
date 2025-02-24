namespace Game.Server.RingStation
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server.RingStation.Battle;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Timers;

    public class BaseRoomRingStation
    {
        private Timer addrom = new Timer();
        public RingStationBattleServer BattleServer;
        public bool IsAutoBot;
        public bool IsFreedom;
        private AbstractGame m_game;
        private List<RingStationGamePlayer> m_places;
        public int PickUpNpcId;
        public int RoomId;

        public BaseRoomRingStation(int roomId)
        {
            this.RoomId = roomId;
            this.m_places = new List<RingStationGamePlayer>();
        }

        public bool AddPlayer(RingStationGamePlayer player)
        {
            lock (this.m_places)
            {
                player.CurRoom = this;
                this.m_places.Add(player);
            }
            return true;
        }

        internal List<RingStationGamePlayer> GetPlayers()
        {
            return this.m_places;
        }

        public void RemovePlayer(RingStationGamePlayer player)
        {
            if (this.BattleServer != null)
            {
                if (this.m_game != null)
                {
                    this.BattleServer.Server.SendPlayerDisconnet(this.Game.Id, player.GamePlayerId, this.RoomId);
                    this.BattleServer.Server.SendRemoveRoom(this);
                }
                this.IsPlaying = false;
            }
            if (this.Game != null)
            {
                this.Game.Stop();
            }
        }

        internal void SendTCP(GSPacketIn pkg)
        {
            if (this.m_game != null)
            {
                this.BattleServer.Server.SendToGame(this.m_game.Id, pkg);
            }
        }

        public void SendToAll(GSPacketIn pkg)
        {
            this.SendToAll(pkg, null);
        }

        public void SendToAll(GSPacketIn pkg, RingStationGamePlayer except)
        {
            lock (this.m_places)
            {
                foreach (RingStationGamePlayer player in this.m_places)
                {
                    if ((player != null) && (player != except))
                    {
                        player.ProcessPacket(pkg);
                    }
                }
            }
        }

        public void StartGame(AbstractGame game)
        {
            this.m_game = game;
        }

        public AbstractGame Game
        {
            get
            {
                return this.m_game;
            }
        }

        public int GameType { get; set; }

        public int GuildId { get; set; }

        public bool IsPlaying { get; set; }

        public int RoomType { get; set; }
    }
}


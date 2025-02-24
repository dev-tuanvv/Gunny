namespace Fighting.Server.Rooms
{
    using Fighting.Server;
    using Game.Base.Packets;
    using Game.Logic;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Reflection;

    public class ProxyRoom
    {
        public int AvgLevel;
        public int FightPower;
        public eGameType GameType;
        public int GuildId;
        public string GuildName;
        public bool IsFreedom;
        public bool IsPlaying;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ServerClient m_client;
        private BaseGame m_game;
        private bool m_isAutoBot;
        private int m_npcId;
        private int m_orientRoomId;
        private List<IGamePlayer> m_players;
        private int m_roomId;
        public int PickUpCount;
        public bool PickUpNPC;
        public int PickUpNPCID;
        public eRoomType RoomType;
        public int selfId;
        public bool startWithNpc;

        public ProxyRoom(int roomId, int orientRoomId, IGamePlayer[] players, ServerClient client, int totallevel = 30, int totalFightPower = 0x3e8, int npcId = 0, bool isAutoBot = false)
        {
            this.m_npcId = npcId;
            this.m_isAutoBot = isAutoBot;
            this.m_roomId = roomId;
            this.m_orientRoomId = orientRoomId;
            this.m_players = new List<IGamePlayer>();
            this.m_players.AddRange(players);
            this.m_client = client;
            this.PickUpCount = 0;
            this.FightPower = totalFightPower;
            this.AvgLevel = totallevel / players.Count<IGamePlayer>();
            this.PickUpNPC = false;
        }

        public void Dispose()
        {
            this.m_client.RemoveRoom(this.m_orientRoomId, this);
        }

        private void game_GameStopped(AbstractGame game)
        {
            this.m_game.GameStopped -= new GameEventHandle(this.game_GameStopped);
            this.IsPlaying = false;
            this.m_client.SendStopGame(this.m_orientRoomId, this.m_game.Id, this.RoomId);
        }

        public List<IGamePlayer> GetPlayers()
        {
            List<IGamePlayer> list = new List<IGamePlayer>();
            lock (this.m_players)
            {
                list.AddRange(this.m_players);
            }
            return list;
        }

        public bool RemovePlayer(IGamePlayer player)
        {
            bool flag = false;
            lock (this.m_players)
            {
                if (this.m_players.Remove(player))
                {
                    flag = true;
                }
            }
            if (this.PlayerCount == 0)
            {
                ProxyRoomMgr.RemoveRoom(this);
            }
            return flag;
        }

        public void SendToAll(GSPacketIn pkg)
        {
            this.SendToAll(pkg, null);
        }

        public void SendToAll(GSPacketIn pkg, IGamePlayer except)
        {
            this.m_client.SendToRoom(this.m_orientRoomId, pkg, except);
        }

        public void StartGame(BaseGame game)
        {
            this.IsPlaying = true;
            this.m_game = game;
            game.GameStopped += new GameEventHandle(this.game_GameStopped);
            this.m_client.SendStartGame(this.m_orientRoomId, game);
        }

        public override string ToString()
        {
            return string.Format("RoomId:{0} OriendId:{1} PlayerCount:{2},IsPlaying:{3},GuildId:{4},GuildName:{5}", new object[] { this.m_roomId, this.m_orientRoomId, this.m_players.Count, this.IsPlaying, this.GuildId, this.GuildName });
        }

        public ServerClient Client
        {
            get
            {
                return this.m_client;
            }
        }

        public BaseGame Game
        {
            get
            {
                return this.m_game;
            }
        }

        public bool isAutoBot
        {
            get
            {
                return this.m_isAutoBot;
            }
        }

        public int NpcId
        {
            get
            {
                return this.m_npcId;
            }
            set
            {
                this.m_npcId = value;
            }
        }

        public int OrientRoomId
        {
            get
            {
                return this.m_orientRoomId;
            }
        }

        public int PlayerCount
        {
            get
            {
                return this.m_players.Count;
            }
        }

        public int RoomId
        {
            get
            {
                return this.m_roomId;
            }
        }
    }
}


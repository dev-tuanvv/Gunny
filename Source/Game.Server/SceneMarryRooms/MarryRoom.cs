namespace Game.Server.SceneMarryRooms
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class MarryRoom
    {
        private int _count;
        private List<GamePlayer> _guestsList;
        private IMarryProcessor _processor;
        private eRoomState _roomState;
        private static object _syncStop = new object();
        private Timer _timer;
        private Timer _timerForHymeneal;
        private List<int> _userForbid;
        private List<int> _userRemoveList;
        public MarryRoomInfo Info;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MarryRoom(MarryRoomInfo info, IMarryProcessor processor)
        {
            this.Info = info;
            this._processor = processor;
            this._guestsList = new List<GamePlayer>();
            this._count = 0;
            this._roomState = eRoomState.FREE;
            this._userForbid = new List<int>();
            this._userRemoveList = new List<int>();
        }

        public bool AddPlayer(GamePlayer player)
        {
            lock (_syncStop)
            {
                if ((player.CurrentRoom != null) || player.IsInMarryRoom)
                {
                    return false;
                }
                if (this._guestsList.Count > this.Info.MaxCount)
                {
                    player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoom.Msg1", new object[0]));
                    return false;
                }
                this._count++;
                this._guestsList.Add(player);
                player.CurrentMarryRoom = this;
                player.MarryMap = 1;
                if (player.CurrentRoom != null)
                {
                    player.CurrentRoom.RemovePlayerUnsafe(player);
                }
            }
            return true;
        }

        public void BeginTimer(int interval)
        {
            if (this._timer == null)
            {
                this._timer = new Timer(new TimerCallback(this.OnTick), null, interval, interval);
            }
            else
            {
                this._timer.Change(interval, interval);
            }
        }

        public void BeginTimerForHymeneal(int interval)
        {
            if (this._timerForHymeneal == null)
            {
                this._timerForHymeneal = new Timer(new TimerCallback(this.OnTickForHymeneal), null, interval, interval);
            }
            else
            {
                this._timerForHymeneal.Change(interval, interval);
            }
        }

        public bool CheckUserForbid(int userID)
        {
            lock (_syncStop)
            {
                return this._userForbid.Contains(userID);
            }
        }

        public GamePlayer[] GetAllPlayers()
        {
            lock (_syncStop)
            {
                return this._guestsList.ToArray();
            }
        }

        public GamePlayer GetPlayerByUserID(int userID)
        {
            lock (_syncStop)
            {
                foreach (GamePlayer player in this._guestsList)
                {
                    if (player.PlayerCharacter.ID == userID)
                    {
                        return player;
                    }
                }
            }
            return null;
        }

        public void KickAllPlayer()
        {
            foreach (GamePlayer player in this.GetAllPlayers())
            {
                this.RemovePlayer(player);
                player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoom.TimeOver", new object[0]));
            }
        }

        public bool KickPlayerByUserID(GamePlayer player, int userID)
        {
            GamePlayer playerByUserID = this.GetPlayerByUserID(userID);
            if (((playerByUserID != null) && (playerByUserID.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID)) && (playerByUserID.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID))
            {
                this.RemovePlayer(playerByUserID);
                playerByUserID.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom", new object[0]));
                GSPacketIn packet = player.Out.SendMessage(eMessageType.ChatERROR, playerByUserID.PlayerCharacter.NickName + "  " + LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom2", new object[0]));
                player.CurrentMarryRoom.SendToPlayerExceptSelf(packet, player);
                return true;
            }
            return false;
        }

        protected void OnTick(object obj)
        {
            this._processor.OnTick(this);
        }

        protected void OnTickForHymeneal(object obj)
        {
            try
            {
                this._roomState = eRoomState.FREE;
                GSPacketIn packet = new GSPacketIn(0xf9);
                packet.WriteByte(9);
                this.SendToAll(packet);
                this.StopTimerForHymeneal();
                this.SendUserRemoveLate();
                this.SendMarryRoomInfoUpdateToScenePlayers(this);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("OnTickForHymeneal", exception);
                }
            }
        }

        public void ProcessData(GamePlayer player, GSPacketIn data)
        {
            lock (_syncStop)
            {
                this._processor.OnGameData(this, player, data);
            }
        }

        public void RemovePlayer(GamePlayer player)
        {
            lock (_syncStop)
            {
                if (this.RoomState == eRoomState.FREE)
                {
                    this._count--;
                    this._guestsList.Remove(player);
                    GSPacketIn packet = player.Out.SendPlayerLeaveMarryRoom(player);
                    player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(packet, player);
                    player.CurrentMarryRoom = null;
                    player.MarryMap = 0;
                }
                else if (this.RoomState == eRoomState.Hymeneal)
                {
                    this._userRemoveList.Add(player.PlayerCharacter.ID);
                    this._count--;
                    this._guestsList.Remove(player);
                    player.CurrentMarryRoom = null;
                }
                this.SendMarryRoomInfoUpdateToScenePlayers(this);
            }
        }

        public void ReturnPacket(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn @in = packet.Clone();
            @in.ClientID = player.PlayerCharacter.ID;
            this.SendToPlayerExceptSelf(@in, player);
        }

        public void ReturnPacketForScene(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn @in = packet.Clone();
            @in.ClientID = player.PlayerCharacter.ID;
            this.SendToPlayerExceptSelfForScene(@in, player);
        }

        public void RoomContinuation(int time)
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - this.Info.BeginTime);
            int num = ((this.Info.AvailTime * 60) - span.Minutes) + (time * 60);
            this.Info.AvailTime += time;
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                bussiness.UpdateMarryRoomInfo(this.Info);
            }
            this.BeginTimer(0xea60 * num);
        }

        public GSPacketIn SendMarryRoomInfoUpdateToScenePlayers(MarryRoom room)
        {
            GSPacketIn packet = new GSPacketIn(0xff);
            bool val = room != null;
            packet.WriteBoolean(val);
            if (val)
            {
                packet.WriteInt(room.Info.ID);
                packet.WriteBoolean(room.Info.IsHymeneal);
                packet.WriteString(room.Info.Name);
                packet.WriteBoolean(!(room.Info.Pwd == ""));
                packet.WriteInt(room.Info.MapIndex);
                packet.WriteInt(room.Info.AvailTime);
                packet.WriteInt(room.Count);
                packet.WriteInt(room.Info.PlayerID);
                packet.WriteString(room.Info.PlayerName);
                packet.WriteInt(room.Info.GroomID);
                packet.WriteString(room.Info.GroomName);
                packet.WriteInt(room.Info.BrideID);
                packet.WriteString(room.Info.BrideName);
                packet.WriteDateTime(room.Info.BeginTime);
                packet.WriteByte((byte) room.RoomState);
                packet.WriteString(room.Info.RoomIntroduction);
            }
            this.SendToScenePlayer(packet);
            return packet;
        }

        public void SendToAll(GSPacketIn packet)
        {
            this.SendToAll(packet, null, false);
        }

        public void SendToAll(GSPacketIn packet, GamePlayer self, bool isChat)
        {
            GamePlayer[] allPlayers = this.GetAllPlayers();
            if (allPlayers != null)
            {
                foreach (GamePlayer player in allPlayers)
                {
                    if (!(isChat && player.IsBlackFriend(self.PlayerCharacter.ID)))
                    {
                        player.Out.SendTCP(packet);
                    }
                }
            }
        }

        public void SendToAllForScene(GSPacketIn packet, int sceneID)
        {
            GamePlayer[] allPlayers = this.GetAllPlayers();
            if (allPlayers != null)
            {
                foreach (GamePlayer player in allPlayers)
                {
                    if (player.MarryMap == sceneID)
                    {
                        player.Out.SendTCP(packet);
                    }
                }
            }
        }

        public void SendToPlayerExceptSelf(GSPacketIn packet, GamePlayer self)
        {
            GamePlayer[] allPlayers = this.GetAllPlayers();
            if (allPlayers != null)
            {
                foreach (GamePlayer player in allPlayers)
                {
                    if (player != self)
                    {
                        player.Out.SendTCP(packet);
                    }
                }
            }
        }

        public void SendToPlayerExceptSelfForScene(GSPacketIn packet, GamePlayer self)
        {
            GamePlayer[] allPlayers = this.GetAllPlayers();
            if (allPlayers != null)
            {
                foreach (GamePlayer player in allPlayers)
                {
                    if ((player != self) && (player.MarryMap == self.MarryMap))
                    {
                        player.Out.SendTCP(packet);
                    }
                }
            }
        }

        public void SendToRoomPlayer(GSPacketIn packet)
        {
            GamePlayer[] allPlayers = this.GetAllPlayers();
            if (allPlayers != null)
            {
                foreach (GamePlayer player in allPlayers)
                {
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void SendToScenePlayer(GSPacketIn packet)
        {
            WorldMgr.MarryScene.SendToALL(packet);
        }

        public void SendUserRemoveLate()
        {
            lock (_syncStop)
            {
                foreach (int num in this._userRemoveList)
                {
                    GSPacketIn packet = new GSPacketIn(0xf4, num);
                    this.SendToAllForScene(packet, 1);
                }
                this._userRemoveList.Clear();
            }
        }

        public void SetUserForbid(int userID)
        {
            lock (_syncStop)
            {
                this._userForbid.Add(userID);
            }
        }

        public void StopTimer()
        {
            if (this._timer != null)
            {
                this._timer.Dispose();
                this._timer = null;
            }
        }

        public void StopTimerForHymeneal()
        {
            if (this._timerForHymeneal != null)
            {
                this._timerForHymeneal.Dispose();
                this._timerForHymeneal = null;
            }
        }

        public int Count
        {
            get
            {
                return this._count;
            }
        }

        public eRoomState RoomState
        {
            get
            {
                return this._roomState;
            }
            set
            {
                if (this._roomState != value)
                {
                    this._roomState = value;
                    this.SendMarryRoomInfoUpdateToScenePlayers(this);
                }
            }
        }
    }
}


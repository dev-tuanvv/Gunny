namespace Game.Server.HotSpringRooms
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
    using System.Reflection;

    public class HotSpringRoom
    {
        private GInterface2 ginterface2_0;
        private static readonly ILog ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public HotSpringRoomInfo Info;
        private int int_0;
        private List<GamePlayer> list_0;
        private static object object_0 = new object();

        public HotSpringRoom(HotSpringRoomInfo info, GInterface2 processor)
        {
            this.Info = info;
            this.ginterface2_0 = processor;
            this.list_0 = new List<GamePlayer>();
            this.int_0 = 0;
        }

        public bool AddPlayer(GamePlayer player)
        {
            object obj2 = object_0;
            lock (obj2)
            {
                if ((player.CurrentRoom != null) || (player.CurrentHotSpringRoom != null))
                {
                    return false;
                }
                if (this.list_0.Count > this.Info.maxCount)
                {
                    player.Out.SendMessage(eMessageType.Normal, "Ph\x00f2ng đầy");
                    return false;
                }
                if (player.Extra.Info.MinHotSpring <= 0)
                {
                    int spaAddictionMoneyNeeded = GameProperties.SpaAddictionMoneyNeeded;
                    if (player.PlayerCharacter.Money < spaAddictionMoneyNeeded)
                    {
                        player.Out.SendMessage(eMessageType.Normal, "Bạn đ\x00e3 hết giờ tham gia");
                        return false;
                    }
                    player.RemoveMoney(spaAddictionMoneyNeeded);
                    player.Out.SendMessage(eMessageType.Normal, "Hệ thống vừa tự động gia hạn cho bạn.");
                    player.Extra.Info.MinHotSpring = GameProperties.SpaPriRoomContinueTime;
                }
                this.int_0++;
                this.list_0.Add(player);
                player.CurrentHotSpringRoom = this;
                player.Extra.Info.LastTimeHotSpring = DateTime.Now;
                this.SetDefaultPostion(player);
                if (player.CurrentRoom != null)
                {
                    player.CurrentRoom.RemovePlayerUnsafe(player);
                }
                player.Extra.BeginHotSpringTimer();
                player.OnEnterHotSpring();
                HotSpringRoom[] rooms = new HotSpringRoom[] { player.CurrentHotSpringRoom };
                HotSpringMgr.SendUpdateAllRoom(null, rooms);
                GSPacketIn packet = new GSPacketIn(0xc6);
                packet.WriteInt(player.PlayerCharacter.ID);
                packet.WriteInt(player.PlayerCharacter.Grade);
                packet.WriteInt(player.PlayerCharacter.Hide);
                packet.WriteInt(player.PlayerCharacter.Repute);
                packet.WriteString(player.PlayerCharacter.NickName);
                packet.WriteByte(player.PlayerCharacter.typeVIP);
                packet.WriteInt(player.PlayerCharacter.VIPLevel);
                packet.WriteBoolean(player.PlayerCharacter.Sex);
                packet.WriteString(player.PlayerCharacter.Style);
                packet.WriteString(player.PlayerCharacter.Colors);
                packet.WriteString(player.PlayerCharacter.Skin);
                packet.WriteInt(player.Hot_X);
                packet.WriteInt(player.Hot_Y);
                packet.WriteInt(player.PlayerCharacter.FightPower);
                packet.WriteInt(player.PlayerCharacter.Win);
                packet.WriteInt(player.PlayerCharacter.Total);
                packet.WriteInt(player.Hot_Direction);
                this.SendToPlayerExceptSelf(packet, player);
            }
            return true;
        }

        public bool CanEnter()
        {
            return (this.int_0 < this.Info.maxCount);
        }

        public GamePlayer[] GetAllPlayers()
        {
            object obj2 = object_0;
            lock (obj2)
            {
                return this.list_0.ToArray();
            }
        }

        public GamePlayer GetPlayerWithID(int playerId)
        {
            object obj2 = object_0;
            lock (obj2)
            {
                foreach (GamePlayer player in this.list_0)
                {
                    if (player.PlayerCharacter.ID == playerId)
                    {
                        return player;
                    }
                }
                return null;
            }
        }

        protected void OnTick(object obj)
        {
            this.ginterface2_0.OnTick(this);
        }

        public void ProcessData(GamePlayer player, GSPacketIn data)
        {
            object obj2 = object_0;
            lock (obj2)
            {
                this.ginterface2_0.OnGameData(this, player, data);
            }
        }

        public void RemovePlayer(GamePlayer player)
        {
            object obj2 = object_0;
            lock (obj2)
            {
                if (player.CurrentHotSpringRoom != null)
                {
                    this.int_0--;
                    this.list_0.Remove(player);
                    player.Extra.StopHotSpringTimer();
                    GSPacketIn packet = new GSPacketIn(0xc7, player.PlayerCharacter.ID);
                    packet.WriteInt(player.PlayerCharacter.ID);
                    packet.WriteString("");
                    player.CurrentHotSpringRoom.SendToAll(packet);
                    this.SetDefaultPostion(player);
                    HotSpringRoom[] rooms = new HotSpringRoom[] { player.CurrentHotSpringRoom };
                    HotSpringMgr.SendUpdateAllRoom(null, rooms);
                    player.CurrentHotSpringRoom = null;
                }
            }
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

        public void SendToRoomPlayer(GSPacketIn packet)
        {
            GamePlayer[] allPlayers = this.GetAllPlayers();
            if (allPlayers != null)
            {
                GamePlayer[] playerArray2 = allPlayers;
                for (int i = 0; i < playerArray2.Length; i++)
                {
                    playerArray2[i].Out.SendTCP(packet);
                }
            }
        }

        public void SetDefaultPostion(GamePlayer p)
        {
            p.Hot_X = 480;
            p.Hot_Y = 560;
            p.Hot_Direction = 3;
        }

        public int Count
        {
            get
            {
                return this.int_0;
            }
        }
    }
}


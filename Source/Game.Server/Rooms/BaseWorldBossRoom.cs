using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.Rooms
{
    public class BaseWorldBossRoom
    {
        private Dictionary<int, GamePlayer> m_list;
        private long MAX_BLOOD = 350000000L;
        private long m_blood;
        private string m_name;
        private string m_bossResourceId;
        private DateTime m_begin_time;
        private DateTime m_end_time;
        private int m_currentPVE;
        private bool m_fightOver;
        private bool m_roomClose;
        private bool m_worldOpen;
        private int m_fight_time;
        private bool m_die;
        public int playerDefaultPosX = 265;
        public int playerDefaultPosY = 1030;
        public int ticketID = 11573;
        public int need_ticket_count;
        public int timeCD = 15;
        public int reviveMoney = 10000;
        public int reFightMoney = 12000;
        public int addInjureBuffMoney = 30000;
        public int addInjureValue = 200;
        public long MaxBlood
        {
            get
            {
                return this.MAX_BLOOD;
            }
        }
        public long Blood
        {
            get
            {
                return this.m_blood;
            }
            set
            {
                this.m_blood = value;
            }
        }
        public string Name
        {
            get
            {
                return this.m_name;
            }
        }
        public string BossResourceId
        {
            get
            {
                return this.m_bossResourceId;
            }
        }
        public DateTime Begin_time
        {
            get
            {
                return this.m_begin_time;
            }
        }
        public DateTime End_time
        {
            get
            {
                return this.m_end_time;
            }
        }
        public int CurrentPVE
        {
            get
            {
                return this.m_currentPVE;
            }
        }
        public bool FightOver
        {
            get
            {
                return this.m_fightOver;
            }
        }
        public bool RoomClose
        {
            get
            {
                return this.m_roomClose;
            }
        }
        public bool WorldbossOpen
        {
            get
            {
                return this.m_worldOpen;
            }
        }
        public int Fight_time
        {
            get
            {
                return this.m_fight_time;
            }
        }
        public bool IsDie
        {
            get
            {
                return this.m_die;
            }
            set
            {
                this.m_die = value;
            }
        }
        public BaseWorldBossRoom()
        {
            this.m_list = new Dictionary<int, GamePlayer>();
            this.m_name = "boss";
            this.m_bossResourceId = "0";
            this.m_currentPVE = 0;
        }
        public void UpdateRank(int damage, int honor, string nickName)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81);
            gSPacketIn.WriteInt(damage);
            gSPacketIn.WriteInt(honor);
            gSPacketIn.WriteString(nickName);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }
        public void ShowRank()
        {
            GSPacketIn packet = new GSPacketIn(86);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }
        public void UpdateWorldBoss(GSPacketIn pkg)
        {
            long mAX_BLOOD = pkg.ReadLong();
            long blood = pkg.ReadLong();
            string name = pkg.ReadString();
            string bossResourceId = pkg.ReadString();
            int currentPVE = pkg.ReadInt();
            bool flag = pkg.ReadBoolean();
            bool roomClose = pkg.ReadBoolean();
            this.m_begin_time = pkg.ReadDateTime();
            this.m_end_time = pkg.ReadDateTime();
            this.m_fight_time = pkg.ReadInt();
            bool worldOpen = pkg.ReadBoolean();
            if (!this.m_worldOpen)
            {
                this.m_die = flag;
                this.m_fightOver = flag;
                this.m_roomClose = roomClose;
                this.MAX_BLOOD = mAX_BLOOD;
                this.m_blood = blood;
                this.m_name = name;
                this.m_bossResourceId = bossResourceId;
                this.m_currentPVE = currentPVE;
                this.m_worldOpen = worldOpen;
                GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
                GamePlayer[] array = allPlayers;
                for (int i = 0; i < array.Length; i++)
                {
                    GamePlayer gamePlayer = array[i];
                    gamePlayer.Out.SendOpenWorldBoss(gamePlayer.X, gamePlayer.Y);
                }
            }
            if (this.m_fightOver && !this.m_die)
            {
                this.FightOverAll();
                this.m_die = true;
            }
        }
        public void WorldBossClose()
        {
            this.m_worldOpen = false;
            GamePlayer[] playersSafe = this.GetPlayersSafe();
            GamePlayer[] array = playersSafe;
            for (int i = 0; i < array.Length; i++)
            {
                GamePlayer player = array[i];
                this.RemovePlayer(player);
            }
        }
        public void FightOverAll()
        {
            GSPacketIn packet = new GSPacketIn(82);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }
        public void ReduceBlood(int value)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(84);
            gSPacketIn.WriteInt(value);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }
        public void SendFightOver()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(8);
            gSPacketIn.WriteBoolean(true);
            this.SendToALLPlayers(gSPacketIn);
        }
        public void SendRoomClose()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(9);
            this.SendToALLPlayers(gSPacketIn);
        }
        public void UpdateWorldBossRankCrosszone(GSPacketIn packet)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(10);
            bool flag = packet.ReadBoolean();
            int num = packet.ReadInt();
            gSPacketIn.WriteBoolean(flag);
            gSPacketIn.WriteInt(num);
            for (int i = 0; i < num; i++)
            {
                int val = packet.ReadInt();
                string str = packet.ReadString();
                int val2 = packet.ReadInt();
                gSPacketIn.WriteInt(val);
                gSPacketIn.WriteString(str);
                gSPacketIn.WriteInt(val2);
            }
            if (flag)
            {
                this.SendToALLPlayers(gSPacketIn);
                return;
            }
            this.SendToALL(gSPacketIn);
        }
        public void SendPrivateInfo(string name)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(85);
            gSPacketIn.WriteString(name);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }
        public void SendPrivateInfo(string name, int damage, int honor)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(22);
            gSPacketIn.WriteInt(damage);
            gSPacketIn.WriteInt(honor);
            GamePlayer[] playersSafe = this.GetPlayersSafe();
            GamePlayer[] array = playersSafe;
            for (int i = 0; i < array.Length; i++)
            {
                GamePlayer gamePlayer = array[i];
                if (gamePlayer.PlayerCharacter.NickName == name)
                {
                    gamePlayer.Out.SendTCP(gSPacketIn);
                    return;
                }
            }
        }
        public void SendUpdateBlood(GSPacketIn packet)
        {
            long val = packet.ReadLong();
            this.m_blood = packet.ReadLong();
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(5);
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteLong(val);
            gSPacketIn.WriteLong(this.m_blood);
            this.SendToALL(gSPacketIn);
        }
        public bool AddPlayer(GamePlayer player)
        {
            bool flag = false;
            Dictionary<int, GamePlayer> list;
            Monitor.Enter(list = this.m_list);
            try
            {
                if (!this.m_list.ContainsKey(player.PlayerId))
                {
                    player.IsInWorldBossRoom = true;
                    this.m_list.Add(player.PlayerId, player);
                    flag = true;
                    this.ShowRank();
                    this.SendPrivateInfo(player.PlayerCharacter.NickName);
                }
            }
            finally
            {
                Monitor.Exit(list);
            }
            if (flag)
            {
                GSPacketIn gSPacketIn = new GSPacketIn(102);
                gSPacketIn.WriteByte(3);
                gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
                gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
                gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
                gSPacketIn.WriteInt(player.PlayerCharacter.ID);
                gSPacketIn.WriteString(player.PlayerCharacter.NickName);
                gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
                gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
                gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
                gSPacketIn.WriteString(player.PlayerCharacter.Style);
                gSPacketIn.WriteString(player.PlayerCharacter.Colors);
                gSPacketIn.WriteString(player.PlayerCharacter.Skin);
                gSPacketIn.WriteInt(player.X);
                gSPacketIn.WriteInt(player.Y);
                gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
                gSPacketIn.WriteInt(player.PlayerCharacter.Win);
                gSPacketIn.WriteInt(player.PlayerCharacter.Total);
                gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
                gSPacketIn.WriteByte(player.States);
                this.SendToALL(gSPacketIn);
            }
            return flag;
        }
        public void ViewOtherPlayerRoom(GamePlayer player)
        {
            GamePlayer[] playersSafe = this.GetPlayersSafe();
            GamePlayer[] array = playersSafe;
            for (int i = 0; i < array.Length; i++)
            {
                GamePlayer gamePlayer = array[i];
                if (gamePlayer != player)
                {
                    GSPacketIn gSPacketIn = new GSPacketIn(102);
                    gSPacketIn.WriteByte(3);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Grade);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Hide);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Repute);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.NickName);
                    gSPacketIn.WriteByte(gamePlayer.PlayerCharacter.typeVIP);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
                    gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Style);
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Colors);
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Skin);
                    gSPacketIn.WriteInt(gamePlayer.X);
                    gSPacketIn.WriteInt(gamePlayer.Y);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
                    gSPacketIn.WriteByte(gamePlayer.States);
                    player.SendTCP(gSPacketIn);
                }
            }
        }
        public bool RemovePlayer(GamePlayer player)
        {
            bool flag = false;
            Dictionary<int, GamePlayer> list;
            Monitor.Enter(list = this.m_list);
            try
            {
                flag = this.m_list.Remove(player.PlayerId);
                GSPacketIn gSPacketIn = new GSPacketIn(102);
                gSPacketIn.WriteByte(4);
                gSPacketIn.WriteInt(player.PlayerId);
                this.SendToALL(gSPacketIn);
            }
            finally
            {
                Monitor.Exit(list);
            }
            if (flag)
            {
                player.Out.SendSceneRemovePlayer(player);
            }
            return true;
        }
        public GamePlayer[] GetPlayersSafe()
        {
            GamePlayer[] array = null;
            Dictionary<int, GamePlayer> list;
            Monitor.Enter(list = this.m_list);
            try
            {
                array = new GamePlayer[this.m_list.Count];
                this.m_list.Values.CopyTo(array, 0);
            }
            finally
            {
                Monitor.Exit(list);
            }
            if (array != null)
            {
                return array;
            }
            return new GamePlayer[0];
        }
        public void SendToALLPlayers(GSPacketIn packet)
        {
            GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
            GamePlayer[] array = allPlayers;
            for (int i = 0; i < array.Length; i++)
            {
                GamePlayer gamePlayer = array[i];
                gamePlayer.SendTCP(packet);
            }
        }
        public void SendToALL(GSPacketIn packet)
        {
            this.SendToALL(packet, null);
        }
        public void SendToALL(GSPacketIn packet, GamePlayer except)
        {
            GamePlayer[] array = null;
            Dictionary<int, GamePlayer> list;
            Monitor.Enter(list = this.m_list);
            try
            {
                array = new GamePlayer[this.m_list.Count];
                this.m_list.Values.CopyTo(array, 0);
            }
            finally
            {
                Monitor.Exit(list);
            }
            if (array != null)
            {
                GamePlayer[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    GamePlayer gamePlayer = array2[i];
                    if (gamePlayer != null && gamePlayer != except)
                    {
                        gamePlayer.Out.SendTCP(packet);
                    }
                }
            }
        }
    }
}

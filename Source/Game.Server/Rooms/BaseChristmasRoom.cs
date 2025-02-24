using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseChristmasRoom
	{
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		public static int LIVIN = 0;
		public static int DEAD = 2;
		public static int FIGHTING = 1;
		public static int MonterAddCount = 15;
		protected int lastMonterID = 1000;
		private int[] monterType = new int[]
		{
			0,
			1,
			2
		};
		private Point[] brithPoint = new Point[]
		{
			new Point(353, 570),
			new Point(246, 760),
			new Point(593, 590),
			new Point(466, 898),
			new Point(800, 950),
			new Point(946, 748),
			new Point(1152, 873),
			new Point(1172, 874),
			new Point(1766, 630),
			new Point(1342, 581),
			new Point(1732, 401),
			new Point(1462, 326),
			new Point(1187, 207),
			new Point(878, 236),
			new Point(1590, 521)
		};
		private Dictionary<int, GamePlayer> m_players;
		private Dictionary<int, MonterInfo> m_monters;
		public int DefaultPosX = 500;
		public int DefaultPosY = 500;
		public Dictionary<int, MonterInfo> Monters
		{
			get
			{
				return this.m_monters;
			}
		}
		public BaseChristmasRoom()
		{
			this.m_players = new Dictionary<int, GamePlayer>();
			this.m_monters = new Dictionary<int, MonterInfo>();
			this.AddFistMonters();
		}
		public void AddFistMonters()
		{
			Dictionary<int, MonterInfo> monters;
			Monitor.Enter(monters = this.m_monters);
			try
			{
				for (int i = 0; i < BaseChristmasRoom.MonterAddCount; i++)
				{
					MonterInfo monterInfo = new MonterInfo();
					monterInfo.ID = this.lastMonterID;
					monterInfo.type = BaseChristmasRoom.random.Next(this.monterType.Length);
					monterInfo.MonsterPos = this.brithPoint[i];
					monterInfo.MonsterNewPos = this.brithPoint[i];
					monterInfo.state = BaseChristmasRoom.LIVIN;
					monterInfo.PlayerID = 0;
					if (!this.m_monters.ContainsKey(monterInfo.ID))
					{
						this.m_monters.Add(monterInfo.ID, monterInfo);
					}
					this.lastMonterID++;
				}
			}
			finally
			{
				Monitor.Exit(monters);
			}
		}
		private int GetFreeMonter()
		{
			int num = 0;
			foreach (MonterInfo current in this.Monters.Values)
			{
				if (current.state == BaseChristmasRoom.LIVIN)
				{
					num++;
				}
			}
			return num;
		}
		public void AddMoreMonters()
		{
			if (this.GetFreeMonter() < this.m_players.Count)
			{
				this.AddMonters();
			}
		}
		public void AddMonters()
		{
			Dictionary<int, MonterInfo> monters;
			Monitor.Enter(monters = this.m_monters);
			try
			{
				MonterInfo monterInfo = new MonterInfo();
				monterInfo.ID = this.lastMonterID;
				monterInfo.type = BaseChristmasRoom.random.Next(this.monterType.Length);
				int num = BaseChristmasRoom.random.Next(this.brithPoint.Length);
				monterInfo.MonsterPos = this.brithPoint[num];
				monterInfo.MonsterNewPos = this.brithPoint[num];
				monterInfo.state = BaseChristmasRoom.LIVIN;
				monterInfo.PlayerID = 0;
				if (!this.m_monters.ContainsKey(monterInfo.ID))
				{
					this.m_monters.Add(monterInfo.ID, monterInfo);
				}
				this.lastMonterID++;
			}
			finally
			{
				Monitor.Exit(monters);
			}
		}
		public bool SetFightMonter(int Id, int playerId)
		{
			bool result = false;
			Dictionary<int, MonterInfo> monters;
			Monitor.Enter(monters = this.m_monters);
			try
			{
				if (this.m_monters.ContainsKey(Id))
				{
					this.m_monters[Id].state = BaseChristmasRoom.FIGHTING;
					this.m_monters[Id].PlayerID = playerId;
					result = true;
				}
			}
			finally
			{
				Monitor.Exit(monters);
			}
			this.AddMonters();
			return result;
		}
		public void SetMonterDie(int playerId)
		{
			int num = -1;
			foreach (MonterInfo current in this.m_monters.Values)
			{
				if (current.PlayerID == playerId)
				{
					num = current.ID;
					break;
				}
			}
			if (num > -1)
			{
				Dictionary<int, MonterInfo> monters;
				Monitor.Enter(monters = this.m_monters);
				try
				{
					if (this.m_monters.ContainsKey(num))
					{
						this.m_monters.Remove(num);
					}
				}
				finally
				{
					Monitor.Exit(monters);
				}
				GSPacketIn gSPacketIn = new GSPacketIn(145);
				gSPacketIn.WriteByte(22);
				gSPacketIn.WriteByte(1);
				gSPacketIn.WriteInt(num);
				this.SendToALL(gSPacketIn);
			}
		}
		public void AddPlayer(GamePlayer player)
		{
			Dictionary<int, GamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				if (!this.m_players.ContainsKey(player.PlayerId))
				{
					player.IsInChristmasRoom = true;
					this.m_players.Add(player.PlayerId, player);
					player.Actives.BeginChristmasTimer();
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			this.UpdateRoom();
		}
		public void UpdateRoom()
		{
			GamePlayer[] playersSafe = this.GetPlayersSafe();
			GSPacketIn gSPacketIn = new GSPacketIn(145);
			gSPacketIn.WriteByte(18);
			gSPacketIn.WriteInt(playersSafe.Length);
			GamePlayer[] array = playersSafe;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
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
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
				gSPacketIn.WriteInt(gamePlayer.X);
				gSPacketIn.WriteInt(gamePlayer.Y);
				gSPacketIn.WriteByte(gamePlayer.States);
			}
			this.SendToALL(gSPacketIn);
		}
		public void ViewOtherPlayerRoom(GamePlayer player)
		{
			GamePlayer[] playersSafe = this.GetPlayersSafe();
			GSPacketIn gSPacketIn = new GSPacketIn(145);
			gSPacketIn.WriteByte(18);
			gSPacketIn.WriteInt(playersSafe.Length);
			GamePlayer[] array = playersSafe;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
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
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
				gSPacketIn.WriteInt(gamePlayer.X);
				gSPacketIn.WriteInt(gamePlayer.Y);
				gSPacketIn.WriteByte(gamePlayer.States);
			}
			player.SendTCP(gSPacketIn);
		}
		public bool RemovePlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				player.Actives.StopChristmasTimer();
				flag = this.m_players.Remove(player.PlayerId);
			}
			finally
			{
				Monitor.Exit(players);
			}
			if (flag)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(145);
				gSPacketIn.WriteByte(19);
				gSPacketIn.WriteInt(player.PlayerId);
				this.SendToALL(gSPacketIn);
				player.IsInChristmasRoom = false;
				player.Out.SendSceneRemovePlayer(player);
			}
			return flag;
		}
		public GamePlayer[] GetPlayersSafe()
		{
			GamePlayer[] array = null;
			Dictionary<int, GamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				array = new GamePlayer[this.m_players.Count];
				this.m_players.Values.CopyTo(array, 0);
			}
			finally
			{
				Monitor.Exit(players);
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
			GamePlayer[] playersSafe = this.GetPlayersSafe();
			if (playersSafe != null)
			{
				GamePlayer[] array = playersSafe;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					if (gamePlayer != null && gamePlayer != except)
					{
						gamePlayer.Out.SendTCP(packet);
					}
				}
			}
		}
	}
}

using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseCampBattleRoom
	{
		private Dictionary<int, GamePlayer> m_players;
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		private Point[] brithPoint = new Point[]
		{
			new Point(353, 570),
			new Point(246, 760),
			new Point(593, 590),
			new Point(466, 898),
			new Point(800, 950),
			new Point(946, 748)
		};
		public BaseCampBattleRoom()
		{
			this.m_players = new Dictionary<int, GamePlayer>();
		}
		public bool AddPlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				if (!this.m_players.ContainsKey(player.PlayerId))
				{
					this.m_players.Add(player.PlayerId, player);
					flag = true;
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			if (flag)
			{
				this.SendCampInitSecen(player);
				this.SendUpdateRoom(player);
				this.SendCampSocerRank();
			}
			return flag;
		}
		public void SendPerScoreRank(GamePlayer player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(146);
			gSPacketIn.WriteByte(21);
			gSPacketIn.WriteInt(1);
			for (int i = 0; i < 1; i++)
			{
				gSPacketIn.WriteInt(player.ZoneId);
				gSPacketIn.WriteInt(player.PlayerId);
				gSPacketIn.WriteString(player.ZoneName);
				gSPacketIn.WriteString(player.PlayerCharacter.NickName);
				gSPacketIn.WriteInt(33);
			}
			player.SendTCP(gSPacketIn);
		}
		public void SendCampSocerRank()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(146);
			gSPacketIn.WriteByte(20);
			gSPacketIn.WriteInt(4);
			for (int i = 1; i < 5; i++)
			{
				gSPacketIn.WriteInt(i);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(15);
			}
			this.SendToALL(gSPacketIn);
		}
		public void SendCampInitSecen(GamePlayer player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(146);
			gSPacketIn.WriteByte(6);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(18);
			gSPacketIn.WriteBoolean(false);
			gSPacketIn.WriteInt(this.m_players.Count);
			foreach (GamePlayer current in this.m_players.Values)
			{
				gSPacketIn.WriteInt(current.PlayerCharacter.ID);
				gSPacketIn.WriteInt(current.ZoneId);
				gSPacketIn.WriteBoolean(current.PlayerCharacter.Sex);
				gSPacketIn.WriteString(current.PlayerCharacter.NickName);
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(184);
				gSPacketIn.WriteInt(281);
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(current.PlayerCharacter.typeVIP != 0);
				gSPacketIn.WriteInt(current.PlayerCharacter.VIPLevel);
			}
			gSPacketIn.WriteInt(1);
			for (int i = 0; i < 1; i++)
			{
				gSPacketIn.WriteInt(i + 1);
				gSPacketIn.WriteString("Living225");
				gSPacketIn.WriteString("Tà Linh Kate");
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(0);
			}
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteBoolean(false);
			player.SendTCP(gSPacketIn);
		}
		public void SendCampFightMonster(GamePlayer player)
		{
			RoomMgr.CreateCampBattleBossRoom(player, 60018);
		}
		public void SendUpdateMonsterStatus(GamePlayer player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(146);
			gSPacketIn.WriteByte(7);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(1);
			this.SendToALL(gSPacketIn);
		}
		public void SendUpdatePlayerStatus(GamePlayer player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(146);
			gSPacketIn.WriteByte(8);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(1);
			this.SendToALL(gSPacketIn);
		}
		public bool RemovePlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				flag = this.m_players.Remove(player.PlayerId);
			}
			finally
			{
				Monitor.Exit(players);
			}
			if (flag)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(146);
				gSPacketIn.WriteByte(3);
				gSPacketIn.WriteInt(player.ZoneId);
				gSPacketIn.WriteInt(player.PlayerId);
				player.SendTCP(gSPacketIn);
				this.SendToALL(gSPacketIn, player);
			}
			return true;
		}
		public void SendUpdateRoom(GamePlayer player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(146);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(this.m_players.Count);
			foreach (GamePlayer current in this.m_players.Values)
			{
				gSPacketIn.WriteInt(current.PlayerCharacter.ID);
				gSPacketIn.WriteInt(current.ZoneId);
				gSPacketIn.WriteBoolean(current.PlayerCharacter.Sex);
				gSPacketIn.WriteString(current.PlayerCharacter.NickName);
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(current.X);
				gSPacketIn.WriteInt(current.Y);
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(current.PlayerCharacter.typeVIP != 0);
				gSPacketIn.WriteInt(current.PlayerCharacter.VIPLevel);
			}
			this.SendToALL(gSPacketIn, player);
		}
		public void Challenge(int PlayerId, int ChallengeId)
		{
		}
		public void PlayerMove(GamePlayer player, int PosX, int PosY, int zoneId, int PlayerId)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(146);
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(PosX);
			gSPacketIn.WriteInt(PosY);
			gSPacketIn.WriteInt(zoneId);
			gSPacketIn.WriteInt(PlayerId);
			player.SendTCP(gSPacketIn);
			this.SendToALL(gSPacketIn, player);
		}
		public void SendToALL(GSPacketIn packet)
		{
			this.SendToALL(packet, null);
		}
		public void SendToALL(GSPacketIn packet, GamePlayer except)
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
	}
}

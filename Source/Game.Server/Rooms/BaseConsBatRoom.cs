using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseConsBatRoom
	{
		private Dictionary<int, GamePlayer> m_players;
		private Dictionary<int, ConsortiaBattlePlayerInfo> m_consortiaBattlePlayerInfo;
		private Point[] brithPoint = new Point[]
		{
			new Point(353, 570),
			new Point(246, 760),
			new Point(593, 590),
			new Point(466, 898),
			new Point(800, 950),
			new Point(946, 748),
			new Point(1152, 873),
			new Point(1172, 874)
		};
		public BaseConsBatRoom()
		{
			this.m_players = new Dictionary<int, GamePlayer>();
			this.m_consortiaBattlePlayerInfo = new Dictionary<int, ConsortiaBattlePlayerInfo>();
		}
		public ConsortiaBattlePlayerInfo CreateConsortiaBattlePlayerInfo(GamePlayer player)
		{
			return new ConsortiaBattlePlayerInfo
			{
				PlayerID = player.PlayerCharacter.ID,
				Sex = player.PlayerCharacter.Sex,
				curHp = player.PlayerCharacter.hp,
				posX = 631,
				posY = 959,
				consortiaID = player.PlayerCharacter.ConsortiaID,
				consortiaName = player.PlayerCharacter.ConsortiaName,
				tombstoneEndTime = DateTime.Now.AddMinutes(-10.0),
				status = 1,
				victoryCount = 0,
				winningStreak = 0,
				failBuffCount = 0,
				score = 0,
				isPowerFullUsed = false,
				isDoubleScoreUsed = false
			};
		}
		public bool AddPlayer(GamePlayer player)
		{
			bool result = true;
			Dictionary<int, GamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				if (!this.m_players.ContainsKey(player.PlayerId))
				{
					this.m_players.Add(player.PlayerId, player);
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			ConsortiaBattlePlayerInfo consortiaBattlePlayerInfo = this.CreateConsortiaBattlePlayerInfo(player);
			Dictionary<int, ConsortiaBattlePlayerInfo> consortiaBattlePlayerInfo2;
			Monitor.Enter(consortiaBattlePlayerInfo2 = this.m_consortiaBattlePlayerInfo);
			try
			{
				if (!this.m_consortiaBattlePlayerInfo.ContainsKey(player.PlayerId))
				{
					this.m_consortiaBattlePlayerInfo.Add(player.PlayerId, consortiaBattlePlayerInfo);
				}
			}
			finally
			{
				Monitor.Exit(consortiaBattlePlayerInfo2);
			}
			GSPacketIn gSPacketIn = new GSPacketIn(153, player.PlayerCharacter.ID);
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteBoolean(true);
			gSPacketIn.WriteDateTime(consortiaBattlePlayerInfo.tombstoneEndTime);
			gSPacketIn.WriteInt(consortiaBattlePlayerInfo.posX);
			gSPacketIn.WriteInt(consortiaBattlePlayerInfo.posY);
			gSPacketIn.WriteInt(consortiaBattlePlayerInfo.curHp);
			gSPacketIn.WriteInt(consortiaBattlePlayerInfo.victoryCount);
			gSPacketIn.WriteInt(consortiaBattlePlayerInfo.winningStreak);
			gSPacketIn.WriteInt(consortiaBattlePlayerInfo.score);
			gSPacketIn.WriteBoolean(consortiaBattlePlayerInfo.isDoubleScoreUsed);
			gSPacketIn.WriteBoolean(consortiaBattlePlayerInfo.isPowerFullUsed);
			player.SendTCP(gSPacketIn);
			return result;
		}
		public bool RemovePlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				flag = (this.m_players.Remove(player.PlayerId) && this.m_consortiaBattlePlayerInfo.Remove(player.PlayerId));
			}
			finally
			{
				Monitor.Exit(players);
			}
			if (flag)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(153);
				gSPacketIn.WriteByte(5);
				gSPacketIn.WriteInt(player.PlayerCharacter.ID);
				player.SendTCP(gSPacketIn);
				this.SendToALL(gSPacketIn, player);
			}
			return true;
		}
		public void SendUpdateRoom(GamePlayer player)
		{
			int count = this.m_consortiaBattlePlayerInfo.Count;
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(count);
			foreach (ConsortiaBattlePlayerInfo current in this.m_consortiaBattlePlayerInfo.Values)
			{
				gSPacketIn.WriteInt(current.PlayerID);
				gSPacketIn.WriteDateTime(current.tombstoneEndTime);
				gSPacketIn.WriteByte(current.status);
				gSPacketIn.WriteInt(current.posX);
				gSPacketIn.WriteInt(current.posY);
				gSPacketIn.WriteBoolean(current.Sex);
				gSPacketIn.WriteInt(current.consortiaID);
				gSPacketIn.WriteString(current.consortiaName);
				gSPacketIn.WriteInt(current.winningStreak);
				gSPacketIn.WriteInt(current.failBuffCount);
			}
			player.SendTCP(gSPacketIn);
		}
		public void Challenge(int PlayerId, int ChallengeId)
		{
			Dictionary<int, ConsortiaBattlePlayerInfo> consortiaBattlePlayerInfo;
			Monitor.Enter(consortiaBattlePlayerInfo = this.m_consortiaBattlePlayerInfo);
			try
			{
				GamePlayer gamePlayer = null;
				GamePlayer gamePlayer2 = null;
				if (this.m_consortiaBattlePlayerInfo.ContainsKey(ChallengeId) && this.m_players.ContainsKey(ChallengeId) && this.m_consortiaBattlePlayerInfo[ChallengeId].status == 1)
				{
					this.m_consortiaBattlePlayerInfo[ChallengeId].status = 2;
					this.SendUpdatePlayerStatus(this.m_consortiaBattlePlayerInfo[ChallengeId]);
					gamePlayer2 = this.m_players[ChallengeId];
					gamePlayer2.isPowerFullUsed = this.m_consortiaBattlePlayerInfo[ChallengeId].isPowerFullUsed;
					gamePlayer2.winningStreak = this.m_consortiaBattlePlayerInfo[ChallengeId].winningStreak;
					gamePlayer2.PlayerCharacter.hp = this.m_consortiaBattlePlayerInfo[PlayerId].curHp;
					gamePlayer2.CurrentRoomTeam = 2;
				}
				if (this.m_consortiaBattlePlayerInfo.ContainsKey(PlayerId) && this.m_players.ContainsKey(PlayerId) && this.m_consortiaBattlePlayerInfo[PlayerId].status == 1)
				{
					this.m_consortiaBattlePlayerInfo[PlayerId].status = 2;
					this.SendUpdatePlayerStatus(this.m_consortiaBattlePlayerInfo[PlayerId]);
					gamePlayer = this.m_players[PlayerId];
					gamePlayer.isPowerFullUsed = this.m_consortiaBattlePlayerInfo[PlayerId].isPowerFullUsed;
					gamePlayer.winningStreak = this.m_consortiaBattlePlayerInfo[PlayerId].winningStreak;
					gamePlayer.PlayerCharacter.hp = this.m_consortiaBattlePlayerInfo[PlayerId].curHp;
					gamePlayer.CurrentRoomTeam = 1;
				}
				if (gamePlayer != null && gamePlayer2 != null)
				{
					RoomMgr.CreateConsortiaBattleRoom(gamePlayer, gamePlayer2);
				}
			}
			finally
			{
				Monitor.Exit(consortiaBattlePlayerInfo);
			}
		}
		public void SendUpdatePlayerStatus(ConsortiaBattlePlayerInfo info)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153, info.PlayerID);
			gSPacketIn.WriteByte(7);
			gSPacketIn.WriteInt(info.PlayerID);
			gSPacketIn.WriteDateTime(info.tombstoneEndTime);
			gSPacketIn.WriteByte(info.status);
			gSPacketIn.WriteInt(info.posX);
			gSPacketIn.WriteInt(info.posY);
			gSPacketIn.WriteInt(info.winningStreak);
			gSPacketIn.WriteInt(info.failBuffCount);
			this.SendToALL(gSPacketIn);
		}
		public void PlayerMove(int PosX, int PosY, int PlayerId)
		{
			Dictionary<int, ConsortiaBattlePlayerInfo> consortiaBattlePlayerInfo;
			Monitor.Enter(consortiaBattlePlayerInfo = this.m_consortiaBattlePlayerInfo);
			try
			{
				if (this.m_consortiaBattlePlayerInfo.ContainsKey(PlayerId))
				{
					this.m_consortiaBattlePlayerInfo[PlayerId].posX = PosX;
					this.m_consortiaBattlePlayerInfo[PlayerId].posY = PosY;
				}
			}
			finally
			{
				Monitor.Exit(consortiaBattlePlayerInfo);
			}
		}
		public void BattleWin(int WinPlayerId, int lostwinningStreak, int curHp)
		{
			int num = 30;
			Dictionary<int, ConsortiaBattlePlayerInfo> consortiaBattlePlayerInfo;
			Monitor.Enter(consortiaBattlePlayerInfo = this.m_consortiaBattlePlayerInfo);
			try
			{
				if (this.m_consortiaBattlePlayerInfo.ContainsKey(WinPlayerId))
				{
					this.m_consortiaBattlePlayerInfo[WinPlayerId].status = 1;
					this.m_consortiaBattlePlayerInfo[WinPlayerId].tombstoneEndTime = DateTime.Now.AddSeconds(-1.0);
					this.m_consortiaBattlePlayerInfo[WinPlayerId].curHp = curHp;
					this.m_consortiaBattlePlayerInfo[WinPlayerId].winningStreak++;
					this.m_consortiaBattlePlayerInfo[WinPlayerId].victoryCount++;
					if (this.m_consortiaBattlePlayerInfo[WinPlayerId].winningStreak == 3)
					{
						num = 50;
					}
					else
					{
						if (this.m_consortiaBattlePlayerInfo[WinPlayerId].winningStreak == 6)
						{
							num = 70;
						}
						else
						{
							if (this.m_consortiaBattlePlayerInfo[WinPlayerId].winningStreak == 10)
							{
								num = 110;
							}
						}
					}
					if (this.m_consortiaBattlePlayerInfo[WinPlayerId].isDoubleScoreUsed)
					{
						num *= 2;
						this.m_consortiaBattlePlayerInfo[WinPlayerId].isDoubleScoreUsed = false;
					}
					this.m_consortiaBattlePlayerInfo[WinPlayerId].score += num;
					if (lostwinningStreak >= 3 && lostwinningStreak < 6)
					{
						this.m_consortiaBattlePlayerInfo[WinPlayerId].score += 50;
					}
					else
					{
						if (lostwinningStreak >= 6 && lostwinningStreak < 10)
						{
							this.m_consortiaBattlePlayerInfo[WinPlayerId].score += 70;
						}
						else
						{
							if (lostwinningStreak >= 10)
							{
								this.m_consortiaBattlePlayerInfo[WinPlayerId].score += 90;
							}
						}
					}
					this.m_consortiaBattlePlayerInfo[WinPlayerId].isPowerFullUsed = false;
				}
			}
			finally
			{
				Monitor.Exit(consortiaBattlePlayerInfo);
			}
		}
		public void BattleLost(int LostPlayerId)
		{
			int num = 5;
			Dictionary<int, ConsortiaBattlePlayerInfo> consortiaBattlePlayerInfo;
			Monitor.Enter(consortiaBattlePlayerInfo = this.m_consortiaBattlePlayerInfo);
			try
			{
				if (this.m_consortiaBattlePlayerInfo.ContainsKey(LostPlayerId))
				{
					int arg_31_0 = this.m_consortiaBattlePlayerInfo[LostPlayerId].winningStreak;
					this.m_consortiaBattlePlayerInfo[LostPlayerId].status = 1;
					this.m_consortiaBattlePlayerInfo[LostPlayerId].winningStreak = 0;
					this.m_consortiaBattlePlayerInfo[LostPlayerId].tombstoneEndTime = DateTime.Now.AddSeconds(15.0);
					this.m_consortiaBattlePlayerInfo[LostPlayerId].posX = 631;
					this.m_consortiaBattlePlayerInfo[LostPlayerId].posY = 959;
					if (this.m_consortiaBattlePlayerInfo[LostPlayerId].isDoubleScoreUsed)
					{
						num *= 2;
						this.m_consortiaBattlePlayerInfo[LostPlayerId].isDoubleScoreUsed = false;
					}
					this.m_consortiaBattlePlayerInfo[LostPlayerId].isPowerFullUsed = false;
					this.m_consortiaBattlePlayerInfo[LostPlayerId].score += num;
				}
			}
			finally
			{
				Monitor.Exit(consortiaBattlePlayerInfo);
			}
		}
		public void SendConfirmEnterRoom(GamePlayer player)
		{
			int count = this.m_consortiaBattlePlayerInfo.Count;
			GSPacketIn gSPacketIn = new GSPacketIn(153, player.PlayerCharacter.ID);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(count);
			foreach (ConsortiaBattlePlayerInfo current in this.m_consortiaBattlePlayerInfo.Values)
			{
				gSPacketIn.WriteInt(current.PlayerID);
				gSPacketIn.WriteDateTime(current.tombstoneEndTime);
				gSPacketIn.WriteByte(current.status);
				gSPacketIn.WriteInt(current.posX);
				gSPacketIn.WriteInt(current.posY);
				gSPacketIn.WriteBoolean(current.Sex);
				gSPacketIn.WriteInt(current.consortiaID);
				gSPacketIn.WriteString(current.consortiaName);
				gSPacketIn.WriteInt(current.winningStreak);
				gSPacketIn.WriteInt(current.failBuffCount);
			}
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

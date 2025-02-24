using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseWaitingRoom
	{
		private Dictionary<int, GamePlayer> m_list;
		public BaseWaitingRoom()
		{
			this.m_list = new Dictionary<int, GamePlayer>();
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
					this.m_list.Add(player.PlayerId, player);
					flag = true;
				}
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (flag)
			{
				GSPacketIn packet = player.Out.SendSceneAddPlayer(player);
				this.SendToALL(packet, player);
			}
			return flag;
		}
		public bool RemovePlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				flag = this.m_list.Remove(player.PlayerId);
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (flag)
			{
				GSPacketIn packet = player.Out.SendSceneRemovePlayer(player);
				this.SendToALL(packet, player);
			}
			return true;
		}
		public void SendSceneUpdate(GamePlayer player)
		{
			GSPacketIn packet = player.Out.SendSceneAddPlayer(player);
			this.SendToALL(packet, player);
			GamePlayer[] playersSafe = this.GetPlayersSafe();
			GamePlayer[] array = playersSafe;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer != player)
				{
					player.Out.SendSceneAddPlayer(gamePlayer);
				}
			}
		}
		public void SendUpdateRoom(BaseRoom room)
		{
			GamePlayer[] playersSafe = this.GetPlayersSafe();
			List<BaseRoom> allRooms = RoomMgr.GetAllRooms();
			List<GamePlayer> list = new List<GamePlayer>();
			List<GamePlayer> list2 = new List<GamePlayer>();
			List<BaseRoom> list3 = new List<BaseRoom>();
			List<BaseRoom> list4 = new List<BaseRoom>();
			GamePlayer[] array = playersSafe;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerState == ePlayerState.Online)
				{
					list.Add(gamePlayer);
				}
				if (gamePlayer.PlayerState == ePlayerState.Away)
				{
					list2.Add(gamePlayer);
				}
			}
			foreach (BaseRoom current in allRooms)
			{
				if (current.RoomType == eRoomType.Freedom || current.RoomType == eRoomType.Match)
				{
					list3.Add(current);
				}
				if (current.RoomType == eRoomType.Dungeon || current.RoomType == eRoomType.AcademyDungeon || current.RoomType == eRoomType.ActivityDungeon || current.RoomType == eRoomType.SpecialActivityDungeon)
				{
					list4.Add(current);
				}
			}
			this.SendUpdateRoom(list, list3);
			this.SendUpdateRoom(list2, list4);
		}
		public void SendUpdateRoom(List<GamePlayer> players, List<BaseRoom> rooms)
		{
			GSPacketIn gSPacketIn = null;
			foreach (GamePlayer current in players)
			{
				if (gSPacketIn == null)
				{
					gSPacketIn = current.Out.SendUpdateRoomList(rooms);
				}
				else
				{
					current.Out.SendTCP(gSPacketIn);
				}
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
	}
}

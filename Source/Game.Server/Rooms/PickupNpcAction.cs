using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class PickupNpcAction : IAction
	{
		private int m_roomID;
		public PickupNpcAction(int roomID)
		{
			this.m_roomID = roomID;
		}
		public GamePlayer CreateNpcPlayer()
		{
			return new GamePlayer(1, "Null Player", null, null);
		}
		public void Execute()
		{
			BaseRoom baseRoom = null;
			BaseRoom[] rooms = RoomMgr.Rooms;
			for (int i = 0; i < rooms.Length; i++)
			{
				if (rooms[i].RoomId == this.m_roomID)
				{
					baseRoom = rooms[i];
					break;
				}
			}
			if (baseRoom != null)
			{
				GamePlayer playerById = WorldMgr.GetPlayerById(19);
				if (playerById != null)
				{
					RoomMgr.WaitingRoom.RemovePlayer(playerById);
					playerById.Out.SendRoomLoginResult(true);
					playerById.Out.SendRoomCreate(baseRoom);
					baseRoom.AddPlayerUnsafe(playerById);
					if (baseRoom.PlayerCount == 2)
					{
						List<IGamePlayer> list = new List<IGamePlayer>();
						List<IGamePlayer> list2 = new List<IGamePlayer>();
						list.Add(baseRoom.Host);
						list2.Add(playerById);
						BaseGame baseGame = GameMgr.StartPVPGame(baseRoom.RoomId, list, list2, baseRoom.MapId, baseRoom.RoomType, baseRoom.GameType, (int)baseRoom.TimeMode);
						if (baseGame != null)
						{
							baseRoom.IsPlaying = true;
							baseRoom.StartGame(baseGame);
							return;
						}
						baseRoom.IsPlaying = false;
						baseRoom.SendPlayerState();
					}
				}
			}
		}
	}
}

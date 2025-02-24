using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class CreateConsortiaBattleRoomAction : IAction
	{
		private GamePlayer m_player;
		private GamePlayer m_challenge;
		private string m_name;
		private string m_password;
		private eRoomType m_roomType;
		private byte m_timeType;
		public CreateConsortiaBattleRoomAction(GamePlayer player, GamePlayer ChallengePlayer)
		{
			this.m_player = player;
			this.m_challenge = ChallengePlayer;
			this.m_name = "Consortia Battle";
			this.m_password = "12dasSda44";
			this.m_roomType = eRoomType.ConsortiaBattle;
			this.m_timeType = 2;
		}
		public void Execute()
		{
			if (this.m_player.CurrentRoom != null)
			{
				this.m_player.CurrentRoom.RemovePlayerUnsafe(this.m_player);
			}
			if (!this.m_player.IsActive)
			{
				return;
			}
			if (this.m_challenge.CurrentRoom != null)
			{
				this.m_challenge.CurrentRoom.RemovePlayerUnsafe(this.m_player);
			}
			if (!this.m_challenge.IsActive)
			{
				return;
			}
			BaseRoom[] rooms = RoomMgr.Rooms;
			BaseRoom baseRoom = null;
			for (int i = 0; i < rooms.Length; i++)
			{
				if (!rooms[i].IsUsing)
				{
					baseRoom = rooms[i];
					break;
				}
			}
			if (baseRoom != null)
			{
				RoomMgr.WaitingRoom.RemovePlayer(this.m_player);
				baseRoom.Start();
				baseRoom.UpdateRoom(this.m_name, this.m_password, this.m_roomType, this.m_timeType, 0);
				this.m_player.Out.SendRoomCreate(baseRoom);
				if (baseRoom.AddPlayerUnsafe(this.m_player))
				{
					RoomMgr.WaitingRoom.RemovePlayer(this.m_challenge);
					this.m_challenge.Out.SendRoomLoginResult(true);
					this.m_challenge.Out.SendRoomCreate(baseRoom);
					baseRoom.AddPlayerUnsafe(this.m_challenge);
				}
				if (baseRoom.PlayerCount == 2)
				{
					List<IGamePlayer> list = new List<IGamePlayer>();
					List<IGamePlayer> list2 = new List<IGamePlayer>();
					list.Add(this.m_player);
					list2.Add(this.m_challenge);
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

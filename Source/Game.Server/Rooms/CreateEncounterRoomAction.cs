using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;
namespace Game.Server.Rooms
{
	public class CreateEncounterRoomAction : IAction
	{
		private GamePlayer m_player;
		private string m_name;
		private string m_password;
		private eRoomType m_roomType;
		private byte m_timeType;
		public CreateEncounterRoomAction(GamePlayer player, eRoomType roomType)
		{
			this.m_player = player;
			this.m_name = "Couple PvP";
			this.m_password = "12dasSda44";
			this.m_roomType = roomType;
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
			BaseRoom[] rooms = RoomMgr.Rooms;
			BaseRoom baseRoom = this.FindRandomRoom(rooms);
			if (baseRoom == null)
			{
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
					baseRoom.ZoneId = this.m_player.ZoneId;
					this.m_player.Out.SendSingleRoomCreate(baseRoom);
					baseRoom.AddPlayerUnsafe(this.m_player);
				}
			}
			else
			{
				RoomMgr.WaitingRoom.RemovePlayer(this.m_player);
				this.m_player.Out.SendRoomLoginResult(true);
				this.m_player.Out.SendSingleRoomCreate(baseRoom);
				baseRoom.AddPlayerUnsafe(this.m_player);
			}
			if (baseRoom.PlayerCount == 2)
			{
				BattleServer battleServer = BattleMgr.AddRoom(baseRoom);
				if (battleServer != null)
				{
					baseRoom.BattleServer = battleServer;
					baseRoom.IsPlaying = true;
					baseRoom.SendStartPickUp();
					return;
				}
				GSPacketIn pkg = baseRoom.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe", new object[0]));
				baseRoom.SendToAll(pkg, baseRoom.Host);
				baseRoom.SendCancelPickUp();
			}
		}
		private BaseRoom FindRandomRoom(BaseRoom[] rooms)
		{
			for (int i = 0; i < rooms.Length; i++)
			{
				if (rooms[i] != null && rooms[i].RoomType == this.m_roomType && !rooms[i].IsPlaying && rooms[i].PlayerCount < 2 && rooms[i].RoomType == eRoomType.Encounter && rooms[i].Host != null && rooms[i].Host.PlayerCharacter.Sex != this.m_player.PlayerCharacter.Sex)
				{
					return rooms[i];
				}
			}
			return null;
		}
	}
}

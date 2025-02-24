using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;
namespace Game.Server.Rooms
{
	public class CreateGroupBattleRoomAction : IAction
	{
		private GamePlayer m_player;
		private string m_name;
		private string m_password;
		private eRoomType m_roomType;
		private byte m_timeType;
		private int m_groupType;
		public CreateGroupBattleRoomAction(GamePlayer player, int groupType)
		{
			this.m_player = player;
			this.m_name = "GroupBattle PvP";
			this.m_password = "12dasSda44";
			this.m_roomType = eRoomType.SingleBattle;
			this.m_timeType = 2;
			this.m_groupType = groupType;
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
			int group = this.GetGroup(this.m_groupType);
			if (group == 1)
			{
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
					baseRoom.ZoneId = this.m_player.ZoneId;
					this.m_player.Out.SendSingleRoomCreate(baseRoom);
					baseRoom.AddPlayerUnsafe(this.m_player);
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
					return;
				}
			}
			else
			{
				BaseRoom baseRoom2 = this.FindRandomRoom(rooms, group);
				if (baseRoom2 == null)
				{
					for (int j = 0; j < rooms.Length; j++)
					{
						if (!rooms[j].IsUsing)
						{
							baseRoom2 = rooms[j];
							break;
						}
					}
					if (baseRoom2 != null)
					{
						RoomMgr.WaitingRoom.RemovePlayer(this.m_player);
						baseRoom2.Start();
						baseRoom2.UpdateRoom(this.m_name, this.m_password, this.m_roomType, this.m_timeType, 0);
						baseRoom2.ZoneId = this.m_player.ZoneId;
						this.m_player.Out.SendSingleRoomCreate(baseRoom2);
						baseRoom2.AddPlayerUnsafe(this.m_player);
					}
				}
				else
				{
					RoomMgr.WaitingRoom.RemovePlayer(this.m_player);
					this.m_player.Out.SendRoomLoginResult(true);
					this.m_player.Out.SendSingleRoomCreate(baseRoom2);
					baseRoom2.AddPlayerUnsafe(this.m_player);
				}
				if (baseRoom2.PlayerCount == group)
				{
					BattleServer battleServer2 = BattleMgr.AddRoom(baseRoom2);
					if (battleServer2 != null)
					{
						baseRoom2.BattleServer = battleServer2;
						baseRoom2.IsPlaying = true;
						baseRoom2.SendStartPickUp();
						return;
					}
					GSPacketIn pkg2 = baseRoom2.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe", new object[0]));
					baseRoom2.SendToAll(pkg2, baseRoom2.Host);
					baseRoom2.SendCancelPickUp();
				}
			}
		}
		private int GetGroup(int group)
		{
			switch (group)
			{
			case 7:
				return 1;
			case 8:
				return 2;
			case 9:
				return 3;
			case 10:
				return 4;
			default:
				return 1;
			}
		}
		private BaseRoom FindRandomRoom(BaseRoom[] rooms, int group)
		{
			for (int i = 0; i < rooms.Length; i++)
			{
				if (rooms[i] != null && rooms[i].RoomType == this.m_roomType && !rooms[i].IsPlaying && rooms[i].PlayerCount < group && rooms[i].RoomType == eRoomType.SingleBattle)
				{
					return rooms[i];
				}
			}
			return null;
		}
	}
}

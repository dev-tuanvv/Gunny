using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Packets;
using Game.Server.RingStation;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class StartGameAction : IAction
	{
		private BaseRoom m_room;
		public StartGameAction(BaseRoom room)
		{
			this.m_room = room;
            IsAllSameAcademy(room);
		}
		public void Execute()
		{
			if (this.m_room.CanStart())
			{
				List<GamePlayer> players = this.m_room.GetPlayers();
				if (this.m_room.RoomType == eRoomType.Freedom)
				{
					List<IGamePlayer> list = new List<IGamePlayer>();
					List<IGamePlayer> list2 = new List<IGamePlayer>();
					foreach (GamePlayer current in players)
					{
						if (current != null)
						{
							if (current.CurrentRoomTeam == 1)
							{
								list.Add(current);
							}
							else
							{
								list2.Add(current);
							}
						}
					}
					BaseGame game = GameMgr.StartPVPGame(this.m_room.RoomId, list, list2, this.m_room.MapId, this.m_room.RoomType, this.m_room.GameType, (int)this.m_room.TimeMode);
					this.StartGame(game);
				}
				else
				{
					if (this.IsPVE(this.m_room.RoomType))
					{
						List<IGamePlayer> list3 = new List<IGamePlayer>();
						foreach (GamePlayer current2 in players)
						{
							if (current2 != null)
							{
								list3.Add(current2);
							}
						}
						this.UpdatePveRoomTimeMode();
						BaseGame game2 = GameMgr.StartPVEGame(this.m_room.RoomId, list3, this.m_room.MapId, this.m_room.RoomType, this.m_room.GameType, (int)this.m_room.TimeMode, this.m_room.HardLevel, this.m_room.LevelLimits, this.m_room.currentFloor);
						this.StartGame(game2);
					}
					else
					{
						if (this.m_room.RoomType == eRoomType.Match)
						{
							this.m_room.UpdateAvgLevel();
                            if (this.m_room.GetPlayers().Count == 1 && this.m_room.Host != null)
                            {
                                this.m_room.PickUpNpcId = RingStationConfiguration.NextPlayerID();
                            }

							BattleServer battleServer = BattleMgr.AddRoom(this.m_room);
							if (battleServer != null)
							{
								this.m_room.BattleServer = battleServer;
								this.m_room.IsPlaying = true;
								this.m_room.SendStartPickUp();
							}
							else
							{
								GSPacketIn pkg = this.m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe", new object[0]));
								this.m_room.SendToAll(pkg, this.m_room.Host);
								this.m_room.SendCancelPickUp();
							}
						}
					}
				}
				//RoomMgr.WaitingRoom.SendUpdateRoom(this.m_room);
			}
		}
		private void StartGame(BaseGame game)
		{
			if (game != null)
			{
				this.m_room.IsPlaying = true;
				this.m_room.StartGame(game);
				return;
			}
			this.m_room.IsPlaying = false;
			this.m_room.SendPlayerState();
		}
		private bool IsPVE(eRoomType roomType)
		{
			if (roomType <= eRoomType.ConsortiaBoss)
			{
				switch (roomType)
				{
				case eRoomType.Dungeon:
				case eRoomType.FightLib:
					break;
				default:
					switch (roomType)
					{
					case eRoomType.Freshman:
					case eRoomType.AcademyDungeon:
					case eRoomType.WordBossFight:
					case eRoomType.Lanbyrinth:
					case eRoomType.ConsortiaBoss:
						break;
					case eRoomType.ScoreLeage:
					case eRoomType.GuildLeageRank:
					case eRoomType.Encounter:
						return false;
					default:
						return false;
					}
					break;
				}
			}
			else
			{
				switch (roomType)
				{
				case eRoomType.ActivityDungeon:
				case eRoomType.SpecialActivityDungeon:
					break;
				case eRoomType.TransnationalFight:
					return false;
				default:
					if (roomType != eRoomType.Christmas)
					{
						return false;
					}
					break;
				}
			}
			return true;
		}
		private void UpdatePveRoomTimeMode()
		{
			if (this.IsPVE(this.m_room.RoomType))
			{
				switch (this.m_room.HardLevel)
				{
				case eHardLevel.Easy:
					this.m_room.TimeMode = 3;
					return;
				case eHardLevel.Normal:
					this.m_room.TimeMode = 2;
					return;
				case eHardLevel.Hard:
					this.m_room.TimeMode = 1;
					return;
				case eHardLevel.Terror:
					this.m_room.TimeMode = 1;
					break;
				default:
					return;
				}
			}
		}
        public void IsAllSameAcademy(BaseRoom room)
        {
            List<GamePlayer> players = room.GetPlayers();
            foreach (var item in players)
            {
                //item.PlayerCharacter.IsAcadeMy = false;
            }
            if (players.Count == 2)
            {
                //if (players[0].AcdInfo.MasterID == players[1].PlayerCharacter.ID || players[1].AcdInfo.MasterID == players[0].PlayerCharacter.ID)
                //{
                //    players[0].PlayerCharacter.IsAcadeMy = true;
                //    players[1].PlayerCharacter.IsAcadeMy = true;
                //}
            }
        }

	}
}

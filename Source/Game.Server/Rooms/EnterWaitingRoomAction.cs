using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class EnterWaitingRoomAction : IAction
	{
		private GamePlayer m_player;
		public EnterWaitingRoomAction(GamePlayer player)
		{
			this.m_player = player;
		}
		public void Execute()
		{
			if (this.m_player == null)
			{
				return;
			}
			if (this.m_player.CurrentRoom != null)
			{
				this.m_player.CurrentRoom.RemovePlayerUnsafe(this.m_player);
			}
			BaseWaitingRoom waitingRoom = RoomMgr.WaitingRoom;
			if (waitingRoom.AddPlayer(this.m_player))
			{
				List<BaseRoom> allRooms = RoomMgr.GetAllRooms();
				this.m_player.Out.SendUpdateRoomList(allRooms);
				this.m_player.Out.SendSceneAddPlayer(this.m_player);
				GamePlayer[] playersSafe = waitingRoom.GetPlayersSafe();
				for (int i = 0; i < playersSafe.Length; i++)
				{
					GamePlayer gamePlayer = playersSafe[i];
					if (gamePlayer != this.m_player)
					{
						this.m_player.Out.SendSceneAddPlayer(gamePlayer);
					}
				}
			}
		}
	}
}

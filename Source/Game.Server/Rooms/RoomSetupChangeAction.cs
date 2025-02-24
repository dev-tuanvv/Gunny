using Game.Logic;
using System;
namespace Game.Server.Rooms
{
	internal class RoomSetupChangeAction : IAction
	{
		private BaseRoom m_room;
		private eRoomType m_roomType;
		private byte m_timeMode;
		private eHardLevel m_hardLevel;
		private int m_mapId;
		private int m_levelLimits;
		private string m_password;
		private string m_roomName;
		private bool m_isCrosszone;
		private bool m_isOpenBoss;
		private string m_pic;
		private int m_currentFloor;
        public RoomSetupChangeAction(BaseRoom room, eRoomType roomType, byte timeMode, eHardLevel hardLevel, int levelLimits, int mapId, string password, string roomname, bool isCrosszone)
        {
            this.m_room = room;
            this.m_roomType = roomType;
            this.m_timeMode = timeMode;
            this.m_hardLevel = hardLevel;
            this.m_levelLimits = levelLimits;
            this.m_mapId = mapId;
            this.m_roomName = password;
            this.m_password = roomname;
            this.m_isCrosszone = isCrosszone;
            //this.m_isOpenBoss = isOpenBoss;
            //	this.m_pic = Pic;
            //	this.m_currentFloor = currentFloor;
            //if (isOpenBoss)
            //{
            //    if (mapId == 1)
            //    {
            //        if (this.m_hardLevel == eHardLevel.Easy)
            //        {
            //            this.m_currentFloor = 2;
            //            this.m_pic = "show2.jpg";
            //            return;
            //        }
            //        if (this.m_hardLevel == eHardLevel.Normal)
            //        {
            //            this.m_currentFloor = 3;
            //            this.m_pic = "show3.jpg";
            //            return;
            //        }
            //        if (this.m_hardLevel == eHardLevel.Hard)
            //        {
            //            this.m_currentFloor = 4;
            //            this.m_pic = "show4.jpg";
            //            return;
            //        }
            //        this.m_currentFloor = 5;
            //        this.m_pic = "show5.jpg";
            //        return;
            //    }
            //    else
            //    {
            //        if (mapId == 2)
            //        {
            //            this.m_currentFloor = 2;
            //            this.m_pic = "show2.jpg";
            //            return;
            //        }
            //        if (mapId == 4)
            //        {
            //            this.m_currentFloor = 3;
            //            this.m_pic = "show3.jpg";
            //            return;
            //        }
            //        this.m_currentFloor = 4;
            //        this.m_pic = "show4.jpg";
            //    }
            //}
        }
		public void Execute()
		{
			this.m_room.RoomType = this.m_roomType;
			this.m_room.TimeMode = this.m_timeMode;
			this.m_room.HardLevel = this.m_hardLevel;
			this.m_room.LevelLimits = this.m_levelLimits;
			this.m_room.MapId = this.m_mapId;
			this.m_room.Name = this.m_roomName;
			this.m_room.Password = this.m_password;
			this.m_room.isCrosszone = this.m_isCrosszone;
			this.m_room.isOpenBoss = this.m_isOpenBoss;
			this.m_room.currentFloor = this.m_currentFloor;
			this.m_room.Pic = this.m_pic;
			this.m_room.UpdateRoomGameType();
			this.m_room.SendRoomSetupChange(this.m_room);
			RoomMgr.WaitingRoom.SendUpdateRoom(this.m_room);
		}
	}
}

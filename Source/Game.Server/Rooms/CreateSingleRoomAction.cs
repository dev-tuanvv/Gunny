using Game.Logic;
using Game.Server.GameObjects;

namespace Game.Server.Rooms
{
    public class CreateSingleRoomAction : IAction
    {
        private GamePlayer m_player;
        private int m_type;
        private eRoomType m_roomtype;

        public CreateSingleRoomAction(GamePlayer player, int type)
        {
            this.m_player = player;
            this.m_type = type;
        }

        public void Execute()
        {
            if (this.m_player.CurrentRoom != null)
                this.m_player.CurrentRoom.RemovePlayerUnsafe(this.m_player);
            if (!this.m_player.IsActive)
                return;
            BaseRoom[] rooms = RoomMgr.Rooms;
            BaseRoom room = (BaseRoom)null;
            for (int index = 0; index < rooms.Length; ++index)
            {
                if (!rooms[index].IsUsing)
                {
                    room = rooms[index];
                    break;
                }
            }
            if (room == null)
                return;
            room.Start();
            this.m_roomtype = eRoomType.FightGround;
            room.UpdateRoom("Đấu trường quần đùi", "123456", this.m_roomtype, (byte)0, 0);
            this.m_player.Out.SendSingleRoomCreate(room);
            room.AddPlayerUnsafe(this.m_player);
            RoomMgr.StartGame(room);
        }
    }
}

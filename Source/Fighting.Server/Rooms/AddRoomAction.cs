namespace Fighting.Server.Rooms
{
    using System;

    public class AddRoomAction : IAction
    {
        private ProxyRoom m_room;

        public AddRoomAction(ProxyRoom room)
        {
            this.m_room = room;
        }

        public void Execute()
        {
            ProxyRoomMgr.AddRoomUnsafe(this.m_room);
        }
    }
}


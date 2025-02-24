namespace Game.Server.SceneMarryRooms
{
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using System;

    public abstract class AbstractMarryProcessor : IMarryProcessor
    {
        protected AbstractMarryProcessor()
        {
        }

        public virtual void OnGameData(MarryRoom game, GamePlayer player, GSPacketIn packet)
        {
        }

        public virtual void OnTick(MarryRoom room)
        {
        }
    }
}


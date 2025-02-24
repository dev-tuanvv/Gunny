namespace Game.Server.HotSpringRooms
{
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using System;

    public abstract class AbstractHotSpringProcessor : GInterface2
    {
        protected AbstractHotSpringProcessor()
        {
        }

        public virtual void OnGameData(HotSpringRoom game, GamePlayer player, GSPacketIn packet)
        {
        }

        public virtual void OnTick(HotSpringRoom room)
        {
        }
    }
}


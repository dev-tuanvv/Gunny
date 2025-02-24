namespace Game.Server.SceneMarryRooms
{
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using System;

    public interface IMarryProcessor
    {
        void OnGameData(MarryRoom game, GamePlayer player, GSPacketIn packet);
        void OnTick(MarryRoom room);
    }
}


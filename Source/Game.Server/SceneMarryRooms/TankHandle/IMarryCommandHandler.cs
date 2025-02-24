namespace Game.Server.SceneMarryRooms.TankHandle
{
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.SceneMarryRooms;
    using System;

    public interface IMarryCommandHandler
    {
        bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet);
    }
}


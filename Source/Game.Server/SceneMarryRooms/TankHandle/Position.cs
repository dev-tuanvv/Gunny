﻿namespace Game.Server.SceneMarryRooms.TankHandle
{
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.SceneMarryRooms;
    using System;

    [MarryCommandAttbute(10)]
    public class Position : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom != null)
            {
                player.X = packet.ReadInt();
                player.Y = packet.ReadInt();
                return true;
            }
            return false;
        }
    }
}


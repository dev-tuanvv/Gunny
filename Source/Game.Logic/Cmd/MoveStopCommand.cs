﻿namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(10, "开始移动")]
    public class MoveStopCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
        }
    }
}


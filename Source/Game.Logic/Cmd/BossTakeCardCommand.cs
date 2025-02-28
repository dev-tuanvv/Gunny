﻿namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(130, "战胜关卡中Boss翻牌")]
    public class BossTakeCardCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (!player.FinishTakeCard && (player.CanTakeOut > 0))
            {
                int index = packet.ReadByte();
                if ((index < 0) || (index > game.Cards.Length))
                {
                    game.TakeCard(player);
                }
                else
                {
                    game.TakeCard(player, index);
                }
            }
        }
    }
}


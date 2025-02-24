namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x62, "翻牌")]
    public class TakeCardCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
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


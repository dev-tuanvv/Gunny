namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(0x72, "付费翻牌")]
    public class PaymentTakeCardCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (!player.HasPaymentTakeCard)
            {
                if (player.PlayerDetail.RemoveMoney(500) > 0)
                {
                    int index = packet.ReadByte();
                    player.CanTakeOut++;
                    player.FinishTakeCard = false;
                    player.HasPaymentTakeCard = true;
                    player.PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_PaymentTakeCard, player.PlayerDetail.PlayerCharacter.ID, 100, player.PlayerDetail.PlayerCharacter.Money);
                    if ((index < 0) || (index > game.Cards.Length))
                    {
                        game.TakeCard(player);
                    }
                    else
                    {
                        game.TakeCard(player, index);
                    }
                }
                else
                {
                    player.PlayerDetail.SendInsufficientMoney(1);
                }
            }
        }
    }
}


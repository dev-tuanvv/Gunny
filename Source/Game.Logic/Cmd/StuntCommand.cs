namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    [GameCommand(15, "使用必杀技能")]
    public class StuntCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                player.UseSpecialSkill();
                player.CurrentShootMinus *= (float) player.CurrentBall.Power;
            }
        }
    }
}


namespace Game.Logic.Spells.FightingSpell
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;

    [SpellAttibute(12)]
    public class ShootStraightSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsLiving)
            {
                player.ControlBall = true;
                player.CurrentShootMinus *= 0.5f;
            }
            else if (((game.CurrentLiving != null) && (game.CurrentLiving is Player)) && (game.CurrentLiving.Team == player.Team))
            {
                game.CurrentLiving.ControlBall = true;
                TurnedLiving currentLiving = game.CurrentLiving;
                currentLiving.CurrentShootMinus *= 0.5f;
            }
        }
    }
}


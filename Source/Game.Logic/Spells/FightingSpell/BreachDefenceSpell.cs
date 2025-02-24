namespace Game.Logic.Spells.FightingSpell
{
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Object;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;

    [SpellAttibute(8)]
    public class BreachDefenceSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsLiving)
            {
                player.IgnoreArmor = true;
                game.AddAction(new FightAchievementAction(player, 6, player.Direction, 0x4b0));
            }
            else if (((game.CurrentLiving != null) && (game.CurrentLiving is Player)) && (game.CurrentLiving.Team == player.Team))
            {
                game.CurrentLiving.IgnoreArmor = true;
                game.AddAction(new FightAchievementAction(game.CurrentLiving, 6, game.CurrentLiving.Direction, 0x4b0));
            }
        }
    }
}


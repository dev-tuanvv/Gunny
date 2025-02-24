namespace Game.Logic.Spells.FightingSpell
{
    using Game.Logic;
    using Game.Logic.Effects;
    using Game.Logic.Phy.Object;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;

    [SpellAttibute(30)]
    internal class SealSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsLiving)
            {
                new SealEffect(item.Property3).Start(player);
            }
            else if (((game.CurrentLiving != null) && (game.CurrentLiving is Player)) && (game.CurrentLiving.Team == player.Team))
            {
                new SealEffect(item.Property3).Start(game.CurrentLiving);
            }
        }
    }
}


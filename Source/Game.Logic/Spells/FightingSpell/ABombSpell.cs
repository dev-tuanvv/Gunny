﻿namespace Game.Logic.Spells.FightingSpell
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;

    [SpellAttibute(10)]
    public class ABombSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsLiving)
            {
                player.SetBall(4);
            }
            else if (((game.CurrentLiving != null) && (game.CurrentLiving is Player)) && (game.CurrentLiving.Team == player.Team))
            {
                (game.CurrentLiving as Player).SetBall(4);
            }
        }
    }
}


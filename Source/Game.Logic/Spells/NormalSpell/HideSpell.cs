namespace Game.Logic.Spells.NormalSpell
{
    using Game.Logic;
    using Game.Logic.Effects;
    using Game.Logic.Phy.Object;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [SpellAttibute(3)]
    public class HideSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            switch (item.Property2)
            {
                case 0:
                    if (!player.IsLiving)
                    {
                        if (((game.CurrentLiving != null) && (game.CurrentLiving is Player)) && (game.CurrentLiving.Team == player.Team))
                        {
                            new HideEffect(item.Property3).Start(game.CurrentLiving);
                        }
                        break;
                    }
                    new HideEffect(item.Property3).Start(player);
                    break;

                case 1:
                {
                    List<Player> allFightPlayers = player.Game.GetAllFightPlayers();
                    foreach (Player player2 in allFightPlayers)
                    {
                        if (player2.IsLiving && (player2.Team == player.Team))
                        {
                            new HideEffect(item.Property3).Start(player2);
                        }
                    }
                    break;
                }
            }
        }
    }
}


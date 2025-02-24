namespace Game.Logic.Spells.NormalSpell
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [SpellAttibute(1)]
    public class AddLifeSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            switch (item.Property2)
            {
                case 0:
                {
                    int num = item.Property3;
                    if (!player.IsLiving)
                    {
                        if (((game.CurrentLiving != null) && (game.CurrentLiving is Player)) && (game.CurrentLiving.Team == player.Team))
                        {
                            game.CurrentLiving.AddBlood(num);
                        }
                        break;
                    }
                    if (player.FightBuffers.ConsortionAddSpellCount > 0)
                    {
                        num += player.FightBuffers.ConsortionAddSpellCount;
                    }
                    player.AddBlood(num);
                    break;
                }
                case 1:
                {
                    List<Player> allFightPlayers = player.Game.GetAllFightPlayers();
                    foreach (Player player2 in allFightPlayers)
                    {
                        if (player2.IsLiving && (player2.Team == player.Team))
                        {
                            player2.AddBlood(item.Property3);
                        }
                    }
                    break;
                }
            }
        }
    }
}


namespace Game.Logic.Spells.FightingSpell
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;

    [SpellAttibute(14)]
    public class AddAttackSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (!player.IsLiving)
            {
                if (((game.CurrentLiving != null) && (game.CurrentLiving is Player)) && (game.CurrentLiving.Team == player.Team))
                {
                    if ((((player.CurrentBall.ID == 3) || (player.CurrentBall.ID == 5)) || (player.CurrentBall.ID == 1)) && ((item.TemplateID == 0x2711) || (item.TemplateID == 0x2712)))
                    {
                        (game.CurrentLiving as Player).ShootCount = 1;
                    }
                    else
                    {
                        Player currentLiving = game.CurrentLiving as Player;
                        currentLiving.ShootCount += item.Property2;
                        if (item.Property2 == 2)
                        {
                            TurnedLiving living1 = game.CurrentLiving;
                            living1.CurrentShootMinus *= 0.6f;
                        }
                        else
                        {
                            TurnedLiving living2 = game.CurrentLiving;
                            living2.CurrentShootMinus *= 0.9f;
                        }
                    }
                }
            }
            else if ((((player.CurrentBall.ID == 3) || (player.CurrentBall.ID == 5)) || (player.CurrentBall.ID == 1)) && ((item.TemplateID == 0x2711) || (item.TemplateID == 0x2712)))
            {
                player.ShootCount = 1;
            }
            else
            {
                player.ShootCount += item.Property2;
                if (item.Property2 == 2)
                {
                    player.CurrentShootMinus *= 0.6f;
                }
                else
                {
                    player.CurrentShootMinus *= 0.9f;
                }
            }
        }
    }
}


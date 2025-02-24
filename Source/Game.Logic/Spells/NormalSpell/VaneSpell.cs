namespace Game.Logic.Spells.NormalSpell
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;

    [SpellAttibute(7)]
    public class VaneSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            game.UpdateWind(-game.Wind, true);
        }
    }
}


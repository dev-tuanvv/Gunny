namespace Game.Logic.Spells
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using SqlDataProvider.Data;
    using System;

    public interface ISpellHandler
    {
        void Execute(BaseGame game, Player player, ItemTemplateInfo item);
    }
}


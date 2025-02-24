namespace Game.Logic.Actions
{
    using Game.Logic;
    using System;

    public interface IAction
    {
        void Execute(BaseGame game, long tick);
        bool IsFinished(long tick);
    }
}


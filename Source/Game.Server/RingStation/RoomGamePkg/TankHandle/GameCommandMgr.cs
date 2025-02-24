namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
    using Game.Server;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class GameCommandMgr
    {
        private Dictionary<int, IGameCommandHandler> handles = new Dictionary<int, IGameCommandHandler>();

        public GameCommandMgr()
        {
            this.handles.Clear();
            this.SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
        }

        public IGameCommandHandler LoadCommandHandler(int code)
        {
            return this.handles[code];
        }

        protected void RegisterCommandHandler(int code, IGameCommandHandler handle)
        {
            this.handles.Add(code, handle);
        }

        protected int SearchCommandHandlers(Assembly assembly)
        {
            int num = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && (type.GetInterface("Game.Server.RingStation.RoomGamePkg.TankHandle.IGameCommandHandler") != null))
                {
                    GameCommandAttbute[] customAttributes = (GameCommandAttbute[]) type.GetCustomAttributes(typeof(GameCommandAttbute), true);
                    if (customAttributes.Length > 0)
                    {
                        num++;
                        this.RegisterCommandHandler(customAttributes[0].Code, Activator.CreateInstance(type) as IGameCommandHandler);
                    }
                }
            }
            return num;
        }
    }
}


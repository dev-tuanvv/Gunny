namespace Game.Server.SceneMarryRooms.TankHandle
{
    using Game.Server;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class MarryCommandMgr
    {
        private Dictionary<int, IMarryCommandHandler> handles = new Dictionary<int, IMarryCommandHandler>();

        public MarryCommandMgr()
        {
            this.handles.Clear();
            this.SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
        }

        public IMarryCommandHandler LoadCommandHandler(int code)
        {
            return this.handles[code];
        }

        protected void RegisterCommandHandler(int code, IMarryCommandHandler handle)
        {
            this.handles.Add(code, handle);
        }

        protected int SearchCommandHandlers(Assembly assembly)
        {
            int num = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && (type.GetInterface("Game.Server.SceneMarryRooms.TankHandle.IMarryCommandHandler") != null))
                {
                    MarryCommandAttbute[] customAttributes = (MarryCommandAttbute[]) type.GetCustomAttributes(typeof(MarryCommandAttbute), true);
                    if (customAttributes.Length > 0)
                    {
                        num++;
                        this.RegisterCommandHandler(customAttributes[0].Code, Activator.CreateInstance(type) as IMarryCommandHandler);
                    }
                }
            }
            return num;
        }
    }
}


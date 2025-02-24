namespace Game.Server.HotSpringRooms.TankHandle
{
    using Game.Server;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class HotSpringCommandMgr
    {
        private Dictionary<int, IHotSpringCommandHandler> dictionary_0 = new Dictionary<int, IHotSpringCommandHandler>();

        public HotSpringCommandMgr()
        {
            this.dictionary_0.Clear();
            this.SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
        }

        public IHotSpringCommandHandler LoadCommandHandler(int code)
        {
            return this.dictionary_0[code];
        }

        protected void RegisterCommandHandler(int code, IHotSpringCommandHandler handle)
        {
            this.dictionary_0.Add(code, handle);
        }

        protected int SearchCommandHandlers(Assembly assembly)
        {
            int num = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && (type.GetInterface("Game.Server.HotSpringRooms.TankHandle.IHotSpringCommandHandler") != null))
                {
                    HotSpringCommandAttbute[] customAttributes = (HotSpringCommandAttbute[]) type.GetCustomAttributes(typeof(HotSpringCommandAttbute), true);
                    if (customAttributes.Length != 0)
                    {
                        num++;
                        this.RegisterCommandHandler(customAttributes[0].Code, Activator.CreateInstance(type) as IHotSpringCommandHandler);
                    }
                }
            }
            return num;
        }
    }
}


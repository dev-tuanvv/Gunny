namespace Game.Logic.Cmd
{
    using Game.Base.Events;
    using Game.Logic;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class CommandMgr
    {
        private static Dictionary<int, ICommandHandler> handles = new Dictionary<int, ICommandHandler>();

        public static ICommandHandler LoadCommandHandler(int code)
        {
            return handles[code];
        }

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            handles.Clear();
            SearchCommandHandlers(Assembly.GetAssembly(typeof(BaseGame)));
        }

        protected static void RegisterCommandHandler(int code, ICommandHandler handle)
        {
            handles.Add(code, handle);
        }

        protected static int SearchCommandHandlers(Assembly assembly)
        {
            int num = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && (type.GetInterface("Game.Logic.Cmd.ICommandHandler") != null))
                {
                    GameCommandAttribute[] customAttributes = (GameCommandAttribute[]) type.GetCustomAttributes(typeof(GameCommandAttribute), true);
                    if (customAttributes.Length > 0)
                    {
                        num++;
                        RegisterCommandHandler(customAttributes[0].Code, Activator.CreateInstance(type) as ICommandHandler);
                    }
                }
            }
            return num;
        }
    }
}


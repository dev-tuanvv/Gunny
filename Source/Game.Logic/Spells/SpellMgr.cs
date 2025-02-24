namespace Game.Logic.Spells
{
    using Game.Base.Events;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class SpellMgr
    {
        private static Dictionary<int, ISpellHandler> handles = new Dictionary<int, ISpellHandler>();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void ExecuteSpell(BaseGame game, Player player, ItemTemplateInfo item)
        {
            try
            {
                LoadSpellHandler(item.Property1).Execute(game, player, item);
            }
            catch (Exception exception)
            {
                log.Error("Execute Spell Error:", exception);
            }
        }

        public static ISpellHandler LoadSpellHandler(int code)
        {
            return handles[code];
        }

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            handles.Clear();
            int num = SearchSpellHandlers(Assembly.GetAssembly(typeof(BaseGame)));
            if (log.IsInfoEnabled)
            {
                log.Info("SpellMgr: Loaded " + num + " spell handlers from GameServer Assembly!");
            }
        }

        protected static void RegisterSpellHandler(int type, ISpellHandler handle)
        {
            handles.Add(type, handle);
        }

        protected static int SearchSpellHandlers(Assembly assembly)
        {
            int num = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && (type.GetInterface("Game.Logic.Spells.ISpellHandler") != null))
                {
                    SpellAttibute[] customAttributes = (SpellAttibute[]) type.GetCustomAttributes(typeof(SpellAttibute), true);
                    if (customAttributes.Length > 0)
                    {
                        num++;
                        RegisterSpellHandler(customAttributes[0].Type, Activator.CreateInstance(type) as ISpellHandler);
                    }
                }
            }
            return num;
        }
    }
}


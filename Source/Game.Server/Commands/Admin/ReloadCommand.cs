namespace Game.Server.Commands.Admin
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base;
    using Game.Server;
    using System;
    using System.Linq;
    using System.Text;

    [Cmd("&load", ePrivLevel.Player, "Load the metedata.", new string[] { "   /load  [option]...  ", "Option:    /config     :Application config file.", "           /shop       :ShopMgr.ReLoad().", "           /item       :ItemMgr.Reload().", "           /property   :Game properties." })]
    public class ReloadCommand : AbstractCommandHandler, ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length > 1)
            {
                StringBuilder builder = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                if (args.Contains<string>("/cmd"))
                {
                    CommandMgr.LoadCommands();
                    this.DisplayMessage(client, "Command load success!");
                    builder.Append("/cmd,");
                }
                if (args.Contains<string>("/config"))
                {
                    GameServer.Instance.Configuration.Refresh();
                    this.DisplayMessage(client, "Application config file load success!");
                    builder.Append("/config,");
                }
                if (args.Contains<string>("/property"))
                {
                    GameProperties.Refresh();
                    this.DisplayMessage(client, "Game properties load success!");
                    builder.Append("/property,");
                }
                if (args.Contains<string>("/item"))
                {
                    if (ItemMgr.ReLoad())
                    {
                        this.DisplayMessage(client, "Items load success!");
                        builder.Append("/item,");
                    }
                    else
                    {
                        this.DisplayMessage(client, "Items load failed!");
                        builder2.Append("/item,");
                    }
                }
                if (args.Contains<string>("/shop"))
                {
                    if (ItemMgr.ReLoad())
                    {
                        this.DisplayMessage(client, "Shops load success!");
                        builder.Append("/shop,");
                    }
                    else
                    {
                        this.DisplayMessage(client, "Shops load failed!");
                        builder2.Append("/shop,");
                    }
                }
                if ((builder.Length == 0) && (builder2.Length == 0))
                {
                    this.DisplayMessage(client, "Nothing executed!");
                    this.DisplaySyntax(client);
                }
                else
                {
                    this.DisplayMessage(client, "Success Options:    " + builder.ToString());
                    if (builder2.Length > 0)
                    {
                        this.DisplayMessage(client, "Faile Options:      " + builder2.ToString());
                        return false;
                    }
                }
                return true;
            }
            this.DisplaySyntax(client);
            return true;
        }
    }
}


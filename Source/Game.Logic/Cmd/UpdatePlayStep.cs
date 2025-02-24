namespace Game.Logic.Cmd
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using log4net;
    using System;
    using System.Reflection;

    [GameCommand(0x19, "希望成为队长")]
    public class UpdatePlayStep : ICommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            packet.ReadInt();
            packet.ReadString();
        }
    }
}


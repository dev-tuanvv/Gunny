namespace Game.Logic.Actions
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Cmd;
    using Game.Logic.Phy.Object;
    using log4net;
    using System;
    using System.Reflection;

    public class ProcessPacketAction : IAction
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private GSPacketIn m_packet;
        private Player m_player;

        public ProcessPacketAction(Player player, GSPacketIn pkg)
        {
            this.m_player = player;
            this.m_packet = pkg;
        }

        public void Execute(BaseGame game, long tick)
        {
            if (this.m_player.IsActive)
            {
                eTankCmdType type = (eTankCmdType) this.m_packet.ReadByte();
                try
                {
                    ICommandHandler handler = CommandMgr.LoadCommandHandler((int) type);
                    if (handler != null)
                    {
                        handler.HandleCommand(game, this.m_player, this.m_packet);
                    }
                    else
                    {
                        log.Error(string.Format("Player Id: {0}", this.m_player.Id));
                    }
                }
                catch (Exception exception)
                {
                    log.Error(string.Format("Player Id: {0}  cmd:0x{1:X2}", this.m_player.Id, (byte) type), exception);
                }
            }
        }

        public bool IsFinished(long tick)
        {
            return true;
        }
    }
}


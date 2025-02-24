namespace Game.Server.HotSpringRooms
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.HotSpringRooms.TankHandle;
    using log4net;
    using System;
    using System.Reflection;

    [HotSpringProcessor(9, "礼堂逻辑")]
    public class TankHotSpringLogicProcessor : AbstractHotSpringProcessor
    {
        private HotSpringCommandMgr hotSpringCommandMgr_0 = new HotSpringCommandMgr();
        private static readonly ILog ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public readonly int TIMEOUT = 0xea60;
        private ThreadSafeRandom threadSafeRandom_0 = new ThreadSafeRandom();

        public override void OnGameData(HotSpringRoom room, GamePlayer player, GSPacketIn packet)
        {
            HotSpringCmdType type = (HotSpringCmdType) packet.ReadByte();
            try
            {
                IHotSpringCommandHandler handler = this.hotSpringCommandMgr_0.LoadCommandHandler((int) type);
                if (handler != null)
                {
                    handler.HandleCommand(this, player, packet);
                }
                else
                {
                    ilog_0.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
                }
            }
            catch (Exception exception)
            {
                ilog_0.Error(string.Format("IP:{1}, OnGameData is Error: {0}", exception.ToString(), player.Client.TcpEndpoint));
            }
        }

        public override void OnTick(HotSpringRoom room)
        {
            try
            {
            }
            catch (Exception exception)
            {
                if (ilog_0.IsErrorEnabled)
                {
                    ilog_0.Error("OnTick", exception);
                }
            }
        }
    }
}


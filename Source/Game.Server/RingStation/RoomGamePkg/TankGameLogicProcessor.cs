namespace Game.Server.RingStation.RoomGamePkg
{
    using Game.Base.Packets;
    using Game.Server.RingStation;
    using Game.Server.RingStation.RoomGamePkg.TankHandle;
    using log4net;
    using System;
    using System.Reflection;

    [GameProcessor(9, "礼堂逻辑")]
    public class TankGameLogicProcessor : AbstractGameProcessor
    {
        private GameCommandMgr _commandMgr = new GameCommandMgr();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public readonly int TIMEOUT = 0xea60;

        public override void OnGameData(RoomGame room, RingStationGamePlayer player, GSPacketIn packet)
        {
            GameCmdType code = (GameCmdType) packet.Code;
            try
            {
                IGameCommandHandler handler = this._commandMgr.LoadCommandHandler((int) code);
                if (handler != null)
                {
                    handler.HandleCommand(this, player, packet);
                }
                else
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("LoadCommandHandler not found!");
                    Console.WriteLine("_______________END_______________");
                }
            }
            catch (Exception)
            {
            }
        }

        public override void OnTick(RoomGame room)
        {
        }
    }
}


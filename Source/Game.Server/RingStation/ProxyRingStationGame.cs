namespace Game.Server.RingStation
{
    using Game.Base;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server.RingStation.Battle;
    using System;

    public class ProxyRingStationGame : AbstractGame
    {
        private RingStationFightConnector m_fightServer;

        public ProxyRingStationGame(int id, RingStationFightConnector fightServer, eRoomType roomType, eGameType gameType, int timeType) : base(id, roomType, gameType, timeType)
        {
            this.m_fightServer = fightServer;
            this.m_fightServer.Disconnected += new ClientEventHandle(this.m_fightingServer_Disconnected);
        }

        private void m_fightingServer_Disconnected(BaseClient client)
        {
            this.Stop();
        }

        public override void ProcessData(GSPacketIn pkg)
        {
            this.m_fightServer.SendToGame(base.Id, pkg);
        }
    }
}


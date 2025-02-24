namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
    using Game.Base.Packets;
    using Game.Server.RingStation;
    using Game.Server.RingStation.RoomGamePkg;
    using System;

    [GameCommandAttbute(0x5b)]
    public class GamePacket : IGameCommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, RingStationGamePlayer player, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            if (num <= 0x2e)
            {
                if (num != 2)
                {
                    if (num == 6)
                    {
                        player.NextTurn(packet);
                    }
                    else if (num == 0x2e)
                    {
                        player.AddTurn(packet);
                    }
                }
            }
            else
            {
                switch (num)
                {
                    case 100:
                        player.CurRoom.RemovePlayer(player);
                        break;

                    case 0x67:
                        player.SendLoadingComplete(100);
                        break;
                }
            }
            return true;
        }
    }
}


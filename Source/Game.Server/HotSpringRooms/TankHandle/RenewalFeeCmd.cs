namespace Game.Server.HotSpringRooms.TankHandle
{
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.HotSpringRooms;
    using System;

    [HotSpringCommandAttbute(3)]
    public class RenewalFeeCmd : IHotSpringCommandHandler
    {
        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentHotSpringRoom != null)
            {
                packet.ReadInt();
            }
            return false;
        }
    }
}


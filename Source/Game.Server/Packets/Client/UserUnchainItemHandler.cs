namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x2f, "解除物品")]
    public class UserUnchainItemHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if ((client.Player.CurrentRoom == null) || !client.Player.CurrentRoom.IsPlaying)
            {
                int fromSlot = packet.ReadInt();
                int toSlot = client.Player.EquipBag.FindFirstEmptySlot(0x1f);
                client.Player.EquipBag.MoveItem(fromSlot, toSlot, 0);
            }
            return 0;
        }
    }
}


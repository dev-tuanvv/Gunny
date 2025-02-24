namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(30, "打开物品")]
    public class LotteryGetItem : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ReadByte();
            packet.ReadInt();
            PlayerInventory caddyBag = client.Player.CaddyBag;
            PlayerInventory propBag = client.Player.PropBag;
            for (int i = 0; i < caddyBag.Capalility; i++)
            {
                SqlDataProvider.Data.ItemInfo itemAt = caddyBag.GetItemAt(i);
            }
            return 1;
        }
    }
}


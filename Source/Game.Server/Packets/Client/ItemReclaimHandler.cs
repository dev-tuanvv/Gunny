namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x7f, "物品比较")]
    public class ItemReclaimHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eBageType bageType = (eBageType) packet.ReadByte();
            int slot = packet.ReadInt();
            int count = packet.ReadInt();
            PlayerInventory inventory = client.Player.GetInventory(bageType);
            if ((inventory != null) && (inventory.GetItemAt(slot) != null))
            {
                if (inventory.GetItemAt(slot).Count <= count)
                {
                    count = inventory.GetItemAt(slot).Count;
                }
                ItemTemplateInfo template = inventory.GetItemAt(slot).Template;
                int num3 = count * template.ReclaimValue;
                if (template.ReclaimType == 2)
                {
                    client.Player.AddGiftToken(num3);
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.Success1", new object[] { num3 }));
                }
                if (template.ReclaimType == 1)
                {
                    client.Player.AddGold(num3);
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.Success2", new object[] { num3 }));
                }
                inventory.RemoveItemAt(slot);
                return 0;
            }
            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.NoSuccess", new object[0]));
            return 1;
        }
    }
}


namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(77, "物品过期")]
    public class ItemOverdueHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.IsPlaying)
            {
                return 0;
            }
            int num = (int)packet.ReadByte();
            int num2 = packet.ReadInt();
            PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
            ItemInfo itemAt = inventory.GetItemAt(num2);
            if (itemAt != null && !itemAt.IsValidItem())
            {
                if (num == 0 && num2 < 30)
                {
                    int num3 = inventory.FindFirstEmptySlot();
                    if (num3 == -1 || !inventory.MoveItem(itemAt.Place, num3, itemAt.Count))
                    {
                        client.Player.SendItemToMail(itemAt, LanguageMgr.GetTranslation("ItemOverdueHandler.Content", new object[0]), LanguageMgr.GetTranslation("ItemOverdueHandler.Title", new object[0]), eMailType.ItemOverdue);
                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                }
                else
                {
                    inventory.UpdateItem(itemAt);
                }
            }
            return 0;
        }
    }
}


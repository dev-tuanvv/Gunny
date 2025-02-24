namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.GameUtils;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0x6c, "选取")]
    public class GameTakeTempItemsHandler : IPacketHandler
    {
        private bool GetItem(GamePlayer player, SqlDataProvider.Data.ItemInfo item, ref string message)
        {
            if (item != null)
            {
                PlayerInventory itemInventory = player.GetItemInventory(item.Template);
                if (itemInventory.AddItem(item))
                {
                    player.TempBag.RemoveItem(item);
                    item.IsExist = true;
                    return true;
                }
                itemInventory.UpdateChangedPlaces();
                message = LanguageMgr.GetTranslation(item.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("GameTakeTempItemsHandler.Msg", new object[0]);
            }
            return false;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string message = string.Empty;
            int slot = packet.ReadInt();
            if (slot != -1)
            {
                SqlDataProvider.Data.ItemInfo itemAt = client.Player.TempBag.GetItemAt(slot);
                this.GetItem(client.Player, itemAt, ref message);
            }
            else
            {
                List<SqlDataProvider.Data.ItemInfo> items = client.Player.TempBag.GetItems();
                foreach (SqlDataProvider.Data.ItemInfo info2 in items)
                {
                    if (!this.GetItem(client.Player, info2, ref message))
                    {
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                client.Out.SendMessage(eMessageType.ERROR, message);
            }
            return 0;
        }
    }
}


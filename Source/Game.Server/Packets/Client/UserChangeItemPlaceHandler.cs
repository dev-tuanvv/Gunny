namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(49, "改变物品位置")]
    public class UserChangeItemPlaceHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eBageType bageType = (eBageType)packet.ReadByte();
            int slot = packet.ReadInt();
            eBageType type2 = (eBageType)packet.ReadByte();
            int place = packet.ReadInt();
            int count = packet.ReadInt();
            packet.ReadBoolean();
            PlayerInventory storeBag = client.Player.GetInventory(bageType);
            PlayerInventory inventory = client.Player.GetInventory(type2);
            ItemInfo itemAt = storeBag.GetItemAt(slot);
            int result;
            if (storeBag == null || itemAt == null)
            {
                result = 0;
            }
            else
            {
                if (count < 0 || count > itemAt.Count || count > itemAt.Template.MaxCount)
                {
                    Console.WriteLine(string.Concat(new object[]
                    {
                        "--count: ",
                        count,
                        " |itemCount: ",
                        itemAt.Count,
                        "|maxCount: ",
                        itemAt.Template.MaxCount
                    }));
                    result = 0;
                }
                else
                {
                    storeBag.BeginChanges();
                    inventory.BeginChanges();
                    try
                    {
                        if (type2 == eBageType.Consortia)
                        {
                            ConsortiaInfo info2 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                            if (info2 != null)
                            {
                                inventory.Capalility = info2.StoreLevel * 10;
                            }
                        }
                        if (place == -1)
                        {
                            bool flag = false;
                            if (bageType == eBageType.CaddyBag && type2 == eBageType.BeadBag)
                            {
                                place = inventory.FindFirstEmptySlot();
                                if (inventory.AddItemTo(itemAt, place))
                                {
                                    storeBag.TakeOutItem(itemAt);
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                            else
                            {
                                if (bageType == type2 && type2 == eBageType.EquipBag)
                                {
                                    place = inventory.FindFirstEmptySlot();
                                    int categoryID = itemAt.Template.CategoryID;
                                    if (categoryID <= 27)
                                    {
                                        switch (categoryID)
                                        {
                                            case 7:
                                            case 8:
                                            case 9:
                                            case 14:
                                            case 16:
                                            case 17:
                                                break;

                                            case 10:
                                            case 11:
                                            case 12:
                                            case 13:
                                            case 15:
                                                goto IL_258;

                                            default:
                                                if (categoryID != 27)
                                                {
                                                    goto IL_258;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        if (categoryID != 31 && categoryID != 40)
                                        {
                                            switch (categoryID)
                                            {
                                                case 50:
                                                case 51:
                                                case 52:
                                                    break;

                                                default:
                                                    goto IL_258;
                                            }
                                        }
                                    }
                                    int num4 = 1;
                                    goto IL_25D;
                                IL_258:
                                    num4 = 0;
                                IL_25D:
                                    if (place == -1 || (num4 == 1 && place > 81))
                                    {
                                        storeBag.TakeOutItem(itemAt);
                                        client.Player.SendItemToMail(itemAt, "Túi đạo cụ đầy vật phẩm chuyển vào thư", "Hành trang đã đầy. Gửi vào thư.", eMailType.ItemOverdue);
                                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                    }
                                    else
                                    {
                                        if (!storeBag.MoveItem(slot, place, count))
                                        {
                                            flag = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!inventory.StackItemToAnother(itemAt) && !inventory.AddItem(itemAt))
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        storeBag.TakeOutItem(itemAt);
                                    }
                                }
                            }
                            if (flag)
                            {
                                client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]));
                            }
                        }
                        else
                        {
                            if (bageType == type2)
                            {
                                ItemInfo info3 = storeBag.GetItemAt(place);
                                int num5 = 0;
                                if (bageType == eBageType.EquipBag)
                                {
                                    num5 = 31;
                                }
                                if (info3 != null && place >= num5)
                                {
                                    place = storeBag.FindFirstEmptySlot();
                                    if (place == -1)
                                    {
                                        storeBag.TakeOutItem(itemAt);
                                        client.Player.SendItemToMail(itemAt, "Túi đạo cụ đầy vật phẩm chuyển vào thư", "Hành trang đã đầy. Gửi vào thư.", eMailType.ItemOverdue);
                                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                    }
                                    else
                                    {
                                        storeBag.MoveItem(slot, place, count);
                                    }
                                }
                                else
                                {
                                    storeBag.MoveItem(slot, place, count);
                                }
                                client.Player.OnNewGearEvent(itemAt.Template.CategoryID);
                            }
                            else
                            {
                                switch (bageType)
                                {
                                    case eBageType.Consortia:
                                        if (inventory.GetItemAt(place) != null)
                                        {
                                            UserChangeItemPlaceHandler.MoveFromBank(client, slot, place, storeBag, inventory, itemAt);
                                        }
                                        else
                                        {
                                            UserChangeItemPlaceHandler.MoveFromBank(client, slot, place, storeBag, inventory, itemAt);
                                        }
                                        break;

                                    case eBageType.Store:
                                        this.MoveFromStore(client, storeBag, itemAt, place, inventory, count);
                                        break;

                                    default:
                                        if (type2 == eBageType.Store)
                                        {
                                            if (itemAt.IsAdvanceDate())
                                            {
                                                itemAt.StrengthenExp = 0;
                                                itemAt.AdvanceDate = DateTime.Now;
                                            }
                                            if (itemAt.Template.CategoryID == 14)
                                            {
                                                result = 0;
                                                return result;
                                            }
                                            if (itemAt.TemplateID == 200198 || itemAt.TemplateID == 11049)
                                            {
                                                place = 1;
                                            }
                                            this.MoveToStore(client, storeBag, itemAt, place, inventory, count);
                                        }
                                        else
                                        {
                                            if (type2 == eBageType.Consortia)
                                            {
                                                if (inventory.GetItemAt(place) != null)
                                                {
                                                    place = inventory.FindFirstEmptySlot();
                                                    if (place == -1)
                                                    {
                                                        storeBag.TakeOutItem(itemAt);
                                                        client.Player.SendItemToMail(itemAt, "Két đầy vật phẩm chuyển vào thư", "Hành trang đã đầy. Gửi vào thư.", eMailType.ItemOverdue);
                                                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                                    }
                                                    else
                                                    {
                                                        UserChangeItemPlaceHandler.MoveToBank(slot, place, storeBag, inventory, itemAt);
                                                    }
                                                }
                                                else
                                                {
                                                    UserChangeItemPlaceHandler.MoveToBank(slot, place, storeBag, inventory, itemAt);
                                                }
                                            }
                                            else
                                            {
                                                if (inventory.AddItemTo(itemAt, place))
                                                {
                                                    storeBag.TakeOutItem(itemAt);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    finally
                    {
                        storeBag.CommitChanges();
                        inventory.CommitChanges();
                    }
                    result = 0;
                }
            }
            return result;
        }
        private static void MoveFromBank(GameClient client, int place, int toplace, PlayerInventory bag, PlayerInventory tobag, ItemInfo item)
        {
            if (item != null)
            {
                PlayerInventory itemInventory = client.Player.GetItemInventory(item.Template);
                if (itemInventory != tobag)
                {
                    Console.WriteLine("???????????");
                    if (itemInventory.AddItem(item))
                    {
                        bag.TakeOutItem(item);
                    }
                }
                else
                {
                    int categoryID = item.Template.CategoryID;
                    if (categoryID <= 27)
                    {
                        switch (categoryID)
                        {
                            case 7:
                            case 8:
                            case 9:
                            case 14:
                            case 16:
                            case 17:
                                break;

                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 15:
                                goto IL_DA;

                            default:
                                if (categoryID != 27)
                                {
                                    goto IL_DA;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (categoryID != 31 && categoryID != 40)
                        {
                            switch (categoryID)
                            {
                                case 50:
                                case 51:
                                case 52:
                                    break;

                                default:
                                    goto IL_DA;
                            }
                        }
                    }
                    int num = 1;
                    goto IL_DE;
                IL_DA:
                    num = 0;
                IL_DE:
                    int num2;
                    switch (item.Template.CategoryID)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 13:
                        case 15:
                            num2 = 1;
                            goto IL_13B;
                    }
                    num2 = 0;
                IL_13B:
                    ItemInfo itemAt = itemInventory.GetItemAt(toplace);
                    if (num == 1 && (toplace > 80 || toplace < 31))
                    {
                        client.Player.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, "Túi đồ không phù hợp.");
                    }
                    else
                    {
                        if (num2 == 1 && (toplace > 385 || toplace < 81))
                        {
                            client.Player.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, "Túi đồ không phù hợp.");
                        }
                        else
                        {
                            if (itemAt == null)
                            {
                                if (itemInventory.AddItemTo(item, toplace))
                                {
                                    bag.TakeOutItem(item);
                                }
                            }
                            else
                            {
                                if (!item.CanStackedTo(itemAt) || item.Count + itemAt.Count > item.Template.MaxCount)
                                {
                                    itemInventory.TakeOutItem(itemAt);
                                    bag.TakeOutItem(item);
                                    itemInventory.AddItemTo(item, toplace);
                                    bag.AddItemTo(itemAt, place);
                                }
                                else
                                {
                                    if (itemInventory.AddCountToStack(itemAt, item.Count))
                                    {
                                        bag.RemoveCountFromStack(item, item.Count);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void MoveFromStore(GameClient client, PlayerInventory storeBag, ItemInfo item, int toSlot, PlayerInventory bag, int count)
        {
            if (client.Player == null || item == null || storeBag == null || bag == null || item.Template.BagType != (eBageType)bag.BagType)
            {
                if (toSlot < bag.BeginSlot || toSlot > bag.Capalility)
                {
                    if (bag.StackItemToAnother(item))
                    {
                        storeBag.RemoveItem(item, eItemRemoveType.Stack);
                        return;
                    }
                    string key = string.Format("temp_place_{0}", item.ItemID);
                    if (client.Player.TempProperties.ContainsKey(key))
                    {
                        toSlot = (int)storeBag.Player.TempProperties[key];
                        storeBag.Player.TempProperties.Remove(key);
                    }
                    else
                    {
                        toSlot = bag.FindFirstEmptySlot();
                    }
                }
                if (bag.StackItemToAnother(item) || bag.AddItemTo(item, toSlot))
                {
                    storeBag.TakeOutItem(item);
                }
                else
                {
                    toSlot = bag.FindFirstEmptySlot();
                    if (bag.AddItemTo(item, toSlot))
                    {
                        storeBag.TakeOutItem(item);
                    }
                    else
                    {
                        storeBag.TakeOutItem(item);
                        client.Player.SendItemToMail(item, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), eMailType.ItemOverdue);
                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                }
            }
        }
        private static void MoveToBank(int place, int toplace, PlayerInventory bag, PlayerInventory bank, ItemInfo item)
        {
            if (bag != null && item != null && bag != null)
            {
                ItemInfo itemAt = bank.GetItemAt(toplace);
                if (itemAt != null)
                {
                    if (item.CanStackedTo(itemAt) && item.Count + itemAt.Count <= item.Template.MaxCount)
                    {
                        if (bank.AddCountToStack(itemAt, item.Count))
                        {
                            bag.RemoveCountFromStack(item, item.Count);
                        }
                    }
                    else
                    {
                        if (itemAt.Template.BagType != (eBageType)bag.BagType)
                        {
                            bag.TakeOutItem(item);
                            bank.TakeOutItem(itemAt);
                            bag.AddItemTo(itemAt, place);
                            bank.AddItemTo(item, toplace);
                        }
                    }
                }
                else
                {
                    if (bank.AddItemTo(item, toplace))
                    {
                        bag.TakeOutItem(item);
                    }
                }
            }
        }
        public void MoveToStore(GameClient client, PlayerInventory bag, ItemInfo item, int toSlot, PlayerInventory storeBag, int count)
        {
            if (client.Player != null && bag != null && item != null && storeBag != null)
            {
                int place = item.Place;
                ItemInfo itemAt = storeBag.GetItemAt(toSlot);
                if (itemAt != null)
                {
                    if (item.Count == 1 && item.BagType == itemAt.BagType)
                    {
                        bag.TakeOutItem(item);
                        storeBag.TakeOutItem(itemAt);
                        bag.AddItemTo(itemAt, place);
                        storeBag.AddItemTo(item, toSlot);
                        return;
                    }
                    string str = string.Format("temp_place_{0}", itemAt.ItemID);
                    PlayerInventory itemInventory = client.Player.GetItemInventory(itemAt.Template);
                    if (client.Player.TempProperties.ContainsKey(str) && itemInventory.BagType == 0)
                    {
                        int num2 = (int)client.Player.TempProperties[str];
                        client.Player.TempProperties.Remove(str);
                        if (itemInventory.AddItemTo(itemAt, num2))
                        {
                            storeBag.TakeOutItem(itemAt);
                        }
                    }
                    else
                    {
                        if (itemInventory.StackItemToAnother(itemAt))
                        {
                            storeBag.RemoveItem(itemAt, eItemRemoveType.Stack);
                        }
                        else
                        {
                            if (itemInventory.AddItem(itemAt))
                            {
                                storeBag.TakeOutItem(itemAt);
                            }
                            else
                            {
                                client.Player.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]));
                            }
                        }
                    }
                }
                if (storeBag.IsEmpty(toSlot))
                {
                    if (item.Count == 1)
                    {
                        if (storeBag.AddItemTo(item, toSlot))
                        {
                            bag.TakeOutItem(item);
                            if (item.Template.BagType == eBageType.EquipBag && place < 31)
                            {
                                string str = string.Format("temp_place_{0}", item.ItemID);
                                if (client.Player.TempProperties.ContainsKey(str))
                                {
                                    client.Player.TempProperties[str] = place;
                                }
                                else
                                {
                                    client.Player.TempProperties.Add(str, place);
                                }
                            }
                        }
                    }
                    else
                    {
                        ItemInfo info2 = item.Clone();
                        info2.Count = count;
                        if (bag.RemoveCountFromStack(item, count, eItemRemoveType.Stack) && !storeBag.AddItemTo(info2, toSlot))
                        {
                            bag.AddCountToStack(item, count);
                        }
                    }
                }
            }
        }
    }
}


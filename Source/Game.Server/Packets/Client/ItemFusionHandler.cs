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
    using System.Collections.Generic;
    using System.Text;

    [PacketHandler(0x4e, "熔化")]
    public class ItemFusionHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            new StringBuilder();
            int num = packet.ReadByte();
            int minValid = 0x7fffffff;
            List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo>();
            List<SqlDataProvider.Data.ItemInfo> appendItems = new List<SqlDataProvider.Data.ItemInfo>();
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 1;
            }
            if (minValid == 0x7fffffff)
            {
                minValid = 0;
                items.Clear();
            }
            PlayerInventory storeBag = client.Player.StoreBag;
            for (int i = 1; i <= 4; i++)
            {
                SqlDataProvider.Data.ItemInfo itemAt = storeBag.GetItemAt(i);
                if (itemAt != null)
                {
                    items.Add(itemAt);
                }
            }
            if (items.Count != 4)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.ItemNotEnough", new object[0]));
                return 0;
            }
            if (num == 0)
            {
                bool isBind = false;
                Dictionary<int, double> previewItemList = FusionMgr.FusionPreview(items, appendItems, ref isBind);
                if (previewItemList != null)
                {
                    if (previewItemList.Count != 0)
                    {
                        client.Out.SendFusionPreview(client.Player, previewItemList, isBind, minValid);
                    }
                    else
                    {
                        client.Player.SendMessage("Vật phẩm n\x00e0y kh\x00f4ng thể dung luyện.");
                    }
                }
                else
                {
                    Console.WriteLine("previewItemList is NULL");
                }
            }
            else
            {
                int num4 = 400;
                if (client.Player.PlayerCharacter.Gold < 400)
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney", new object[0]));
                    return 0;
                }
                bool flag2 = false;
                bool result = false;
                ItemTemplateInfo goods = FusionMgr.Fusion(items, appendItems, ref flag2, ref result);
                if (goods != null)
                {
                    SqlDataProvider.Data.ItemInfo item = storeBag.GetItemAt(0);
                    if (item != null)
                    {
                        if (!(client.Player.StackItemToAnother(item) || client.Player.AddItem(item)))
                        {
                            List<SqlDataProvider.Data.ItemInfo> list3 = new List<SqlDataProvider.Data.ItemInfo> {
                                item
                            };
                            client.Player.SendItemsToMail(list3, "Vật phẩm từ dung luyện th\x00e0nh c\x00f4ng trả về thư do t\x00fai đầy", "Vật phẩm dung luyện", eMailType.StoreCanel);
                        }
                        storeBag.TakeOutItemAt(0);
                    }
                    storeBag.ClearBag();
                    client.Player.RemoveGold(num4);
                    for (int j = 0; j < items.Count; j++)
                    {
                        SqlDataProvider.Data.ItemInfo info4 = items[j];
                        info4.Count--;
                        client.Player.UpdateItem(items[j]);
                    }
                    for (int k = 0; k < appendItems.Count; k++)
                    {
                        SqlDataProvider.Data.ItemInfo info5 = appendItems[k];
                        info5.Count--;
                        client.Player.UpdateItem(appendItems[k]);
                    }
                    if (result)
                    {
                        SqlDataProvider.Data.ItemInfo info6 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x69);
                        if (info6 == null)
                        {
                            return 0;
                        }
                        info6.IsBinds = flag2;
                        info6.ValidDate = minValid;
                        client.Player.OnItemFusion(info6.Template.FusionType);
                        client.Out.SendMessage(eMessageType.Normal, "Dung luyện th\x00e0nh c\x00f4ng " + info6.Template.Name);
                        if (((info6.Template.CategoryID == 7) || (info6.Template.CategoryID == 0x11)) || ((info6.Template.CategoryID == 0x13) || (info6.Template.CategoryID == 0x10)))
                        {
                            client.Player.SaveNewItems();
                            object[] args = new object[] { client.Player.PlayerCharacter.NickName, "@" };
                            string translation = LanguageMgr.GetTranslation("ItemFusionHandler.Notice", args);
                            GSPacketIn @in = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, (info6.ItemID == 0) ? 1 : info6.ItemID, info6.TemplateID, "", 4);
                            GameServer.Instance.LoginServer.SendPacket(@in);
                            object[] objArray2 = new object[] { "TemplateID: ", info6.TemplateID, "|Name: ", info6.Template.Name };
                            client.Player.AddLog("Fusion", string.Concat(objArray2));
                        }
                        if (!client.Player.StoreBag.AddItemTo(info6, 0))
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(info6.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace", new object[0]));
                            object[] objArray3 = new object[] { "ItemFusionError", info6.Template.Name, "|TemplateID:", info6.TemplateID };
                            client.Player.AddLog("Error", string.Concat(objArray3));
                            List<SqlDataProvider.Data.ItemInfo> list5 = new List<SqlDataProvider.Data.ItemInfo> {
                                info6
                            };
                            client.Player.SendItemsToMail(list5, "Dung luyện th\x00e0nh c\x00f4ng " + info6.Template.Name + " t\x00fai đầy trả về thư", "Dung luyện th\x00e0nh c\x00f4ng", eMailType.BuyItem);
                        }
                        client.Out.SendFusionResult(client.Player, result);
                    }
                    else
                    {
                        client.Out.SendFusionResult(client.Player, result);
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Failed", new object[0]));
                    }
                    client.Player.SaveIntoDatabase();
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition", new object[0]));
                }
            }
            return 0;
        }
    }
}


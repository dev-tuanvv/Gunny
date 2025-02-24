namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Buffer;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;

    [PacketHandler(0xb7, "卡片使用")]
    public class CardUseHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadInt();
            int slot = packet.ReadInt();
            SqlDataProvider.Data.ItemInfo item = null;
            ShopItemInfo info2 = new ShopItemInfo();
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            if (DateTime.Compare(client.Player.LastOpenCard.AddSeconds(1.0), DateTime.Now) > 0)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Qu\x00e1 nhiều thao t\x00e1c!", new object[0]));
                return 0;
            }
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            if ((num == -1) && (slot == -1))
            {
                int num3 = packet.ReadInt();
                int templatID = packet.ReadInt();
                packet.ReadInt();
                int num5 = 0;
                int num6 = 0;
                for (int i = 0; i < num3; i++)
                {
                    info2 = ShopMgr.FindShopbyTemplateID(templatID);
                    if (info2 != null)
                    {
                        item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info2.TemplateID), 1, 0x66);
                        num6 = info2.AValue1;
                        item.ValidDate = info2.AUnit;
                    }
                    if (item != null)
                    {
                        if ((num5 <= client.Player.PlayerCharacter.Gold) && client.Player.MoneyDirect(num6))
                        {
                            client.Player.RemoveGold(num5);
                            list.Add(item);
                        }
                    }
                    else
                    {
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardUseHandler.Fail", new object[0]));
                    }
                }
            }
            else
            {
                item = client.Player.GetInventory((eBageType) num).GetItemAt(slot);
                if (item != null)
                {
                    list.Add(item);
                }
                string translateId = "CardUseHandler.Success";
                if (list.Count > 0)
                {
                    string str2 = string.Empty;
                    foreach (SqlDataProvider.Data.ItemInfo info4 in list)
                    {
                        if ((((info4.Template.Property1 == 13) || (info4.Template.Property1 == 11)) || (info4.Template.Property1 == 12)) || (info4.Template.Property1 == 0x1a))
                        {
                            AbstractBuffer buffer = BufferList.CreateBuffer(info4.Template, info4.ValidDate);
                            if (buffer != null)
                            {
                                buffer.Start(client.Player);
                                if ((slot != -1) && (num != -1))
                                {
                                    client.Player.GetInventory((eBageType) num).RemoveCountFromStack(info4, 1);
                                }
                            }
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
                        }
                        else if (info4.Template.Property5 == 3)
                        {
                            if (info4.ValidDate != 30)
                            {
                                info4.ValidDate = info4.Template.Property5 * 10;
                            }
                            AbstractBuffer buffer2 = BufferList.CreateBufferMinutes(info4.Template, info4.ValidDate);
                            if (buffer2 != null)
                            {
                                buffer2.Start(client.Player);
                                if ((slot != -1) && (num != -1))
                                {
                                    client.Player.GetInventory((eBageType) num).RemoveCountFromStack(info4, 1);
                                }
                            }
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
                        }
                        else
                        {
                            if (info4.IsValidItem() && (info4.Template.Property1 == 0x15))
                            {
                                if (info4.TemplateID == 0x311b9)
                                {
                                    int min = info4.Template.Property2 * item.Count;
                                    client.Player.Actives.AddTime(min);
                                    str2 = "TimerDanUser.Success";
                                }
                                else
                                {
                                    int gp = info4.Template.Property2 * item.Count;
                                    if (client.Player.Level == LevelMgr.MaxLevel)
                                    {
                                        int num10 = gp / 100;
                                        if (num10 > 0)
                                        {
                                            client.Player.AddOffer(num10);
                                            str2 = string.Format("Max level khinh nghiệm quy đổi th\x00e0nh {0} c\x00f4ng trạng", num10);
                                        }
                                    }
                                    else
                                    {
                                        client.Player.AddGP(gp);
                                        str2 = "GPDanUser.Success";
                                    }
                                }
                                if (info4.Template.CanDelete)
                                {
                                    client.Player.RemoveAt((eBageType) num, slot);
                                }
                            }
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(str2, new object[] { item.Template.Property2 * item.Count }));
                        }
                    }
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardUseHandler.Fail", new object[0]));
                }
            }
            client.Player.LastOpenCard = DateTime.Now;
            return 0;
        }
    }
}


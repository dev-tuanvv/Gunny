namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Text;

    [PacketHandler(0x3a, "物品合成")]
    public class ItemComposeHandler : IPacketHandler
    {
        private static readonly double[] composeRate = new double[] { 0.8, 0.5, 0.3, 0.1, 0.05 };
        public static ThreadSafeRandom random = new ThreadSafeRandom();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn @in = new GSPacketIn(0x3a, client.Player.PlayerCharacter.ID);
            StringBuilder builder = new StringBuilder();
            int num = GameProperties.PRICE_COMPOSE_GOLD;
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            if (client.Player.PlayerCharacter.Gold < num)
            {
                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemComposeHandler.NoMoney", new object[0]));
                return 0;
            }
            int num2 = -1;
            int slot = -1;
            bool flag = false;
            bool flag2 = packet.ReadBoolean();
            SqlDataProvider.Data.ItemInfo itemAt = client.Player.StoreBag.GetItemAt(1);
            SqlDataProvider.Data.ItemInfo info2 = client.Player.StoreBag.GetItemAt(2);
            SqlDataProvider.Data.ItemInfo info3 = null;
            SqlDataProvider.Data.ItemInfo item = null;
            if (((info2 == null) || (itemAt == null)) || (info2.Count <= 0))
            {
                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Đặt đ\x00e1 tăng cấp v\x00e0 trang bị cần hợp th\x00e0nh v\x00e0o!", new object[0]));
                return 0;
            }
            string property = null;
            string str2 = null;
            using (ItemRecordBussiness bussiness = new ItemRecordBussiness())
            {
                bussiness.PropertyString(itemAt, ref property);
            }
            if ((((itemAt == null) || (info2 == null)) || !itemAt.Template.CanCompose) || ((itemAt.Template.CategoryID >= 10) && ((info2.Template.CategoryID != 11) || (info2.Template.Property1 != 1))))
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemComposeHandler.Fail", new object[0]));
            }
            else
            {
                flag = (flag || itemAt.IsBinds) || info2.IsBinds;
                builder.Append(string.Concat(new object[] { itemAt.ItemID, ":", itemAt.TemplateID, ",", info2.ItemID, ":", info2.TemplateID, "," }));
                bool flag3 = false;
                byte val = 1;
                double num5 = composeRate[info2.Template.Quality - 1] * 100.0;
                if (client.Player.StoreBag.GetItemAt(0) != null)
                {
                    info3 = client.Player.StoreBag.GetItemAt(0);
                    if (((info3 != null) && (info3.Template.CategoryID == 11)) && (info3.Template.Property1 == 3))
                    {
                        flag = flag || info3.IsBinds;
                        object obj2 = str2;
                        str2 = string.Concat(new object[] { obj2, "|", info3.ItemID, ":", info3.Template.Name, "|", info2.ItemID, ":", info2.Template.Name });
                        builder.Append(string.Concat(new object[] { info3.ItemID, ":", info3.TemplateID, "," }));
                        num5 += (num5 * info3.Template.Property2) / 100.0;
                    }
                }
                else
                {
                    num5 += (num5 * 1.0) / 100.0;
                }
                if (slot != -1)
                {
                    item = client.Player.PropBag.GetItemAt(slot);
                    if (((item != null) && (item.Template.CategoryID == 11)) && (item.Template.Property1 == 7))
                    {
                        flag = flag || item.IsBinds;
                        builder.Append(string.Concat(new object[] { item.ItemID, ":", item.TemplateID, "," }));
                        object obj3 = str2;
                        str2 = string.Concat(new object[] { obj3, ",", item.ItemID, ":", item.Template.Name });
                    }
                    else
                    {
                        item = null;
                    }
                }
                if (flag2)
                {
                    ConsortiaInfo info5 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                    ConsortiaEquipControlInfo info6 = new ConsortiaBussiness().GetConsortiaEuqipRiches(client.Player.PlayerCharacter.ConsortiaID, 0, 2);
                    if (info5 == null)
                    {
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Fail", new object[0]));
                    }
                    else
                    {
                        if (client.Player.PlayerCharacter.Riches < info6.Riches)
                        {
                            client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemStrengthenHandler.FailbyPermission", new object[0]));
                            return 1;
                        }
                        num5 *= 1.0 + (0.1 * info5.SmithLevel);
                    }
                }
                num5 = Math.Floor((double) (num5 * 10.0)) / 10.0;
                int num6 = random.Next(100);
                switch (info2.Template.Property3)
                {
                    case 1:
                        if (info2.Template.Property4 > itemAt.AttackCompose)
                        {
                            flag3 = true;
                            if (num5 > num6)
                            {
                                val = 0;
                                itemAt.AttackCompose = info2.Template.Property4;
                            }
                        }
                        break;

                    case 2:
                        if (info2.Template.Property4 > itemAt.DefendCompose)
                        {
                            flag3 = true;
                            if (num5 > num6)
                            {
                                val = 0;
                                itemAt.DefendCompose = info2.Template.Property4;
                            }
                        }
                        break;

                    case 3:
                        if (info2.Template.Property4 > itemAt.AgilityCompose)
                        {
                            flag3 = true;
                            if (num5 > num6)
                            {
                                val = 0;
                                itemAt.AgilityCompose = info2.Template.Property4;
                            }
                        }
                        break;

                    case 4:
                        if (info2.Template.Property4 > itemAt.LuckCompose)
                        {
                            flag3 = true;
                            if (num5 > num6)
                            {
                                val = 0;
                                itemAt.LuckCompose = info2.Template.Property4;
                            }
                        }
                        break;
                }
                if (flag3)
                {
                    itemAt.IsBinds = flag;
                    if (val != 0)
                    {
                        builder.Append("false!");
                    }
                    else
                    {
                        builder.Append("true!");
                        client.Player.OnItemCompose(info2.TemplateID);
                    }
                    client.Player.StoreBag.RemoveTemplate(info2.TemplateID, 1);
                    if (info3 != null)
                    {
                        client.Player.StoreBag.RemoveTemplate(info3.TemplateID, 1);
                    }
                    if (item != null)
                    {
                        client.Player.RemoveItem(item);
                    }
                    client.Player.RemoveGold(num);
                    client.Player.StoreBag.UpdateItem(itemAt);
                    @in.WriteByte(val);
                    client.Out.SendTCP(@in);
                    if (num2 < 0x1f)
                    {
                        client.Player.EquipBag.UpdatePlayerProperties();
                    }
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemComposeHandler.NoLevel", new object[0]));
                }
            }
            return 0;
        }
    }
}


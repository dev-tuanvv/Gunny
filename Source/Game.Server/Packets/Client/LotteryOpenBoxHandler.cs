namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [PacketHandler(0x1a, "打开物品")]
    public class LotteryOpenBoxHandler : IPacketHandler
    {
        private GSPacketIn caddygetaward(List<SqlDataProvider.Data.ItemInfo> list, string name, int zoneID)
        {
            GSPacketIn @in = new GSPacketIn(0xf5);
            @in.WriteBoolean(true);
            @in.WriteInt(list.Count);
            foreach (SqlDataProvider.Data.ItemInfo info in list)
            {
                @in.WriteString(name);
                @in.WriteInt(info.TemplateID);
                @in.WriteInt(info.Count);
                @in.WriteInt(zoneID);
                @in.WriteBoolean(false);
            }
            return @in;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            new ProduceBussiness();
            if (client.Lottery != -1)
            {
                client.Out.SendMessage(eMessageType.Normal, "Rương đang hoạt động!");
                return 1;
            }
            int num = packet.ReadByte();
            int num2 = packet.ReadInt();
            int templateID = packet.ReadInt();
            List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo>();
            PlayerInventory inventory = client.Player.GetInventory((eBageType) num);
            string nickName = client.Player.PlayerCharacter.NickName;
            if (inventory.FindFirstEmptySlot() == -1)
            {
                client.Out.SendMessage(eMessageType.Normal, "Rương đ\x00e3 đầy kh\x00f4ng thể mở th\x00eam!");
                return 1;
            }
            int num4 = 0x2cc0;
            SqlDataProvider.Data.ItemInfo itemByTemplateID = client.Player.GetItemByTemplateID(num4);
            SqlDataProvider.Data.ItemInfo info2 = client.Player.GetItemByTemplateID(templateID);
            if ((info2 == null) || (info2.Count < 1))
            {
                Console.WriteLine("eBageType.{1} slot {1} templateID {2}", (eBageType) num, num2, templateID);
                return 1;
            }
            if (info2 == null)
            {
                items = client.Player.CaddyBag.GetItems();
                client.Out.SendTCP(this.caddygetaward(items, nickName, client.Player.ZoneId));
                return 0;
            }
            if (info2.Count < 1)
            {
                items = client.Player.CaddyBag.GetItems();
                client.Out.SendTCP(this.caddygetaward(items, nickName, client.Player.ZoneId));
                return 0;
            }
            List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
            StringBuilder builder = new StringBuilder();
            int point = 0;
            int gold = 0;
            int giftToken = 0;
            int medal = 0;
            int exp = 0;
            int honor = 0;
            int hardCurrency = 0;
            int leagueMoney = 0;
            int useableScore = 0;
            int prestge = 0;
            int magicStonePoint = 0;
            if (!ItemBoxMgr.CreateItemBox(info2.TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref magicStonePoint))
            {
                client.Player.SendMessage("Xảy ra lổi hảy thử lại sau.");
                return 0;
            }
            if (point != 0)
            {
                builder.Append(point + LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]));
                client.Player.AddMoney(point);
            }
            if (gold != 0)
            {
                builder.Append(gold + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]));
                client.Player.AddGold(gold);
            }
            if (giftToken != 0)
            {
                builder.Append(giftToken + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]));
                client.Player.AddGiftToken(giftToken);
            }
            if (medal != 0)
            {
                builder.Append(medal + LanguageMgr.GetTranslation("OpenUpArkHandler.Medal", new object[0]));
                client.Player.AddMedal(medal);
            }
            if (exp != 0)
            {
                builder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.Exp", new object[0]));
                client.Player.AddGP(exp);
            }
            if (honor != 0)
            {
                builder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.honor", new object[0]));
                client.Player.AddHonor(honor);
            }
            if (hardCurrency != 0)
            {
                builder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.hardCurrency", new object[0]));
                client.Player.AddHardCurrency(hardCurrency);
            }
            if (magicStonePoint != 0)
            {
                builder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.MagicStonePoint", new object[0]));
                client.Player.AddMagicStonePoint(magicStonePoint);
            }
            if (leagueMoney != 0)
            {
                builder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.leagueMoney", new object[0]));
                client.Player.AddLeagueMoney(leagueMoney);
            }
            if (useableScore != 0)
            {
                builder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.useableScore", new object[0]));
                client.Player.AddGP(useableScore);
            }
            if (prestge != 0)
            {
                builder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.prestge", new object[0]));
                client.Player.AddGP(prestge);
            }
            if (itemInfos.Count > 0)
            {
                SqlDataProvider.Data.ItemInfo item = itemInfos[0];
                switch (templateID)
                {
                    case 0x1b5e4:
                    case 0x1b5e5:
                    case 0x1b5af:
                        if (itemByTemplateID.Count < 4)
                        {
                            client.Out.SendMessage(eMessageType.Normal, string.Format("{0} kh\x00f4ng đủ.", itemByTemplateID.Template.Name));
                            client.Out.SendTCP(this.caddygetaward(items, nickName, client.Player.ZoneId));
                            return 0;
                        }
                        client.Player.RemoveTemplate(num4, 4);
                        break;

                    default:
                        nickName = item.Template.Name;
                        break;
                }
                inventory.AddItem(item);
                builder.Append(item.Template.Name);
                client.Player.RemoveTemplate(templateID, 1);
            }
            items = client.Player.CaddyBag.GetItems();
            client.Out.SendTCP(this.caddygetaward(items, nickName, client.Player.ZoneId));
            client.Lottery = -1;
            if (builder != null)
            {
                client.Out.SendMessage(eMessageType.Normal, "Bạn nhận được " + builder.ToString());
            }
            return 1;
        }
    }
}


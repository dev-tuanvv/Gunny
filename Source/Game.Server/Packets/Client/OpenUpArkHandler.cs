namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [PacketHandler(0x3f, "打开物品")]
    public class OpenUpArkHandler : IPacketHandler
    {
        public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadByte();
            int slot = packet.ReadInt();
            int count = packet.ReadInt();
            PlayerInventory inventory = client.Player.GetInventory((eBageType) num);
            SqlDataProvider.Data.ItemInfo itemAt = inventory.GetItemAt(slot);
            string str = "";
            if ((((itemAt != null) && itemAt.IsValidItem()) && ((itemAt.Template.CategoryID == 11) && (itemAt.Template.Property1 == 6))) && (client.Player.PlayerCharacter.Grade >= itemAt.Template.NeedLevel))
            {
                if ((count < 1) || (count > itemAt.Count))
                {
                    count = itemAt.Count;
                }
                int num4 = 0;
                string translation = "";
                StringBuilder builder = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                if (!inventory.RemoveCountFromStack(itemAt, count))
                {
                    return 0;
                }
                Dictionary<int, SqlDataProvider.Data.ItemInfo> dictionary = new Dictionary<int, SqlDataProvider.Data.ItemInfo>();
                builder2.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start", new object[0]));
                for (int i = 0; i < count; i++)
                {
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
                    List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                    ItemBoxMgr.CreateItemBox(itemAt.TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref magicStonePoint);
                    if (point != 0)
                    {
                        num4 += point;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]);
                        client.Player.AddMoney(point);
                    }
                    if (gold != 0)
                    {
                        num4 += gold;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]);
                        client.Player.AddGold(gold);
                    }
                    if (giftToken != 0)
                    {
                        num4 += giftToken;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]);
                        client.Player.AddGiftToken(giftToken);
                    }
                    if (medal != 0)
                    {
                        num4 += medal;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.Medal", new object[0]);
                        client.Player.AddMedal(medal);
                    }
                    if (exp != 0)
                    {
                        num4 += exp;
                        if (client.Player.Level == LevelMgr.MaxLevel)
                        {
                            int num17 = num4 / 100;
                            if (num17 > 0)
                            {
                                client.Player.AddOffer(num17);
                                translation = string.Format("Max level khinh nghiệm quy đổi th\x00e0nh {0} c\x00f4ng trạng", num17);
                            }
                        }
                        else
                        {
                            translation = LanguageMgr.GetTranslation("OpenUpArkHandler.Exp", new object[0]);
                            client.Player.AddGP(exp);
                        }
                    }
                    if (honor != 0)
                    {
                        num4 += honor;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.honor", new object[0]);
                        client.Player.AddHonor(honor);
                    }
                    if (hardCurrency != 0)
                    {
                        num4 += hardCurrency;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.hardCurrency", new object[0]);
                        client.Player.AddHardCurrency(hardCurrency);
                    }
                    if (magicStonePoint != 0)
                    {
                        num4 += magicStonePoint;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.MagicStonePoint", new object[0]);
                        client.Player.AddMagicStonePoint(magicStonePoint);
                    }
                    if (leagueMoney != 0)
                    {
                        num4 += leagueMoney;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.leagueMoney", new object[0]);
                        client.Player.AddLeagueMoney(leagueMoney);
                    }
                    if (useableScore != 0)
                    {
                        num4 += useableScore;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.useableScore", new object[0]);
                    }
                    if (prestge != 0)
                    {
                        num4 += prestge;
                        translation = LanguageMgr.GetTranslation("OpenUpArkHandler.prestge", new object[0]);
                    }
                    foreach (SqlDataProvider.Data.ItemInfo info2 in itemInfos)
                    {
                        if (!dictionary.Keys.Contains<int>(info2.TemplateID))
                        {
                            dictionary.Add(info2.TemplateID, info2);
                        }
                        else
                        {
                            SqlDataProvider.Data.ItemInfo local1 = dictionary[info2.TemplateID];
                            local1.Count += info2.Count;
                        }
                    }
                }
                string name = itemAt.Template.Name;
                if (num4 > 0)
                {
                    builder2.Append(num4 + translation);
                }
                if (builder.Length > 0)
                {
                    builder.Remove(builder.Length - 1, 1);
                    string[] strArray = builder.ToString().Split(new char[] { ',' });
                    for (int j = 0; j < strArray.Length; j++)
                    {
                        string[] strArray2;
                        IntPtr ptr;
                        int num19 = 1;
                        for (int k = j + 1; k < strArray.Length; k++)
                        {
                            if (strArray[j].Contains(strArray[k]) && (strArray[k].Length == strArray[j].Length))
                            {
                                num19++;
                                strArray[k] = k.ToString();
                            }
                        }
                        if (num19 > 1)
                        {
                            strArray[j] = strArray[j].Remove(strArray[j].Length - 1, 1);
                            (strArray2 = strArray)[(int) (ptr = (IntPtr) j)] = strArray2[(int) ptr] + num19.ToString();
                        }
                        if (strArray[j] != j.ToString())
                        {
                            (strArray2 = strArray)[(int) (ptr = (IntPtr) j)] = strArray2[(int) ptr] + ",";
                            builder2.Append(strArray[j]);
                        }
                    }
                }
                builder2.Remove(builder2.Length - 1, 1);
                builder2.Append(".");
                if (num4 > 0)
                {
                    client.Out.SendMessage(eMessageType.Normal, str + builder2.ToString());
                }
                else
                {
                    GSPacketIn pkg = new GSPacketIn(0x3f, client.Player.PlayerCharacter.ID);
                    pkg.WriteString(name);
                    pkg.WriteByte((byte) dictionary.Count);
                    foreach (SqlDataProvider.Data.ItemInfo info3 in dictionary.Values)
                    {
                        pkg.WriteInt(info3.TemplateID);
                        pkg.WriteInt(info3.Count);
                        pkg.WriteBoolean(info3.IsBinds);
                        pkg.WriteInt(info3.ValidDate);
                        pkg.WriteInt(info3.StrengthenLevel);
                        pkg.WriteInt(info3.AttackCompose);
                        pkg.WriteInt(info3.DefendCompose);
                        pkg.WriteInt(info3.AgilityCompose);
                        pkg.WriteInt(info3.LuckCompose);
                        builder.Append(info3.Template.Name + "x" + info3.Count.ToString() + ",");
                        client.Player.AddTemplate(info3, info3.Template.BagType, info3.Count, eGameView.OtherTypeGet, name);
                    }
                    client.Player.SendTCP(pkg);
                }
            }
            return 1;
        }
    }
}


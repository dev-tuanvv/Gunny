namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xd1, "场景用户离开")]
    public class FigSpiritUpGradeHandler : IPacketHandler
    {
        private static readonly int[] exps = new int[] { 0, 600, 0x1464, 0x3de0, 0x986c, 0x15dec };
        private static readonly string[] places = new string[] { "0", "1", "2" };

        private bool canUpLv(int exp, int _curLv)
        {
            return (((((exp >= exps[1]) && (_curLv == 0)) || ((exp >= exps[2]) && (_curLv == 1))) || (((exp >= exps[3]) && (_curLv == 2)) || ((exp >= exps[4]) && (_curLv == 3)))) || ((exp >= exps[5]) && (_curLv == 4)));
        }

        private bool getMax(string[] SpiritIdValue)
        {
            int num = 0;
            if (SpiritIdValue[0].Split(new char[] { ',' })[0] == "5")
            {
                num = 1;
            }
            if (SpiritIdValue[1].Split(new char[] { ',' })[0] == "5")
            {
                num = 2;
            }
            if (SpiritIdValue[2].Split(new char[] { ',' })[0] == "5")
            {
                num = 3;
            }
            return (num == 3);
        }

        private int getNeedExp(int _curExp, int _curLv)
        {
            int num = exps[_curLv + 1];
            return (num - _curExp);
        }

        private int[] getOldExp(string[] curLvs)
        {
            int[] numArray = new int[curLvs.Length];
            for (int i = 0; i < curLvs.Length; i++)
            {
                numArray[i] = Convert.ToInt32(curLvs[i].Split(new char[] { ',' })[1]);
            }
            return numArray;
        }

        private int[] getOldLv(string[] curLvs)
        {
            int[] numArray = new int[curLvs.Length];
            for (int i = 0; i < curLvs.Length; i++)
            {
                numArray[i] = Convert.ToInt32(curLvs[i].Split(new char[] { ',' })[0]);
            }
            return numArray;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            Console.WriteLine("209");
            if (client.Player.PlayerCharacter.Grade < 30)
            {
                client.Out.SendMessage(eMessageType.Normal, "E h\x00e8m Ph\x00e1t hiện hack Level!");
                return 0;
            }
            packet.ReadByte();
            int num = packet.ReadInt();
            packet.ReadInt();
            packet.ReadInt();
            int templateId = packet.ReadInt();
            int num3 = packet.ReadInt();
            int place = packet.ReadInt();
            packet.ReadInt();
            packet.ReadInt();
            SqlDataProvider.Data.ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
            int itemCount = client.Player.PropBag.GetItemCount(0x18704);
            UserGemStone gemStone = client.Player.GetGemStone(place);
            string[] spiritIdValue = gemStone.FigSpiritIdValue.Split(new char[] { '|' });
            int iD = client.Player.PlayerCharacter.ID;
            bool isUp = false;
            bool isMaxLevel = this.getMax(spiritIdValue);
            bool isFall = true;
            int num7 = 1;
            int dir = 0;
            int[] numArray = this.getOldExp(spiritIdValue);
            int[] numArray2 = this.getOldLv(spiritIdValue);
            if ((itemCount <= 0) || (itemByTemplateID == null))
            {
                client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, isUp, isMaxLevel, isFall, 0, dir);
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Chiến hồn kh\x00f4ng đủ!", new object[0]));
                return 0;
            }
            if (!itemByTemplateID.isGemStone())
            {
                client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, isUp, isMaxLevel, isFall, 0, dir);
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Chiến hồn kh\x00f4ng đủ!", new object[0]));
                return 0;
            }
            if (!isMaxLevel && (itemByTemplateID != null))
            {
                switch (num)
                {
                    case 0:
                    {
                        int num9 = itemByTemplateID.Template.Property2;
                        for (int j = 0; j < places.Length; j++)
                        {
                            if (numArray2[j] < 5)
                            {
                                numArray[j] += num9;
                                if (isUp = this.canUpLv(numArray[j], numArray2[j]))
                                {
                                    numArray2[j]++;
                                    numArray[j] = 0;
                                }
                            }
                        }
                        client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
                        break;
                    }
                    case 1:
                    {
                        int count = 1;
                        for (int k = 0; k < places.Length; k++)
                        {
                            count = this.getNeedExp(numArray[k], numArray2[k]) / itemByTemplateID.Template.Property2;
                            if (itemCount < count)
                            {
                                count = itemCount;
                            }
                            int num13 = itemByTemplateID.Template.Property2 * count;
                            if (numArray2[k] < 5)
                            {
                                numArray[k] += num13;
                                if (isUp = this.canUpLv(numArray[k], numArray2[k]))
                                {
                                    numArray2[k]++;
                                    numArray[k] = 0;
                                }
                            }
                        }
                        client.Player.PropBag.RemoveTemplate(templateId, count);
                        break;
                    }
                }
            }
            if (isUp)
            {
                isFall = false;
                dir = 1;
                client.Player.EquipBag.UpdatePlayerProperties();
            }
            string str = string.Concat(new object[] { numArray2[0], ",", numArray[0], ",", places[0] });
            for (int i = 1; i < places.Length; i++)
            {
                object obj2 = str;
                str = string.Concat(new object[] { obj2, "|", numArray2[i], ",", numArray[i], ",", places[i] });
            }
            gemStone.FigSpiritId = num3;
            gemStone.FigSpiritIdValue = str;
            client.Player.UpdateGemStone(place, gemStone);
            client.Player.OnUserToemGemstoneEvent();
            client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, isUp, isMaxLevel, isFall, num7, dir);
            return 0;
        }
    }
}


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
    using System.Text.RegularExpressions;

    internal class AASInfoSetHandle : IPacketHandler
    {
        private static Regex _objRegex = new Regex(@"\d{18}|\d{15}");
        private static Regex _objRegex1 = new Regex(@"/^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$/");
        private static Regex _objRegex2 = new Regex(@"/^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$/");
        private static string[] cities;
        private static char[] checkCode;
        private static int[] WI;

        static AASInfoSetHandle()
        {
            string[] strArray = new string[0x5c];
            strArray[11] = "北京";
            strArray[12] = "天津";
            strArray[13] = "河北";
            strArray[14] = "山西";
            strArray[15] = "内蒙古";
            strArray[0x15] = "辽宁";
            strArray[0x16] = "吉林";
            strArray[0x17] = "黑龙江";
            strArray[0x1f] = "上海";
            strArray[0x20] = "江苏";
            strArray[0x21] = "浙江";
            strArray[0x22] = "安微";
            strArray[0x23] = "福建";
            strArray[0x24] = "江西";
            strArray[0x25] = "山东";
            strArray[0x29] = "河南";
            strArray[0x2a] = "湖北";
            strArray[0x2b] = "湖南";
            strArray[0x2c] = "广东";
            strArray[0x2d] = "广西";
            strArray[0x2e] = "海南";
            strArray[50] = "重庆";
            strArray[0x33] = "四川";
            strArray[0x34] = "贵州";
            strArray[0x35] = "云南";
            strArray[0x36] = "西藏";
            strArray[0x3d] = "陕西";
            strArray[0x3e] = "甘肃";
            strArray[0x3f] = "青海";
            strArray[0x40] = "宁夏";
            strArray[0x41] = "新疆";
            strArray[0x47] = "台湾";
            strArray[0x51] = "香港";
            strArray[0x52] = "澳门";
            strArray[0x5b] = "国外";
            cities = strArray;
            WI = new int[] { 
                7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 
                2
             };
            checkCode = new char[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
        }

        private bool CheckIDNumber(string IDNum)
        {
            bool flag = false;
            if (!_objRegex.IsMatch(IDNum))
            {
                return false;
            }
            int index = int.Parse(IDNum.Substring(0, 2));
            if (cities[index] == null)
            {
                return false;
            }
            if (IDNum.Length == 0x12)
            {
                int num2 = 0;
                for (int i = 0; i < 0x11; i++)
                {
                    char ch = IDNum[i];
                    num2 += int.Parse(ch.ToString()) * WI[i];
                }
                int num4 = num2 % 11;
                if (IDNum[0x11] == checkCode[num4])
                {
                    flag = true;
                }
            }
            return flag;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool flag3;
            AASInfo info = new AASInfo {
                UserID = client.Player.PlayerCharacter.ID
            };
            bool result = false;
            if (packet.ReadBoolean())
            {
                info.Name = "";
                info.IDNumber = "";
                info.State = 0;
                flag3 = true;
            }
            else
            {
                info.Name = packet.ReadString();
                info.IDNumber = packet.ReadString();
                flag3 = this.CheckIDNumber(info.IDNumber);
                if (info.IDNumber != "")
                {
                    client.Player.IsAASInfo = true;
                    int num = Convert.ToInt32(info.IDNumber.Substring(6, 4));
                    int num2 = Convert.ToInt32(info.IDNumber.Substring(10, 2));
                    if ((DateTime.Now.Year.CompareTo((int) (num + 0x12)) > 0) || ((DateTime.Now.Year.CompareTo((int) (num + 0x12)) == 0) && (DateTime.Now.Month.CompareTo(num2) >= 0)))
                    {
                        client.Player.IsMinor = false;
                    }
                }
                if ((info.Name != "") && flag3)
                {
                    info.State = 1;
                }
                else
                {
                    info.State = 0;
                }
            }
            if (flag3)
            {
                client.Out.SendAASState(false);
                using (ProduceBussiness bussiness = new ProduceBussiness())
                {
                    result = bussiness.AddAASInfo(info);
                    client.Out.SendAASInfoSet(result);
                }
            }
            if (result && (info.State == 1))
            {
                ItemTemplateInfo goods = ItemMgr.FindItemTemplate(0x2b0b);
                if (goods != null)
                {
                    SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 0x6b);
                    if (item != null)
                    {
                        item.IsBinds = true;
                        AbstractInventory itemInventory = client.Player.GetItemInventory(item.Template);
                        if (itemInventory.AddItem(item, itemInventory.BeginSlot))
                        {
                            client.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ASSInfoSetHandle.Success", new object[] { item.Template.Name }));
                        }
                        else
                        {
                            client.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ASSInfoSetHandle.NoPlace", new object[0]));
                        }
                    }
                }
            }
            return 0;
        }
    }
}


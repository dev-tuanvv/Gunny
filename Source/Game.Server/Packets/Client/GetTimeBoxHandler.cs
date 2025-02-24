namespace Game.Server.Packets.Client
{
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [PacketHandler(0x35, "场景用户离开")]
    public class GetTimeBoxHandler : IPacketHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool flag;
            int needGetBoxTime;
            int num = packet.ReadInt();
            int condition = packet.ReadInt();
            packet.ReadInt();
            packet.ReadInt();
            switch (num)
            {
                case 0:
                    if ((UserBoxMgr.GetTimeBoxWithCondition(condition) == null) || (client.Player.PlayerCharacter.receiebox >= condition))
                    {
                        client.Player.SendMessage("Kh\x00f4ng tr\x00f9ng khớp hộp thời gian. H\x00e3y th\x00f4ng b\x00e1o việc n\x00e0y cho GameMaster.");
                    }
                    else if (client.Player.BossBoxStartTime.AddMinutes((double) condition) <= DateTime.Now)
                    {
                        client.Player.PlayerCharacter.needGetBoxTime = condition;
                    }
                    goto Label_01FC;

                case 1:
                    flag = false;
                    needGetBoxTime = client.Player.PlayerCharacter.needGetBoxTime;
                    if (client.Player.PlayerCharacter.needGetBoxTime > 0)
                    {
                        UserBoxInfo timeBoxWithCondition = UserBoxMgr.GetTimeBoxWithCondition(needGetBoxTime);
                        if (timeBoxWithCondition == null)
                        {
                            client.Player.SendMessage("Hộp thời gian kh\x00f4ng tồn tại.");
                            break;
                        }
                        flag = true;
                        needGetBoxTime = timeBoxWithCondition.Condition;
                        client.Player.BossBoxStartTime = DateTime.Now;
                        client.Player.PlayerCharacter.receiebox = timeBoxWithCondition.Condition;
                        client.Player.PlayerCharacter.needGetBoxTime = 0;
                        List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                        int gold = 0;
                        int point = 0;
                        int giftToken = 0;
                        int exp = 0;
                        ItemBoxMgr.CreateItemBox(timeBoxWithCondition.TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref exp);
                        if (itemInfos.Count <= 0)
                        {
                            client.Player.SendMessage("Tạm kh\x00f4ng c\x00f3 phần thưởng!");
                            break;
                        }
                        client.Player.SendItemsToMail(itemInfos, "Bạn nhận được qu\x00e0 tặng online " + timeBoxWithCondition.Condition + " ph\x00fat. Ch\x00fac bạn chơi game vui vẻ!", "Qu\x00e0 Online nhận thưởng", eMailType.DailyAward);
                        client.Player.SendMessage("Th\x00e0nh c\x00f4ng! Kiểm tra thư để nhận.");
                    }
                    break;

                default:
                    goto Label_01FC;
            }
            client.Out.SendOpenTimeBox(needGetBoxTime, flag);
        Label_01FC:
            return 0;
        }
    }
}

